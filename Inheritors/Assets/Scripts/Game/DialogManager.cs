/* INHERITORS by Nick Perrin (c) 2020 */
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DG.Tweening;
using TMPro;
using UnityEngine.UI;

// Class for encapsulating dialogs sent from other classes to be played back.
public class Dialog
{
    public string name;
    public string[] lines;
    public DialogManager.Speed speed;
    public bool skippable = true;
    public Transform target;

    public Dialog()
    {
        this.name = "NO NAME GIVEN";
        this.speed = DialogManager.Speed.FAST;
        target = null;
    }
}

// The dialog manager handles all dialogs and sends info to the UI to control it.
// It is called from the Day by any type of trigger, or manual call (e.g. for narration).
public class DialogManager : MonoBehaviour
{
    StateManager stateManager;
    CameraManager cameraManager;
    InputManager inputManager;
    InteractManager interactManager;
    UIManager uiManager;

    bool dialogNext = false;
    bool dialogFinished = true;

    public class Tools
    {
        public static string DELAY = "                    ";
    }
    public enum Speed
    {
        SLOW,
        MED,
        FAST
    }

    float slow = 0.03f;
    float med = 0.01f;
    float fast = 0.005f;
    float[] speeds;

    void Awake()
    {
        stateManager = FindObjectOfType<StateManager>();
        cameraManager = FindObjectOfType<CameraManager>();
        uiManager = FindObjectOfType<UIManager>();
        inputManager = FindObjectOfType<InputManager>();
        inputManager.OnButtonDown += HandleInputEvent;
        interactManager = FindObjectOfType<InteractManager>();
    }

    // Unsubscribe from all events
    void OnDestroy()
    {
        inputManager.OnButtonDown -= HandleInputEvent;
    }

    void Start()
    {
        speeds = new float[] { slow, med, fast };

    }

    // Start a new dialog and switch into specified state after dialog finishes
    public void NewDialog(Dialog dialog, State finishState = State.Normal)
    {
        dialogFinished = false;
        stateManager.SetState(State.Dialog);
        StartCoroutine(DialogPlay(dialog, finishState));
    }

    private void HandleInputEvent(object sender, InputManager.ButtonArgs args)
    {
        if (stateManager.GetState() == State.Dialog && args.buttonCode == InputManager.A)
            dialogNext = true;
    }

    public void EndDialog()
    {
        if (stateManager.GetState() == State.Dialog && !dialogFinished)
        {
            uiManager.dialogBox.TearDown();
            dialogFinished = true;
        }
    }

    IEnumerator DialogPlay(Dialog dialog, State finishState)
    {
        string name = dialog.name;
        string[] lines = dialog.lines;
        float speed = speeds[(int)dialog.speed];
        bool skippable = dialog.skippable;
        bool hasTarget = dialog.target != null;

        // STEP 1 : Set up, change cam & bring dialog box up to screen
        dialogFinished = false;
        if (hasTarget)
        {
            cameraManager.FocusCamOn("Dialog", dialog.target);
            cameraManager.SwitchToCam("Dialog");
        }
        Tween setup = uiManager.dialogBox.SetUp(name);
        yield return setup.WaitForCompletion();

        // STEP 2 : Dialog display and input to go through it.
        for (int line = 0; line < lines.Length; line++)
        {
            dialogNext = false;
            uiManager.dialogBox.SetLine(lines[line]);
            for (int i = 0; i <= lines[line].Length; i++)
            {
                uiManager.dialogBox.tmpText.maxVisibleCharacters = i;
                // yield return null;
                yield return new WaitForSecondsRealtime(0.001f);
                if (skippable && dialogNext)
                {
                    uiManager.dialogBox.tmpText.maxVisibleCharacters = lines[line].Length;
                    break;
                }
            }
            uiManager.dialogBox.ShowPrompt();
            dialogNext = false;
            yield return new WaitUntil(() => dialogNext);
            // yield return null; // Must put a frame between inputs
            uiManager.dialogBox.HidePrompt();
        }

        // STEP 3 : Finish, tear down dialog box, set state to specified.
        uiManager.dialogBox.TearDown();
        dialogFinished = true;
        if (hasTarget)
            cameraManager.SwitchToLastCam();
        stateManager.SetState(finishState);
    }

    public bool IsDialogFinished()
    {
        return dialogFinished;
    }
}


// IEnumerator DialogPlay(string[] lines, float speed)
// {
//     // STEP 1 : Set up, bring dialog box up to screen
//     dialogBoxText.maxVisibleCharacters = 0;
//     dialogBoxPrompt.color = Helper.ChangedAlpha(dialogBoxPrompt.color, 0);
//     Tween t1 = TweenBox("Up", 1f);
//     canvasGroup.DOFade(1f, 1f).From(0f);
//     yield return t1.WaitForCompletion();

//     // STEP 2 : Dialog display and input to go through it.
//     for (int line = 0; line < lines.Length; line++)
//     {
//         dialogBoxText.text = lines[line];
//         for (int i = 0; i <= dialogBoxText.text.Length; i++)
//         {
//             dialogBoxText.maxVisibleCharacters = i;
//             yield return new WaitForSecondsRealtime(speed);
//         }
//         dialogBoxPrompt.enabled = true;
//         Tween t2 = DOTween.To(() => dialogBoxPrompt.color, x => dialogBoxPrompt.color = x, Helper.ChangedAlpha(dialogBoxPrompt.color, 1f), .25f);
//         dialogNext = false;
//         yield return new WaitUntil(() => dialogNext);
//         yield return null; // Must put a frame between inputs
//         if (t2 != null) t2.Kill();
//         dialogBoxPrompt.color = Helper.ChangedAlpha(dialogBoxPrompt.color, 0f);
//     }

//     // STEP 3 : Finish, deconstruct, and send dialog box back down
//     TweenBox("Down", 1f);
//     canvasGroup.DOFade(0f, 0.8f);
//     stateManager.SetState(State.Normal);
//     OnDialogFinish?.Invoke(this, EventArgs.Empty);
// }
