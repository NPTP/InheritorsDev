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
    public Transform wateringHoleQuadrantTransform;
    public Transform fatherQuadrantTransform; //

    /* -------------------------------------- */
    /* -------------------------------------- */

    void Awake()
    {
        PlayerPrefs.SetInt("currentDayNumber", dayNumber);
        if (!enableDayScripts && !Application.isEditor)
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

        // Zoom up and queue the opening dialog, leave inert after dialog.
        dialogManager.NewDialog(dialogs["Day1Opening"], State.Inert);
        yield return new WaitUntil(dialogManager.IsDialogFinished);
        uiManager.SetUpTasksInventory();
        yield return new WaitForSeconds(.5f);

        // Give us context for watering hole task.
        cameraManager.SendCamTo(wateringHoleQuadrantTransform);
        yield return new WaitWhile(cameraManager.IsSwitching);
        taskManager.AddTask(TaskType.Mother, "Fetch water for mother.");
        yield return new WaitForSeconds(1f);

        // Give us context for hunting task.
        cameraManager.SendCamTo(fatherQuadrantTransform);
        yield return new WaitWhile(cameraManager.IsSwitching);
        taskManager.AddTask(TaskType.Father, "Meet father in the woods.");
        yield return new WaitForSeconds(1f);

        // Return to player to debrief before letting them loose.
        cameraManager.QuadrantCamActivate(motherQuadrantTransform);
        yield return new WaitWhile(cameraManager.IsSwitching);
        dialogManager.NewDialog(dialogs["Dialog_TaskExplanation"]);
        yield return new WaitUntil(dialogManager.IsDialogFinished);

        // Player is now loose, and can repeat the task dialog with mother.
        triggers["Dialog_TaskExplanation"].Enable();

        yield return new WaitForSeconds(3f);
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
        triggers["Dialog_HuntBegin"].Remove();
        yield return new WaitUntil(dialogManager.IsDialogFinished);
        taskManager.SetActiveTask(TaskType.Father, false);
        taskManager.ChangeTask(TaskType.Father, "Kill pig with the bow.");

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
        triggers["Dialog_HuntOver"].Enable();
    }

    void PickupPig()
    {
        taskManager.ChangeTask(TaskType.Father, "Bring the meat home.");
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
        taskManager.SetActiveTask(TaskType.Mother);
        taskManager.ChangeTask(TaskType.Mother, "Bring the water to mother.");
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
        triggers["Dialog_HuntBegin"].Enable();
    }

    IEnumerator AllTasksProcess()
    {
        yield return new WaitForSeconds(1f);
        dialogManager.NewDialog(dialogs["DayOver"]);
        yield return new WaitUntil(dialogManager.IsDialogFinished);
        StartCoroutine(SendNPCsHome());
        taskManager.AddAndSetActive(TaskType.DayEnd, "Return home for siesta.", false);
        triggers["Walk_End"].Enable();
    }

    IEnumerator SendNPCsHome()
    {
        yield return null;
        // TODO: direct NPCs to walk inside their malocas if they have'em and disappear.
        Destroy(GameObject.FindWithTag("MotherNPC"));
        Destroy(GameObject.FindWithTag("FatherNPC"));
        Destroy(GameObject.FindWithTag("SisterNPC"));
        triggers["Dialog_HuntOver"].Disable();
        triggers["Dialog_Sister"].Disable();
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
        dialogs.Add("Day1Opening", new Dialog
        {
            character = Character.Mother,
            lines = new string[] {
                "Good morning, son. You slept well?",
                "Look! Last night, you left a trail behind you. This land has memory.",
                "If you walk a path over and over again, it remembers you. Keep that in mind.",
                "Now, you are old enough to help with the work. Here is what we need today!"
            }
        });

        dialogs.Add("Dialog_TaskExplanation", new Dialog
        {
            character = Character.Mother,
            lines = new string[] {
                "Begin the work in any order you like. Once you have begun a task, finish it before pursuing another.",
                "You already have everything you need for this work.",
                "This is a great help to the family, son. I am proud of you!"
            }
        });

        dialogs.Add("Dialog_HavePig", new Dialog
        {
            character = Character.Mother,
            lines = new string[] {
                "You and father have caught a pig? It is beautiful.",
                "Place it over the fire. We will cook it later."
            }
        });

        dialogs.Add("Dialog_HaveWater", new Dialog
        {
            character = Character.Mother,
            lines = new string[] {
                "Just pour the water into the big grey pot."
            }
        });

        dialogs.Add("Dialog_HuntBegin", new Dialog
        {
            character = Character.Father,
            lines = new string[] {
                "Son! Finally you are old enough and we have enough men to hunt. I will teach you.",
                "The women tried their hand at it, but only out of necessity, and had not the skill. But you will be great.",
                "Today we hunt wild pig. Aim the bow carefully - do not scare it away. Go, my son."
            }
        });

        dialogs.Add("Dialog_HuntEnd", new Dialog
        {
            character = Character.Father,
            lines = new string[] {
                "Well done, my son!",
                "We are blessed to have wild animals here that we can eat.",
                "There is a plain, through that path to the north, leaving the forest. I have seen cows there.",
                "Yes, cows would make for good meat, and cows are not found in the forest. But it not safe out there, so make me a promise.",
                "I will teach you the hunt, but you must promise to never leave this forest. Ever. Understand?",
                "Good. Now, take the meat home. I will see you for siesta."
            }
        });

        dialogs.Add("Dialog_NoHunt", new Dialog
        {
            character = Character.Father,
            lines = new string[] {
                "Funny boy! That water belongs with your mother, not here!"
            }
        });

        dialogs.Add("Dialog_HuntOver", new Dialog
        {
            character = Character.Father,
            lines = new string[] {
                "I will teach you the hunt, but you must promise to never leave this forest. Ever. Understand?",
            }
        });

        dialogs.Add("DayOver", new Dialog
        {
            character = Character.Mother,
            lines = new string[] {
                "Thank you, son. That's all the work today!",
                "You have worked so hard, it's time for siesta. Come inside."
            }
        });

        dialogs.Add("Dialog_Sister", new Dialog
        {
            character = Character.Sister,
            lines = new string[] {
                "Hello little brother! You are working hard today?",
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
