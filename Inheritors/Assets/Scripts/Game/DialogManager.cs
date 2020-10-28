using System;
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

    GameObject dialogBox;
    CanvasGroup canvasGroup;
    RectTransform rectTransform;
    TMP_Text dialogText;
    Image dialogPrompt;
    float dialogBoxYPosition = 192f;

    bool dialogNext = false;
    string[] lines;
    float textSpeed = 0;
    float[] speeds;
    float slow = 0.1f;
    float med = 0.05f;
    float fast = 0.01f;

    void Start()
    {
        dialogBox = GameObject.FindGameObjectWithTag("DialogBox");
        canvasGroup = dialogBox.GetComponent<CanvasGroup>();
        rectTransform = dialogBox.GetComponent<RectTransform>();
        dialogText = GameObject.Find("DialogText").GetComponent<TMP_Text>();
        dialogPrompt = GameObject.Find("DialogPrompt").GetComponent<Image>();

        dayManager = GameObject.Find("DayManager").GetComponent<DayManager>();
        dayManager.OnDialog += HandleDialogEvent;
        inputManager = GameObject.Find("InputManager").GetComponent<InputManager>();
        inputManager.OnButtonDown += HandleInputEvent;

        speeds = new float[] { slow, med, fast };
    }

    private void HandleInputEvent(object sender, InputManager.ButtonArgs args)
    {
        DayManager.State state = dayManager.state;
        if (dayManager.state == DayManager.State.Dialog)
        {
            if (args.buttonCode == InputManager.A)
                dialogNext = true;
        }
    }

    private void HandleDialogEvent(object sender, DayManager.DialogArgs args)
    {
        // TODO: either here in the args or in daymanager, have an option to make player automatically turn to face the subject of dialog
        // TODO: either here in the args or in daymanager, have an option to make the camera focus on a different object temporarily
        lines = args.lines;
        textSpeed = speeds[args.speed];
        StartCoroutine(DialogPlay());
    }

    IEnumerator DialogPlay()
    {
        // 1. Set up, bring dialog box up to screen
        // TODO: make char face the thing if option says so
        // TODO: make camera focus on the thing if option says so
        dialogText.maxVisibleCharacters = 0;
        dialogPrompt.color = Helper.ChangedAlpha(dialogPrompt.color, 0);
        Tween t1 = TweenBox("Up", 1f);
        canvasGroup.DOFade(1f, 1f).From(0f);
        yield return new WaitWhile(() => t1.IsPlaying());

        // 2. Dialog display and input to go through it.
        inputManager.DialogInputsOnly();
        for (int line = 0; line < lines.Length; line++)
        {
            dialogText.text = lines[line];
            for (int i = 0; i <= dialogText.text.Length; i++)
            {
                dialogText.maxVisibleCharacters = i;
                yield return new WaitForSeconds(textSpeed);
            }
            dialogPrompt.enabled = true;
            Tween t2 = DOTween.To(() => dialogPrompt.color, x => dialogPrompt.color = x, Helper.ChangedAlpha(dialogPrompt.color, 1f), .25f);
            dialogNext = false;
            yield return new WaitUntil(() => dialogNext);
            yield return null; // Must put a frame between inputs
            if (t2 != null) t2.Kill();
            dialogPrompt.color = Helper.ChangedAlpha(dialogPrompt.color, 0f);
        }

        // 3. Finish, deconstruct, send dialog box back down
        TweenBox("Down", 1f);
        canvasGroup.DOFade(0f, 0.8f);
        dayManager.SetState(DayManager.State.Normal);
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
        dayManager.OnDialog -= HandleDialogEvent;
        inputManager.OnButtonDown -= HandleInputEvent;
    }
}
