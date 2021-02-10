/* INHERITORS by Nick Perrin (c) 2020 */
using System;
using System.Linq;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DG.Tweening;

[RequireComponent(typeof(DialogContent))]
[RequireComponent(typeof(DialogTaskState))]
public class Day2 : MonoBehaviour
{
    int dayNumber = 2;
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
    public Transform motherQuadrantTransform;
    public Transform sisterQuadrantTransform;
    public Transform fatherNPCTransform;

    [Space]
    public Animator fatherAnimator;
    public Transform fatherLookTarget;
    public GameObject fishPrefab;
    public Transform fishPos1;
    public Transform fishPos2;
    public GameObject fishPickup;
    public AudioClip fishingSound;
    public AudioClip splashSound;
    public AudioClip pickupSound;
    public ParticleSystem waterParticles;

    GameObject fish;

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
        dialogManager.NewDialog(dialogContent.Get("Day2Opening_1"), State.Inert);
        yield return new WaitUntil(dialogManager.IsDialogFinished);
        uiManager.SetUpTasksInventory();
        yield return new WaitForSeconds(1f);

        // Show the tasks.
        cameraManager.SendCamTo(fatherNPCTransform);
        yield return new WaitWhile(cameraManager.IsSwitching);
        taskManager.AddTask(TaskType.Father, "Fishing with father.");
        yield return new WaitForSeconds(1f);
        cameraManager.SendCamTo(sisterQuadrantTransform);
        yield return new WaitWhile(cameraManager.IsSwitching);
        taskManager.AddTask(TaskType.Sister, "Talk to sister.");
        yield return new WaitForSeconds(1f);
        cameraManager.QuadrantCamActivate(motherQuadrantTransform);
        yield return new WaitWhile(cameraManager.IsSwitching);

        // Final dialog of opening.
        dialogManager.NewDialog(dialogContent.Get("Day2Opening_2"));
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
            // case ItemType.Wood:
            //     PickupWood(inventory.itemQuantity);
            //     break;

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
            // case "Dropoff_Wood":
            //     taskManager.CompleteActiveTask();
            //     break;

            case "Dropoff_Fish":
                StartCoroutine(DropoffFish());
                break;

            case "Dropoff_Papaya":
                StartCoroutine(DropoffPapaya());
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

    IEnumerator SisterStart()
    {
        yield return new WaitUntil(dialogManager.IsDialogFinished);

        taskManager.SetActiveTask(TaskType.Sister);
        taskManager.ChangeTask(TaskType.Sister, "Gather 3 papayas.");
        Transform papayaPickups = GameObject.Find("PapayaPickups").transform;
        foreach (Transform child in papayaPickups)
        {
            child.GetComponent<Trigger>().Enable();
        }
    }

    IEnumerator HuntBegin()
    {
        yield return new WaitUntil(dialogManager.IsDialogFinished);
        dialogTriggers[Character.Father].Disable();
        taskManager.SetActiveTask(TaskType.Father);
        taskManager.ChangeTask(TaskType.Father, "Watch father carefully.");
        stateManager.SetState(State.Inert);

        // Fishing animation happens here
        fatherNPCTransform.gameObject.GetComponent<LookAtPlayer>().ChangeTarget(fatherLookTarget);
        yield return new WaitForSeconds(2f);
        fatherAnimator.SetTrigger("Fishing");
        float volumeScale = 0.5f;
        audioManager.PlayOneShot(fishingSound, volumeScale);
        yield return new WaitForSeconds(3f);
        waterParticles.Play();
        fish = GameObject.Instantiate(fishPrefab, fishPos1.position, Quaternion.identity);
        audioManager.PlayOneShot(splashSound, volumeScale);

        Transform playerTransform = GameObject.FindWithTag("Player").transform;
        Sequence sequence = DOTween.Sequence();
        sequence.Append(fish.transform.DOMove(fishPos2.transform.position, .25f).SetEase(Ease.OutCubic))
                .Append(fish.transform.DOMove(playerTransform.position + 1.5f * Vector3.up, .5f).SetEase(Ease.InCubic))
                .Append(fish.transform.DOScale(0f, .25f))
                .OnComplete(CatchFish);
    }

    void CatchFish()
    {
        Destroy(fish);
        fatherNPCTransform.gameObject.GetComponent<LookAtPlayer>().ResetTarget();

        Transform playerTransform = GameObject.FindWithTag("Player").transform;
        PickupTrigger fishItem = GameObject.Instantiate(fishPickup,
                pickupManager.GetItemHoldPosition(),
                playerTransform.rotation).GetComponent<PickupTrigger>();
        fishItem.transform.SetParent(playerTransform);
        fishItem.GetPickedUp();
        pickupManager.PickUp(fishItem);
        audioManager.PlayOneShot(pickupSound, .2f);

        StartCoroutine(CatchFishProcess());
    }

    IEnumerator CatchFishProcess()
    {
        dialogManager.NewDialog(dialogContent.Get("Father_HaveFish"));
        yield return new WaitUntil(dialogManager.IsDialogFinished);

        taskManager.ChangeTask(TaskType.Father, "Put fish in the bucket.");
        dialogTriggers[Character.Father].Enable();
        triggers["Dropoff_Fish"].Enable();
    }

    IEnumerator DropoffFish()
    {
        recordManager.StopRecording();

        dialogManager.NewDialog(dialogContent.Get("Father_HuntEnd"));
        yield return new WaitUntil(dialogManager.IsDialogFinished);
        taskManager.CompleteActiveTask();
    }

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

    void PickupPapaya(int itemQuantity)
    {
        if (itemQuantity == 1)
        {
            taskManager.ChangeTask(TaskType.Sister, "Gather 2 more papayas.");
        }
        else if (itemQuantity == 2)
        {
            taskManager.ChangeTask(TaskType.Sister, "Gather 1 more papaya.");
        }
        else if (itemQuantity == 3)
        {
            taskManager.ChangeTask(TaskType.Sister, "Bring papayas back to sister.");
            triggers["Dropoff_Papaya"].Enable();
        }
    }

    IEnumerator DropoffPapaya()
    {
        recordManager.StopRecording();
        dialogManager.NewDialog(dialogContent.Get("Sister_Completed"));
        yield return new WaitUntil(dialogManager.IsDialogFinished);
        taskManager.CompleteActiveTask();
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
            if (dialogContent.ContainsKey(startingKey))
                dialogs[character] = dialogContent.Get(startingKey);
        }
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
