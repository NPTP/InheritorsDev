/* INHERITORS by Nick Perrin (c) 2020 */
using System;
using System.Linq;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DG.Tweening;

[RequireComponent(typeof(DialogContent))]
[RequireComponent(typeof(DialogTaskState))]
public class Day9 : MonoBehaviour
{
    int dayNumber = 9;
    public bool enableDayScripts = true;

    Dictionary<string, Trigger> triggers = new Dictionary<string, Trigger>();
    Dictionary<TaskType, AreaTrigger> areas = new Dictionary<TaskType, AreaTrigger>();

    Task activeTask;
    Dictionary<TaskType, Task> taskList;

    /* -------------------------------------- */
    /* Day-specific objects, transforms, etc. */
    /* -------------------------------------- */
    [Header("Day-specific Objects")]
    public Material redheadMaterial;
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
        dialogManager.NewDialog(dialogContent.Get("Day9Opening_1"), State.Inert);
        yield return new WaitUntil(dialogManager.IsDialogFinished);
        uiManager.SetUpTasksInventory();
        yield return new WaitForSeconds(1f);

        // Show the tasks, only cam send on the new one.
        taskManager.AddTask(TaskType.Father, "Find father.");
        yield return new WaitForSeconds(1f);
        taskManager.AddTask(TaskType.Sister, "Bring sister back.");
        yield return new WaitForSeconds(1f);
        taskManager.AddTask(TaskType.Grandfather, "Talk to grandfather.");
        yield return new WaitForSeconds(1f);

        // Final dialog of opening.
        dialogManager.NewDialog(dialogContent.Get("Day9Opening_2"));
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
            case ItemType.Flute:
                PickupFlute();
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
            default:
                break;
        }
    }

    void HandleDropoffEvent(object sender, InteractManager.DropoffArgs args)
    {
        string tag = args.tag;

        switch (tag)
        {
            case "Dropoff_Flute":
                StartCoroutine(DropoffFlute());
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
            case "Walk_Grandfather":
                StartCoroutine(GrandfatherStart());
                break;

            case "Walk_Father":
                StartCoroutine(HuntBegin());
                break;

            case "Walk_Sister":
                StartCoroutine(SisterStart());
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

    // ████████████████████████████████████████████████████████████████████████
    // ███ HAPPENINGS OF THE DAY
    // ████████████████████████████████████████████████████████████████████████

    // ██████████████████████████ GRANDFATHER █████████████████████████████████

    IEnumerator GrandfatherStart()
    {
        taskManager.SetActiveTask(TaskType.Grandfather, false);
        dialogManager.NewDialog(dialogContent.Get("Grandfather_Start"));
        yield return new WaitUntil(dialogManager.IsDialogFinished);
        taskManager.ChangeTask(TaskType.Grandfather, "Get grandfather's matété.");
        triggers["Pickup_Flute"].Enable();
    }

    void PickupFlute()
    {
        taskManager.ChangeTask(TaskType.Grandfather, "Bring matété to grandfather.");
        triggers["Dropoff_Flute"].Enable();
    }

    IEnumerator DropoffFlute()
    {
        recordManager.StopRecording();
        dialogManager.NewDialog(dialogContent.Get("Grandfather_FinishTask"));
        yield return new WaitUntil(dialogManager.IsDialogFinished);

        yield return new WaitForSeconds(1f);
        taskManager.CompleteActiveTask();
    }

    // ████████████████████████████ SISTER ████████████████████████████████████

    IEnumerator SisterStart()
    {
        taskManager.SetActiveTask(TaskType.Sister, false);
        dialogManager.NewDialog(dialogContent.Get("Sister_Start"));
        yield return new WaitUntil(dialogManager.IsDialogFinished);
        taskManager.CompleteActiveTask();
    }

    // ████████████████████████████ FATHER ████████████████████████████████████

    IEnumerator HuntBegin()
    {
        taskManager.SetActiveTask(TaskType.Father, false);
        dialogManager.NewDialog(dialogContent.Get("Father_Start"));
        yield return new WaitUntil(dialogManager.IsDialogFinished);
        taskManager.CompleteActiveTask();
    }

    // ████████████████████████████ MOTHER ████████████████████████████████████


    // ██████████████████████████ GRANDMOTHER █████████████████████████████████


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
        transitionManager.SetColor(Color.black);
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
            dialogs[Character.Mother] = dialogContent.Get("Mother_Active");
            dialogTriggers[Character.Mother].Enable();
        }
        else if (taskList[TaskType.Mother].status == TaskStatus.Completed)
        {
            dialogs[Character.Mother] = dialogContent.Get("Mother_Completed");
        }

        // Father
        // --------------------------------------------------------------------
        if (taskList[TaskType.Father].status == TaskStatus.Active)
        {
            // Nothing: handled in script
        }
        else if (taskList[TaskType.Father].status == TaskStatus.Completed)
        {
            dialogs[Character.Father] = dialogContent.Get("Father_Completed");
        }

        // Sister
        // --------------------------------------------------------------------
        if (taskList[TaskType.Sister].status == TaskStatus.Active)
        {
            dialogs[Character.Sister] = dialogContent.Get("Sister_Active");
        }
        else if (taskList[TaskType.Sister].status == TaskStatus.Completed)
        {
            dialogs[Character.Sister] = dialogContent.Get("Sister_Completed");
        }

        // Grandmother
        // --------------------------------------------------------------------
        if (taskList[TaskType.Grandmother].status == TaskStatus.Active)
        {
            dialogs[Character.Grandmother] = dialogContent.Get("Grandmother_Active");
        }
        else if (taskList[TaskType.Grandmother].status == TaskStatus.Completed)
        {
            dialogs[Character.Grandmother] = dialogContent.Get("Grandmother_Completed");
        }

        // Grandfather
        // --------------------------------------------------------------------
        if (taskList[TaskType.Grandfather].status == TaskStatus.Active)
        {
            dialogs[Character.Grandfather] = dialogContent.Get("Grandfather_Active");
        }
        else if (taskList[TaskType.Grandfather].status == TaskStatus.Completed)
        {
            dialogs[Character.Grandfather] = dialogContent.Get("Grandfather_Completed");
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

    // ALL types of triggers
    void InitializeTriggers()
    {
        IEnumerable<Trigger> worldTriggers = FindObjectsOfType<MonoBehaviour>().OfType<Trigger>();
        foreach (Trigger trigger in worldTriggers)
        {
            triggers[trigger.GetTag()] = trigger;
        }
    }

    Dictionary<Character, Dialog> dialogs = new Dictionary<Character, Dialog>();
    Dictionary<Character, DialogTrigger> dialogTriggers = new Dictionary<Character, DialogTrigger>();

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
                dialogs[character] = dialogContent.Get(startingKey);
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
