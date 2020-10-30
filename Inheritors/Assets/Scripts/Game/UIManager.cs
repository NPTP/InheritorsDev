using System;
using System.Collections;
using UnityEngine;
using DG.Tweening;
using TMPro;
using UnityEngine.UI;


public class UIManager : MonoBehaviour
{
    DialogManager dialogManager;

    public event EventHandler OnDialogFinish;

    // TODO: a bunch of subclasses of UIManager, one for each UI element, with its own attributes
    // and methods to call to operate it. They don't need to talk to each other, the UIManager is
    // just for displaying stuff, not tracking information (that happens in task, state, pickup, etc).

    DialogBox dialogBox = new DialogBox();
    public class DialogBox
    {
        public CanvasGroup canvasGroup;
        public RectTransform rectTransform;
        public TMP_Text text;
        public Image prompt;
        public Animator animator;
        float yPos = 192f;
    }

    CanvasGroup controls; // TODO: fix up with its own class

    void Start()
    {
        InitializeDialogBox();
        controls = GameObject.Find("Controls").GetComponent<CanvasGroup>(); // TODO: fix up with its own class
        // - Set up task list object
        // - Set up item carry slot object
        // - Set up pickup/drop prompt object
        // - Set up dialog prompt object
        // That's it I think?
    }

    public void ShowControls(float duration = 1f)
    {
        controls.DOFade(1f, duration);
    }

    public void HideControls(float duration = 1f)
    {
        controls.DOFade(0f, duration);
    }


    public void ControlsSetAlpha(float alpha)
    {
        controls.alpha = alpha;
    }

    // IEnumerator DialogPlay(string[] lines, float speed)
    // {
    //     // STEP 1 : Set up, bring dialog box up to screen
    //     dialogText.maxVisibleCharacters = 0;
    //     dialogPrompt.color = Helper.ChangedAlpha(dialogPrompt.color, 0);
    //     Tween t1 = TweenBox("Up", 1f);
    //     canvasGroup.DOFade(1f, 1f).From(0f);
    //     yield return new WaitWhile(() => t1.IsPlaying());

    //     // STEP 2 : Dialog display and input to go through it.
    //     for (int line = 0; line < lines.Length; line++)
    //     {
    //         dialogText.text = lines[line];
    //         for (int i = 0; i <= dialogText.text.Length; i++)
    //         {
    //             dialogText.maxVisibleCharacters = i;
    //             yield return new WaitForSecondsRealtime(speed);
    //         }
    //         dialogPrompt.enabled = true;
    //         Tween t2 = DOTween.To(() => dialogPrompt.color, x => dialogPrompt.color = x, Helper.ChangedAlpha(dialogPrompt.color, 1f), .25f);
    //         dialogNext = false;
    //         yield return new WaitUntil(() => dialogNext);
    //         yield return null; // Must put a frame between inputs
    //         if (t2 != null) t2.Kill();
    //         dialogPrompt.color = Helper.ChangedAlpha(dialogPrompt.color, 0f);
    //     }

    //     // STEP 3 : Finish, deconstruct, and send dialog box back down
    //     TweenBox("Down", 1f);
    //     canvasGroup.DOFade(0f, 0.8f);
    //     stateManager.SetState(StateManager.State.Normal);
    //     OnDialogFinish?.Invoke(this, EventArgs.Empty);
    // }

    // Tween TweenBox(string dir, float duration)
    // {
    //     float yPos = dir == "Up" ? dialogBoxYPosition : -dialogBoxYPosition;
    //     return DOTween.To(
    //         () => rectTransform.anchoredPosition3D,
    //         x => rectTransform.anchoredPosition3D = x,
    //         new Vector3(0f, yPos, 0f),
    //         duration
    //     );
    // }

    // Unsubscribe from all events
    void OnDestroy()
    {

    }

    void InitializeDialogBox()
    {
        GameObject dbgo = GameObject.FindGameObjectWithTag("DialogBox");
        dialogBox.canvasGroup = dbgo.GetComponent<CanvasGroup>();
        dialogBox.rectTransform = dbgo.GetComponent<RectTransform>();
        dialogBox.text = GameObject.Find("DialogText").GetComponent<TMP_Text>();
        dialogBox.prompt = GameObject.Find("DialogPrompt").GetComponent<Image>();
        dialogBox.animator = GameObject.Find("DialogPrompt").GetComponent<Animator>();
    }
}
