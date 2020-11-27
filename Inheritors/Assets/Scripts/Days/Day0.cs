﻿using System;
using System.Linq;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DG.Tweening;
using Cinemachine;
using UnityEngine.SceneManagement;

// TODO: make every day's tasks & dialogs their own classes so we never mis-label something.
public class Day0 : MonoBehaviour
{
    public bool enableDayScripts = true;
    int dayNumber = 0;
    Dictionary<string, Trigger> triggers = new Dictionary<string, Trigger>();

    /* -------------------------------------- */
    /* Day-specific objects, transforms, etc. */
    /* -------------------------------------- */

    [Header("Day-specific Objects")]
    public Transform firepitTransform;
    public Transform firewoodTransform;
    public Transform malocaMotherTransform;

    /* -------------------------------------- */
    /* -------------------------------------- */

    void OnEnable()
    {
        print("set playerprefs in day");
        PlayerPrefs.SetInt("currentDayNumber", dayNumber);
    }

    void Awake()
    {

        if (!enableDayScripts)
            Destroy(this);
        InitializeReferences();
    }

    void Start()
    {
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
            cameraManager.SwitchToCam("Player");
            transitionManager.Hide();
            dialogManager.EndDialog();
            uiManager.controls.Hide();
            triggers["Walk_Firepit"].Enable();
            stateManager.SetState(State.Normal);
            cameraManager.ResetZoom();
            InitializeTasks();
        }

        if (Input.GetKeyDown(KeyCode.Return))
        {
            StartCoroutine("End");
        }
    }

    IEnumerator Intro()
    {
        stateManager.SetState(State.Inert);
        cameraManager.SendCamTo(firepitTransform);
        cameraManager.Zoom(0.375f, 0f);

        /* 01. Darken screen, wait. */
        transitionManager.SetColor(Color.black);
        transitionManager.Show();
        yield return new WaitForSeconds(4f);

        /* 02. Kick off the intro narration dialog. */
        dialogManager.NewDialog(opening, State.Inert);
        yield return new WaitUntil(dialogManager.IsDialogFinished);

        /* 03. Fade away the blackness. */
        yield return new WaitForSeconds(2f);
        transitionManager.Hide(8f);
        yield return new WaitForSeconds(4f);

        /* 04. Change view from fire to player. */
        cameraManager.SwitchToCam("Player");
        cameraManager.ResetZoom(2f);
        yield return new WaitWhile(cameraManager.IsSwitching);

        /* 05. Display tutorial controls. */
        uiManager.controls.Show();
        yield return new WaitForSeconds(.25f);

        /* 06. Let player control. When they move the joystick, fade out prompts. 
        ** Enable the firepite walk trigger. */
        stateManager.SetState(State.Normal);
        yield return new WaitUntil(inputManager.IsLeftJoystickMoving);
        uiManager.controls.Hide();
        triggers["Walk_Firepit"].Enable();
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
                    recordManager.StartNewRecording();
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
        dialogManager.NewDialog(wood3, State.Inert);
        yield return new WaitUntil(dialogManager.IsDialogFinished);

        taskManager.CompleteActiveTask(); // CompleteTask("Branches");
        taskManager.SetActiveTask("WoodFire");

        triggers["Dropoff_Wood"].Enable();

        stateManager.SetState(State.Normal);
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

            case "Walk_SisterSleep":
                dialogManager.NewDialog(sisterSleep);
                break;

            default:
                Debug.Log("Interact Manager gave unknown WALK tag to Day " + dayNumber);
                break;
        }
    }

    IEnumerator Firepit()
    {
        dialogManager.NewDialog(firepit, State.Inert);
        yield return new WaitUntil(dialogManager.IsDialogFinished);

        cameraManager.SendCamTo(firewoodTransform);
        // yield return new WaitForSeconds(1f);
        yield return new WaitWhile(cameraManager.IsSwitching);

        triggers["Pickup_Wood1"].Enable();
        yield return new WaitForSeconds(1f);
        triggers["Pickup_Wood2"].Enable();
        yield return new WaitForSeconds(1f);
        triggers["Pickup_Wood3"].Enable();
        yield return new WaitForSeconds(1f);

        cameraManager.SwitchToLastCam();
        taskManager.SetActiveTask("Branches");
        uiManager.SetUpTasksInventory();
        // yield return new WaitForSeconds(0.5f);
        yield return new WaitWhile(cameraManager.IsSwitching);

        stateManager.SetState(State.Normal);
    }

    IEnumerator DropoffWood()
    {
        recordManager.StopRecording();

        stateManager.SetState(State.Inert);
        yield return new WaitForSeconds(1f);

        dialogManager.NewDialog(maloca, State.Inert);
        yield return new WaitUntil(dialogManager.IsDialogFinished);

        yield return new WaitForSeconds(.25f);
        taskManager.SetActiveTask("Maloca");
        triggers["Walk_End"].Enable();

        stateManager.SetState(State.Normal);
    }

    IEnumerator End()
    {

        stateManager.SetState(State.Inert);
        uiManager.TearDownTasksInventory();
        Tween t = transitionManager.Show(2f);
        audioManager.FadeOtherSources("Down", 2f); // audioManager.FadeTo(0f, 2f, Ease.InOutQuad);
        yield return new WaitWhile(() => t != null & t.IsPlaying());

        saveManager.SaveGame(dayNumber + 1);
        Helper.LoadNextSceneInBuildOrder();
    }


    void InitializeTasks()
    {
        taskManager.AddTask("Branches", "Fetch 3 branches.");
        taskManager.AddTask("WoodFire", "Bring wood to fire.");
        taskManager.AddTask("Maloca", "Return to Maloca to sleep.");
    }

    Dialog opening = new Dialog();
    Dialog firepit = new Dialog();
    Dialog wood1 = new Dialog();
    Dialog wood2 = new Dialog();
    Dialog wood3 = new Dialog();
    Dialog maloca = new Dialog();
    Dialog sisterSleep = new Dialog();
    void InitializeDialogs()
    {
        string delay = DialogManager.Tools.DELAY;

        opening.name = "Mother";
        opening.skippable = false;
        opening.lines = new string[] {
            "The Omerê is our home." + delay,
            "Our people have lived here for hundreds of years." + delay,
            "Your mother is of <b>Kanoê</b>." + delay + "\nYour father, of <b>Akuntsu</b>." + delay,
            "You are young, <b>son</b>." + delay + "\nYou are the inheritor of this land." + delay + "\nThe inheritor of our tradition." + delay,
            "You will bring us hope." + delay + delay
        };
        opening.speed = DialogManager.Speed.SLOW;

        firepit.name = "Mother";
        firepit.skippable = false;
        firepit.lines = new string[] {
            "This fire is dying.",
            "Fetch <color=blue>3 branches</color> from our pile of dried wood, so that it might find new life."
        };

        wood1.name = "Mother";
        wood1.skippable = false;
        wood1.lines = new string[] {
            "The Cumaru wood. Tough, ancient, everlasting."
        };
        wood2.name = "Mother";
        wood2.skippable = false;
        wood2.lines = new string[] {
            "Massaranduba tree. Hard-won, deep, rich, and beautiful."
        };
        wood3.name = "Mother";
        wood3.skippable = false;
        wood3.lines = new string[] {
            "The wood of the Ipê. Life-giving, sturdy, ever-present... and coveted.",
            "That is enough wood for now. Return to the fire."
        };

        maloca.name = "Mother";
        maloca.skippable = false;
        maloca.lines = new string[] {
             "Well done, my son! I can see the shadows dancing on the inside of the maloca.",
             "It is late now. Come join me inside to sleep.",
             "Tomorrow is an important day."
        };

        sisterSleep.name = "Sister";
        sisterSleep.skippable = false;
        sisterSleep.lines = new string[] {
            "...",
            "Hn... huh? Oh, you woke me up!",
            "Brother, what are you doing here so late? Go to sleep.",
            "Mother must be so worried..."
        };
    }



}
