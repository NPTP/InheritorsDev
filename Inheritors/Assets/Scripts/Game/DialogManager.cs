using System;
using System.Collections;
using UnityEngine;
using DG.Tweening;
using TMPro;
using UnityEngine.UI;

// The dialog manager will handle all dialog moments/gameplay and send dialog events to the UI
// to be performed. It assumes the existence of the other management systems (Day, Input)
// and of course the dialog box UI itself, and is coupled only with them.

// Gts told by Day when to perform a certain dialog, and what the dialog is.
public class DialogManager : MonoBehaviour
{
    DayManager dayManager;
    InputManager inputManager;
    // TODO: Make the StateManager a reality. This would tell us: are we picking something up?
    // are we in a dialog? Are we in a cutscene? Are we in normal movement? etc etc
    // Then we actually don't need inputManager to tell us about blocked inputs or whatever,
    // Just to get input events.
    // StateManager stateManager;

    GameObject dialogBox;
    RectTransform boxRectTransform;
    TMP_Text dialogText;
    Image dialogPrompt;
    float dialogBoxYPosition = 192f;

    string[] lines;

    void Start()
    {
        dialogBox = GameObject.FindGameObjectWithTag("DialogBox");
        boxRectTransform = dialogBox.GetComponent<RectTransform>();
        dialogText = GameObject.Find("DialogText").GetComponent<TMP_Text>();
        dialogPrompt = GameObject.Find("DialogPrompt").GetComponent<Image>();
        string[] l = {
            "Lorem ipsum dolor sit amet, consectetur adipiscing elit, sed do eiusmod tempor incididunt ut labore et dolore magna aliqua.",
            "Ut enim ad minim veniam, quis nostrud exercitation ullamco laboris nisi ut aliquip ex ea commodo consequat.",
            "Duis aute irure dolor in reprehenderit in voluptate velit esse cillum dolore eu fugiat nulla pariatur.",
            "Excepteur sint occaecat cupidatat non proident, sunt in culpa qui officia deserunt mollit anim id est laborum."
        };
        lines = l;

        inputManager = GameObject.Find("InputManager").GetComponent<InputManager>();
        inputManager.OnButtonDown += HandleInputEvent;
    }

    private void HandleInputEvent(object sender, InputManager.ButtonArgs args)
    {
        if (args.buttonCode == 9)
        {
            StartCoroutine(NewDialog());
        }
    }

    // TODO: implement state management once you have it
    IEnumerator NewDialog()
    {
        // 1. Set up, bring dialog box up to screen
        inputManager.BlockInput();
        dialogText.maxVisibleCharacters = 0;
        // TODO: Set prompt alpha to 0
        Tween t1 = TweenBoxUp();
        yield return new WaitUntil(() => t1 == null || !t1.IsPlaying());

        // 2. Dialog display and input to go through it.
        inputManager.DialogInputsOnly();
        for (int line = 0; line < lines.Length; line++)
        {
            // TODO: Set prompt alpha to 0
            dialogText.text = lines[line];
            for (int i = 0; i <= dialogText.text.Length; i++)
            {
                if (Input.GetButtonDown("A")) // TODO: hard to catch the actual frame here because of the WaitForSeconds between input chances
                {
                    dialogText.maxVisibleCharacters = dialogText.text.Length;
                    yield return null; // Must put a frame between inputs
                    break;
                }
                dialogText.maxVisibleCharacters = i;
                yield return new WaitForSeconds(0.01f);
            }
            dialogPrompt.enabled = true;
            // TODO: Tween prompt alpha to 1
            yield return new WaitUntil(() => Input.GetButtonDown("A"));
            yield return null; // Must put a frame between inputs
        }

        // 3. Finish, deconstruct, send dialog box back down
        inputManager.AllowAllInputs();
        Tween t2 = TweenBoxDown();
    }

    Color ReturnColorWithNewAlpha(Color color, float alpha)
    {
        Color c = color;
        c.a = alpha;
        return c;
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
}
