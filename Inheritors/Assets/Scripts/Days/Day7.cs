/* INHERITORS by Nick Perrin (c) 2020 */
using System;
using System.Linq;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DG.Tweening;

[RequireComponent(typeof(Day7DialogContent))]
public class Day7 : MonoBehaviour
{
    int dayNumber = 7;

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
    [Space]
    public GameObject grandfatherDialogTrigger1;
    public GameObject grandfatherChar1;
    public GameObject grandfatherDialogTrigger2;
    public GameObject grandfatherChar2;
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

        // Deactivate grandfather duplicate
        grandfatherDialogTrigger2.GetComponent<DialogTrigger>().Disable();
        grandfatherDialogTrigger2.SetActive(false);
        grandfatherChar2.SetActive(false);
        dialogTriggers[Character.Grandfather] = grandfatherDialogTrigger1.GetComponent<DialogTrigger>();

        // Fade in from WHITE.
        stateManager.SetState(State.Inert);
        transitionManager.SetAlpha(1f);
        transitionManager.SetColor(Color.white);
        yield return new WaitForSeconds(.5f);
        transitionManager.Hide(3f);
        yield return new WaitForSeconds(2f);

        // Cue the opening dialog.
        dialogManager.NewDialog(GetDialog("Day7Opening_1"), State.Inert);
        yield return new WaitUntil(dialogManager.IsDialogFinished);
        uiManager.SetUpTasksInventory();
        yield return new WaitForSeconds(1f);

        // Show the tasks, only cam send on the new one.
        taskManager.AddTask(TaskType.Mother, "Fetch firewood.");
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
        dialogManager.NewDialog(GetDialog("Day7Opening_2"));
        yield return new WaitUntil(dialogManager.IsDialogFinished);
        dialogTriggers[Character.Mother].Enable();

        stateManager.SetState(State.Normal);
        recordManager.PlayRecordings();

        yield return null;
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
            case ItemType.Wood:
                PickupWood(inventory.itemQuantity);
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
            case "Dropoff_Wood":
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
            case "Walk_Hammock":
                StartCoroutine(WalkHammock());
                break;

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
        SetTaskState(args.activeTask, args.taskList);
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
        yield return new WaitUntil(dialogManager.IsDialogFinished);

        taskManager.SetActiveTask(TaskType.Grandfather);
        taskManager.ChangeTask(TaskType.Grandfather, "Rest in the hammock.");
        triggers["Walk_Hammock"].Enable();
    }

    IEnumerator WalkHammock()
    {
        stateManager.SetState(State.Inert);
        recordManager.StopRecording();
        transitionManager.SetColor(Color.black);
        transitionManager.Show(1f);
        yield return new WaitForSeconds(4f);
        grandfatherDialogTrigger1.SetActive(false);
        grandfatherChar1.SetActive(false);
        grandfatherDialogTrigger2.SetActive(true);
        grandfatherChar2.SetActive(true);
        dialogTriggers[Character.Grandfather] = grandfatherDialogTrigger2.GetComponent<DialogTrigger>();
        dialogTriggers[Character.Grandfather].Enable();
        transitionManager.Hide(2f);

        yield return new WaitForSeconds(1.5f);
        dialogManager.NewDialog(GetDialog("Grandfather_FinishTask"));
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
            dialogManager.NewDialog(GetDialog("Sister_FinishTask"));
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
        taskManager.SetActiveTask(TaskType.Father, false);
        dialogTriggers[Character.Father].Remove();
        yield return new WaitUntil(dialogManager.IsDialogFinished);
        stateManager.SetState(State.Inert);
        pickupManager.LoseTaskTool();

        Animation animation = GameObject.Find("FatherLeaves").GetComponent<Animation>();
        animation.Play();
        yield return new WaitWhile(() => animation.isPlaying);

        taskManager.CompleteActiveTask();
        stateManager.SetState(State.Normal);
    }

    // ████████████████████████████ MOTHER ████████████████████████████████████

    void PickupWood(int itemQuantity)
    {
        if (itemQuantity == 1)
        {
            taskManager.ChangeTask(TaskType.Mother, "Collect 2 more logs.");
            taskManager.SetActiveTask(TaskType.Mother);
        }
        else if (itemQuantity == 2)
        {
            taskManager.ChangeTask(TaskType.Mother, "Collect 1 more log.");
        }
        else if (itemQuantity == 3)
        {
            taskManager.ChangeTask(TaskType.Mother, "Bring logs to firepit.");
            triggers["Dropoff_Wood"].Enable();
        }
    }

    // ██████████████████████████ GRANDMOTHER █████████████████████████████████

    IEnumerator GrandmotherStart()
    {
        yield return new WaitUntil(dialogManager.IsDialogFinished);

        taskManager.SetActiveTask(TaskType.Grandmother);
        taskManager.ChangeTask(TaskType.Grandmother, "Find herbs near the river.");
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
        dialogManager.NewDialog(GetDialog("Grandmother_FinishTask"));
        yield return new WaitUntil(dialogManager.IsDialogFinished);
        taskManager.CompleteActiveTask();

        Destroy(GameObject.FindWithTag("GrandmotherNPC"));
        dialogTriggers[Character.Grandmother].Remove();
    }

    // ████████████████████████████ GENERAL ███████████████████████████████████

    IEnumerator AllTasksProcess()
    {
        yield return new WaitForSeconds(1f);
        dialogManager.NewDialog(GetDialog("DayOver"));
        yield return new WaitUntil(dialogManager.IsDialogFinished);
        // StartCoroutine(SendNPCsHome());
        taskManager.AddAndSetActive(TaskType.DayEnd, "Return home.", false);
        Destroy(GameObject.FindWithTag("MotherNPC"));
        dialogTriggers[Character.Mother].Remove();
        triggers["Walk_End"].Enable();
    }

    IEnumerator SendNPCsHome()
    {
        yield return null;

        RemoveAllDialogTriggers();

        Destroy(GameObject.FindWithTag("MotherNPC"));
        Destroy(GameObject.FindWithTag("FatherNPC"));
        Destroy(GameObject.FindWithTag("SisterNPC"));
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

    Dialog motherLast;
    Dialog fatherLast;
    Dialog sisterLast;

    void SetTaskState(Task activeTask, Dictionary<TaskType, Task> taskList)
    {
        this.activeTask = activeTask;
        this.taskList = taskList;

        // Mother
        // --------------------------------------------------------------------
        if (taskList[TaskType.Mother].status == TaskStatus.Active)
        {
            dialogs[Character.Mother] = GetDialog("Mother_Active");
            dialogTriggers[Character.Mother].Enable();
        }
        else if (taskList[TaskType.Mother].status == TaskStatus.Completed)
        {
            dialogs[Character.Mother] = GetDialog("Mother_Completed");
        }

        // Father
        // --------------------------------------------------------------------
        if (taskList[TaskType.Father].status == TaskStatus.Active)
        {
            // Nothing: handled in script
        }
        else if (taskList[TaskType.Father].status == TaskStatus.Completed)
        {
            dialogs[Character.Father] = GetDialog("Father_Completed");
        }

        // Sister
        // --------------------------------------------------------------------
        if (taskList[TaskType.Sister].status == TaskStatus.Active)
        {
            dialogs[Character.Sister] = GetDialog("Sister_Active");
        }
        else if (taskList[TaskType.Sister].status == TaskStatus.Completed)
        {
            dialogs[Character.Sister] = GetDialog("Sister_Completed");
        }

        // Grandmother
        // --------------------------------------------------------------------
        if (taskList[TaskType.Grandmother].status == TaskStatus.Active)
        {
            dialogs[Character.Grandmother] = GetDialog("Grandmother_Active");
        }
        else if (taskList[TaskType.Grandmother].status == TaskStatus.Completed)
        {
            dialogs[Character.Grandmother] = GetDialog("Grandmother_Completed");
        }

        // Grandfather
        // --------------------------------------------------------------------
        if (taskList[TaskType.Grandfather].status == TaskStatus.Active)
        {
            dialogs[Character.Grandfather] = GetDialog("Grandfather_Active");
        }
        else if (taskList[TaskType.Grandfather].status == TaskStatus.Completed)
        {
            dialogs[Character.Grandfather] = GetDialog("Grandfather_Completed");
        }
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

    Day7DialogContent day7DialogContent;

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
        day7DialogContent = GetComponent<Day7DialogContent>();

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
    Dictionary<string, Dialog> dialogContent = new Dictionary<string, Dialog>();

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
            if (day7DialogContent.content.ContainsKey(startingKey))
            {
                dialogs[character] = GetDialog(startingKey);
            }
        }
    }

    Dialog GetDialog(string dialogTag)
    {
        return day7DialogContent.content[dialogTag];
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
