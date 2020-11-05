using System;
using System.Linq;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DG.Tweening;
using Cinemachine;
using UnityEngine.SceneManagement;

public class Day0 : MonoBehaviour
{
    int today = 0;
    Dictionary<string, Trigger> triggers = new Dictionary<string, Trigger>();

    void Start()
    {
        InitializeReferences();
        InitializeTriggers();
        SubscribeToEvents();
        InitializeDialogs();
        InitializeTasks();
        StartCoroutine("Intro");
        Debug.Log("Press Backspace to kill the intro.");
    }

    void Update()
    {
        if (Input.GetKeyDown(KeyCode.Backspace))
        {
            StopCoroutine("Intro");
            cameraManager.SwitchToPlayerCam();
            transitionManager.Hide();
            dialogManager.EndDialog();
            uiManager.controls.Hide();
            triggers["Walk_Firepit"].Enable();
            stateManager.SetState(StateManager.State.Normal);
            cameraManager.ResetSize();
        }
    }

    IEnumerator Intro()
    {
        uiManager.controls.SetAlpha(0f);
        cameraManager.SendCamTo(GameObject.Find("FirepitCollider").transform);
        cameraManager.ZoomToSize(5f, 2f);

        /* 01. Darken screen, fade in sound. */
        stateManager.SetState(StateManager.State.Inert);
        transitionManager.SetColor(Color.black);
        transitionManager.Show();
        audioManager.SetVolume(0f);
        yield return new WaitForSeconds(2f);
        audioManager.Play(true);
        audioManager.FadeTo(0.5f, 5f, Ease.InOutCubic);
        yield return new WaitForSeconds(4f);

        /* 02. Kick off the intro narration dialog. */
        dialogManager.NewDialog(opening, StateManager.State.Inert);
        yield return new WaitUntil(dialogManager.IsDialogFinished);

        /* 03. Fade away the blackness. */
        yield return new WaitForSeconds(2f);
        transitionManager.Hide(8f);
        yield return new WaitForSeconds(4f);

        /* 04. Change view from fire to player. */
        cameraManager.SwitchToPlayerCam();
        cameraManager.ResetSize(2f);
        yield return new WaitForSeconds(2f);

        /* 05. Display tutorial controls. */
        uiManager.controls.Show();
        yield return new WaitForSeconds(.5f);

        /* 06. Let player control. When they move the joystick, fade out prompts. 
        ** Enable the firepite walk trigger. */
        stateManager.SetState(StateManager.State.Normal);
        yield return new WaitUntil(inputManager.IsLeftJoystickMoving);
        uiManager.controls.Hide();
        triggers["Walk_Firepit"].Enable();
    }

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
                Debug.Log("Interact Manager gave unknown PICKUP tag to Day " + today);
                break;
        }
    }

    IEnumerator AllWoodCollected()
    {
        dialogManager.NewDialog(wood3, StateManager.State.Inert);
        yield return new WaitUntil(dialogManager.IsDialogFinished);

        taskManager.CompleteTask(1);
        taskManager.AddTask("Bring wood to fire.");
        taskManager.SetActiveTask(2);

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
                Debug.Log("Interact Manager gave unknown DIALOG tag to Day " + today);
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
                Debug.Log("Interact Manager gave unknown DROPOFF tag to Day " + today);
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
                Debug.Log("Interact Manager gave unknown WALK tag to Day " + today);
                break;
        }
    }

    IEnumerator Firepit()
    {
        dialogManager.NewDialog(firepit, StateManager.State.Inert);
        yield return new WaitUntil(dialogManager.IsDialogFinished);

        taskManager.AddTask("Fetch 3 branches for the fire.");
        taskManager.SetActiveTask(1);

        cameraManager.SendCamTo(GameObject.Find("woodchoprock").transform);
        yield return new WaitForSeconds(1f);

        triggers["Pickup_Wood1"].Enable();
        yield return new WaitForSeconds(1f);
        triggers["Pickup_Wood2"].Enable();
        yield return new WaitForSeconds(1f);
        triggers["Pickup_Wood3"].Enable();
        yield return new WaitForSeconds(1f);

        cameraManager.SwitchToPlayerCam();
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
        SceneManager.LoadScene("Day1");
    }


    void InitializeTasks()
    {
        // None given all at once today
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
            "The Omerê is our home." + delay,
            "Our people have lived here for hundreds of years." + delay,
            "Your mother is of <b>Kanoê</b>." + delay + "\nYour father, of <b>Akuntsu</b>." + delay,
            "You are young, <b>Operaeika</b>." + delay + "\nYou are the inheritor of this land." + delay + "\nThe inheritor of our tradition." + delay, // TODO: change Operaeika to "son"
            "You will bring us hope." + delay + delay
        };
        opening.speed = DialogManager.Speed.MED;

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

    void OnDestroy()
    {
        interactManager.OnPickup -= HandlePickupEvent;
        interactManager.OnDropoff -= HandleDropoffEvent;
        interactManager.OnDialog -= HandleDialogEvent;
        interactManager.OnWalk -= HandleWalkEvent;
    }

}
