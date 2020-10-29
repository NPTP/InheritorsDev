﻿using System;
using System.Collections;
using UnityEngine;
using DG.Tweening;
using TMPro;
using UnityEngine.UI;

// The dialog manager handles all dialog moments and performs dialog "playback" on the UI.
// It assumes the existence of DayManager and InputManager and of course the dialog box
// UI element itself, and is coupled only with these three things.
// Gets told by events from DayManager when, how & what to playback in a dialog.
public class DialogManager : MonoBehaviour
{
    DayManager dayManager;
    InputManager inputManager;
    InteractManager interactManager;

    public event EventHandler OnDialogFinish;

    GameObject dialogBox;
    CanvasGroup canvasGroup;
    RectTransform rectTransform;
    TMP_Text dialogText;
    Image dialogPrompt;
    Animator dialogPromptAnim;
    float dialogBoxYPosition = 192f;
    bool dialogNext = false;

    public class Tools
    {
        public static string DELAY = "                    ";
    }
    public class Speed
    {
        public static float SLOW = 0.05f;
        public static float MED = 0.03f;
        public static float FAST = 0.01f;
    }

    void Start()
    {
        dialogBox = GameObject.FindGameObjectWithTag("DialogBox");
        canvasGroup = dialogBox.GetComponent<CanvasGroup>();
        rectTransform = dialogBox.GetComponent<RectTransform>();
        dialogText = GameObject.Find("DialogText").GetComponent<TMP_Text>();
        dialogPrompt = GameObject.Find("DialogPrompt").GetComponent<Image>();
        dialogPromptAnim = GameObject.Find("DialogPrompt").GetComponent<Animator>();

        dayManager = GameObject.Find("DayManager").GetComponent<DayManager>();
        dayManager.OnDialog += HandleGlobalDialogEvent;
        inputManager = GameObject.Find("InputManager").GetComponent<InputManager>();
        inputManager.OnButtonDown += HandleInputEvent;
        interactManager = GameObject.Find("InteractManager").GetComponent<InteractManager>();
        interactManager.OnLocalDialog += HandleLocalDialogEvent;
    }

    private void HandleInputEvent(object sender, InputManager.ButtonArgs args)
    {
        if (dayManager.state == DayManager.State.Dialog && args.buttonCode == InputManager.A)
            dialogNext = true;
    }

    private void HandleGlobalDialogEvent(object sender, DayManager.DialogArgs args)
    {
        dayManager.SetState(DayManager.State.Dialog);
        // TODO: either here in the args or in daymanager, have an option to make player automatically turn to face the subject of dialog
        // TODO: either here in the args or in daymanager, have an option to make the camera focus on a different object temporarily
        StartCoroutine(DialogPlay(args.lines, args.speed));
    }

    private void HandleLocalDialogEvent(object sender, InteractManager.LocalDialogArgs args)
    {
        dayManager.SetState(DayManager.State.Dialog);
        StartCoroutine(DialogPlay(args.lines, args.speed));
    }

    IEnumerator DialogPlay(string[] lines, float speed)
    {
        // STEP 1 : Set up, bring dialog box up to screen
        dialogText.maxVisibleCharacters = 0;
        dialogPrompt.color = Helper.ChangedAlpha(dialogPrompt.color, 0);
        Tween t1 = TweenBox("Up", 1f);
        canvasGroup.DOFade(1f, 1f).From(0f);
        yield return new WaitWhile(() => t1.IsPlaying());

        // STEP 2 : Dialog display and input to go through it.
        for (int line = 0; line < lines.Length; line++)
        {
            dialogText.text = lines[line];
            for (int i = 0; i <= dialogText.text.Length; i++)
            {
                dialogText.maxVisibleCharacters = i;
                yield return new WaitForSecondsRealtime(speed);
            }
            dialogPrompt.enabled = true;
            Tween t2 = DOTween.To(() => dialogPrompt.color, x => dialogPrompt.color = x, Helper.ChangedAlpha(dialogPrompt.color, 1f), .25f);
            dialogNext = false;
            yield return new WaitUntil(() => dialogNext);
            yield return null; // Must put a frame between inputs
            if (t2 != null) t2.Kill();
            dialogPrompt.color = Helper.ChangedAlpha(dialogPrompt.color, 0f);
        }

        // STEP 3 : Finish, deconstruct, and send dialog box back down
        TweenBox("Down", 1f);
        canvasGroup.DOFade(0f, 0.8f);
        dayManager.SetState(DayManager.State.Normal);
        OnDialogFinish?.Invoke(this, EventArgs.Empty);
    }

    Tween TweenBox(string dir, float duration)
    {
        float yPos = dir == "Up" ? dialogBoxYPosition : -dialogBoxYPosition;
        return DOTween.To(
            () => rectTransform.anchoredPosition3D,
            x => rectTransform.anchoredPosition3D = x,
            new Vector3(0f, yPos, 0f),
            duration
        );
    }

    // Unsubscribe from all events
    void OnDestroy()
    {
        dayManager.OnDialog -= HandleGlobalDialogEvent;
        interactManager.OnLocalDialog -= HandleLocalDialogEvent;
        inputManager.OnButtonDown -= HandleInputEvent;
    }
}
