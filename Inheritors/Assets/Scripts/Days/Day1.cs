/* INHERITORS by Nick Perrin (c) 2020 */
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
        taskManager.AddTask(TaskType.MotherWater, "Fetch water for mother.");
        yield return new WaitForSeconds(1f);

        // Give us context for hunting task.
        cameraManager.SendCamTo(fatherHuntingTransform);
        yield return new WaitWhile(cameraManager.IsSwitching);
        taskManager.AddTask(TaskType.Father, "Meet father to hunt.");
        yield return new WaitForSeconds(1f);

        // Return to player to debrief before letting them loose.
        cameraManager.QuadrantCamActivate(motherQuadrantTransform);
        yield return new WaitWhile(cameraManager.IsSwitching);
        dialogManager.NewDialog(taskExplanation);
        yield return new WaitUntil(dialogManager.IsDialogFinished);

        // Player is now loose, and can repeat the task dialog with mother.
        triggers["Dialog_TaskExplanation"].Enable(); // TODO: make dialog triggers snatch their dialog early on so we don't have to specify it in here.

        yield return new WaitForSeconds(3f);
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
        Dialog dialog = args.dialog;

        switch (tag)
        {
            case "Dialog_TaskExplanation":
                dialogManager.NewDialog(taskExplanation);
                break;

            case "Dialog_HavePig":
                dialogManager.NewDialog(havePig);
                break;

            case "Dialog_HaveWater":
                dialogManager.NewDialog(haveWater);
                break;

            case "Dialog_HuntBegin":
                StartCoroutine(HuntBegin());
                break;

            case "Dialog_Sister":
                dialogManager.NewDialog(sister);
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
        dialogManager.NewDialog(huntBegin);
        yield return new WaitUntil(dialogManager.IsDialogFinished);

        triggers["Dialog_HuntBegin"].Disable();
        areas["Area_Father"].Remove();
        taskManager.SetActiveTask(TaskType.Father, false);
        taskManager.ChangeTask(TaskType.Father, "Kill the pig with the bow.");

        // PIG KILLING MINIGAME GOES ON HERE
        // stateManager.SetState(State.Hunting); ???
        yield return new WaitForSeconds(2f);
        // END PIG KILLING

        Destroy(GameObject.Find("Pig").GetComponent<Animator>());
        dialogManager.NewDialog(huntEnd);
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
    }

    void DropoffWater()
    {
        pickupManager.LoseTaskTool();
        taskManager.CompleteActiveTask();
        triggers["Dialog_HaveWater"].Disable();
    }

    IEnumerator AllTasksProcess()
    {
        yield return new WaitForSeconds(1f);
        dialogManager.NewDialog(dayOver);
        yield return new WaitUntil(dialogManager.IsDialogFinished);
        StartCoroutine(RemoveNPCs());
        taskManager.AddAndSetActive(TaskType.DayEnd, "Head inside the maloca for siesta.", false);
        triggers["Walk_End"].Enable();
    }

    IEnumerator RemoveNPCs()
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


    Dialog day1Opening = new Dialog();
    Dialog taskExplanation = new Dialog();
    Dialog havePig = new Dialog();
    Dialog haveWater = new Dialog();
    Dialog huntBegin = new Dialog();
    Dialog huntEnd = new Dialog();
    Dialog huntOver = new Dialog();
    Dialog dayOver = new Dialog();
    Dialog sister = new Dialog();
    Dialog noHunt = new Dialog();
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
            "Go to a task's area to get started. You will have everything you need for that task when you get there!",
            "Once you begin a task, you must complete it before you can begin another.",
            "Begin in any order you like.",
            "And if you need to take a break, don't worry. Your progress is saved at the start of each day.",
            "Talk to me again if you forget any of that. Off you go now, son!"
        };

        havePig.name = "Mother";
        havePig.lines = new string[] {
            "Oh, you brought fresh pig!",
            "Just put it there over the fire. We will cook it later."
        };

        haveWater.name = "Mother";
        haveWater.lines = new string[] {
            "Just pour the water into the big grey pot."
        };

        huntBegin.name = "Father";
        huntBegin.lines = new string[] {
            "Son! Good to see you're finally old enough to come to the hunt.",
            "We didn't have enough men to hunt before. The women tried their hand at it, but only out of necessity.",
            "They didn't grow up with it, so they couldn't catch anything! But you, you might make a great hunter one day.",
            "Never as good as your father though, heh heh!",
            "We're hunting wild pig. Aim the bow, use as many arrows as you need, and kill it for today's meat."
        };

        huntEnd.name = "Father";
        huntEnd.lines = new string[] {
            "Great start, son.",
            "We're blessed to have wild animals here that we can eat. But we must rely on luck as well.",
            "There is a plain through that path to the north leaving the forest. I've seen cows there.",
            "Oh I agree son, they would be nice to eat. But outside of the forest, it's not safe. So promise me one thing, boy.",
            "I will continue teaching you to hunt if you promise to never hunt outside the forest. Ever. Understand?",
            "Good. Now, take the meat and bring it home to mother. I'll see you for siesta later."
        };

        noHunt.name = "Father";
        noHunt.lines = new string[] {
            "Shouldn't you be taking that water back to your mother?"
        };

        huntOver.name = "Father";
        huntOver.lines = new string[] {
            "I will continue teaching you to hunt if you promise to never hunt outside the forest. Ever. Understand?"
        };

        dayOver.name = "Mother";
        dayOver.lines = new string[] {
            "Thank you, son. That's everything for today!",
            "You've been hard at work, it's time for a siesta. Come on inside."
        };

        sister.name = "Sister";
        sister.lines = new string[] {
            "Oh hey little bro. Mom and dad got you working hard today?",
            "I mean, <b>finally!</b> Someone else should do some of the work around here...",
            "Come around tomorrow, I may need your help with something."
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
        }
    }


}
