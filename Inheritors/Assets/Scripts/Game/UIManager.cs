using System;
using System.Collections;
using UnityEngine;
using DG.Tweening;
using TMPro;
using UnityEngine.UI;


public class UIManager : MonoBehaviour
{
    DialogManager dialogManager;

    // TODO: a bunch of subclasses of UIManager, one for each UI element, with its own attributes
    // and methods to call to operate it. They don't need to talk to each other, the UIManager is
    // just for displaying stuff, not tracking information (that happens in task, state, pickup, etc).

    public class TaskList
    {

    }

    public DialogBox dialogBox = new DialogBox();
    public class DialogBox
    {
        public CanvasGroup canvasGroup;
        public RectTransform rectTransform;
        public TMP_Text text;
        public Image prompt;
        public Animator animator;
        Tween promptTween = null;
        float moveTime = 1f;
        float fadeTime = 0.8f;
        float yPos = 192f;

        public Tween SetUp()
        {
            text.maxVisibleCharacters = 0;
            prompt.color = Helper.ChangedAlpha(prompt.color, 0);
            Tween t = BringUpDown("Up", moveTime);
            canvasGroup.DOFade(1f, fadeTime).From(0f);
            return t;
        }

        public Tween TearDown()
        {
            prompt.enabled = false;
            Tween t = BringUpDown("Down", moveTime);
            canvasGroup.DOFade(0f, fadeTime);
            return t;
        }

        Tween BringUpDown(string dir, float duration)
        {
            float y = dir == "Up" ? yPos : -yPos;
            return DOTween.To(
                () => rectTransform.anchoredPosition3D,
                x => rectTransform.anchoredPosition3D = x,
                new Vector3(0f, y, 0f),
                duration
            );
        }

        public void SetLine(string line)
        {
            text.text = line;
        }

        public void ShowPrompt()
        {
            prompt.enabled = true;
            promptTween = prompt.DOFade(1f, 0.25f);
        }

        public void HidePrompt()
        {
            if (promptTween != null) promptTween.Kill();
            prompt.color = Helper.ChangedAlpha(prompt.color, 0f);
            prompt.enabled = false;
        }
    }

    CanvasGroup controls; // TODO: fix up with its own class

    void Start()
    {
        InitializeDialogBox();
        controls = GameObject.Find("Controls").GetComponent<CanvasGroup>(); // TODO: fix up with its own class
        // - Set up task header, list, and item carry slot object (all top left UI as one thing)
        // - Set up pickup/drop prompt object
        // - Set up dialog prompt object
        // - Set up controls prompt object
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
