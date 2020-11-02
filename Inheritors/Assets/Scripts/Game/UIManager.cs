using System;
using System.Collections;
using UnityEngine;
using DG.Tweening;
using TMPro;
using UnityEngine.UI;

public class UIManager : MonoBehaviour
{
    StateManager stateManager;
    InteractManager interactManager;
    PickupManager pickupManager;

    public UIResources uiResources;

    public TaskList taskList = new TaskList();
    public class TaskList
    {
        // TODO
    }

    public Prompt pickupPrompt = new Prompt();
    public Prompt dialogPrompt = new Prompt();
    public class Prompt
    {
        public RectTransform rectTransform;
        public Image image;
        public TMP_Text tmpText;
        public Tween currentTween;

        public void Show()
        {
            image.enabled = true;
            tmpText.enabled = true;
        }

        public void Hide()
        {
            image.enabled = false;
            tmpText.enabled = false;
        }

        public void SetSize(float x, float y, float z)
        {
            rectTransform.localScale = new Vector3(x, y, z);
        }
    }


    public DialogBox dialogBox = new DialogBox();
    public class DialogBox
    {
        public CanvasGroup canvasGroup;
        public RectTransform rectTransform;
        public TMP_Text tmpText;
        public Image prompt;
        public Animator animator; // TODO: polish stage: animate the waiting dialog prompt
        Tween promptTween = null;
        float moveTime = 1f;
        float fadeTime = 0.8f;
        float yPos = 192f;

        public Tween SetUp()
        {
            tmpText.maxVisibleCharacters = 0;
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
            tmpText.text = line;
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

    public Controls controls = new Controls();
    public class Controls // Probably just going to be used for joystick, and once only...
    {
        public CanvasGroup canvasGroup;
        public TMP_Text tmpText;

        public void Show(float duration = 1f)
        {
            canvasGroup.DOFade(1f, duration);
        }

        public void Hide(float duration = 1f)
        {
            canvasGroup.DOFade(0f, duration);
        }

        public void SetAlpha(float alpha)
        {
            canvasGroup.alpha = alpha;
        }
    }

    void Awake()
    {
        InitializeReferences();
        InitializeDialogBox();
        InitializePrompt(pickupPrompt, "PickupPrompt");
        InitializePrompt(dialogPrompt, "DialogPrompt");
        InitializeControls();
        // TODO: Post-alpha, set up the nice proper task header, list, and item
        // carry slot object (all top left UI as one thing with multiple pieces?)
    }

    // ████████████████████████████████████████████████████████████████████████
    // ███ TEMPORARY INVENTORY (until we have that stuff in fully)
    // ████████████████████████████████████████████████████████████████████████

    // TODO: everything inside this boundary is hacky as fuck right now. Fix'er!
    // Maybe some kind of InitializeUI() function to kick off every scene, called
    // in the Start() method.

    Image pickupStatusImage;
    Text pickupStatusText;

    public void UpdateHolding(PickupManager.ItemTypes type, int quantity)
    {
        pickupStatusImage.enabled = true;
        pickupStatusImage.sprite = uiResources.GetItemIcon(type);
        pickupStatusText.enabled = true;
        pickupStatusText.text = "×" + quantity.ToString();
    }

    // ████████████████████████████████████████████████████████████████████████

    public void EnterRange(Transform target, string type)
    {
        Prompt p = type == "Pickup" ? pickupPrompt : dialogPrompt;
        if (p.currentTween != null) p.currentTween.Kill();
        p.image.enabled = true;
        p.image.sprite = type == "Pickup" ? uiResources.A_Button : uiResources.Y_Button;
        p.currentTween = p.image.DOFade(1f, .25f).From(0f);
        StartCoroutine(AlignPromptInRange(target, p, type));
    }

    IEnumerator AlignPromptInRange(Transform target, Prompt p, string type)
    {
        Func<bool> TargetInRange;
        if (type == "Pickup")
            TargetInRange = interactManager.IsPickupInRange;
        else
            TargetInRange = interactManager.IsDialogInRange;
        while (TargetInRange())
        {
            Vector3 pos = Camera.main.WorldToScreenPoint(target.position);
            // pos.y += 100f;
            p.rectTransform.position = pos;
            yield return null;
        }
        p.currentTween.Kill();
        p.image.enabled = false;
    }

    public void ExitRange(Transform target, string type)
    {
        Prompt p = type == "Pickup" ? pickupPrompt : dialogPrompt;
        StartCoroutine(AlignPromptOutOfRange(target.position, p));
    }

    IEnumerator AlignPromptOutOfRange(Vector3 targetPos, Prompt p)
    {
        Tween t = p.image.DOFade(0f, .25f);
        p.currentTween = t;
        while (t != null & t.IsPlaying())
        {
            Vector3 pos = Camera.main.WorldToScreenPoint(targetPos);
            // pos.y += 100f;
            p.rectTransform.position = pos;
            yield return null;
        }
        p.image.enabled = false;
    }

    void InitializeReferences()
    {
        stateManager = FindObjectOfType<StateManager>();
        interactManager = FindObjectOfType<InteractManager>();
        pickupManager = FindObjectOfType<PickupManager>();

        // TODO: this is part of the hacky stuff we are removing above in temp inventory
        pickupStatusImage = GameObject.Find("PickupStatusImage").GetComponent<Image>();
        pickupStatusText = GameObject.Find("PickupStatusText").GetComponent<Text>();
        pickupStatusImage.sprite = null;
        pickupStatusImage.enabled = false;
        pickupStatusText.enabled = false;
    }

    void InitializeDialogBox()
    {
        GameObject db = GameObject.FindGameObjectWithTag("DialogBox");
        dialogBox.canvasGroup = db.GetComponent<CanvasGroup>();
        dialogBox.rectTransform = db.GetComponent<RectTransform>();
        dialogBox.tmpText = GameObject.Find("DialogBoxText").GetComponent<TMP_Text>();
        GameObject dbp = GameObject.Find("DialogBoxPrompt");
        dialogBox.prompt = dbp.GetComponent<Image>();
        dialogBox.animator = dbp.GetComponent<Animator>();
    }

    void InitializePrompt(Prompt prompt, string gameobjectName)
    {
        GameObject pgo = GameObject.Find(gameobjectName);
        prompt.rectTransform = pgo.GetComponent<RectTransform>();
        prompt.image = pgo.GetComponent<Image>();
        prompt.image.enabled = false;
        prompt.tmpText = GameObject.Find(gameobjectName + "Text").GetComponent<TMP_Text>();
        prompt.tmpText.enabled = false;
    }

    void InitializeControls()
    {
        controls.canvasGroup = GameObject.Find("Controls").GetComponent<CanvasGroup>();
        controls.tmpText = GameObject.Find("ControlsText").GetComponent<TMP_Text>();
    }
}


// Functions formerly in use for when you could drop anywhere, and a prompt followed you.

//   public void HoldingItem(Transform target)
//     {
//         StartCoroutine(HoldingItemProcess(target));
//     }

//     IEnumerator HoldingItemProcess(Transform target)
//     {
//         pickupPrompt.rectTransform.localScale = new Vector3(.6f, .6f, 1f);
//         pickupPrompt.image.enabled = true;
//         pickupPrompt.tmpText.enabled = true;
//         pickupPrompt.image.sprite = uiResources.X_Button;
//         while (stateManager.state == StateManager.State.Holding)
//         {
//             Vector3 pos = Camera.main.WorldToScreenPoint(target.position);
//             pos.y -= 25f;
//             pickupPrompt.rectTransform.position = pos;
//             yield return null;
//         }
//     }