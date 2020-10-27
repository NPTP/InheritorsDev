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
    RectTransform boxRectTransform;
    TMP_Text dialogText;
    Image dialogPrompt;
    float dialogBoxYPosition = 192f;

    string[] lines;
    // TODO: use these presets for the dialog arg of speed
    float textSpeedFast = 0.01f;
    float textSpeedMed = 0.05f;
    float textSpeedSlow = 0.1f;
    bool dialogNext = false;

    void Start()
    {
        dialogBox = GameObject.FindGameObjectWithTag("DialogBox");
        boxRectTransform = dialogBox.GetComponent<RectTransform>();
        dialogText = GameObject.Find("DialogText").GetComponent<TMP_Text>();
        dialogPrompt = GameObject.Find("DialogPrompt").GetComponent<Image>();

        dayManager = GameObject.Find("DayManager").GetComponent<DayManager>();
        dayManager.OnDialog += HandleDialogEvent;
        inputManager = GameObject.Find("InputManager").GetComponent<InputManager>();
        inputManager.OnButtonDown += HandleInputEvent;
    }

    private void HandleInputEvent(object sender, InputManager.ButtonArgs args)
    {
        int state = dayManager.state;
        if (args.buttonCode == InputManager.A)
        {
            if (state == (int)DayManager.State.Normal)
                Debug.Log("Pressed A in Normal state");
            else if (state == (int)DayManager.State.Dialog)
                dialogNext = true;
        }
    }

    private void HandleDialogEvent(object sender, DayManager.DialogArgs args)
    {
        // TODO: either here in the args or in daymanager, have an option to make player automatically turn to face the subject of dialog
        lines = args.lines;
        StartCoroutine(DialogPlay());
    }

    IEnumerator DialogPlay()
    {
        // 1. Set up, bring dialog box up to screen
        // TODO: make char face the thing
        dialogText.maxVisibleCharacters = 0;
        dialogPrompt.color = ChangedAlpha(dialogPrompt.color, 0);
        Tween t1 = TweenBoxUp();
        yield return new WaitUntil(() => t1 == null || t1.IsPlaying());

        // 2. Dialog display and input to go through it.
        // TODO: support the speed from the dialog args (changes the waitforseconds);
        inputManager.DialogInputsOnly();
        for (int line = 0; line < lines.Length; line++)
        {
            dialogText.text = lines[line];
            for (int i = 0; i <= dialogText.text.Length; i++)
            {
                dialogText.maxVisibleCharacters = i;
                yield return new WaitForSeconds(textSpeedFast);
            }
            dialogPrompt.enabled = true;
            Tween t2 = DOTween.To(() => dialogPrompt.color, x => dialogPrompt.color = x, ChangedAlpha(dialogPrompt.color, 1f), .25f);
            dialogNext = false;
            yield return new WaitUntil(() => dialogNext);
            yield return null; // Must put a frame between inputs
            if (t2 != null) t2.Kill();
            dialogPrompt.color = ChangedAlpha(dialogPrompt.color, 0f);
        }

        // 3. Finish, deconstruct, send dialog box back down
        dayManager.SetState(DayManager.State.Normal);
        TweenBoxDown();
    }

    Tween TweenBoxUp()
    {
        return DOTween.To(
            () => boxRectTransform.anchoredPosition3D,
            x => boxRectTransform.anchoredPosition3D = x,
            new Vector3(0f, dialogBoxYPosition, 0f),
            1f
        );
    }

    Tween TweenBoxDown()
    {
        return DOTween.To(
            () => boxRectTransform.anchoredPosition3D,
            x => boxRectTransform.anchoredPosition3D = x,
            new Vector3(0f, -dialogBoxYPosition, 0f),
            1f
        );
    }

    // Handy helper which returns the same color, but with alpha set to input parameter.
    // TODO: consider moving this and other helper functions like it to a static generalized helper class.
    Color ChangedAlpha(Color color, float alpha)
    {
        Color c = color;
        c.a = alpha;
        return c;
    }
}
