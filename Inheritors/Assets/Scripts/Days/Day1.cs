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
    int dayNumber = 1;
    Dictionary<string, Trigger> triggers = new Dictionary<string, Trigger>();

    void Start()
    {
        InitializeReferences();
        InitializeTriggers();
        SubscribeToEvents();
        InitializeDialogs();
        InitializeTasks();
        // saveManager.LoadGame(dayNumber);
        StartCoroutine("Intro");
    }

    IEnumerator Intro()
    {
        transitionManager.Hide(0f);
        yield return new WaitForSeconds(.25f);

        taskManager.ClearTasks();
        taskManager.AddTask("Test", "This is a test task.");
        taskManager.AddTask("Inactive", "This should be inactive.");
        taskManager.SetActiveTask("Test");
        uiManager.SetUpTasksInventory();

        yield return new WaitForSeconds(3f);

        taskManager.CompleteActiveTask();
        yield return new WaitForSeconds(3f);
        uiManager.TearDownTasksInventory();

        // dialogManager.NewDialog(opening);

    }

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

    void SubscribeToEvents()
    {
        interactManager.OnPickup += HandlePickupEvent;
        interactManager.OnDropoff += HandleDropoffEvent;
        interactManager.OnDialog += HandleDialogEvent;
        interactManager.OnWalk += HandleWalkEvent;
    }


    void OnDestroy()
    {
        interactManager.OnPickup -= HandlePickupEvent;
        interactManager.OnDropoff -= HandleDropoffEvent;
        interactManager.OnDialog -= HandleDialogEvent;
        interactManager.OnWalk -= HandleWalkEvent;
    }

    void HandlePickupEvent(object sender, InteractManager.PickupArgs args)
    {
        PickupManager.Inventory inventory = args.inventory;
        switch (inventory.itemType)
        {
            case PickupManager.ItemTypes.WOOD:
                if (inventory.itemQuantity == 1)
                {
                    dialogManager.NewDialog(wood1);
                }
                else if (inventory.itemQuantity == 2)
                {
                    dialogManager.NewDialog(wood2);
                }
                else if (inventory.itemQuantity == 3)
                {
                    StartCoroutine(AllWoodCollected());
                }
                break;

            case PickupManager.ItemTypes.NULL:
                Debug.Log("NULL pickup event");
                break;

            default:
                Debug.Log("Interact Manager gave unknown PICKUP tag to Day " + dayNumber);
                break;
        }
    }

    IEnumerator AllWoodCollected()
    {
        dialogManager.NewDialog(wood3, StateManager.State.Inert);
        yield return new WaitUntil(dialogManager.IsDialogFinished);

        taskManager.CompleteActiveTask();
        taskManager.AddTask("WoodFire", "Bring wood to fire.");
        taskManager.SetActiveTask("");

        triggers["Dropoff_Wood"].Enable();

        stateManager.SetState(StateManager.State.Normal);
    }

    void HandleDialogEvent(object sender, InteractManager.DialogArgs args)
    {
        string tag = args.tag;
        Dialog dialog = args.dialog;

        switch (tag)
        {
            case "DUMMY":
                break;

            default:
                Debug.Log("Interact Manager gave unknown DIALOG tag to Day " + dayNumber);
                dialogManager.NewDialog(dialog);
                StartCoroutine(WaitDialogEnd());
                break;
        }
    }

    IEnumerator WaitDialogEnd()
    {
        yield return new WaitUntil(dialogManager.IsDialogFinished);
        print("Dialog done, m'boy!");
    }

    void HandleDropoffEvent(object sender, InteractManager.DropoffArgs args)
    {
        string tag = args.tag;

        switch (tag)
        {
            case "Dropoff_Wood":
                StartCoroutine(DropoffWood());
                break;

            default:
                Debug.Log("Interact Manager gave unknown DROPOFF tag to Day " + dayNumber);
                break;
        }
    }

    IEnumerator DropoffWood()
    {
        stateManager.SetState(StateManager.State.Inert);
        yield return new WaitForSeconds(1f);

        cameraManager.SendCamTo(GameObject.Find("maloca").transform);
        dialogManager.NewDialog(maloca, StateManager.State.Inert);
        yield return new WaitUntil(dialogManager.IsDialogFinished);

        triggers["Walk_End"].Enable();
        yield return new WaitForSeconds(.25f);

        cameraManager.SwitchToPlayerCam();
        stateManager.SetState(StateManager.State.Normal);
    }

    void HandleWalkEvent(object sender, InteractManager.WalkArgs args)
    {
        string tag = args.tag;

        switch (tag)
        {
            case "Walk_Firepit":
                StartCoroutine(Firepit());
                break;

            case "Walk_End":
                StartCoroutine(End());
                break;

            default:
                Debug.Log("Interact Manager gave unknown WALK tag to Day " + dayNumber);
                break;
        }
    }

    IEnumerator Firepit()
    {

        yield return new WaitForSeconds(0.5f);

        stateManager.SetState(StateManager.State.Normal);
    }

    IEnumerator End()
    {
        stateManager.SetState(StateManager.State.Inert);
        Tween t = transitionManager.Show(2f);
        audioManager.FadeTo(0f, 2f, Ease.InOutQuad);
        yield return new WaitWhile(() => t != null & t.IsPlaying());
        yield return new WaitForSeconds(.5f);
        SceneManager.LoadScene(0);
    }


    void InitializeTasks()
    {
        // None given all at once dayNumber
    }

    Dialog opening = new Dialog();
    Dialog firepit = new Dialog();
    Dialog wood1 = new Dialog();
    Dialog wood2 = new Dialog();
    Dialog wood3 = new Dialog();
    Dialog maloca = new Dialog();
    void InitializeDialogs()
    {
        string delay = DialogManager.Tools.DELAY;

        opening.lines = new string[] {
            "Good morning, son.\nIt's a beautiful day.",
            "You see the imprints and trails you left behind last night? This land has memory.",
            "Every step has meaning, and deepens our connection to this place, and to our past.",
            "Our ancestors laid paths for us, and you will tread your own!",
            "If you walk a path over and over again, the land bends under your feet, the forest learns who you are...",
            "And it will keep a memory of you.",
            "Just something to keep in mind as you pursue each task."
        };

        firepit.lines = new string[] {
            "This fire is dying.",
            "Fetch dry branches from our collection, so that it might find new life."
        };

        wood1.lines = new string[] {
            "The Cumaru wood. Tough, ancient, everlasting."
        };
        wood2.lines = new string[] {
            "Massaranduba. Hard-won, deep, rich, and beautiful."
        };
        wood3.lines = new string[] {
            "The wood of the Ipê. Life-giving, sturdy, ever-present... and coveted.",
            "That is enough wood for now. Return to the fire."
        };

        maloca.lines = new string[] {
             "Well done, my son! The fire must be beautiful. It is too bad I cannot see it from here.",
             "But it is late now. Come to the maloca to sleep. Tomorrow is an important day."
        };
    }

}
