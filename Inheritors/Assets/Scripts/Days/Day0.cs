using System;
using System.Collections;
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

    Dialog opening;

    void Start()
    {
        InitializeReferences();
        SubscribeToEvents();
        InitializeDialogs();
        StartCoroutine("Intro");
        Debug.Log("Press Backspace to kill the intro.");

        // Unused
        // string[] tutorialTasks = {
        //     "Get wood for the fire",
        //     "Put wood on the fire",
        //     "Listen to mama's story"
        // };

        // foreach (string task in tutorialTasks)
        //     taskManager.AddTask(task);
        // taskManager.SetActiveTask(1);
    }

    void Update()
    {
        if (Input.GetKeyDown(KeyCode.Backspace))
        {
            StopCoroutine("Intro");
            cameraManager.SwitchToPlayerCam();
            transitionManager.Hide();
            dialogManager.EndDialog();
            // uiManager?
            // audioManager ?
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

        /* 06. Let player control. When they move the joystick, fade out prompts. */
        stateManager.SetState(StateManager.State.Normal);
        yield return new WaitUntil(inputManager.IsLeftJoystickMoving);
        uiManager.controls.Hide();
    }

    void InitializeReferences()
    {
        audioManager = GameObject.Find("AudioManager").GetComponent<AudioManager>();
        taskManager = GameObject.Find("TaskManager").GetComponent<TaskManager>();
        dialogManager = GameObject.Find("DialogManager").GetComponent<DialogManager>();
        stateManager = GameObject.Find("StateManager").GetComponent<StateManager>();
        transitionManager = GameObject.Find("TransitionManager").GetComponent<TransitionManager>();
        inputManager = GameObject.Find("InputManager").GetComponent<InputManager>();
        cameraManager = GameObject.Find("CameraManager").GetComponent<CameraManager>();
        uiManager = GameObject.Find("UIManager").GetComponent<UIManager>();
    }

    void SubscribeToEvents()
    {
        // Nothing at present.
    }

    void InitializeDialogs()
    {
        string delay = DialogManager.Tools.DELAY;
        opening = new Dialog(
            new string[] {
                "The Omerê is our home." + delay,
                "Our people have lived here for hundreds of years." + delay,
                "Your mother is of <b>Kanoê</b>." + delay + "\nYour father, of <b>Akuntsu</b>." + delay,
                "You are young, <b>Operaeika</b>." + delay + "\nYou are the inheritor of this land." + delay + "\nThe inheritor of our tradition." + delay, // TODO: change Operaeika to "son"
                "You will bring us hope." + delay + delay,
            },
            DialogManager.Speed.MED
        );
    }

}
