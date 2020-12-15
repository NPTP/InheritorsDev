/* INHERITORS by Nick Perrin (c) 2020 */
using System;
using System.Linq;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DG.Tweening;

[RequireComponent(typeof(DialogContent))]
[RequireComponent(typeof(DialogTaskState))]
public class Day6 : MonoBehaviour
{
    int dayNumber = 6;

    public bool enableDayScripts = true;
    Dictionary<string, Trigger> triggers = new Dictionary<string, Trigger>();
    Dictionary<Character, DialogTrigger> dialogTriggers = new Dictionary<Character, DialogTrigger>();
    Dictionary<TaskType, AreaTrigger> areas = new Dictionary<TaskType, AreaTrigger>();
    Task activeTask;
    Dictionary<TaskType, Task> taskList;

    /* -------------------------------------- */
    /* Day-specific objects, transforms, etc. */
    /* -------------------------------------- */
    [Header("Day-specific Objects")]
    public Material redheadMaterial;
    public GameObject seedsPickup;
    // public Transform motherQuadrant;
    // public Transform grandfatherQuadrant;
    // public Transform grandmotherQuadrant;

    /* -------------------------------------- */
    /* -------------------------------------- */

    void Awake()
    {
        PlayerPrefs.SetInt("currentDayNumber", dayNumber);
        if (!enableDayScripts && Application.isEditor)
            Destroy(this);
        else
            InitializeReferences();
    }

    void Start()
    {
        InitializeTriggers();
        InitializeAreas();
        SubscribeToEvents();
        InitializeDialogs();
        InitializeCharDialogTriggers();
        StartCoroutine("Intro");
    }

    // ████████████████████████████████████████████████████████████████████████
    // ███ INTRO
    // ████████████████████████████████████████████████████████████████████████

    IEnumerator Intro()
    {
        // Change player's hair to red
        Renderer headRenderer = GameObject.Find("C_man_1_FBX2013").GetComponent<Renderer>();
        var newMaterials = headRenderer.materials;
        newMaterials[1] = redheadMaterial;
        headRenderer.materials = newMaterials;

        // Fade in from WHITE.
        stateManager.SetState(State.Inert);
        transitionManager.SetAlpha(1f);
        transitionManager.SetColor(Color.white);
        yield return new WaitForSeconds(.5f);
        transitionManager.Hide(3f);
        yield return new WaitForSeconds(2f);

        // Cue the opening dialog.
        dialogManager.NewDialog(dialogContent.Get("Day6Opening_1"), State.Inert);
        yield return new WaitUntil(dialogManager.IsDialogFinished);
        uiManager.SetUpTasksInventory();
        yield return new WaitForSeconds(1f);

        // Show the tasks, only cam send on the new one.
        taskManager.AddTask(TaskType.Mother, "Fetch water.");
        yield return new WaitForSeconds(1f);
        taskManager.AddTask(TaskType.Father, "Hunt with father.");
        yield return new WaitForSeconds(1f);
        taskManager.AddTask(TaskType.Sister, "Help sister.");
        yield return new WaitForSeconds(1f);
        taskManager.AddTask(TaskType.Grandmother, "See grandmother.");
        yield return new WaitForSeconds(1f);
        taskManager.AddTask(TaskType.Grandfather, "See grandfather.");
        yield return new WaitForSeconds(1f);

        // Final dialog of opening.
        dialogManager.NewDialog(dialogContent.Get("Day6Opening_2"));
        yield return new WaitUntil(dialogManager.IsDialogFinished);
        dialogTriggers[Character.Mother].Enable();

        stateManager.SetState(State.Normal);
        recordManager.PlayRecordings();

        yield return new WaitForSeconds(1f);
    }

    // ████████████████████████████████████████████████████████████████████████
    // ███ EVENT HANDLERS
    // ████████████████████████████████████████████████████████████████████████

    // Currently unused.
    // void HandleAreaEvent(object sender, InteractManager.AreaArgs args)
    // {
    //     string tag = args.tag;
    //     bool inside = args.inside;

    //     switch (tag)
    //     {
    //         default:
    //             Debug.Log("Interact Manager gave unknown AREA tag to Day " + dayNumber);
    //             break;
    //     }
    // }

    void HandlePickupEvent(object sender, InteractManager.PickupArgs args)
    {
        PickupManager.Inventory inventory = args.inventory;
        switch (inventory.itemType)
        {
            case ItemType.Water:
                PickupWater();
                break;

            case ItemType.Herbs:
                PickupHerbs();
                break;

            case ItemType.Null:
                Debug.Log("NULL pickup event: HandlePickupEvent in Day");
                break;

            default:
                Debug.Log("Interact Manager gave unknown PICKUP tag to Day " + dayNumber);
                break;
        }
    }

    void HandleDialogEvent(object sender, InteractManager.DialogArgs args)
    {
        Character character = args.dialog.character;
        dialogManager.NewDialog(dialogs[character]);

        switch (character)
        {
            case Character.Father:
                if (activeTask.type != TaskType.Null)
                    return;

                if (taskList[TaskType.Father].status == TaskStatus.Waiting)
                    StartCoroutine(HuntBegin());
                break;


            case Character.Sister:
                if (activeTask.type != TaskType.Null)
                    return;

                if (taskList[TaskType.Sister].status == TaskStatus.Waiting)
                    StartCoroutine(SisterStart());
                break;


            case Character.Grandfather:
                if (taskList[TaskType.Grandfather].status == TaskStatus.Waiting)
                    StartCoroutine(GrandfatherStart());
                break;


            case Character.Grandmother:
                if (activeTask.type != TaskType.Null)
                    return;


                if (taskList[TaskType.Grandmother].status == TaskStatus.Waiting)
                    StartCoroutine(GrandmotherStart());
                break;


            default:
                break;
        }
    }

    void HandleDropoffEvent(object sender, InteractManager.DropoffArgs args)
    {
        string tag = args.tag;

        switch (tag)
        {
            case "Dropoff_Water":
                DropoffWater();
                break;

            case "Dropoff_Meat":
                taskManager.CompleteActiveTask();
                break;

            case "Dropoff_Herbs":
                StartCoroutine(DropoffHerbs());
                break;

            case "Dropoff_Seed":
                StartCoroutine(DropoffSeed());
                break;

            default:
                Debug.Log("Interact Manager gave unknown DROPOFF tag to Day " + dayNumber);
                break;
        }
    }

    void HandleWalkEvent(object sender, InteractManager.WalkArgs args)
    {
        string tag = args.tag;

        switch (tag)
        {
            case "Walk_End":
                StartCoroutine(End());
                break;

            default:
                Debug.Log("Interact Manager gave unknown WALK tag to Day " + dayNumber);
                break;
        }
    }

    void EnableChildTriggers(string parentGameobjectName)
    {
        Transform parent = GameObject.Find(parentGameobjectName).transform;
        foreach (Transform child in parent)
        {
            child.GetComponent<Trigger>().Enable();
        }
    }

    void HandleUpdateTasks(object sender, TaskManager.TaskArgs args)
    {
        this.activeTask = args.activeTask;
        this.taskList = args.taskList;
        dialogTaskState.SetDialogs(ref activeTask,
                                    ref taskList,
                                    ref dialogs,
                                    ref dialogTriggers);
    }

    void HandleAllTasksComplete(object sender, EventArgs args)
    {
        StartCoroutine(AllTasksProcess());
    }

    IEnumerator WaitDialogEnd()
    {
        yield return new WaitUntil(dialogManager.IsDialogFinished);
    }

    // ████████████████████████████████████████████████████████████████████████
    // ███ HAPPENINGS OF THE DAY
    // ████████████████████████████████████████████████████████████████████████

    // ██████████████████████████ GRANDFATHER █████████████████████████████████

    IEnumerator GrandfatherStart()
    {
        taskManager.SetActiveTask(TaskType.Grandfather, false);
        yield return new WaitUntil(dialogManager.IsDialogFinished);
        taskManager.CompleteActiveTask();
    }

    // ████████████████████████████ SISTER ████████████████████████████████████

    IEnumerator SisterStart()
    {
        yield return new WaitUntil(dialogManager.IsDialogFinished);

        taskManager.SetActiveTask(TaskType.Sister);
        taskManager.ChangeTask(TaskType.Sister, "Plant 4 manioc seeds.");

        PickupTrigger seeds = GameObject.Instantiate(seedsPickup,
            pickupManager.GetItemHoldPosition(),
            Quaternion.identity,
            GameObject.FindWithTag("Player").transform).GetComponent<PickupTrigger>();
        seeds.GetPickedUp();
        pickupManager.PickUp(seeds);

        EnableChildTriggers("SeedDropoffTriggers");
    }

    int numSeedsPlanted = 0;
    IEnumerator DropoffSeed()
    {
        numSeedsPlanted++;

        if (numSeedsPlanted < 4)
        {
            PickupTrigger seeds = GameObject.Instantiate(seedsPickup,
                pickupManager.GetItemHoldPosition(),
                Quaternion.identity,
                GameObject.FindWithTag("Player").transform).GetComponent<PickupTrigger>();
            seeds.GetPickedUp();
            pickupManager.PickUp(seeds);

            if (numSeedsPlanted == 3)
                taskManager.ChangeTask(TaskType.Sister, "Plant 1 more seed.");
            else
                taskManager.ChangeTask(TaskType.Sister, "Plant " + (4 - numSeedsPlanted).ToString() + " more seeds.");
        }
        else
        {
            recordManager.StopRecording();
            dialogManager.NewDialog(dialogContent.Get("Sister_FinishTask"));
            yield return new WaitUntil(dialogManager.IsDialogFinished);
            taskManager.CompleteActiveTask();
        }
    }

    IEnumerator SisterFinish()
    {
        recordManager.StopRecording();
        yield return new WaitUntil(dialogManager.IsDialogFinished);
        taskManager.CompleteActiveTask();
    }

    // ████████████████████████████ FATHER ████████████████████████████████████

    IEnumerator HuntBegin()
    {
        yield return new WaitUntil(dialogManager.IsDialogFinished);
        taskManager.SetActiveTask(TaskType.Father);
        taskManager.ChangeTask(TaskType.Father, "Kill the pig.");

        stateManager.SetState(State.Inert);

        // Fireworks
        yield return new WaitForSeconds(.5f);
        Animation animation = GameObject.Find("HuntFireworks").GetComponent<Animation>();
        animation.Play();
        yield return new WaitWhile(() => animation.isPlaying);

        recordManager.StopRecording();
        dialogManager.NewDialog(dialogContent.Get("Father_HuntEnd"));
        yield return new WaitUntil(dialogManager.IsDialogFinished);

        pickupManager.LoseTaskTool();
        taskManager.CompleteActiveTask();

        stateManager.SetState(State.Normal);
    }

    // ████████████████████████████ MOTHER ████████████████████████████████████

    void PickupWater()
    {
        taskManager.SetActiveTask(TaskType.Mother);
        taskManager.ChangeTask(TaskType.Mother, "Bring the water to mother.");
        triggers["Dropoff_Water"].Enable();
    }

    void DropoffWater()
    {
        pickupManager.LoseTaskTool();
        taskManager.CompleteActiveTask();
    }

    // ██████████████████████████ GRANDMOTHER █████████████████████████████████

    IEnumerator GrandmotherStart()
    {
        yield return new WaitUntil(dialogManager.IsDialogFinished);

        taskManager.SetActiveTask(TaskType.Grandmother);
        taskManager.ChangeTask(TaskType.Grandmother, "Find herbs on hilltop.");
        triggers["Pickup_Herbs"].Enable();
    }

    void PickupHerbs()
    {
        taskManager.ChangeTask(TaskType.Grandmother, "Return herbs to grandmother.");
        triggers["Dropoff_Herbs"].Enable();
    }

    IEnumerator DropoffHerbs()
    {
        recordManager.StopRecording();
        dialogManager.NewDialog(dialogContent.Get("Grandmother_FinishTask"));
        yield return new WaitUntil(dialogManager.IsDialogFinished);
        taskManager.CompleteActiveTask();
    }

    // ████████████████████████████ GENERAL ███████████████████████████████████

    IEnumerator AllTasksProcess()
    {
        DisableAllDialogTriggers();
        yield return new WaitForSeconds(1f);
        dialogManager.NewDialog(dialogContent.Get("DayOver"));
        yield return new WaitUntil(dialogManager.IsDialogFinished);
        EnableAllDialogTriggers();

        dialogTriggers[Character.Mother].Remove();
        Destroy(GameObject.FindWithTag("MotherNPC"));

        taskManager.AddAndSetActive(TaskType.DayEnd, "Return home.", false);
        triggers["Walk_End"].Enable();
    }

    void DisableAllDialogTriggers()
    {
        foreach (DialogTrigger dialogTrigger in dialogTriggers.Values)
        {
            dialogTrigger.Disable();
        }
    }

    void EnableAllDialogTriggers()
    {
        foreach (DialogTrigger dialogTrigger in dialogTriggers.Values)
        {
            dialogTrigger.Enable();
        }
    }

    IEnumerator End()
    {
        stateManager.SetState(State.Inert);
        uiManager.TearDownTasksInventory();
        Tween t = transitionManager.Show(2f);
        audioManager.FadeOtherSources("Down", 2f); // audioManager.FadeTo(0f, 2f, Ease.InOutQuad);
        yield return t.WaitForCompletion();

        saveManager.SaveGame(dayNumber);
        Helper.LoadScene("Loading");
    }

    void RemoveAllDialogTriggers()
    {
        foreach (DialogTrigger dialogTrigger in dialogTriggers.Values)
        {
            dialogTrigger.Remove();
        }
    }

    // ████████████████████████████████████████████████████████████████████████
    // ███ INITIALIZERS & DESTROYERS
    // ████████████████████████████████████████████████████████████████████████

    DialogContent dialogContent;

    DialogTaskState dialogTaskState;

    SaveManager saveManager;
    StateManager stateManager;
    TaskManager taskManager;
    DialogManager dialogManager;
    InputManager inputManager;
    AudioManager audioManager;
    TransitionManager transitionManager;
    CameraManager cameraManager;
    UIManager uiManager;
    InteractManager interactManager;
    PickupManager pickupManager;
    RecordManager recordManager;
    void InitializeReferences()
    {
        dialogContent = GetComponent<DialogContent>();

        dialogTaskState = GetComponent<DialogTaskState>();

        saveManager = FindObjectOfType<SaveManager>();
        audioManager = FindObjectOfType<AudioManager>();
        taskManager = FindObjectOfType<TaskManager>();
        dialogManager = FindObjectOfType<DialogManager>();
        stateManager = FindObjectOfType<StateManager>();
        transitionManager = FindObjectOfType<TransitionManager>();
        inputManager = FindObjectOfType<InputManager>();
        cameraManager = FindObjectOfType<CameraManager>();
        uiManager = FindObjectOfType<UIManager>();
        interactManager = FindObjectOfType<InteractManager>();
        interactManager = FindObjectOfType<InteractManager>();
        pickupManager = FindObjectOfType<PickupManager>();
        recordManager = FindObjectOfType<RecordManager>();
    }

    Dictionary<Character, Dialog> dialogs = new Dictionary<Character, Dialog>();

    // ALL types of triggers
    void InitializeTriggers()
    {
        IEnumerable<Trigger> worldTriggers = FindObjectsOfType<MonoBehaviour>().OfType<Trigger>();
        foreach (Trigger trigger in worldTriggers)
        {
            triggers[trigger.GetTag()] = trigger;
        }
    }

    // Dialog triggers only, referenced by Character key.
    void InitializeCharDialogTriggers()
    {
        DialogTrigger[] dt = FindObjectsOfType<DialogTrigger>();
        foreach (DialogTrigger dialogTrigger in dt)
        {
            dialogTriggers[dialogTrigger.character] = dialogTrigger;
            // dialogTriggers[dialogTrigger.character].dialog = dialogs[dialogTrigger.character.ToString() + "_Start"];
        }
    }

    void InitializeDialogs()
    {
        foreach (Character character in Enum.GetValues(typeof(Character)))
        {
            string startingKey = character.ToString() + "_Start";
            if (dialogContent.ContainsKey(startingKey))
            {
                dialogs[character] = dialogContent.Get(startingKey);
            }
        }
    }

    void InitializeAreas()
    {
        AreaTrigger[] worldAreas = FindObjectsOfType<AreaTrigger>();
        foreach (AreaTrigger areaTrigger in worldAreas)
        {
            areas[areaTrigger.taskType] = areaTrigger;
        }
    }

    void SubscribeToEvents()
    {
        // interactManager.OnArea += HandleAreaEvent;
        interactManager.OnPickup += HandlePickupEvent;
        interactManager.OnDropoff += HandleDropoffEvent;
        interactManager.OnDialog += HandleDialogEvent;
        interactManager.OnWalk += HandleWalkEvent;

        taskManager.OnUpdateTasks += HandleUpdateTasks;
        taskManager.OnAllTasks += HandleAllTasksComplete;
    }

    void OnDestroy()
    {
        if (enableDayScripts)
        {
            // interactManager.OnArea -= HandleAreaEvent;
            interactManager.OnPickup -= HandlePickupEvent;
            interactManager.OnDropoff -= HandleDropoffEvent;
            interactManager.OnDialog -= HandleDialogEvent;
            interactManager.OnWalk -= HandleWalkEvent;

            taskManager.OnUpdateTasks -= HandleUpdateTasks;
            taskManager.OnAllTasks -= HandleAllTasksComplete;
        }
    }

}
