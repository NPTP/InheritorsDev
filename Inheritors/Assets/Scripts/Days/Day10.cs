﻿/* INHERITORS by Nick Perrin (c) 2020 */
using System;
using System.Linq;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DG.Tweening;

[RequireComponent(typeof(Day10DialogContent))]
public class Day10 : MonoBehaviour
{
    int dayNumber = 10;

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
    public GameObject ManofholeWateringHole;
    public GameObject ManofholeSister;
    public GameObject ManofholeFather;
    public GameObject ManofholeGrandmother;
    public GameObject ManofholeGrandfather;

    [Space]
    public GameObject hilltopBlockage;
    public GameObject hilltopUnblocked;
    public GameObject endingClosedBorders;
    public GameObject endingOpenBorders;

    TaskType lastRemainingTask;
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

    // ████████████████████████████████████████████████████████████████████████
    // ███ INTRO
    // ████████████████████████████████████████████████████████████████████████

    IEnumerator Intro()
    {
        yield return null;
        ManofholeWateringHole.SetActive(false);
        ManofholeSister.SetActive(false);
        ManofholeFather.SetActive(false);
        ManofholeGrandmother.SetActive(false);
        ManofholeGrandfather.SetActive(false);

        hilltopUnblocked.SetActive(false);
        endingOpenBorders.SetActive(false);

        // Fade in from BLACK.
        stateManager.SetState(State.Inert);
        transitionManager.SetAlpha(1f);
        transitionManager.SetColor(Color.red);
        yield return new WaitForSeconds(.5f);
        transitionManager.Hide(3f);
        yield return new WaitForSeconds(2f);

        // Cue the opening dialog.
        dialogManager.NewDialog(GetDialog("Day10Opening_1"), State.Inert);
        yield return new WaitUntil(dialogManager.IsDialogFinished);
        uiManager.SetUpTasksInventory();
        yield return new WaitForSeconds(1f);

        // Show the tasks, only cam send on the new one.
        taskManager.AddTask(TaskType.Mother, "Check the water pond.");
        yield return new WaitForSeconds(1f);
        taskManager.AddTask(TaskType.Father, "Check hunting ground.");
        yield return new WaitForSeconds(1f);
        taskManager.AddTask(TaskType.Sister, "Check sister's.");
        yield return new WaitForSeconds(1f);
        taskManager.AddTask(TaskType.Grandmother, "Check grandmother's.");
        yield return new WaitForSeconds(1f);
        taskManager.AddTask(TaskType.Grandfather, "Check grandfather's.");
        yield return new WaitForSeconds(1f);

        // Final dialog of opening.
        dialogManager.NewDialog(GetDialog("Day10Opening_2"));
        yield return new WaitUntil(dialogManager.IsDialogFinished);

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
            case Character.Manofhole:
                if (activeTask.type == TaskType.Null)
                    StartCoroutine(ManofholeStart());
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
            case "Walk_WateringHole":
                StartCoroutine(CheckedLocation(TaskType.Mother, "Mother"));
                break;

            case "Walk_Father":
                StartCoroutine(CheckedLocation(TaskType.Father, "Father"));
                break;

            case "Walk_Sister":
                StartCoroutine(CheckedLocation(TaskType.Sister, "Sister"));
                break;

            case "Walk_Grandmother":
                StartCoroutine(CheckedLocation(TaskType.Grandmother, "Grandmother"));
                break;

            case "Walk_Grandfather":
                StartCoroutine(CheckedLocation(TaskType.Grandfather, "Grandfather"));
                break;

            case "Walk_Hilltop":
                StartCoroutine(EndingStart());
                break;

            case "Walk_End":
                StartCoroutine(LeaveForest());
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
        // Unused on Day 10
    }

    IEnumerator WaitDialogEnd()
    {
        yield return new WaitUntil(dialogManager.IsDialogFinished);
    }

    // ████████████████████████████████████████████████████████████████████████
    // ███ HAPPENINGS OF THE DAY
    // ████████████████████████████████████████████████████████████████████████

    int numLocationsChecked = 0;

    // ████████████████████████████ LOCATION CHECKS ███████████████████████████

    IEnumerator CheckedLocation(TaskType taskType, string name)
    {
        taskManager.SetActiveTask(taskType, false);
        stateManager.SetState(State.Inert);
        FindObjectOfType<PlayerMovement>().Halt();
        yield return new WaitForSeconds(1);

        dialogManager.NewDialog(GetDialog(name));
        yield return new WaitUntil(dialogManager.IsDialogFinished);
        taskManager.CompleteActiveTask();

        numLocationsChecked++;
        if (numLocationsChecked == 4)
        {
            // Spawn the man of the hole at the last remaining location.
            SetUpManofhole();
        }
    }

    void SetUpManofhole()
    {

        WalkTrigger[] wt = FindObjectsOfType<WalkTrigger>();
        foreach (WalkTrigger walkTrigger in wt)
        {
            lastRemainingTask = walkTrigger.gameObject.GetComponent<TaskTypeAddOn>().taskType;
            if (walkTrigger != null && lastRemainingTask != TaskType.DayEnd)
            {
                switch (lastRemainingTask)
                {
                    case TaskType.Mother:
                        ManofholeWateringHole.SetActive(true);
                        break;

                    case TaskType.Father:
                        ManofholeFather.SetActive(true);
                        break;

                    case TaskType.Sister:
                        ManofholeSister.SetActive(true);
                        break;

                    case TaskType.Grandmother:
                        ManofholeGrandmother.SetActive(true);
                        break;

                    case TaskType.Grandfather:
                        ManofholeGrandfather.SetActive(true);
                        break;
                }
                walkTrigger.Remove();
                break;
            }
        }
    }

    IEnumerator ManofholeStart()
    {
        taskManager.SetActiveTask(lastRemainingTask, false);
        taskManager.ChangeTask(lastRemainingTask, "Talk to the strange man.");
        yield return new WaitUntil(dialogManager.IsDialogFinished);
        taskManager.CompleteActiveTask();
        dialogs[Character.Manofhole] = GetDialog("Manofhole_Repeat");

        taskManager.AddAndSetActive(TaskType.DayEnd, "Find mother on the hilltop.", false);
        hilltopBlockage.SetActive(false);
        hilltopUnblocked.SetActive(true);
    }

    // ████████████████████████████ ENDING ████████████████████████████████████

    IEnumerator EndingStart()
    {
        taskManager.CompleteActiveTask();
        dialogManager.NewDialog(GetDialog("Mother_Start"));
        yield return new WaitUntil(dialogManager.IsDialogFinished);

        Animation animation = GameObject.Find("MotherLeaves").GetComponent<Animation>();
        animation.Play();
        yield return new WaitWhile(() => animation.isPlaying);

        taskManager.AddAndSetActive(TaskType.DayEnd, "Leave the forest.", false);

        endingClosedBorders.SetActive(false);
        endingOpenBorders.SetActive(true);
        triggers["Walk_End"].Enable();
    }

    IEnumerator LeaveForest()
    {
        stateManager.SetState(State.Inert);
        uiManager.TearDownTasksInventory();
        transitionManager.SetColor(Color.white);
        Tween t = transitionManager.Show(4f);
        audioManager.FadeOtherSources("Down", 4f);
        yield return t.WaitForCompletion();

        // saveManager.SaveGame(dayNumber);
        Helper.LoadScene("EndingText");
    }

    void SetTaskState(Task activeTask, Dictionary<TaskType, Task> taskList)
    {
        this.activeTask = activeTask;
        this.taskList = taskList;

        // Mother
        // --------------------------------------------------------------------
        if (taskList[TaskType.Mother].status == TaskStatus.Active)
        {
            dialogs[Character.Mother] = GetDialog("Mother_Active");
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

    Day10DialogContent day10DialogContent;

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
        day10DialogContent = GetComponent<Day10DialogContent>();

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
            if (day10DialogContent.content.ContainsKey(startingKey))
            {
                dialogs[character] = GetDialog(startingKey);
            }
        }
    }

    Dialog GetDialog(string dialogTag)
    {
        return day10DialogContent.content[dialogTag];
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