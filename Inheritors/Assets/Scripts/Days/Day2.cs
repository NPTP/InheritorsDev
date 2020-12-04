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
        InitializeDialogContent();
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
        // Fade in from a white screen.
        stateManager.SetState(State.Inert);
        transitionManager.SetAlpha(1f);
        transitionManager.SetColor(Color.white);
        transitionManager.Hide(1f);
        yield return new WaitForSeconds(1f);

        stateManager.SetState(State.Normal);

        // Cue the opening dialog.
        dialogManager.NewDialog(dialogContent["Day2Opening_1"], State.Inert);
        yield return new WaitUntil(dialogManager.IsDialogFinished);
        uiManager.SetUpTasksInventory();

        // Show the tasks, only cam send on the new one.
        cameraManager.SendCamTo(sisterQuadrantTransform);
        yield return new WaitWhile(cameraManager.IsSwitching);
        taskManager.AddTask(TaskType.Sister, "Gather papayas for sister.");
        yield return new WaitForSeconds(1f);
        cameraManager.QuadrantCamActivate(motherQuadrantTransform);
        yield return new WaitWhile(cameraManager.IsSwitching);
        taskManager.AddTask(TaskType.Mother, "Fetch 3 logs for firewood.");
        yield return new WaitForSeconds(1f);
        taskManager.AddTask(TaskType.Father, "See father for next hunting lesson.");
        yield return new WaitForSeconds(1f);

        // Final dialog of opening.
        dialogManager.NewDialog(dialogContent["Day2Opening_2"]);
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
        taskManager.ChangeTask(TaskType.Father, "Kill the agoutis with the bow.");

        // PIG KILLING MINIGAME GOES ON HERE
        // stateManager.SetState(State.Hunting); ???
        yield return new WaitForSeconds(2f);
        // END PIG KILLING

        Destroy(GameObject.Find("Agoutis").GetComponent<Animator>());
        recordManager.StopRecording();
        dialogManager.NewDialog(dialogContent["Father_HuntEnd"]);
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
        dialogManager.NewDialog(dialogContent["DayOver"]);
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


    void SetTaskState(Task activeTask, Dictionary<TaskType, Task> taskList)
    {
        this.activeTask = activeTask;
        this.taskList = taskList;

        // Mother
        // --------------------------------------------------------------------
        if (taskList[TaskType.Mother].status == TaskStatus.Active)
        {
            dialogs[Character.Mother] = dialogContent["Mother_Active"];
            dialogTriggers[Character.Mother].Enable();
        }
        else if (taskList[TaskType.Mother].status == TaskStatus.Completed)
        {
            dialogs[Character.Mother] = dialogContent["Mother_Completed"];
        }

        // Father
        // --------------------------------------------------------------------
        if (taskList[TaskType.Father].status == TaskStatus.Active)
        {
            // Nothing: handled in script
        }
        else if (taskList[TaskType.Father].status == TaskStatus.Completed)
        {
            dialogs[Character.Sister] = dialogContent["Father_Completed"];
        }

        // Sister
        // --------------------------------------------------------------------
        if (taskList[TaskType.Sister].status == TaskStatus.Active)
        {
            dialogs[Character.Sister] = dialogContent["Sister_Active"];
        }
        else if (taskList[TaskType.Sister].status == TaskStatus.Completed)
        {
            dialogs[Character.Sister] = dialogContent["Sister_Completed"];
        }
    }

    // ████████████████████████████████████████████████████████████████████████
    // ███ DIALOGS OF THE DAY
    // ████████████████████████████████████████████████████████████████████████

    Dictionary<Character, Dialog> dialogs = new Dictionary<Character, Dialog>();
    Dictionary<string, Dialog> dialogContent = new Dictionary<string, Dialog>();

    void InitializeDialogs()
    {
        foreach (Character character in Enum.GetValues(typeof(Character)))
        {
            string startingKey = character.ToString() + "_Start";
            if (dialogContent.ContainsKey(startingKey))
                dialogs[character] = dialogContent[startingKey];
        }
    }

    void InitializeDialogContent()
    {
        dialogContent.Add("Day2Opening_1", new Dialog
        {
            character = Character.Mother,
            // skippable = false,
            lines = new string[] {
                "It’s so nice to see you running around making your own path here.",
                "I couldn’t have imagined it. When I was young, the Kanoe and the Akuntsu would never...",
                "Well, maybe that’s a story for when you’re older.",
                "Here’s what I need you to do today."
            }
        });

        dialogContent.Add("Day2Opening_2", new Dialog
        {
            character = Character.Mother,
            lines = new string[] {
                "You already know where to find your father, and the firewood.",
                "But this will be your first time helping your sister! Ask her what she needs. Off you go now."
            }
        });

        dialogContent.Add("Mother_Start", new Dialog
        {
            character = Character.Mother,
            lines = new string[] {
                "There are so few of us now, but we are together. That is what matters.",
                "Son, you still have work to do!"
            }
        });

        dialogContent.Add("Mother_Active", new Dialog
        {
            character = Character.Mother,
            lines = new string[] {
                "Once you have all the wood, just drop it onto the firepit."
            }
        });

        dialogContent.Add("Mother_Completed", new Dialog
        {
            character = Character.Mother,
            lines = new string[] {
                "You are doing well, son. I am proud of you. Keep going!"
            }
        });

        dialogContent.Add("Father_Start", new Dialog
        {
            character = Character.Father,
            lines = new string[] {
                "Hey! Son! You did good yesterday. Now, today, we’ve got a faster and smaller target than a pig: the agoutis.",
                 "Try to hit one of their group; you’ll have to lead your target. That’ll <i>really</i> teach you how to use a bow."
            }
        });

        dialogContent.Add("Father_HuntEnd", new Dialog
        {
            character = Character.Father,
            lines = new string[] {
                "Nice shot!",
                "Don’t worry about bringing any meat home today. One agoutis is too small, so dad is going to catch a few more first.",
                "Hey, me and your grandfather were hoping to get the bridge back up over the river tomorrow.",
                "When we do, you should go across and say hi to him. It’s been a while."
            }
        });

        dialogContent.Add("Father_Completed", new Dialog
        {
            character = Character.Father,
            lines = new string[] {
                "Tomorrow we'll have the bridge put up over the river again. Your grandfather's been waiting to see you."
            }
        });

        dialogContent.Add("Sister_Start", new Dialog
        {
            character = Character.Sister,
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

        dialogContent.Add("Sister_Active", new Dialog
        {
            character = Character.Sister,
            lines = new string[] {
                "Have you found all the papayas yet? We need 6!",
                "There are 3 in my garden, and 3 near the <color=green>south entrance</color> to this forest."
            }
        });

        dialogContent.Add("Sister_Completed", new Dialog
        {
            character = Character.Sister,
            lines = new string[] {
                "Thanks little bro! That's all we need!",
                "Maybe you're not so lazy after all. Come back again tomorrow, and we'll grow something new."
            }
        });

        dialogContent.Add("DayOver", new Dialog
        {
            character = Character.Mother,
            lines = new string[] {
                "Thank you, son. That's everything for today!",
                "You've been hard at work, it's time for a siesta. Come on inside."
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
