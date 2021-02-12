/* INHERITORS by Nick Perrin (c) 2020 */
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DG.Tweening;
using TMPro;
using UnityEngine.UI;

public class UIManager : MonoBehaviour
{
    StateManager stateManager;
    InteractManager interactManager;
    PickupManager pickupManager;
    AudioManager audioManager;

    GameObject player;
    Vector3 playerHeight;

    bool controllerPlugged = false;

    public UI_PauseMenu pauseMenu = new UI_PauseMenu();
    public UI_TasksInventory tasksInventory = new UI_TasksInventory();
    public UI_Prompt pickupPrompt = new UI_Prompt();
    public UI_Prompt dialogPrompt = new UI_Prompt();
    public UI_DialogBox dialogBox = new UI_DialogBox();
    public UI_Controls controls = new UI_Controls();

    public UIResources uiResources;

    float promptFadeTime = .25f;

    void Awake()
    {
        if (Input.GetJoystickNames().Length > 0)
            controllerPlugged = true;

        print("Controller plugged? " + controllerPlugged);

        InitializeReferences();
        InitializePauseMenu();
        InitializeDialogBox();
        InitializeTasksInventory();
        InitializePrompt(pickupPrompt, "PickupPrompt");
        InitializePrompt(dialogPrompt, "DialogPrompt");
        InitializeControls();
    }

    void Start()
    {
        player = GameObject.FindWithTag("Player");
        playerHeight = new Vector3(0f, player.GetComponent<CapsuleCollider>().height, 0f);
        Cursor.visible = false;
    }

    void UIPop(ref RectTransform rectTransform)
    {
        float popScale = 1.25f;
        float popTime = 0.25f;
        Sequence s = DOTween.Sequence();
        s.Append(rectTransform.DOScale(popScale, popTime).SetEase(Ease.OutQuad))
            .Append(rectTransform.DOScale(Vector3.one, popTime).SetEase(Ease.InQuad));
    }

    void ColorFlash(ref TMP_Text text, Color originalColor, Color flashColor)
    {
        float flashTime = 0.1f;
        float dieOffTime = 0.75f;
        Sequence s = DOTween.Sequence();
        s.Append(text.DOColor(flashColor, flashTime))
            .Append(text.DOColor(originalColor, dieOffTime));
    }

    // ████████████████████████████████████████████████████████████████████████
    // ███ PAUSE MENU
    // ████████████████████████████████████████████████████████████████████████

    public Sequence Pause()
    {
        pauseMenu.Activate();
        Sequence pauseSeq = DOTween.Sequence()
            .Append(pauseMenu.background.DOFade(pauseMenu.backgroundAlpha, pauseMenu.fadeTime))
            .AppendInterval(pauseMenu.betweenFadesTime)
            .Append(pauseMenu.buttonsCG.DOFade(1f, pauseMenu.fadeTime));
        return pauseSeq;
    }

    public Sequence Unpause()
    {
        Sequence unpauseSeq = DOTween.Sequence()
            .Append(pauseMenu.buttonsCG.DOFade(0f, pauseMenu.fadeTime))
            .AppendInterval(pauseMenu.betweenFadesTime)
            .Append(pauseMenu.background.DOFade(0f, pauseMenu.fadeTime))
            .OnComplete(pauseMenu.Deactivate);
        return unpauseSeq;
    }

    public Tween PauseConfirmationUp(string text)
    {
        pauseMenu.confirmText.text = text;
        return pauseMenu.confirmWindowCG.DOFade(1f, pauseMenu.betweenFadesTime);
    }

    public Tween PauseConfirmationDown()
    {
        return pauseMenu.confirmWindowCG.DOFade(0f, pauseMenu.betweenFadesTime);
    }

    // ████████████████████████████████████████████████████████████████████████
    // ███ TASKS / INVENTORY SETUP/TEARDOWN
    // ████████████████████████████████████████████████████████████████████████

    public void SetUpTasksInventory()
    {
        StartCoroutine(SetUpAnimation());
    }

    IEnumerator SetUpAnimation()
    {
        float volumeScale = 0.25f;
        
        tasksInventory.TweenInventory("In", .6f, Ease.OutBack);
        audioManager.PlayOneShot(uiResources.tasksInventorySwishes[0], volumeScale);
        yield return new WaitForSeconds(.3f);
        tasksInventory.TweenActiveBar("In", .6f, Ease.OutBack);
        audioManager.PlayOneShot(uiResources.tasksInventorySwishes[1], volumeScale);
        yield return new WaitForSeconds(.2f);
        tasksInventory.TweenTaskList("In", .6f, Ease.OutQuad);
        audioManager.PlayOneShot(uiResources.tasksInventorySwishes[2], volumeScale);
        yield return new WaitForSeconds(.5f);
        tasksInventory.finishedAnimating = true;
    }

    public void TearDownTasksInventory()
    {
        tasksInventory.finishedAnimating = false;
        StartCoroutine(TearDownAnimation());
    }

    IEnumerator TearDownAnimation()
    {
        tasksInventory.TweenActiveBar("Out", .6f, Ease.InBack);
        yield return new WaitForSeconds(.3f);
        tasksInventory.TweenInventory("Out", .6f, Ease.InBack);
        yield return new WaitForSeconds(.2f);
        tasksInventory.TweenTaskList("Out", .6f, Ease.InQuad);
        yield return new WaitForSeconds(.5f);
        tasksInventory.finishedAnimating = true;
    }

    // ████████████████████████████████████████████████████████████████████████
    // ███ TASKS
    // ████████████████████████████████████████████████████████████████████████

    public void UpdateTasks(Task activeTask, Dictionary<TaskType, Task> taskList, bool completingTask = false)
    {
        if (activeTask.status == TaskStatus.Active)
        {
            tasksInventory.ResetActiveBarTextColor();
            tasksInventory.activeBarTxt.text = activeTask.text;
            tasksInventory.activeBarArrow.DOColor(Color.green, 0.25f);
            UIPop(ref tasksInventory.activeBarRT);
        }
        else if (completingTask)
        {
            // Strikethrough for a completed task
            StartCoroutine(CompleteTaskAnimation());
        }
        else
        {
            tasksInventory.activeBarTxt.text = "";
            tasksInventory.activeBarArrow.DOColor(Color.white, 0.25f);
        }

        string listBuilder = "";
        foreach (Task task in taskList.Values)
        {
            if (task.status == TaskStatus.Waiting)
                listBuilder += task.text + "\n\n";
        }
        if (listBuilder.Length > tasksInventory.taskListTxt.text.Length)
        {
            Color taskListTxtColor = tasksInventory.taskListTxt.color;
            tasksInventory.taskListTxt.DOColor(taskListTxtColor, 0.4f).From(Color.black).SetEase(Ease.InQuad);
            UIPop(ref tasksInventory.taskListRT);
        }
        tasksInventory.taskListTxt.text = listBuilder;
    }

    IEnumerator CompleteTaskAnimation()
    {
        float scaleTime = 1f;
        float volumeScale = 0.75f;
        tasksInventory.strikethruImg.color = Color.black;
        Tween t = tasksInventory.strikethruRT.DOScaleX(1f, scaleTime).SetEase(Ease.InCubic);
        yield return t.WaitForCompletion();

        yield return new WaitForSeconds(0.25f);

        audioManager.PlayOneShot(uiResources.sound_taskCompleted, volumeScale);
        float taskFadeoutTime = 0.5f;
        tasksInventory.activeBarTxt.DOFade(0f, taskFadeoutTime).OnComplete(()=>tasksInventory.activeBarTxt.text = "");
        tasksInventory.strikethruImg.DOFade(0f, taskFadeoutTime).OnComplete(()=>tasksInventory.strikethruRT.DOScaleX(0,0));
        tasksInventory.activeBarArrow.DOColor(Color.white, taskFadeoutTime);
    }

    // ████████████████████████████████████████████████████████████████████████
    // ███ INVENTORY
    // ████████████████████████████████████████████████████████████████████████

    public void UpdateInventory(PickupManager.Inventory inventory)
    {
        float itemFadeTime = 0.25f;

        if (inventory.itemQuantity > 1)
        {
            UIPop(ref tasksInventory.inventoryRT);
            tasksInventory.inventoryTxt.enabled = true;
            tasksInventory.inventoryItemImg.enabled = true;
            tasksInventory.inventoryItemImg.sprite = uiResources.GetItemIcon(inventory.itemType);
            tasksInventory.inventoryTxt.text = "×" + inventory.itemQuantity.ToString();
            tasksInventory.inventoryTxt.DOColor(tasksInventory.inventoryTxt.color, 0.25f).From(Color.cyan);
        }
        else if (inventory.itemQuantity == 1)
        {
            UIPop(ref tasksInventory.inventoryRT);
            tasksInventory.inventoryTxt.enabled = false;
            tasksInventory.inventoryItemImg.enabled = true;
            tasksInventory.inventoryItemImg.sprite = uiResources.GetItemIcon(inventory.itemType);
            tasksInventory.inventoryItemImg.DOFade(1f, itemFadeTime).From(0f);
        }
        else
        {
            tasksInventory.inventoryTxt.enabled = false;
            tasksInventory.inventoryItemImg.enabled = false;
        }
    }

    // ████████████████████████████████████████████████████████████████████████
    // ███ ENTER/EXIT RANGE
    // ████████████████████████████████████████████████████████████████████████

    // void Update()
    // {
    //     if (Input.GetKeyDown(KeyCode.P))
    //     {
    //         pickupPrompt.text.enabled = !pickupPrompt.text.enabled;
    //     }
    // }

    public void EnterRange(Transform target, string triggerType, string prompText = "TEXT NOT PASSED")
    {
        UI_Prompt p;
        if (triggerType == "Pickup")
        {
            p = pickupPrompt;
            p.image.enabled = true;
            p.image.sprite = controllerPlugged ? uiResources.A_Button_Inworld : uiResources.E_Key_Inworld;
            p.text.enabled = false;
        }
        else if (triggerType == "Dropoff")
        {
            p = pickupPrompt;
            p.image.enabled = true;
            p.image.sprite = controllerPlugged ? uiResources.A_Button_Inworld : uiResources.E_Key_Inworld;
            p.text.enabled = true;
            p.text.text = prompText;
        }
        else // (triggerType == "Dialog")
        {
            p = dialogPrompt;
            p.image.enabled = true;
            p.image.sprite = controllerPlugged ? uiResources.Y_Button_Inworld : uiResources.Q_Key_Inworld;
            p.text.enabled = false;
        }

        StartCoroutine(AlignPromptInRange(target, p, triggerType));
    }

    IEnumerator AlignPromptInRange(Transform target, UI_Prompt p, string triggerType)
    {
        if (p.imageTween != null) { p.imageTween.Kill(); }
        p.imageTween = p.ImageAppear(promptFadeTime);
        if (p.textTween != null) { p.textTween.Kill(); }
        if (p.text.enabled) { p.textTween = p.text.DOFade(1f, promptFadeTime).From(0f); }

        Func<bool> TargetInRange = GetInRangeFunction(triggerType);
        bool firstFrameAligned = false;
        while (TargetInRange())
        {
            // Vector3 pos = Camera.main.WorldToScreenPoint(target.position + player.transform.position + player.transform.TransformVector(new Vector3(0f, player.GetComponent<CapsuleCollider>().height, 0f)));
            Vector3 pos = Camera.main.WorldToScreenPoint(target.position + player.transform.TransformVector(playerHeight));
            // if (triggerType == "Dropoff") pos.y += 50; // Account for "DROP" text
            if (!firstFrameAligned)
            {
                p.rectTransform.position = pos;
                firstFrameAligned = true;
            }
            p.rectTransform.position = Vector3.Lerp(p.rectTransform.position, pos, 45 * Time.deltaTime);
            yield return new WaitForFixedUpdate();
        }
    }

    public void ExitRange(Transform target, string triggerType)
    {
        UI_Prompt p;
        if (triggerType == "Pickup")
            p = pickupPrompt;
        else if (triggerType == "Dropoff")
            p = pickupPrompt;
        else // (triggerType == "Dialog")
            p = dialogPrompt;

        StartCoroutine(AlignPromptOutOfRange(target, p, triggerType));
    }

    IEnumerator AlignPromptOutOfRange(Transform target, UI_Prompt p, string triggerType)
    {
        if (p.imageTween != null) p.imageTween.Kill();
        p.imageTween = p.Disappear(promptFadeTime); // p.image.DOFade(0f, promptFadeTime).From(p.image.color.a);
        if (p.textTween != null) p.textTween.Kill();
        if (p.text.enabled) p.textTween = p.text.DOFade(0f, promptFadeTime).From(p.text.alpha);

        bool firstFrameAligned = false;
        while (p.imageTween.IsActive())
        {
            // Vector3 pos = Camera.main.WorldToScreenPoint(player.transform.position + player.transform.TransformVector(new Vector3(0f, player.GetComponent<CapsuleCollider>().height, 0f)));
            Vector3 pos = Camera.main.WorldToScreenPoint(target.position + player.transform.TransformVector(playerHeight));
            if (triggerType == "Dropoff") pos.y += 50; // Account for "DROP" text
            if (!firstFrameAligned) p.rectTransform.position = pos;
            firstFrameAligned = true;
            p.rectTransform.position = Vector3.Lerp(p.rectTransform.position, pos, 45 * Time.deltaTime);
            yield return null;
        }

        Func<bool> TargetInRange = GetInRangeFunction(triggerType);
        if (!TargetInRange())
        {
            p.Hide();
        }
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

    // ████████████████████████████████████████████████████████████████████████
    // ███ INITIALIZERS
    // ████████████████████████████████████████████████████████████████████████

    void InitializeReferences()
    {
        stateManager = FindObjectOfType<StateManager>();
        interactManager = FindObjectOfType<InteractManager>();
        pickupManager = FindObjectOfType<PickupManager>();
        audioManager = FindObjectOfType<AudioManager>();
    }

    void InitializePauseMenu()
    {
        pauseMenu.parent = GameObject.Find("PauseCanvas");

        pauseMenu.background = GameObject.Find("PauseBG").GetComponent<Image>();
        pauseMenu.backgroundAlpha = pauseMenu.background.color.a;
        pauseMenu.background.color = Helper.ChangedAlpha(pauseMenu.background.color, 0);

        pauseMenu.buttonsCG = GameObject.Find("PauseMenu").GetComponent<CanvasGroup>();
        pauseMenu.buttonsCG.alpha = 0;

        pauseMenu.confirmWindowCG = GameObject.Find("ConfirmWindow").GetComponent<CanvasGroup>();
        pauseMenu.confirmWindowCG.alpha = 0;
        pauseMenu.confirmText = GameObject.Find("ConfirmText").GetComponent<TMP_Text>();

        pauseMenu.defaultSelectedButton = GameObject.Find("ResumeButton");
        pauseMenu.restartButton = GameObject.Find("RestartDayButton");
        pauseMenu.quitButton = GameObject.Find("SaveAndQuitButton");
        pauseMenu.confirmNoButton = GameObject.Find("ConfirmNoButton");
    }

    void InitializeDialogBox()
    {
        GameObject db = GameObject.FindGameObjectWithTag("DialogBox");
        dialogBox.canvasGroup = db.GetComponent<CanvasGroup>();
        dialogBox.rectTransform = db.GetComponent<RectTransform>();
        dialogBox.nameBox = GameObject.Find("DialogBoxNameImage");
        dialogBox.nameText = GameObject.Find("DialogBoxNameText").GetComponent<TMP_Text>();
        dialogBox.tmpText = GameObject.Find("DialogBoxText").GetComponent<TMP_Text>();
        GameObject dbp = GameObject.Find("DialogBoxPrompt");
        dialogBox.prompt = dbp.GetComponent<Image>();

        dialogBox.prompt.sprite = controllerPlugged ? uiResources.A_Button : uiResources.E_Key;
        
        dialogBox.animator = dbp.GetComponent<Animator>();
    }

    void InitializeTasksInventory()
    {
        tasksInventory.front = GameObject.Find("TasksInventoryFront").transform;

        GameObject activeBar = GameObject.Find("ActiveBar");
        tasksInventory.activeBarRT = activeBar.GetComponent<RectTransform>();
        tasksInventory.activeBarImg = activeBar.GetComponent<Image>();
        tasksInventory.activeBarArrow = activeBar.transform.GetChild(1).GetComponent<Image>();
        tasksInventory.activeBarTxt = activeBar.transform.GetChild(0).gameObject.GetComponent<TMP_Text>();
        tasksInventory.strikethruRT = activeBar.transform.GetChild(0).GetChild(0).gameObject.GetComponent<RectTransform>();
        tasksInventory.strikethruImg = activeBar.transform.GetChild(0).GetChild(0).gameObject.GetComponent<Image>();

        GameObject taskList = GameObject.Find("TaskList");
        tasksInventory.taskListRT = taskList.GetComponent<RectTransform>();
        tasksInventory.taskListImg = taskList.GetComponent<Image>();
        tasksInventory.taskListTxt = taskList.transform.GetChild(0).gameObject.GetComponent<TMP_Text>();

        GameObject inventory = GameObject.Find("Inventory");
        tasksInventory.inventoryRT = inventory.GetComponent<RectTransform>();
        tasksInventory.inventoryImg = inventory.GetComponent<Image>();
        tasksInventory.inventoryItemImg = inventory.transform.GetChild(0).gameObject.GetComponent<Image>();
        tasksInventory.inventoryTxt = inventory.transform.GetChild(1).gameObject.GetComponent<TMP_Text>();

        tasksInventory.Initialize();
    }

    void InitializePrompt(UI_Prompt prompt, string gameobjectName)
    {
        GameObject pgo = GameObject.Find(gameobjectName);
        prompt.rectTransform = pgo.GetComponent<RectTransform>();
        prompt.ogLocalScale = prompt.rectTransform.localScale;
        prompt.SetYZero();
        prompt.image = pgo.GetComponent<Image>();
        prompt.image.enabled = false;
        prompt.text = GameObject.Find(gameobjectName + "Text").GetComponent<TMP_Text>();
        prompt.text.enabled = false;
    }

    void InitializeControls()
    {
        controls.canvasGroup = GameObject.Find("Controls").GetComponent<CanvasGroup>();
        controls.image = GameObject.Find("ControlsImage").GetComponent<Image>();

        controls.image.sprite = controllerPlugged ? uiResources.joystick : uiResources.WASD;

        controls.text = GameObject.Find("ControlsText").GetComponent<TMP_Text>();
        controls.Hide(0f);
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
//         while (stateManager.GetState() == State.Holding)
//         {
//             Vector3 pos = Camera.main.WorldToScreenPoint(target.position);
//             pos.y -= 25f;
//             pickupPrompt.rectTransform.position = pos;
//             yield return null;
//         }
//     }