/* INHERITORS by Nick Perrin (c) 2020 */
using System;
using System.Linq;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DG.Tweening;

[RequireComponent(typeof(Day2DialogContent))]
public class Day2 : MonoBehaviour
{
    public bool enableDayScripts = true;
    int dayNumber = 2;
    Dictionary<string, Trigger> triggers = new Dictionary<string, Trigger>();
    Dictionary<Character, DialogTrigger> dialogTriggers = new Dictionary<Character, DialogTrigger>();
    Dictionary<TaskType, AreaTrigger> areas = new Dictionary<TaskType, AreaTrigger>();
    Task activeTask;
    Dictionary<TaskType, Task> taskList;

    /* -------------------------------------- */
    /* Day-specific objects, transforms, etc. */
    /* -------------------------------------- */
    [Header("Day-specific Objects")]
    public Transform motherQuadrantTransform;
    public Transform sisterQuadrantTransform;

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

        // Cue the opening dialog.
        dialogManager.NewDialog(GetDialog("Day2Opening_1"), State.Inert);
        yield return new WaitUntil(dialogManager.IsDialogFinished);
        uiManager.SetUpTasksInventory();
        yield return new WaitForSeconds(1f);

        // Show the tasks, only cam send on the new one.
        taskManager.AddTask(TaskType.Mother, "Fetch firewood.");
        yield return new WaitForSeconds(1f);
        taskManager.AddTask(TaskType.Father, "Hunt with father.");
        yield return new WaitForSeconds(1f);
        cameraManager.SendCamTo(sisterQuadrantTransform);
        yield return new WaitWhile(cameraManager.IsSwitching);
        taskManager.AddTask(TaskType.Sister, "Talk to sister.");
        yield return new WaitForSeconds(1f);
        cameraManager.QuadrantCamActivate(motherQuadrantTransform);
        yield return new WaitWhile(cameraManager.IsSwitching);

        // Final dialog of opening.
        dialogManager.NewDialog(GetDialog("Day2Opening_2"));
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
            case ItemType.Wood:
                PickupWood(inventory.itemQuantity);
                break;

            case ItemType.Papaya:
                PickupPapaya(inventory.itemQuantity);
                break;

            case ItemType.Null:
                Debug.Log("NULL pickup event: HandlePickupEvent in Day2");
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

        print(activeTask.type);

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

            case "Dropoff_Papaya":
                print("Completed papaya");
                taskManager.CompleteActiveTask();
                print(activeTask.type);
                break;

            case "Dropoff_Meat":
                taskManager.CompleteActiveTask();
                break;

            case "Dropoff_Water":
                DropoffWater();
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

    IEnumerator SisterStart()
    {
        yield return new WaitUntil(dialogManager.IsDialogFinished);

        taskManager.SetActiveTask(TaskType.Sister);
        taskManager.ChangeTask(TaskType.Sister, "Gather 6 papayas.");
        Transform papayaPickups = GameObject.Find("PapayaPickups").transform;
        foreach (Transform child in papayaPickups)
        {
            child.GetComponent<Trigger>().Enable();
        }
    }

    IEnumerator HuntBegin()
    {
        yield return new WaitUntil(dialogManager.IsDialogFinished);
        taskManager.SetActiveTask(TaskType.Father);
        taskManager.ChangeTask(TaskType.Father, "Catch fish in the net.");

        // PIG KILLING MINIGAME GOES ON HERE
        // stateManager.SetState(State.Hunting); ???
        yield return new WaitForSeconds(2f);
        // END PIG KILLING

        Destroy(GameObject.Find("Agoutis").GetComponent<Animator>());
        recordManager.StopRecording();
        dialogManager.NewDialog(GetDialog("Father_HuntEnd"));
        yield return new WaitUntil(dialogManager.IsDialogFinished);
        pickupManager.LoseTaskTool();

        taskManager.CompleteActiveTask();
    }

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

    void PickupPapaya(int itemQuantity)
    {
        if (itemQuantity > 0 && itemQuantity < 5)
        {
            taskManager.ChangeTask(TaskType.Sister, "Gather " + (6 - itemQuantity).ToString() + " more papayas.");
        }
        else if (itemQuantity == 5)
        {
            taskManager.ChangeTask(TaskType.Sister, "Gather 1 more papaya.");
        }
        else if (itemQuantity == 6)
        {
            taskManager.ChangeTask(TaskType.Sister, "Bring papayas back to sister.");
            triggers["Dropoff_Papaya"].Enable();
        }
    }

    void DropoffWater()
    {
        pickupManager.LoseTaskTool();
        taskManager.CompleteActiveTask();
        triggers["Dialog_HaveWater"].Disable();
        triggers["Dialog_HuntBegin"].Disable();
    }

    IEnumerator AllTasksProcess()
    {
        yield return new WaitForSeconds(1f);
        dialogManager.NewDialog(GetDialog("DayOver"));
        yield return new WaitUntil(dialogManager.IsDialogFinished);
        StartCoroutine(SendNPCsHome());
        taskManager.AddAndSetActive(TaskType.DayEnd, "Return home.", false);
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

    Day2DialogContent day2DialogContent;

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
        day2DialogContent = GetComponent<Day2DialogContent>();

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

    void InitializeDialogs()
    {
        foreach (Character character in Enum.GetValues(typeof(Character)))
        {
            string startingKey = character.ToString() + "_Start";
            if (day2DialogContent.content.ContainsKey(startingKey))
                dialogs[character] = GetDialog(startingKey);
        }
    }

    Dialog GetDialog(string dialogTag)
    {
        return day2DialogContent.content[dialogTag];
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
