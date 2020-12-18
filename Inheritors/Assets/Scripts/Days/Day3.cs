/* INHERITORS by Nick Perrin (c) 2020 */
using System;
using System.Linq;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DG.Tweening;

[RequireComponent(typeof(DialogContent))]
[RequireComponent(typeof(DialogTaskState))]
public class Day3 : MonoBehaviour
{
    int dayNumber = 3;
    public bool enableDayScripts = true;

    Dictionary<string, Trigger> triggers = new Dictionary<string, Trigger>();
    Dictionary<TaskType, AreaTrigger> areas = new Dictionary<TaskType, AreaTrigger>();

    Task activeTask;
    Dictionary<TaskType, Task> taskList;

    /* -------------------------------------- */
    /* Day-specific objects, transforms, etc. */
    /* -------------------------------------- */
    [Header("Day-specific Objects")]
    public Transform motherQuadrant;
    public Transform grandfatherQuadrant;
    public Transform grandmotherQuadrant;
    [Space]
    public AudioClip[] mateteSounds;
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
        // Fade in from a white screen.
        stateManager.SetState(State.Inert);
        transitionManager.SetAlpha(1f);
        transitionManager.SetColor(Color.white);
        transitionManager.Hide(1f);
        yield return new WaitForSeconds(1f);

        stateManager.SetState(State.Normal);

        // // Cue the opening dialog.
        dialogManager.NewDialog(dialogContent.Get("Day3Opening_1"), State.Inert);
        yield return new WaitUntil(dialogManager.IsDialogFinished);
        uiManager.SetUpTasksInventory();
        yield return new WaitForSeconds(1f);

        // // Show the tasks, only cam send on the new one.
        taskManager.AddTask(TaskType.Mother, "Fetch water.");
        yield return new WaitForSeconds(1f);
        taskManager.AddTask(TaskType.Father, "Hunt with father.");
        yield return new WaitForSeconds(1f);
        taskManager.AddTask(TaskType.Sister, "Talk to sister.");
        yield return new WaitForSeconds(1f);

        cameraManager.SendCamTo(grandmotherQuadrant);
        yield return new WaitWhile(cameraManager.IsSwitching);
        taskManager.AddTask(TaskType.Grandmother, "Visit grandmother.");
        yield return new WaitForSeconds(2f);

        cameraManager.SendCamTo(grandfatherQuadrant);
        yield return new WaitWhile(cameraManager.IsSwitching);
        taskManager.AddTask(TaskType.Grandfather, "Visit grandfather.");
        yield return new WaitForSeconds(2f);

        cameraManager.QuadrantCamActivate(motherQuadrant);
        yield return new WaitWhile(cameraManager.IsSwitching);

        // Final dialog of opening.
        dialogManager.NewDialog(dialogContent.Get("Day3Opening_2"));
        yield return new WaitUntil(dialogManager.IsDialogFinished);
        dialogTriggers[Character.Mother].Enable();

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

            case ItemType.Corn:
                PickupCorn(inventory.itemQuantity);
                break;

            case ItemType.Agoutis: // Tapir, actually
                PickupTapir();
                break;

            case ItemType.Yopo:
                PickupYopo(inventory.itemQuantity);
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

        if (activeTask.type != TaskType.Null)
            return;

        switch (character)
        {
            case Character.Father:
                if (taskList[TaskType.Father].status == TaskStatus.Waiting)
                    StartCoroutine(HuntBegin());
                break;

            case Character.Sister:
                if (taskList[TaskType.Sister].status == TaskStatus.Waiting)
                    StartCoroutine(SisterStart());
                break;

            case Character.Grandfather:
                if (taskList[TaskType.Grandfather].status == TaskStatus.Waiting)
                    StartCoroutine(GrandfatherStart());
                break;

            case Character.Grandmother:
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

            case "Dropoff_Corn":
                StartCoroutine(DropoffCorn());
                break;

            case "Dropoff_Meat":
                taskManager.CompleteActiveTask();
                break;

            case "Dropoff_Water":
                DropoffWater();
                break;

            case "Dropoff_Yopo":
                StartCoroutine(DropoffYopo());
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

    // ████████████████████████████████████████████████████████████████████████
    // ███ HAPPENINGS OF THE DAY
    // ████████████████████████████████████████████████████████████████████████

    // ██████████████████████████ GRANDFATHER █████████████████████████████████

    IEnumerator GrandfatherStart()
    {
        yield return new WaitUntil(dialogManager.IsDialogFinished);

        taskManager.SetActiveTask(TaskType.Grandfather);
        taskManager.ChangeTask(TaskType.Grandfather, "Pick up the matété flute.");
        triggers["Pickup_Flute"].Enable();
    }

    IEnumerator PickupFlute()
    {
        stateManager.SetState(State.Inert);
        taskManager.ChangeTask(TaskType.Grandfather, "Play with grandfather.");
        FindObjectOfType<PlayerMovement>().LookAtTarget(GameObject.FindWithTag("GrandfatherNPC").transform);
        yield return new WaitForSeconds(.5f);

        float delay = 1f;

        dialogManager.NewDialog(dialogContent.Get("Grandfather_StartTask"), State.Inert);
        yield return new WaitUntil(dialogManager.IsDialogFinished);
        yield return new WaitForSeconds(delay);

        audioManager.Play(mateteSounds[0]);
        yield return new WaitForSeconds(mateteSounds[0].length);
        yield return new WaitForSeconds(delay);

        dialogManager.NewDialog(dialogContent.Get("Grandfather_ContinueTask"), State.Inert);
        yield return new WaitUntil(dialogManager.IsDialogFinished);
        yield return new WaitForSeconds(delay);

        audioManager.Play(mateteSounds[1]);
        yield return new WaitForSeconds(mateteSounds[1].length);
        yield return new WaitForSeconds(delay);

        recordManager.StopRecording();
        dialogManager.NewDialog(dialogContent.Get("Grandfather_FinishTask"));
        yield return new WaitUntil(dialogManager.IsDialogFinished);

        stateManager.SetState(State.Normal);
        pickupManager.LoseItems();
        taskManager.CompleteActiveTask();
    }

    // ████████████████████████████ SISTER ████████████████████████████████████

    IEnumerator SisterStart()
    {
        yield return new WaitUntil(dialogManager.IsDialogFinished);

        taskManager.SetActiveTask(TaskType.Sister);
        taskManager.ChangeTask(TaskType.Sister, "Gather 4 ears of corn.");
        EnableChildTriggers("CornPickups");
    }

    void PickupCorn(int itemQuantity)
    {
        if (itemQuantity > 0 && itemQuantity < 3)
        {
            taskManager.ChangeTask(TaskType.Sister, "Gather " + (4 - itemQuantity).ToString() + " more ears of corn.");
        }
        else if (itemQuantity == 3)
        {
            taskManager.ChangeTask(TaskType.Sister, "Gather 1 more ear of corn.");
        }
        else if (itemQuantity == 4)
        {
            taskManager.ChangeTask(TaskType.Sister, "Bring corn back to sister.");
            triggers["Dropoff_Corn"].Enable();
        }
    }

    IEnumerator DropoffCorn()
    {
        recordManager.StopRecording();
        dialogManager.NewDialog(dialogContent.Get("Sister_FinishTask"));
        yield return new WaitUntil(dialogManager.IsDialogAnimationFinished);
        taskManager.CompleteActiveTask();
    }

    // ████████████████████████████ FATHER ████████████████████████████████████

    IEnumerator HuntBegin()
    {
        yield return new WaitUntil(dialogManager.IsDialogFinished);
        taskManager.SetActiveTask(TaskType.Father, false);
        taskManager.ChangeTask(TaskType.Father, "Kill the tapir.");

        // PIG KILLING MINIGAME GOES ON HERE
        yield return new WaitForSeconds(1.25f);
        // END PIG KILLING

        Destroy(GameObject.Find("Tapir").GetComponent<Animator>());
        dialogManager.NewDialog(dialogContent.Get("Father_HuntEnd"));
        yield return new WaitUntil(dialogManager.IsDialogFinished);

        pickupManager.LoseTaskTool();
        triggers["Pickup_Tapir"].Enable();
        triggers["Dropoff_Meat"].Enable();
    }

    void PickupTapir()
    {
        recordManager.StartNewRecording();
        taskManager.ChangeTask(TaskType.Father, "Bring the meat home.");
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
        taskManager.ChangeTask(TaskType.Grandmother, "Gather 3 yopo beans.");
        EnableChildTriggers("YopoPickups");
    }

    void PickupYopo(int itemQuantity)
    {
        if (itemQuantity == 1)
        {
            taskManager.ChangeTask(TaskType.Grandmother, "Gather 2 more yopo beans.");
        }
        else if (itemQuantity == 2)
        {
            taskManager.ChangeTask(TaskType.Grandmother, "Gather 1 more yopo bean.");
        }
        else if (itemQuantity == 3)
        {
            taskManager.ChangeTask(TaskType.Grandmother, "Bring beans to grandmother.");
            triggers["Dropoff_Yopo"].Enable();
        }
    }

    IEnumerator DropoffYopo()
    {
        recordManager.StopRecording();
        dialogManager.NewDialog(dialogContent.Get("Grandmother_FinishTask"));
        yield return new WaitUntil(dialogManager.IsDialogAnimationFinished);
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
        transitionManager.SetColor(Color.black);
        Tween t = transitionManager.Show(3f);
        audioManager.FadeOtherSources("Down", 3f); // audioManager.FadeTo(0f, 2f, Ease.InOutQuad);
        yield return t.WaitForCompletion();

        saveManager.SaveGame(dayNumber);
        Helper.LoadScene("Loading");
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
