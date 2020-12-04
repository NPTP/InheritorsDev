﻿/* INHERITORS by Nick Perrin (c) 2020 */
// UI elements for use inside UIManager only.
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using DG.Tweening;

// ████████████████████████████████████████████████████████████████████████
// ███ UI_TasksInventory
// ████████████████████████████████████████████████████████████████████████

public class UI_TasksInventory
{
    public Transform front; // For pulling UI element to the front.

    Vector3 activeBarStartPos; // For use with rect transform.
    Vector3 activeBarEndPos;   // For use with rect transform.
    public RectTransform activeBarRT;
    public Image activeBarImg;
    public Image activeBarArrow;
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
        // The position numbers in here come from working on the canvas visually.
        activeBarEndPos = activeBarRT.anchoredPosition3D;
        activeBarStartPos = new Vector3(-400, activeBarEndPos.y, activeBarEndPos.z);
        activeBarRT.anchoredPosition3D = activeBarStartPos;

        taskListEndPos = taskListRT.anchoredPosition3D;
        taskListStartPos = new Vector3(-269, taskListEndPos.y, taskListEndPos.z);
        taskListRT.anchoredPosition3D = taskListStartPos;

        inventoryEndPos = inventoryRT.anchoredPosition3D;
        inventoryStartPos = new Vector3(inventoryEndPos.x, 126, inventoryEndPos.z);
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
    public TMP_Text nameText;
    public TMP_Text tmpText;
    public Image prompt;
    public Animator animator; // TODO: polish stage: animate the waiting dialog prompt
    float moveUpTime = 0.5f;
    float fadeUpTime = 0.4f;
    float moveDownTime = 1f;
    float fadeDownTime = 0.8f;
    float yPosUp = 145f;
    float yPosDown = -315.11f;
    Dictionary<string, Color> nameColors = new Dictionary<string, Color>();

    public UI_DialogBox()
    {
        nameColors["mother"] = Color.white;
        nameColors["father"] = Color.blue;
        nameColors["sister"] = Color.yellow;
        nameColors["grandmother"] = Color.gray;
        nameColors["grandfather"] = Color.green;
    }

    public Tween SetUp(Character character)
    {
        string name = character.ToString();
        nameText.text = name.ToUpper();
        nameText.faceColor = nameColors[name.ToLower()];
        tmpText.maxVisibleCharacters = 0;
        prompt.enabled = true;
        // prompt.gameObject.GetComponent<RectTransform>().localScale = new Vector3(0, 0, 1);
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
        float y = dir == "Up" ? yPosUp : yPosDown;
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
        // prompt.enabled = true;
        animator.SetBool("animate", true);
    }

    public void HidePrompt()
    {
        animator.SetBool("animate", false);
        // prompt.enabled = false;
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
    public Tween imageTween;
    public Tween textTween;
    public Vector3 ogLocalScale;

    public void Hide()
    {
        image.enabled = false;
        text.enabled = false;
    }

    public Tween ImageAppear(float time)
    {
        image.enabled = true;
        image.color = Helper.ChangedAlpha(image.color, 1);
        return rectTransform.DOScaleY(ogLocalScale.y, time).SetEase(Ease.OutBounce);
    }

    public Tween Disappear(float time)
    {
        image.color = Helper.ChangedAlpha(image.color, 1);
        return rectTransform.DOScaleY(0f, time * .5f).SetEase(Ease.InBack);
    }

    public void SetYZero()
    {
        rectTransform.localScale = new Vector3(rectTransform.localScale.x, 0f, rectTransform.localScale.z);
    }

    public void SetSize(float x, float y, float z)
    {
        rectTransform.localScale = new Vector3(x, y, z);
    }
}

// ████████████████████████████████████████████████████████████████████████
// ███ UI_Controls
// ████████████████████████████████████████████████████████████████████████

public class UI_Controls
{
    public CanvasGroup canvasGroup;
    public Image image;
    public TMP_Text text;

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