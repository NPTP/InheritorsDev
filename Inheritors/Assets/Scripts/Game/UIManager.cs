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
        float moveUpTime = 0.5f;
        float fadeUpTime = 0.4f;
        float moveDownTime = 1f;
        float fadeDownTime = 0.8f;
        float yPos = 192f;

        public Tween SetUp()
        {
            tmpText.maxVisibleCharacters = 0;
            prompt.color = Helper.ChangedAlpha(prompt.color, 0);
            Tween t = BringUpDown("Up", moveUpTime);
            canvasGroup.DOFade(1f, fadeUpTime).From(0f);
            return t;
        }

        public Tween TearDown()
        {
            prompt.enabled = false;
            Tween t = BringUpDown("Down", moveDownTime);
            canvasGroup.DOFade(0f, fadeDownTime);
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

    public void UpdateInventory(PickupManager.Inventory inventory)
    {
        if (inventory.itemQuantity > 0)
        {
            pickupStatusText.enabled = true;
            pickupStatusImage.enabled = true;
            pickupStatusImage.sprite = uiResources.GetItemIcon(inventory.itemType);
            pickupStatusText.text = "×" + inventory.itemQuantity.ToString();
        }
        else
        {
            pickupStatusText.enabled = false;
            pickupStatusImage.enabled = false;
        }
    }

    // ████████████████████████████████████████████████████████████████████████

    public void EnterRange(Transform target, string triggerType)
    {
        Prompt p;
        if (triggerType == "Pickup")
        {
            p = pickupPrompt;
            p.image.enabled = true;
            p.image.sprite = uiResources.A_Button;
        }
        else if (triggerType == "Dropoff")
        {
            p = pickupPrompt;
            p.image.enabled = true;
            p.image.sprite = uiResources.X_Button;
        }
        else // (triggerType == "Dialog")
        {
            p = dialogPrompt;
            p.image.enabled = true;
            p.image.sprite = uiResources.Y_Button;
        }

        StartCoroutine(AlignPromptInRange(target, p, triggerType));
    }

    IEnumerator AlignPromptInRange(Transform target, Prompt p, string triggerType)
    {
        if (p.currentTween != null) p.currentTween.Kill();
        p.currentTween = p.image.DOFade(1f, .25f).From(0f);

        Func<bool> TargetInRange = GetInRangeFunction(triggerType);
        while (TargetInRange())
        {
            Vector3 pos = Camera.main.WorldToScreenPoint(target.position);
            pos.y += 50;
            p.rectTransform.position = pos;
            yield return new WaitForFixedUpdate();
        }
    }

    public void ExitRange(Transform target, string triggerType)
    {
        Prompt p;
        if (triggerType == "Pickup")
            p = pickupPrompt;
        else if (triggerType == "Dropoff")
            p = pickupPrompt;
        else // (triggerType == "Dialog")
            p = dialogPrompt;

        StartCoroutine(AlignPromptOutOfRange(target, p, triggerType));
    }

    IEnumerator AlignPromptOutOfRange(Transform target, Prompt p, string triggerType)
    {
        if (p.currentTween != null) p.currentTween.Kill();
        p.currentTween = p.image.DOFade(0f, .25f).From(p.image.color.a);
        while (p.currentTween != null & p.currentTween.IsPlaying())
        {
            Vector3 pos = Camera.main.WorldToScreenPoint(target.position);
            pos.y += 50f;
            p.rectTransform.position = pos;
            yield return null;
        }

        Func<bool> TargetInRange = GetInRangeFunction(triggerType);
        if (!TargetInRange())
            p.image.enabled = false;
    }

    Func<bool> GetInRangeFunction(string triggerType)
    {
        Func<bool> InRangeFunc;
        if (triggerType == "Pickup")
            InRangeFunc = interactManager.IsPickupInRange;
        else if (triggerType == "Dropoff")
            InRangeFunc = interactManager.IsDropoffInRange;
        else // (triggerType == "Dialog")
            InRangeFunc = interactManager.IsDialogInRange;

        return InRangeFunc;
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