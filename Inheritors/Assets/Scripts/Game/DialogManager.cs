/* INHERITORS by Nick Perrin (c) 2020 */
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DG.Tweening;
using TMPro;
using UnityEngine.UI;

public enum Character
{
    Null,
    Mother,
    Father,
    Sister,
    Grandfather,
    Grandmother,
    Narrator
}

// Class for encapsulating dialogs sent from other classes to be played back.
public class Dialog
{
    public Character character;
    public string[] lines;
    public bool skippable = true;
    public Transform target;
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
    PlayerMovement playerMovement;

    Dictionary<Character, string> charNames = new Dictionary<Character, string>();

    bool dialogNext = false;
    bool dialogFinished = true;
    bool dialogAnimationFinished = false;
    int charsPerSecond = 60;

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
        InitializeReferences();
        SubscribeToEvents();
        InitializeCharNames();
        speeds = new float[] { slow, med, fast };
    }

    // Start a new dialog and switch into specified state after dialog finishes
    public void NewDialog(Dialog dialog, State finishState = State.Normal)
    {
        dialogFinished = false;
        stateManager.SetState(State.Dialog);
        playerMovement.Halt();

        StartCoroutine(DialogPlay(dialog, finishState));
    }

    private void HandleInputEvent(object sender, InputManager.ButtonArgs args)
    {
        if (stateManager.GetState() == State.Dialog)
        {
            if (args.buttonCode == InputManager.A)
                dialogNext = true;
        }
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
        Character character = dialog.character;
        string[] lines = dialog.lines;
        bool skippable = dialog.skippable;
        bool hasTarget = dialog.target != null;

        // STEP 1 : Set up, change cam & bring dialog box up to screen
        dialogFinished = false;
        if (hasTarget)
        {
            cameraManager.FocusCamOn("Dialog", dialog.target);
            cameraManager.SwitchToCam("Dialog");
        }
        dialogAnimationFinished = false;
        Tween setup = uiManager.dialogBox.SetUp(character);
        yield return setup.WaitForCompletion();

        // STEP 2 : Dialog display and input to go through it.
        for (int line = 0; line < lines.Length; line++)
        {
            dialogNext = false;
            string nextLine = lines[line];
            uiManager.dialogBox.SetLine(nextLine);
            int runningCharCount = 0;
            // for (int i = 0; i <= nextLine.Length; i++)
            uiManager.dialogBox.tmpText.maxVisibleCharacters = 0;
            while (uiManager.dialogBox.tmpText.maxVisibleCharacters < nextLine.Length)
            {
                int charAddition = (int)(charsPerSecond * Time.deltaTime);
                if (charAddition < 1) { charAddition = 1; }
                runningCharCount = runningCharCount + charAddition;
                if (runningCharCount > nextLine.Length) { runningCharCount = nextLine.Length; }

                uiManager.dialogBox.tmpText.maxVisibleCharacters = runningCharCount;

                if (skippable && dialogNext)
                {
                    uiManager.dialogBox.tmpText.maxVisibleCharacters = nextLine.Length;
                    break;
                }
                // if (i > 0 && nextLine[i - 1] != ' ') { yield return null; }
                yield return null;
            }
            uiManager.dialogBox.ShowPrompt();
            dialogNext = false;
            yield return new WaitUntil(() => dialogNext);
            // yield return null; // Must put a frame between inputs
            uiManager.dialogBox.HidePrompt();
        }

        // STEP 3 : Finish, tear down dialog box, set state to specified.
        Tween tearDown = uiManager.dialogBox.TearDown();
        dialogFinished = true;
        if (hasTarget)
            cameraManager.SwitchToLastCam();
        stateManager.SetState(finishState);

        yield return tearDown.WaitForCompletion();
        dialogAnimationFinished = true;
    }

    public bool IsDialogFinished()
    {
        return dialogFinished;
    }

    public bool IsDialogAnimationFinished()
    {
        return dialogAnimationFinished;
    }

    void InitializeReferences()
    {
        stateManager = FindObjectOfType<StateManager>();
        cameraManager = FindObjectOfType<CameraManager>();
        uiManager = FindObjectOfType<UIManager>();
        inputManager = FindObjectOfType<InputManager>();
        interactManager = FindObjectOfType<InteractManager>();
        playerMovement = FindObjectOfType<PlayerMovement>();
    }

    void SubscribeToEvents()
    {
        inputManager.OnButtonDown += HandleInputEvent;
    }

    void OnDestroy()
    {
        inputManager.OnButtonDown -= HandleInputEvent;
    }

    void InitializeCharNames()
    {
        foreach (Character character in Enum.GetValues(typeof(Character)))
        {
            charNames[character] = character.ToString();
        }
    }
}
