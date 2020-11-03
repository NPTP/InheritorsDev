using System;
using System.Collections;
using UnityEngine;
using DG.Tweening;
using TMPro;
using UnityEngine.UI;

// Class for encapsulating dialogs sent from other classes to be played back.
public class Dialog
{
    public string[] lines;
    public float speed;

    public Dialog()
    {
        this.speed = DialogManager.Speed.FAST;
    }
}

// The dialog manager handles all dialog events and sends info the UI to control it.
public class DialogManager : MonoBehaviour
{
    StateManager stateManager;
    InputManager inputManager;
    InteractManager interactManager;
    UIManager uiManager;

    float dialogBoxYPosition = 192f;
    bool dialogNext = false;
    bool dialogFinished = true;

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

    void Awake()
    {
        stateManager = FindObjectOfType<StateManager>();
        uiManager = FindObjectOfType<UIManager>();
        inputManager = FindObjectOfType<InputManager>();
        inputManager.OnButtonDown += HandleInputEvent;
        interactManager = FindObjectOfType<InteractManager>();
    }

    // Start a new dialog and switch into specified state after dialog finishes
    public void NewDialog(Dialog dialog, StateManager.State finishState = StateManager.State.Normal)
    {
        dialogFinished = false;
        stateManager.SetState(StateManager.State.Dialog);
        StartCoroutine(DialogPlay(dialog.lines, dialog.speed, finishState));
    }

    private void HandleInputEvent(object sender, InputManager.ButtonArgs args)
    {
        if (stateManager.GetState() == StateManager.State.Dialog && args.buttonCode == InputManager.A)
            dialogNext = true;
    }

    public void EndDialog()
    {
        if (stateManager.GetState() == StateManager.State.Dialog && !dialogFinished)
        {
            uiManager.dialogBox.TearDown();
            dialogFinished = true;
        }
    }

    IEnumerator DialogPlay(string[] lines, float speed, StateManager.State finishState)
    {
        // STEP 1 : Set up, bring dialog box up to screen
        dialogFinished = false;
        Tween setup = uiManager.dialogBox.SetUp();
        yield return new WaitWhile(() => setup != null && setup.IsPlaying());

        // STEP 2 : Dialog display and input to go through it.
        for (int line = 0; line < lines.Length; line++)
        {
            uiManager.dialogBox.SetLine(lines[line]);
            for (int i = 0; i <= lines[line].Length; i++)
            {
                uiManager.dialogBox.tmpText.maxVisibleCharacters = i;
                yield return new WaitForSeconds(speed);
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
        stateManager.SetState(finishState);
    }

    public bool IsDialogFinished()
    {
        return dialogFinished;
    }

    // Unsubscribe from all events
    void OnDestroy()
    {
        inputManager.OnButtonDown -= HandleInputEvent;
    }
}


// IEnumerator DialogPlay(string[] lines, float speed)
// {
//     // STEP 1 : Set up, bring dialog box up to screen
//     dialogBoxText.maxVisibleCharacters = 0;
//     dialogBoxPrompt.color = Helper.ChangedAlpha(dialogBoxPrompt.color, 0);
//     Tween t1 = TweenBox("Up", 1f);
//     canvasGroup.DOFade(1f, 1f).From(0f);
//     yield return new WaitWhile(() => t1.IsPlaying());

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
//     stateManager.SetState(StateManager.State.Normal);
//     OnDialogFinish?.Invoke(this, EventArgs.Empty);
// }
