using System;
using System.Linq;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DG.Tweening;
using Cinemachine;
using UnityEngine.SceneManagement;

public class Day1 : MonoBehaviour
{
    public bool enableDayScripts = true;
    int dayNumber = 1;
    Dictionary<string, Trigger> triggers = new Dictionary<string, Trigger>();
    Dictionary<string, AreaTrigger> areas = new Dictionary<string, AreaTrigger>();

    /* -------------------------------------- */
    /* Day-specific objects, transforms, etc. */
    /* -------------------------------------- */

    [Header("Day-specific Objects")]
    public Transform motherQuadrantTransform;
    public Transform firewoodTransform;
    public Transform wateringHoleTransform;
    public Transform fatherHuntingTransform; //

    /* -------------------------------------- */
    /* -------------------------------------- */

    void Awake()
    {
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
    // ███ SCRIPTED HAPPENINGS
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

        // Zoom up and queue the opening dialog, leave inert after dialog.
        dialogManager.NewDialog(day1Opening, State.Inert);
        yield return new WaitUntil(dialogManager.IsDialogFinished);
        uiManager.SetUpTasksInventory();
        yield return new WaitForSeconds(.5f);

        // Give us context for watering hole task.
        cameraManager.SendCamTo(wateringHoleTransform);
        yield return new WaitWhile(cameraManager.IsSwitching);
        taskManager.AddTask("Water", "Fetch water for mother.");
        yield return new WaitForSeconds(1f);

        // Give us context for hunting task.
        cameraManager.SendCamTo(fatherHuntingTransform);
        yield return new WaitWhile(cameraManager.IsSwitching);
        taskManager.AddTask("Father", "Meet father to hunt.");
        yield return new WaitForSeconds(1f);

        // Return to player to debrief before letting them loose.
        cameraManager.QuadrantCamActivate(motherQuadrantTransform);
        yield return new WaitWhile(cameraManager.IsSwitching);
        dialogManager.NewDialog(taskExplanation);
        yield return new WaitUntil(dialogManager.IsDialogFinished);

        // Player is now loose, and can repeat the task dialog with mother.
        triggers["Dialog_TaskExplanation"].Enable(); // TODO: make dialog triggers snatch their dialog early on so we don't have to specify it in here.
    }

    IEnumerator End()
    {
        stateManager.SetState(State.Inert);
        uiManager.TearDownTasksInventory();
        Tween t = transitionManager.Show(2f);
        audioManager.FadeTo(0f, 2f, Ease.InOutQuad);
        yield return new WaitWhile(() => t != null & t.IsPlaying());

        saveManager.SaveGame(dayNumber + 1);
        Helper.LoadScene("MainMenu");
    }

    void WoodPickups(PickupManager.Inventory inventory)
    {
        if (inventory.itemQuantity == 1)
        {
            // TODO: this can probably all be one function
            areas["Firewood"].BeginTaskInArea();
            TurnOffAreas();
            taskManager.SetActiveTask("Firewood"); // TODO: enum the tasks since they'll be repeating and so we don't fuck up name matchings. Make day 0 & day 10 tasks unique
        }
        else if (inventory.itemQuantity == 3)
        {
            triggers["Dropoff_Firewood"].Enable();
        }
    }

    // ████████████████████████████████████████████████████████████████████████
    // ███ EVENT HANDLERS
    // ████████████████████████████████████████████████████████████████████████

    // TODO: a better way to handle enabling areas (set them up beforehand?)
    void HandleAreaEvent(object sender, InteractManager.AreaArgs args)
    {
        string tag = args.tag;
        bool inside = args.inside;

        switch (tag)
        {
            case "Firewood":
                SetAreaTriggers(tag, inside);
                break;

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
            case PickupManager.ItemTypes.WOOD:
                WoodPickups(inventory);
                break;

            case PickupManager.ItemTypes.NULL:
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
        Dialog dialog = args.dialog;

        switch (tag)
        {
            case "Dialog_TaskExplanation":
                dialogManager.NewDialog(taskExplanation);
                break;

            case "Dialog_FatherStart":
                dialogManager.NewDialog(fatherStart);
                break;

            default:
                Debug.Log("Interact Manager gave unknown DIALOG tag to Day " + dayNumber);
                dialogManager.NewDialog(dialog);
                StartCoroutine(WaitDialogEnd());
                break;
        }
    }

    void HandleDropoffEvent(object sender, InteractManager.DropoffArgs args)
    {
        string tag = args.tag;

        switch (tag)
        {
            case "Dropoff_Firewood":
                taskManager.CompleteActiveTask(); // TODO: this doesn't work. Get it working even sans animation, at least
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
            case "NULL":
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

    void SetAreaTriggers(string tag, bool insideArea)
    {
        switch (tag)
        {
            case "Firewood":
                if (insideArea)
                {
                    triggers["Pickup_Wood1"].Enable();
                    triggers["Pickup_Wood2"].Enable();
                    triggers["Pickup_Wood3"].Enable();
                }
                else
                {
                    triggers["Pickup_Wood1"].Disable();
                    triggers["Pickup_Wood2"].Disable();
                    triggers["Pickup_Wood3"].Disable();
                }
                break;

            default:
                break;
        }
    }

    // 
    void TurnOffAreas()
    {
        areas["Firewood"].Disable();
        // More... make a list?
    }

    void TurnOnAreas()
    {
        areas["Firewood"].Enable();
        // More... make a list?
    }

    // ████████████████████████████████████████████████████████████████████████
    // ███ DIALOGS OF THE DAY
    // ████████████████████████████████████████████████████████████████████████


    Dialog day1Opening = new Dialog();
    Dialog taskExplanation = new Dialog();
    Dialog fatherStart = new Dialog();
    void InitializeDialogs()
    {
        day1Opening.name = "Mother";
        day1Opening.skippable = false;
        day1Opening.lines = new string[] {
            "Good morning, son.\nIt's a beautiful day.",
            "You see the imprints and trails you left behind last night? This land has memory.",
            "Every step has meaning, and deepens our connection to this place, and to our past.",
            "Our ancestors laid paths for us, and you will tread your own!",
            "If you walk a path over and over again, the grass bends under your feet, the forest learns who you are...",
            "And it will keep a memory of you.",
            "Just something to keep in mind as you pursue each task. Here is the list for today."
        };

        taskExplanation.name = "Mother";
        taskExplanation.lines = new string[] {
            "Go to a task’s area to get started. You will have everything you need for that task when you get there!",
            "Once you begin a task, you must complete it before you can begin another.",
            "Begin the tasks in any order you like.",
            "And if you need to take a break, don't worry. Your progress is saved at the start of each day.",
            "Talk to me again if you forget any of that. Off you go now, son!"
        };

        fatherStart.name = "Father";
        fatherStart.lines = new string[] {
            "My son! Good to see you finally come to the hunt. I’m glad you’re with us now.",
            "We did not have enough men to hunt before. The women tried their hand at it, but only out of necessity.",
            "They had not grown up with it, and caught little food. But you, you’ll make a great hunter one day - just like your father!",
            "Let’s kill the wild pig. Take the bow."
        };
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
    }

    void InitializeTriggers()
    {
        IEnumerable<Trigger> worldTriggers = FindObjectsOfType<MonoBehaviour>().OfType<Trigger>();
        foreach (Trigger trigger in worldTriggers)
        {
            triggers[trigger.GetTag()] = trigger;
        }
    }

    // TODO: find triggers in bounds, make lists, profit?
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
        }
    }


}
