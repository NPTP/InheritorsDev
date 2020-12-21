﻿/* INHERITORS by Nick Perrin (c) 2020 */
using System;
using System.Linq;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DG.Tweening;

[RequireComponent(typeof(Day5DialogContent))]
[RequireComponent(typeof(DialogTaskState))]
public class Day5 : MonoBehaviour
{
    int dayNumber = 5;
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
    public GameObject manofholeNPC;
    public GameObject papayaTreeFire;
    public Transform urukuLookTarget;
    [Space]
    public AudioClip[] mateteSounds;
    public Animator grandfatherAnimator;

    bool spokeToManofhole = false;
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
        manofholeNPC.SetActive(false);

        // Fade in from BLACK.
        stateManager.SetState(State.Inert);
        transitionManager.SetAlpha(1f);
        transitionManager.SetColor(Color.black);
        yield return new WaitForSeconds(.5f);
        transitionManager.Hide(3f);
        yield return new WaitForSeconds(2f);

        // Set up facings.
        PlayerMovement pm = FindObjectOfType<PlayerMovement>();
        GameObject mother = GameObject.FindWithTag("MotherNPC");
        GameObject father = GameObject.FindWithTag("FatherNPC");
        LookAtPlayer motherLook = mother.GetComponent<LookAtPlayer>();
        LookAtPlayer fatherLook = father.GetComponent<LookAtPlayer>();

        // Cue the opening dialog.
        pm.LookAtTarget(father.transform);
        motherLook.ChangeTarget(father.transform);
        dialogManager.NewDialog(dialogContent.Get("Day5Opening_1"), State.Inert);
        yield return new WaitUntil(dialogManager.IsDialogFinished);
        uiManager.SetUpTasksInventory();
        yield return new WaitForSeconds(1f);

        // Show the tasks, only cam send on the new one.
        taskManager.AddTask(TaskType.Sister, "Help sister.");
        yield return new WaitForSeconds(1f);
        taskManager.AddTask(TaskType.Grandfather, "Talk to grandfather.");
        yield return new WaitForSeconds(1f);

        // Final dialog of opening.
        pm.LookAtTarget(mother.transform);
        motherLook.ResetTarget();
        fatherLook.ChangeTarget(mother.transform);
        dialogManager.NewDialog(dialogContent.Get("Day5Opening_2"));
        yield return new WaitUntil(dialogManager.IsDialogFinished);
        fatherLook.ResetTarget();
        dialogTriggers[Character.Mother].Enable();

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
            case ItemType.Wood:
                PickupWood(inventory.itemQuantity);
                break;

            case ItemType.Uruku:
                StartCoroutine(PickupUruku());
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
            // case Character.Father:
            //     if (activeTask.type != TaskType.Null)
            //         return;

            //     if (taskList[TaskType.Father].status == TaskStatus.Waiting)
            //         StartCoroutine(HuntBegin());
            //     break;

            case Character.Sister:
                if (taskList[TaskType.Sister].status == TaskStatus.Waiting && activeTask.type == TaskType.Null)
                    StartCoroutine(SisterStart());
                else if (taskList[TaskType.Sister].status == TaskStatus.Active && spokeToManofhole)
                    StartCoroutine(SisterFinish());
                break;

            case Character.Manofhole:
                if (!spokeToManofhole)
                    StartCoroutine(SisterManofholeTalk());
                break;

            case Character.Grandfather:
                if (activeTask.type != TaskType.Null)
                    return;

                if (taskList[TaskType.Grandfather].status == TaskStatus.Waiting)
                    StartCoroutine(GrandfatherStart());
                break;

            // case Character.Grandmother:
            //     if (activeTask.type != TaskType.Null)
            //         return;

            //     if (taskList[TaskType.Grandmother].status == TaskStatus.Waiting)
            //         StartCoroutine(GrandmotherStart());
            //     break;

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
                StartCoroutine(DropoffWood());
                break;

            case "Dropoff_Meat":
                taskManager.CompleteActiveTask();
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
        taskManager.ChangeTask(TaskType.Grandfather, "Pick up the matété.");
        triggers["Pickup_Flute"].Enable();
    }

    IEnumerator PickupFlute()
    {
        stateManager.SetState(State.Inert);
        taskManager.ChangeTask(TaskType.Grandfather, "Play with grandfather.");
        PlayerMovement pm = FindObjectOfType<PlayerMovement>();
        pm.LookAtTarget(GameObject.FindWithTag("GrandfatherNPC").transform);
        FindObjectOfType<PlayerSpecialAnimations>().PlayFluteAnimation();
        yield return new WaitForSeconds(.5f);
        pm.Halt();

        float delay = 1f;

        dialogManager.NewDialog(dialogContent.Get("Grandfather_StartTask"), State.Inert);
        yield return new WaitUntil(dialogManager.IsDialogFinished);

        grandfatherAnimator.SetBool("Playing", true);
        yield return new WaitForSeconds(delay);

        audioManager.Play(mateteSounds[0]);
        yield return new WaitForSeconds(mateteSounds[0].length);
        yield return new WaitForSeconds(delay);

        grandfatherAnimator.SetBool("Playing", false);

        dialogManager.NewDialog(dialogContent.Get("Grandfather_ContinueTask"), State.Inert);
        yield return new WaitUntil(dialogManager.IsDialogFinished);
        yield return new WaitForSeconds(delay);

        audioManager.Play(mateteSounds[1]);
        yield return new WaitForSeconds(mateteSounds[1].length);
        yield return new WaitForSeconds(delay);

        recordManager.StopRecording();
        dialogManager.NewDialog(dialogContent.Get("Grandfather_FinishTask"));
        yield return new WaitUntil(dialogManager.IsDialogFinished);

        pickupManager.LoseItems();
        FindObjectOfType<PlayerSpecialAnimations>().StopFluteAnimation();
        taskManager.CompleteActiveTask();
        stateManager.SetState(State.Normal);
    }

    // ████████████████████████████ SISTER ████████████████████████████████████

    IEnumerator SisterStart()
    {
        yield return new WaitUntil(dialogManager.IsDialogFinished);

        taskManager.SetActiveTask(TaskType.Sister);
        taskManager.ChangeTask(TaskType.Sister, "Pick 3 papayas.");
        dialogTriggers[Character.Manofhole].Enable();
        dialogs[Character.Manofhole] = dialogContent.Get("Manofhole_Start");
        manofholeNPC.SetActive(true);
    }

    IEnumerator SisterManofholeTalk()
    {
        spokeToManofhole = true;
        yield return new WaitUntil(dialogManager.IsDialogFinished);
        dialogs[Character.Manofhole] = dialogContent.Get("Manofhole_Completed");

        taskManager.ChangeTask(TaskType.Sister, "Go back to sister.");
        dialogs[Character.Sister] = dialogContent.Get("Sister_FinishTask");
    }

    IEnumerator SisterFinish()
    {
        recordManager.StopRecording();
        yield return new WaitUntil(dialogManager.IsDialogFinished);
        taskManager.CompleteActiveTask();
        manofholeNPC.SetActive(false);
        dialogTriggers[Character.Manofhole].Remove();
        papayaTreeFire.SetActive(false);
    }

    // ████████████████████████████ FATHER ████████████████████████████████████

    IEnumerator HuntBegin()
    {
        yield return new WaitUntil(dialogManager.IsDialogFinished);
        taskManager.SetActiveTask(TaskType.Father);
        taskManager.ChangeTask(TaskType.Father, "Catch the fish.");

        // PIG KILLING MINIGAME GOES ON HERE
        yield return new WaitForSeconds(1.25f);
        // END PIG KILLING

        recordManager.StopRecording();
        Destroy(GameObject.Find("Tapir").GetComponent<Animator>());
        dialogManager.NewDialog(dialogContent.Get("Father_HuntEnd"));
        yield return new WaitUntil(dialogManager.IsDialogFinished);

        pickupManager.LoseTaskTool();
        taskManager.CompleteActiveTask();
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

    IEnumerator DropoffWood()
    {
        recordManager.StopRecording();
        dialogManager.NewDialog(dialogContent.Get("Mother_FinishTask"));
        yield return new WaitUntil(dialogManager.IsDialogFinished);
        taskManager.CompleteActiveTask();
    }

    // ██████████████████████████ GRANDMOTHER █████████████████████████████████

    IEnumerator GrandmotherStart()
    {
        yield return new WaitUntil(dialogManager.IsDialogFinished);

        taskManager.SetActiveTask(TaskType.Grandmother);
        taskManager.ChangeTask(TaskType.Grandmother, "Grind and apply uruku dye.");
        triggers["Pickup_Uruku"].Enable();
    }

    IEnumerator PickupUruku()
    {
        stateManager.SetState(State.Inert);

        GameObject player = GameObject.FindWithTag("Player");
        player.GetComponent<PlayerMovement>().LookAtTarget(urukuLookTarget);
        player.GetComponent<Animator>().SetTrigger("Pickup");

        GameObject.Find("UrukuExplosion").GetComponent<ParticleSystem>().Play();
        yield return new WaitForSeconds(.25f);
        Renderer headRenderer = GameObject.Find("C_man_1_FBX2013").GetComponent<Renderer>();
        var newMaterials = headRenderer.materials;
        newMaterials[1] = redheadMaterial;
        headRenderer.materials = newMaterials;

        yield return new WaitForSeconds(2f);
        pickupManager.LoseItems();
        recordManager.StopRecording();

        dialogManager.NewDialog(dialogContent.Get("Grandmother_FinishTask"), State.Normal);
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
        dialogTriggers[Character.Father].Remove();
        Destroy(GameObject.FindWithTag("FatherNPC"));

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
