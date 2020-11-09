// UI elements for use inside UIManager only.

using UnityEngine;
using UnityEngine.UI;
using TMPro;
using DG.Tweening;

// ████████████████████████████████████████████████████████████████████████
// ███ UI_TasksInventory
// ████████████████████████████████████████████████████████████████████████

public class UI_TasksInventory
{
    Vector3 activeBarStartPos; // For use with rect transform.
    Vector3 activeBarEndPos;   // For use with rect transform.
    public RectTransform activeBarRT;
    public Image activeBarImg;
    public TMP_Text activeBarTxt;
    public RectTransform strikethruRT;
    public Image strikethruImg;

    Vector3 taskListStartPos; // For use with rect transform.
    Vector3 taskListEndPos; // For use with rect transform.
    public RectTransform taskListRT;
    public Image taskListImg;
    public TMP_Text taskListTxt;

    Vector3 inventoryStartPos; // For use with rect transform.
    Vector3 inventoryEndPos; // For use with rect transform.
    public RectTransform inventoryRT;
    public Image inventoryImg;
    public Image inventoryItemImg;
    public TMP_Text inventoryTxt;

    public bool finishedAnimating = false;

    public void Initialize()
    {
        activeBarEndPos = activeBarRT.anchoredPosition3D;
        activeBarStartPos = new Vector3(-201, activeBarEndPos.y, activeBarEndPos.z);
        activeBarRT.anchoredPosition3D = activeBarStartPos;

        taskListEndPos = taskListRT.anchoredPosition3D;
        taskListStartPos = new Vector3(-136, taskListEndPos.y, taskListEndPos.z);
        taskListRT.anchoredPosition3D = taskListStartPos;

        inventoryEndPos = inventoryRT.anchoredPosition3D;
        inventoryStartPos = new Vector3(inventoryEndPos.x, 65, inventoryEndPos.z);
        inventoryRT.anchoredPosition3D = inventoryStartPos;

        inventoryItemImg.enabled = false;
        inventoryTxt.enabled = false;
    }

    public void TweenInventory(string inOut, float duration, Ease easing)
    {
        Vector3 endPos;
        if (inOut == "In") endPos = inventoryEndPos;
        else endPos = inventoryStartPos;
        inventoryRT.DOAnchorPos3D(
            endPos, duration
        ).SetEase(easing);
    }

    public void TweenActiveBar(string inOut, float duration, Ease easing)
    {
        Vector3 endPos;
        if (inOut == "In") endPos = activeBarEndPos;
        else endPos = activeBarStartPos;
        activeBarRT.DOAnchorPos3D(
            endPos, duration
        ).SetEase(easing);
    }

    public void TweenTaskList(string inOut, float duration, Ease easing)
    {
        Vector3 endPos;
        if (inOut == "In") endPos = taskListEndPos;
        else endPos = taskListStartPos;
        taskListRT.DOAnchorPos3D(
            endPos, duration
        ).SetEase(easing);
    }
}

// ████████████████████████████████████████████████████████████████████████
// ███ UI_DialogBox
// ████████████████████████████████████████████████████████████████████████

public class UI_DialogBox
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
        prompt.enabled = false;
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

    public Tween ShowPrompt()
    {
        prompt.enabled = true;
        promptTween = prompt.DOFade(1f, 0.25f);
        return promptTween;
    }

    public void HidePrompt()
    {
        if (promptTween != null) promptTween.Kill();
        prompt.color = Helper.ChangedAlpha(prompt.color, 0f);
        StopPromptWaitAnim();
        prompt.enabled = false;
    }

    public void StartPromptWaitAnim()
    {
        animator.SetBool("animate", true);
    }

    public void StopPromptWaitAnim()
    {
        animator.SetBool("animate", false);
    }
}

// ████████████████████████████████████████████████████████████████████████
// ███ UI_Prompt
// ████████████████████████████████████████████████████████████████████████

public class UI_Prompt
{
    public RectTransform rectTransform;
    public Image image;
    public TMP_Text text;
    public Tween currentTween;

    public void Show()
    {
        image.enabled = true;
        text.enabled = true;
    }

    public void Hide()
    {
        image.enabled = false;
        text.enabled = false;
    }

    public void SetSize(float x, float y, float z)
    {
        rectTransform.localScale = new Vector3(x, y, z);
    }
}

// ████████████████████████████████████████████████████████████████████████
// ███ UI_Controls
// ████████████████████████████████████████████████████████████████████████

// Probably just going to be used for joystick, and once only. Could replace with a UI_Prompt.
public class UI_Controls
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