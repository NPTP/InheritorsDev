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
    StateManager stateManager;
    TaskManager taskManager;
    DialogManager dialogManager;
    InputManager inputManager;
    AudioManager audioManager;
    TransitionManager transitionManager;
    CameraManager cameraManager;
    UIManager uiManager;
    InteractManager interactManager;

    Dictionary<string, Trigger> triggers = new Dictionary<string, Trigger>();
    // TODO: make it a dict of just plain 'Trigger' an interface class we will define for all of them
    // Then it's just on us to not duplicate trigger tags 

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
            triggers["Firepit"].Enable();
            stateManager.SetState(StateManager.State.Normal);
        }
    }

    IEnumerator Intro()
    {
        uiManager.controls.SetAlpha(0f);
        cameraManager.SendCamTo(GameObject.Find("FirepitCollider").transform);

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
        yield return new WaitForSeconds(5f);

        /* 04. Change view from fire to player. */
        cameraManager.SwitchToPlayerCam();
        yield return new WaitForSeconds(2f);

        /* 05. Display tutorial controls. */
        uiManager.controls.Show();
        yield return new WaitForSeconds(.5f);

        /* 06. Let player control. When they move the joystick, fade out prompts. 
        ** Enable the firepite walk trigger. */
        stateManager.SetState(StateManager.State.Normal);
        yield return new WaitUntil(inputManager.IsLeftJoystickMoving);
        uiManager.controls.Hide();
        triggers["Firepit"].Enable();
    }

    void InitializeReferences()
    {
        audioManager = GameObject.FindObjectOfType<AudioManager>();
        taskManager = GameObject.FindObjectOfType<TaskManager>();
        dialogManager = GameObject.FindObjectOfType<DialogManager>();
        stateManager = GameObject.FindObjectOfType<StateManager>();
        transitionManager = GameObject.FindObjectOfType<TransitionManager>();
        inputManager = GameObject.FindObjectOfType<InputManager>();
        cameraManager = GameObject.FindObjectOfType<CameraManager>();
        uiManager = GameObject.FindObjectOfType<UIManager>();
        interactManager = GameObject.FindObjectOfType<InteractManager>();
    }

    void InitializeTriggers()
    {
        IEnumerable<Trigger> worldTriggers = FindObjectsOfType<MonoBehaviour>().OfType<Trigger>();
        foreach (Trigger trigger in worldTriggers)
        {
            trigger.OnTriggerActivate += HandleTriggerEvent;
            triggers[trigger.GetTag()] = trigger;
        }
    }

    void SubscribeToEvents()
    {

    }

    void HandleTriggerEvent(object sender, EventArgs args)
    {
        Trigger trigger = (Trigger)sender;
        string tag = trigger.GetTag();

        switch (tag)
        {
            case "Firepit":
                trigger.Remove();
                StartCoroutine(Firepit());
                break;

            case "End":
                trigger.Remove();
                StartCoroutine(End());
                break;

            default:
                Debug.Log("Trigger gave unknown tag.");
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

        triggers["Wood1"].Enable();
        yield return new WaitForSeconds(1f);
        triggers["Wood2"].Enable();
        yield return new WaitForSeconds(1f);
        triggers["Wood3"].Enable();
        yield return new WaitForSeconds(2f);

        cameraManager.SwitchToPlayerCam();
        yield return new WaitForSeconds(1f);

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
        // None
    }

    Dialog opening = new Dialog();
    Dialog firepit = new Dialog();

    void InitializeDialogs()
    {
        string delay = DialogManager.Tools.DELAY;

        // opening = new Dialog(
        //     new string[] {
        //         "The Omerê is our home." + delay,
        //         "Our people have lived here for hundreds of years." + delay,
        //         "Your mother is of <b>Kanoê</b>." + delay + "\nYour father, of <b>Akuntsu</b>." + delay,
        //         "You are young, <b>Operaeika</b>." + delay + "\nYou are the inheritor of this land." + delay + "\nThe inheritor of our tradition." + delay, // TODO: change Operaeika to "son"
        //         "You will bring us hope." + delay + delay,
        //     },
        //     DialogManager.Speed.MED
        // );

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
    }

}
