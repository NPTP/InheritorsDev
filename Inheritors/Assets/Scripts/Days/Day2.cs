/* INHERITORS by Nick Perrin (c) 2020 */
using System;
using System.Linq;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DG.Tweening;
using Cinemachine;
using UnityEngine.SceneManagement;

public class Day2 : MonoBehaviour
{
    public bool enableDayScripts = true;
    int dayNumber = 2;
    Dictionary<string, Trigger> triggers = new Dictionary<string, Trigger>();
    Dictionary<string, AreaTrigger> areas = new Dictionary<string, AreaTrigger>();

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
        // Fade in from a white screen.
        stateManager.SetState(State.Inert);
        transitionManager.SetAlpha(1f);
        transitionManager.SetColor(Color.white);
        transitionManager.Hide(1f);
        yield return new WaitForSeconds(1f);

        stateManager.SetState(State.Normal);

        // Cue the opening dialog.
        dialogManager.NewDialog(dialogs["Day2Opening_1"], State.Inert);
        yield return new WaitUntil(dialogManager.IsDialogFinished);
        uiManager.SetUpTasksInventory();

        // Show the tasks, only cam send on the new one.
        cameraManager.SendCamTo(sisterQuadrantTransform);
        yield return new WaitWhile(cameraManager.IsSwitching);
        taskManager.AddTask(TaskType.Sister, "Gather papayas for sister.");
        yield return new WaitForSeconds(1f);
        cameraManager.QuadrantCamActivate(motherQuadrantTransform);
        yield return new WaitWhile(cameraManager.IsSwitching);
        taskManager.AddTask(TaskType.MotherWood, "Fetch 3 logs for firewood.");
        yield return new WaitForSeconds(1f);
        taskManager.AddTask(TaskType.Father, "See father for next hunting lesson.");
        yield return new WaitForSeconds(1f);

        // Final dialog of opening.
        dialogManager.NewDialog(dialogs["Day2Opening_2"]);
        yield return new WaitUntil(dialogManager.IsDialogFinished);
        triggers["Dialog_MotherNoTask"].Enable();

        yield return new WaitForSeconds(1f);
    }

    // ████████████████████████████████████████████████████████████████████████
    // ███ EVENT HANDLERS
    // ████████████████████████████████████████████████████████████████████████

    // Currently unused.
    void HandleAreaEvent(object sender, InteractManager.AreaArgs args)
    {
        string tag = args.tag;
        bool inside = args.inside;

        switch (tag)
        {
            default:
                Debug.Log("Interact Manager gave unknown AREA tag to Day " + dayNumber);
                break;
        }
    }

    void HandlePickupEvent(object sender, InteractManager.PickupArgs args)
    {
        PickupManager.Inventory inventory = args.inventory;
        switch (inventory.itemType)
        {
            case ItemType.Pig:
                PickupPig();
                break;

            case ItemType.Wood:
                PickupWood(inventory.itemQuantity);
                break;

            case ItemType.Papaya:
                PickupPapaya(inventory.itemQuantity);
                break;

            case ItemType.Null:
                Debug.Log("NULL pickup event");
                break;

            default:
                Debug.Log("Interact Manager gave unknown PICKUP tag to Day " + dayNumber);
                break;
        }
    }

    void HandleDialogEvent(object sender, InteractManager.DialogArgs args)
    {
        string tag = args.tag;

        // Day dialog takes priority over the one stored in the trigger.
        if (dialogs.ContainsKey(tag))
            dialogManager.NewDialog(dialogs[tag]);
        else
            dialogManager.NewDialog(args.dialog);

        switch (tag)
        {
            case "Dialog_HuntBegin":
                StartCoroutine(HuntBegin());
                break;

            case "Dialog_SisterStart":
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
                taskManager.CompleteActiveTask();
                break;

            case "Dropoff_Meat":
                DropoffMeat();
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
        SetTaskState(args.tasks);
    }

    void SetTaskState(Dictionary<TaskType, Task> taskList)
    {
        print("Got task state set");
        Dictionary<TaskType, Task> t = taskList;

        // On starting ANY task:
        triggers["Dialog_MotherNoTask"].Disable();

        // Mother
        // --------------------------------------------------------------------
        if (t[TaskType.MotherWood].status == TaskStatus.Active)
        {
            triggers["Dialog_DoingWood"].Enable();
        }
        else if (t[TaskType.MotherWood].status == TaskStatus.Completed)
        {
            triggers["Dialog_DoingWood"].Disable();
        }

        // Father
        // --------------------------------------------------------------------
        if (t[TaskType.Father].status == TaskStatus.Active)
        {

        }
        else if (t[TaskType.Father].status == TaskStatus.Completed)
        {
            triggers["Dialog_HuntOver"].Enable();
        }

        // Sister
        // --------------------------------------------------------------------
        if (t[TaskType.Sister].status == TaskStatus.Active)
        {
            triggers["Dialog_SisterStart"].Disable();
            triggers["Dialog_SisterActive"].Enable();
        }
        else if (t[TaskType.Sister].status == TaskStatus.Completed)
        {
            triggers["Dialog_SisterActive"].Disable();
            triggers["Dialog_SisterComplete"].Enable();
        }
    }

    IEnumerator WaitDialogEnd()
    {
        yield return new WaitUntil(dialogManager.IsDialogFinished);
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
        taskManager.ChangeTask(TaskType.Father, "Kill the agoutis with the bow.");

        // PIG KILLING MINIGAME GOES ON HERE
        // stateManager.SetState(State.Hunting); ???
        yield return new WaitForSeconds(2f);
        // END PIG KILLING

        Destroy(GameObject.Find("Agoutis").GetComponent<Animator>());
        recordManager.StopRecording();
        dialogManager.NewDialog(dialogs["Dialog_HuntEnd"]);
        yield return new WaitUntil(dialogManager.IsDialogFinished);
        pickupManager.LoseTaskTool();

        taskManager.CompleteActiveTask();
    }

    void PickupPig()
    {
        taskManager.ChangeTask(TaskType.Father, "Bring the meat to mother's fire.");
        triggers["Dropoff_Meat"].Enable();
        triggers["Dialog_TaskExplanation"].Disable();
        triggers["Dialog_HavePig"].Enable();
        recordManager.StartNewRecording();
    }

    void DropoffMeat()
    {
        taskManager.CompleteActiveTask();
        triggers["Dialog_HavePig"].Disable();
    }

    void PickupWood(int itemQuantity)
    {
        if (itemQuantity == 1)
        {
            taskManager.SetActiveTask(TaskType.MotherWood);
            taskManager.ChangeTask(TaskType.MotherWood, "Collect 2 more logs.");
        }
        else if (itemQuantity == 2)
        {
            taskManager.ChangeTask(TaskType.MotherWood, "Collect 1 more log.");
        }
        else if (itemQuantity == 3)
        {
            taskManager.ChangeTask(TaskType.MotherWood, "Bring logs back to fire pit.");
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
        dialogManager.NewDialog(dialogs["DayOver"]);
        yield return new WaitUntil(dialogManager.IsDialogFinished);
        StartCoroutine(SendNPCsHome());
        taskManager.AddAndSetActive(TaskType.DayEnd, "Head inside the maloca for siesta.", false);
        triggers["Walk_End"].Enable();
    }

    IEnumerator SendNPCsHome()
    {
        yield return null;
        // TODO: direct NPCs to walk inside their malocas if they have'em and disappear.
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

    // ████████████████████████████████████████████████████████████████████████
    // ███ DIALOGS OF THE DAY
    // ████████████████████████████████████████████████████████████████████████

    Dictionary<string, Dialog> dialogs = new Dictionary<string, Dialog>();

    void InitializeDialogs()
    {
        dialogs.Add("Day2Opening_1", new Dialog
        {
            name = "Mother",
            // skippable = false,
            lines = new string[] {
                "It’s so nice to see you running around making your own path here.",
                "I couldn’t have imagined it. When I was young, the Kanoe and the Akuntsu would never...",
                "Well, maybe that’s a story for when you’re older.",
                "Here’s what I need you to do today."
            }
        });

        dialogs.Add("Day2Opening_2", new Dialog
        {
            name = "Mother",
            lines = new string[] {
                "You already know where to find your father, and the firewood.",
                "But this will be your first time helping your sister! Ask her what she needs. Off you go now."
            }
        });

        dialogs.Add("Dialog_MotherNoTask", new Dialog
        {
            name = "Mother",
            lines = new string[] {
                "Ohh... There are so few of us now, but we are together. That's what counts.",
                "Son, what are you hanging around for? You still have work to do, off you go now!"
            }
        });

        dialogs.Add("Dialog_DoingWood", new Dialog
        {
            name = "Mother",
            lines = new string[] {
                "Once you have all the wood, just drop it onto the firepit."
            }
        });

        dialogs.Add("Dialog_HuntBegin", new Dialog
        {
            name = "Father",
            lines = new string[] {
                "Hey! Son! You did good yesterday. Now, today, we’ve got a faster and smaller target than a pig: the agoutis.",
                 "Try to hit one of their group; you’ll have to lead your target. That’ll <i>really</i> teach you how to use a bow."
            }
        });

        dialogs.Add("Dialog_HuntEnd", new Dialog
        {
            name = "Father",
            lines = new string[] {
                "Nice shot!",
                "Don’t worry about bringing any meat home today. One agoutis is too small, so dad is going to catch a few more first.",
                "Hey, me and your grandfather were hoping to get the bridge back up over the river tomorrow.",
                "When we do, you should go across and say hi to him. It’s been a while."
            }
        });

        dialogs.Add("Dialog_HuntOver", new Dialog
        {
            name = "Father",
            lines = new string[] {
                "Tomorrow we'll have the bridge put up over the river again. Your grandfather's been waiting to see you."
            }
        });

        dialogs.Add("Dialog_SisterStart", new Dialog
        {
            name = "Sister",
            lines = new string[] {
                "I heard that on the other side of the river, there’s a tree bigger and older than any other.",
                "Grandmother said she climbed it as a child, as her own grandmother did before her.",
                "I've grown a lot of plants, but nothing like that. If we get to cross the river soon, let's mark our names on it!",
                "I wonder how deep its roots go...",
                "But first! Let’s get the <color=blue>papayas</color> we need. 6 in total - one for each of the family.",
                "Gather some from my garden, but it may not be enough. I think I saw some more growing at the <color=green>south entrance</color> to this forest.",
                "Come back when you have <color=blue>6 papayas</color>, and drop them in my bucket. I’m sure I could eat all 6 myself…"
            }
        });

        dialogs.Add("Dialog_SisterActive", new Dialog
        {
            name = "Sister",
            lines = new string[] {
                "Have you found all the papayas yet? We need 6!",
                "There are 3 in my garden, and 3 near the <color=green>south entrance</color> to this forest."
            }
        });

        dialogs.Add("Dialog_SisterComplete", new Dialog
        {
            name = "Sister",
            lines = new string[] {
                "Thanks little bro! That's all we need!",
                "Maybe you're not so lazy after all. Come back again tomorrow, and we'll grow something new."
            }
        });

        dialogs.Add("DayOver", new Dialog
        {
            name = "Mother",
            lines = new string[] {
                "Thank you, son. That's everything for today!",
                "You've been hard at work, it's time for a siesta. Come on inside."
            }
        });

        dialogs.Add("Dialog_Sister", new Dialog
        {
            name = "Sister",
            lines = new string[] {
                "Oh hey little bro. Mom and dad got you working hard today?",
                "I mean, <b>finally!</b> Someone else should do some of the work around here...",
                "Come around tomorrow, I may need your help with something."
            }
        });
    }

    // ████████████████████████████████████████████████████████████████████████
    // ███ INITIALIZERS & DESTROYERS
    // ████████████████████████████████████████████████████████████████████████

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

    void InitializeTriggers()
    {
        IEnumerable<Trigger> worldTriggers = FindObjectsOfType<MonoBehaviour>().OfType<Trigger>();
        foreach (Trigger trigger in worldTriggers)
        {
            triggers[trigger.GetTag()] = trigger;
        }
    }

    void InitializeAreas()
    {
        AreaTrigger[] worldAreas = FindObjectsOfType<AreaTrigger>();
        foreach (AreaTrigger areaTrigger in worldAreas)
        {
            areas[areaTrigger.GetTag()] = areaTrigger;
        }
    }

    void SubscribeToEvents()
    {
        interactManager.OnArea += HandleAreaEvent;
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
            interactManager.OnArea -= HandleAreaEvent;
            interactManager.OnPickup -= HandlePickupEvent;
            interactManager.OnDropoff -= HandleDropoffEvent;
            interactManager.OnDialog -= HandleDialogEvent;
            interactManager.OnWalk -= HandleWalkEvent;

            taskManager.OnUpdateTasks -= HandleUpdateTasks;
            taskManager.OnAllTasks -= HandleAllTasksComplete;
        }
    }


}
