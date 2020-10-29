using System;
using System.Collections;
using UnityEngine;
using DG.Tweening;

public class Day0 : MonoBehaviour
{
    DayManager dayManager;
    TaskManager taskManager;
    DialogManager dialogManager;
    InputManager inputManager;
    AudioManager audioManager;
    TransitionManager transitionManager;
    bool dialogFinished = false;

    CanvasGroup controls;

    class Dialog
    {
        public string[] lines;
        public float speed;

        public Dialog(string[] lines, float speed)
        {
            this.lines = lines;
            this.speed = speed;
        }
    }

    Dialog opening;

    void Start()
    {
        InitializeReferences();
        SubscribeToEvents();
        InitializeDialogs();
        StartCoroutine("Intro");
        Debug.Log("Press Enter to STOP the intro.");
    }

    void Update()
    {
        if (Input.GetKeyDown(KeyCode.Return))
        {
            StopCoroutine("Intro");
            dayManager.SetState(DayManager.State.Normal);
        }
    }

    IEnumerator Intro()
    {
        yield return null;
        controls.alpha = 0f; // TODO: break off into UI manager

        // 01. Darken screen, fade in sound.
        dayManager.SetState(DayManager.State.Inert);
        transitionManager.SetAlpha(1f);
        audioManager.SetVolume(0f);
        yield return new WaitForSecondsRealtime(2f);
        audioManager.Play(true);
        audioManager.FadeTo(0.5f, 5f, Ease.InOutCubic);
        yield return new WaitForSecondsRealtime(4f);

        // 02. Kick off the intro narration dialog.
        dayManager.NewDialog(opening.lines, opening.speed);
        yield return new WaitUntil(() => dialogFinished);
        dialogFinished = false;
        dayManager.SetState(DayManager.State.Inert);

        // 03. Fade away the blackness.
        yield return new WaitForSecondsRealtime(2f);
        transitionManager.FadeTo(0f, 8f);
        yield return new WaitForSecondsRealtime(4f);

        // 04. Display tutorial controls.
        controls.DOFade(1f, 1f); // TODO: break off into UI manager
        yield return new WaitForSecondsRealtime(2f);

        // 05. Let player control, fade out tutorial controls.
        dayManager.SetState(DayManager.State.Normal);
        yield return new WaitUntil(() => inputManager.leftStickHorizontal != 0 || inputManager.rightStickHorizontal != 0);
        controls.DOFade(0f, 1f); // TODO: break off into UI manager
    }

    void InitializeReferences()
    {
        audioManager = GameObject.Find("AudioManager").GetComponent<AudioManager>();
        taskManager = GameObject.Find("TaskManager").GetComponent<TaskManager>();
        dialogManager = GameObject.Find("DialogManager").GetComponent<DialogManager>();
        dayManager = GameObject.Find("DayManager").GetComponent<DayManager>();
        transitionManager = GameObject.Find("TransitionManager").GetComponent<TransitionManager>();
        inputManager = GameObject.Find("InputManager").GetComponent<InputManager>();

        controls = GameObject.Find("Controls").GetComponent<CanvasGroup>(); // TODO: break off into UI manager
    }

    void SubscribeToEvents()
    {
        dialogManager.OnDialogFinish += HandleDialogFinish;
    }

    void HandleDialogFinish(object sender, EventArgs args)
    {
        dialogFinished = true;
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
