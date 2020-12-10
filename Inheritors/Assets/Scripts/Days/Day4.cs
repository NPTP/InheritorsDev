﻿/* INHERITORS by Nick Perrin (c) 2020 */
using System;
using System.Linq;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DG.Tweening;

[RequireComponent(typeof(Day4DialogContent))]
public class Day4 : MonoBehaviour
{
    int dayNumber = 4;

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
    public Transform motherQuadrant;
    public Transform hillPathTransform;
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
        // Fade in from BLACK.
        stateManager.SetState(State.Inert);
        transitionManager.SetAlpha(1f);
        transitionManager.SetColor(Color.black);
        transitionManager.Hide(3f);
        yield return new WaitForSeconds(3.5f);

        cameraManager.SendCamTo(hillPathTransform);
        uiManager.SetUpTasksInventory();
        yield return new WaitWhile(cameraManager.IsSwitching);
        taskManager.AddAndSetActive(TaskType.Grandmother, "Go to the hilltop.", false);
        yield return new WaitForSeconds(2f);

        cameraManager.QuadrantCamActivate(motherQuadrant);
        yield return new WaitWhile(cameraManager.IsSwitching);

        // Activate all necessary triggers HERE

        stateManager.SetState(State.Normal);
    }

    // ████████████████████████████████████████████████████████████████████████
    // ███ EVENT HANDLERS
    // ████████████████████████████████████████████████████████████████████████

    bool haveReed = false;
    bool done = false;

    void HandlePickupEvent(object sender, InteractManager.PickupArgs args)
    {
        PickupManager.Inventory inventory = args.inventory;
        switch (inventory.itemType)
        {
            case ItemType.Reed:
                haveReed = true;
                taskManager.ChangeTask(TaskType.Grandmother, "Bring reed to grandmother.");
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

        switch (character)
        {
            case Character.Grandmother:
                if (done)
                {
                    dialogManager.NewDialog(dialogs[Character.Grandmother]);
                }
                else if (!haveReed)
                {
                    StartCoroutine(GetReed());
                }
                else if (haveReed)
                {
                    haveReed = false;
                    StartCoroutine(Festival());
                }
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
        // Unused for day 4
    }

    IEnumerator WaitDialogEnd()
    {
        yield return new WaitUntil(dialogManager.IsDialogFinished);
    }

    // ████████████████████████████████████████████████████████████████████████
    // ███ HAPPENINGS OF THE DAY
    // ████████████████████████████████████████████████████████████████████████

    IEnumerator GetReed()
    {
        dialogManager.NewDialog(GetDialog("Grandmother_Start"));
        yield return new WaitUntil(dialogManager.IsDialogFinished);

        taskManager.ChangeTask(TaskType.Grandmother, "Retrieve the reed.");
        triggers["Pickup_Reed"].Enable();
    }

    IEnumerator Festival()
    {
        dialogTriggers[Character.Grandmother].Disable();
        dialogManager.NewDialog(GetDialog("Grandmother_Festival1"), State.Inert);
        yield return new WaitUntil(dialogManager.IsDialogFinished);

        uiManager.TearDownTasksInventory();
        yield return new WaitForSeconds(1.5f);

        ShockwaveProjector sp = FindObjectOfType<ShockwaveProjector>();
        sp.Shockwave();
        yield return new WaitUntil(sp.ShockwaveFinished);

        dialogManager.NewDialog(GetDialog("Grandmother_Festival2"), State.Inert);
        yield return new WaitUntil(dialogManager.IsDialogFinished);
        interactManager.DialogExitRange(dialogTriggers[Character.Grandmother]);
        dialogTriggers[Character.Grandmother].Disable();
        yield return new WaitForSeconds(1f);

        float transitionTime = .5f;
        transitionManager.ChangeColor(Color.white, 0f);
        Tween t = transitionManager.Show(transitionTime);
        yield return t.WaitForCompletion();
        recordManager.PlayRecordings();
        pickupManager.LoseItems();
        transitionManager.Hide(transitionTime);
        yield return new WaitForSeconds(1);

        dialogManager.NewDialog(GetDialog("Grandmother_Festival3"));
        yield return new WaitUntil(dialogManager.IsDialogFinished);

        uiManager.SetUpTasksInventory();
        taskManager.CompleteActiveTask();
        taskManager.AddAndSetActive(TaskType.DayEnd, "Return home.", false);
        dialogTriggers[Character.Grandmother].Enable();
        dialogs[Character.Grandmother] = GetDialog("Grandmother_Completed");
        triggers["Walk_End"].Enable();
        done = true;
    }

    // ████████████████████████████ GENERAL ███████████████████████████████████

    IEnumerator End()
    {
        stateManager.SetState(State.Inert);
        uiManager.TearDownTasksInventory();
        transitionManager.ChangeColor(Color.black, 0f);
        Tween t = transitionManager.Show(4f);
        audioManager.FadeOtherSources("Down", 2f); // audioManager.FadeTo(0f, 2f, Ease.InOutQuad);
        yield return t.WaitForCompletion();

        saveManager.SaveGame(dayNumber);
        Helper.LoadScene("Loading");
    }

    void SetTaskState(Task activeTask, Dictionary<TaskType, Task> taskList)
    {
        this.activeTask = activeTask;
        this.taskList = taskList;

        // Unused for Day 4
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

    Day4DialogContent day4DialogContent;

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
        day4DialogContent = GetComponent<Day4DialogContent>();

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
            if (day4DialogContent.content.ContainsKey(startingKey))
            {
                dialogs[character] = GetDialog(startingKey);
            }
        }
    }

    Dialog GetDialog(string dialogTag)
    {
        return day4DialogContent.content[dialogTag];
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