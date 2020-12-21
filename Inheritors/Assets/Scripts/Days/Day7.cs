/* INHERITORS by Nick Perrin (c) 2020 */
using System;
using System.Linq;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DG.Tweening;
using Cinemachine;

[RequireComponent(typeof(DialogContent))]
[RequireComponent(typeof(DialogTaskState))]
public class Day7 : MonoBehaviour
{
    int dayNumber = 7;
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
    public Material redheadMaterial;
    public GameObject seedsPickup;
    public AudioClip urukuSound;
    public AudioSource ambientSound;
    // public Transform motherQuadrant;
    // public Transform grandfatherQuadrant;
    // public Transform grandmotherQuadrant;
    [Space]
    public GameObject grandfatherDialogTrigger1;
    public GameObject grandfatherChar1;
    public GameObject grandfatherDialogTrigger2;
    public GameObject grandfatherChar2;
    [Space]
    public CinemachineVirtualCamera day7StartCam;
    public CinemachineVirtualCamera day7HammockCam;
    public AudioClip hammockStinger;
    [Space]
    public Transform fatherLeavesLookTarget;
    // public GameObject bigTree;
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
        // Set start cam.
        day7StartCam.Priority = 100;

        // Change player's hair to red
        // Renderer headRenderer = GameObject.Find("C_man_1_FBX2013").GetComponent<Renderer>();
        // var newMaterials = headRenderer.materials;
        // newMaterials[1] = redheadMaterial;
        // headRenderer.materials = newMaterials;

        // Make grandma sit.
        GameObject.FindWithTag("GrandmotherNPC").GetComponent<Animator>().SetBool("Sitting", true);

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

        // Hold the view for a second as music plays.
        yield return new WaitForSeconds(2f);

        // Cue the opening dialog.
        dialogManager.NewDialog(dialogContent.Get("Day7Opening_1"), State.Inert);
        yield return new WaitUntil(dialogManager.IsDialogFinished);
        uiManager.SetUpTasksInventory();

        // Switch to normal cam
        day7StartCam.Priority = 0;
        yield return new WaitForSeconds(2f);

        // Show the tasks, only cam send on the new one.
        taskManager.AddTask(TaskType.Father, "Hunt with father.");
        yield return new WaitForSeconds(1f);

        cameraManager.SendCamTo(GameObject.FindWithTag("SisterNPC").transform);
        yield return new WaitWhile(cameraManager.IsSwitching);
        taskManager.AddTask(TaskType.Sister, "Help sister.");
        yield return new WaitForSeconds(1f);

        cameraManager.QuadrantCamActivate(motherQuadrant);
        yield return new WaitWhile(cameraManager.IsSwitching);
        taskManager.AddTask(TaskType.Grandmother, "See grandmother.");
        yield return new WaitForSeconds(1f);
        taskManager.AddTask(TaskType.Grandfather, "See grandfather.");
        yield return new WaitForSeconds(1f);

        // Final dialog of opening.
        dialogManager.NewDialog(dialogContent.Get("Day7Opening_2"));
        yield return new WaitUntil(dialogManager.IsDialogFinished);
        dialogTriggers[Character.Mother].Enable();

        stateManager.SetState(State.Normal);
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
            // case ItemType.Wood:
            //     PickupWood(inventory.itemQuantity);
            //     break;

            // case ItemType.Herbs:
            //     PickupHerbs();
            //     break;

            case ItemType.Uruku:
                StartCoroutine(PickupUruku());
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
                if (activeTask.type != TaskType.Null)
                    return;

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
            // case "Dropoff_Wood":
            //     taskManager.CompleteActiveTask();
            //     break;

            // case "Dropoff_Herbs":
            //     StartCoroutine(DropoffHerbs());
            //     break;

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
        taskManager.ChangeTask(TaskType.Grandfather, "Rest in the hammock.");
        triggers["Walk_Hammock"].Enable();
    }

    IEnumerator WalkHammock()
    {
        stateManager.SetState(State.Inert);
        recordManager.StopRecording();
        transitionManager.SetColor(Color.black);
        transitionManager.Show(1f);

        float savedVolume = ambientSound.volume;
        ambientSound.DOFade(0f, 2f);

        uiManager.TearDownTasksInventory();

        yield return new WaitForSeconds(2f);
        grandfatherDialogTrigger1.SetActive(false);
        grandfatherChar1.SetActive(false);

        day7HammockCam.Priority = 100;

        yield return transitionManager.Hide(2f).WaitForCompletion();
        float stingerVolume = 0.15f;
        audioManager.PlayOneShot(hammockStinger, stingerVolume);
        yield return new WaitForSeconds(4f);
        yield return transitionManager.Show(2f).WaitForCompletion();

        day7HammockCam.Priority = 0;
        yield return new WaitForSeconds(2f);
        grandfatherDialogTrigger2.SetActive(true);
        grandfatherChar2.SetActive(true);
        dialogTriggers[Character.Grandfather] = grandfatherDialogTrigger2.GetComponent<DialogTrigger>();
        dialogTriggers[Character.Grandfather].Enable();
        dialogTriggers[Character.Grandfather].DisableTriggerProjector();
        GameObject.FindWithTag("Player").GetComponent<PlayerMovement>().LookAtTarget(grandfatherChar2.transform);

        transitionManager.Hide(2f);
        ambientSound.DOFade(savedVolume, 2f);

        uiManager.SetUpTasksInventory();

        yield return new WaitForSeconds(1.5f);
        dialogManager.NewDialog(dialogContent.Get("Grandfather_FinishTask"));
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
        taskManager.SetActiveTask(TaskType.Father, false);
        dialogTriggers[Character.Father].Remove();
        yield return new WaitUntil(dialogManager.IsDialogFinished);
        stateManager.SetState(State.Inert);
        pickupManager.LoseTaskTool();
        FindObjectOfType<PlayerMovement>().LookAtTarget(fatherLeavesLookTarget);

        Animation animation = GameObject.Find("FatherLeaves").GetComponent<Animation>();
        GameObject father = GameObject.FindWithTag("FatherNPC");
        father.GetComponent<Animator>().SetBool("Walking", true);
        animation.Play();
        yield return new WaitForSeconds(animation.clip.length - 1.5f);
        Destroy(father);

        taskManager.CompleteActiveTask();
        stateManager.SetState(State.Normal);
    }

    // ████████████████████████████ MOTHER ████████████████████████████████████

    // void PickupWood(int itemQuantity)
    // {
    //     if (itemQuantity == 1)
    //     {
    //         taskManager.ChangeTask(TaskType.Mother, "Collect 2 more logs.");
    //         taskManager.SetActiveTask(TaskType.Mother);
    //     }
    //     else if (itemQuantity == 2)
    //     {
    //         taskManager.ChangeTask(TaskType.Mother, "Collect 1 more log.");
    //     }
    //     else if (itemQuantity == 3)
    //     {
    //         taskManager.ChangeTask(TaskType.Mother, "Bring logs to firepit.");
    //         triggers["Dropoff_Wood"].Enable();
    //     }
    // }

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

        audioManager.PlayOneShot(urukuSound);
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
