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
        taskManager.AddTask(TaskType.Sister, "Gather 6 papaya with sister.");
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
        // triggers["Dialog_Mother"].Enable();

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

            case ItemType.Water:
                PickupWater();
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

            default:
                break;
        }
    }

    void HandleDropoffEvent(object sender, InteractManager.DropoffArgs args)
    {
        string tag = args.tag;

        switch (tag)
        {
            case "Dropoff_Firewood":
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

    IEnumerator HuntBegin()
    {
        yield return new WaitUntil(dialogManager.IsDialogFinished);
        taskManager.SetActiveTask(TaskType.Father, false);
        taskManager.ChangeTask(TaskType.Father, "Kill the pig with the bow.");

        // PIG KILLING MINIGAME GOES ON HERE
        // stateManager.SetState(State.Hunting); ???
        yield return new WaitForSeconds(2f);
        // END PIG KILLING

        Destroy(GameObject.Find("Pig").GetComponent<Animator>());
        dialogManager.NewDialog(dialogs["Dialog_HuntEnd"]);
        yield return new WaitUntil(dialogManager.IsDialogFinished);

        pickupManager.LoseTaskTool();
        taskManager.ChangeTask(TaskType.Father, "Collect the meat.");
        triggers["Pickup_Pig"].Enable();
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

    void PickupWater()
    {
        taskManager.SetActiveTask(TaskType.MotherWater);
        taskManager.ChangeTask(TaskType.MotherWater, "Bring the water to mother's pot.");
        triggers["Dropoff_Water"].Enable();
        triggers["Dialog_TaskExplanation"].Disable();
        triggers["Dialog_HaveWater"].Enable();
        triggers["Dialog_HuntBegin"].Disable();
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
            skippable = false,
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
                "But this will be your first time helping your sister! Off you go now."
            }
        });

        dialogs.Add("MotherNoTask", new Dialog
        {
            name = "Mother",
            lines = new string[] {
                "There are so few of us now, but we are together. That's what counts.",
                "You still have work to do, don't you? Off you go now!"
            }
        });

        dialogs.Add("Dialog_HavePig", new Dialog
        {
            name = "Mother",
            lines = new string[] {
                "Oh, you brought fresh pig!",
                "Just put it there over the fire. We will cook it later."
            }
        });

        dialogs.Add("Dialog_HaveWater", new Dialog
        {
            name = "Mother",
            lines = new string[] {
                "Just pour the water into the big grey pot."
            }
        });

        dialogs.Add("Dialog_HuntBegin", new Dialog
        {
            name = "Father",
            lines = new string[] {
                "Son! Good to see you're finally old enough to come to the hunt.",
                "We didn't have enough men to hunt before. The women tried their hand at it, but only out of necessity.",
                "They didn't grow up with it, so they couldn't catch anything! But you, you might make a great hunter one day.",
                "Never as good as your father though, heh heh!",
                "We're hunting wild pig. Aim the bow, use as many arrows as you need, and kill it for today's meat."
            }
        });

        dialogs.Add("Dialog_HuntEnd", new Dialog
        {
            name = "Father",
            lines = new string[] {
                "Great start, son.",
                "We're blessed to have wild animals here that we can eat. But we're relying on luck too.",
                "There is a plain through that path to the north leaving the forest. I've seen cows there.",
                "Oh I agree son, they would be nice to eat. But outside of the forest, it's not safe. So promise me one thing, boy.",
                "I will continue teaching you to hunt if you promise to never hunt outside the forest. Ever. Understand?",
                "Good. Now, take the meat and bring it home to mother. I'll see you for siesta later."
            }
        });

        dialogs.Add("Dialog_NoHunt", new Dialog
        {
            name = "Father",
            lines = new string[] {
                "Shouldn't you be taking that water back to your mother?"
            }
        });

        dialogs.Add("Dialog_HuntOver", new Dialog
        {
            name = "Father",
            lines = new string[] {
                "I will continue teaching you to hunt if you promise to never hunt outside the forest. Ever. Understand?"
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

            taskManager.OnAllTasks -= HandleAllTasksComplete;
        }
    }


}
