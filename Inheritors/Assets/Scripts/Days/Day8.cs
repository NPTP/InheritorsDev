/* INHERITORS by Nick Perrin (c) 2020 */
using System;
using System.Linq;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DG.Tweening;

[RequireComponent(typeof(Day8DialogContent))]
public class Day8 : MonoBehaviour
{
    int dayNumber = 8;

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
        if (!enableDayScripts)
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

    void Update()
    {
        if (Input.GetKeyDown(KeyCode.Return))
        {
            StartCoroutine(End());
        }
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
        dialogManager.NewDialog(GetDialog("Day8Opening_1"), State.Inert);
        yield return new WaitUntil(dialogManager.IsDialogFinished);
        uiManager.SetUpTasksInventory();
        yield return new WaitForSeconds(1f);

        // Show the tasks, only cam send on the new one.
        taskManager.AddTask(TaskType.Mother, "Fetch water.");
        yield return new WaitForSeconds(1f);
        taskManager.AddTask(TaskType.Father, "Check for father.");
        yield return new WaitForSeconds(1f);
        taskManager.AddTask(TaskType.Sister, "Console sister.");
        yield return new WaitForSeconds(1f);
        taskManager.AddTask(TaskType.Grandmother, "Wake grandmother.");
        yield return new WaitForSeconds(1f);
        taskManager.AddTask(TaskType.Grandfather, "Talk to grandfather.");
        yield return new WaitForSeconds(1f);

        // Final dialog of opening.
        dialogManager.NewDialog(GetDialog("Day8Opening_2"));
        yield return new WaitUntil(dialogManager.IsDialogFinished);
        dialogTriggers[Character.Mother].Enable();

        stateManager.SetState(State.Normal);

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

            case ItemType.Flute:
                StartCoroutine(PickupFlute());
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
            case Character.Grandfather:
                if (taskList[TaskType.Grandfather].status == TaskStatus.Waiting)
                    StartCoroutine(GrandfatherStart());
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

            case "Dropoff_Seed":
                DropoffSeed();
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

            case "Walk_Father":
                StartCoroutine(HuntBegin());
                break;

            case "Walk_Sister":
                StartCoroutine(SisterStart());
                break;

            case "Walk_Grandmother":
                StartCoroutine(GrandmotherStart());
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
        taskManager.ChangeTask(TaskType.Grandfather, "Pick up the matété.");
        triggers["Pickup_Flute"].Enable();
    }

    IEnumerator PickupFlute()
    {
        stateManager.SetState(State.Inert);
        taskManager.ChangeTask(TaskType.Grandfather, "Play with grandfather.");
        yield return new WaitForSeconds(1f);

        recordManager.StopRecording();
        dialogManager.NewDialog(GetDialog("Grandfather_FinishTask"));
        yield return new WaitUntil(dialogManager.IsDialogFinished);

        pickupManager.LoseItems();
        taskManager.CompleteActiveTask();
    }

    IEnumerator WalkHammock()
    {
        stateManager.SetState(State.Inert);
        transitionManager.SetColor(Color.black);
        transitionManager.Show(1f);
        yield return new WaitForSeconds(4f);
        transitionManager.Hide(2f);
        yield return new WaitForSeconds(1.5f);

        dialogManager.NewDialog(GetDialog("Player_Hammock"));
    }

    // ████████████████████████████ SISTER ████████████████████████████████████

    IEnumerator SisterStart()
    {
        taskManager.SetActiveTask(TaskType.Sister, false);
        dialogManager.NewDialog(GetDialog("Sister_Start"));
        yield return new WaitUntil(dialogManager.IsDialogFinished);
        taskManager.CompleteActiveTask();
    }

    int numSeedsPlanted = 0;
    void DropoffSeed()
    {
        print("Made it to dropoffseed");
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
        dialogManager.NewDialog(GetDialog("Father_Start"));
        yield return new WaitUntil(dialogManager.IsDialogFinished);
        taskManager.CompleteActiveTask();
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
        taskManager.SetActiveTask(TaskType.Grandmother, false);
        dialogManager.NewDialog(GetDialog("Grandmother_Start"));
        yield return new WaitUntil(dialogManager.IsDialogFinished);
        taskManager.CompleteActiveTask();
    }

    // ████████████████████████████ GENERAL ███████████████████████████████████

    IEnumerator AllTasksProcess()
    {
        yield return new WaitForSeconds(1f);
        dialogManager.NewDialog(GetDialog("DayOver"));
        yield return new WaitUntil(dialogManager.IsDialogFinished);
        // StartCoroutine(SendNPCsHome());
        taskManager.AddAndSetActive(TaskType.DayEnd, "Go home.", false);
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

    Day8DialogContent day8DialogContent;

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
        day8DialogContent = GetComponent<Day8DialogContent>();

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
            if (day8DialogContent.content.ContainsKey(startingKey))
            {
                dialogs[character] = GetDialog(startingKey);
            }
        }
    }

    Dialog GetDialog(string dialogTag)
    {
        return day8DialogContent.content[dialogTag];
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
