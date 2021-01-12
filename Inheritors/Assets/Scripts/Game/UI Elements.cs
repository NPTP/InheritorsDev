/* INHERITORS by Nick Perrin (c) 2020 */
// UI elements for use inside UIManager only.
using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using DG.Tweening;

// ████████████████████████████████████████████████████████████████████████
// ███ UI_PauseMenu
// ████████████████████████████████████████████████████████████████████████

public class UI_PauseMenu
{
    public GameObject parent;

    public Image background;
    public float backgroundAlpha;
    public CanvasGroup buttonsCG;
    public GameObject defaultSelectedButton;
    public GameObject restartButton;
    public GameObject quitButton;
    public GameObject confirmNoButton;

    public CanvasGroup confirmWindowCG;
    public TMP_Text confirmText;

    public float fadeTime = .25f;
    public float betweenFadesTime = .125f;

    public void Activate()
    {
        parent.SetActive(true);
    }

    public void Deactivate()
    {
        parent.SetActive(false);
    }
}

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
    public GameObject nameBox;
    public TMP_Text nameText;
    public TMP_Text tmpText;
    public Image prompt;
    public Animator animator;
    float moveUpTime = 0.5f;
    float fadeUpTime = 0.4f;
    float moveDownTime = 1f;
    float fadeDownTime = 0.8f;
    float yPosUp = 145f;
    float yPosDown = -315.11f;
    Dictionary<Character, Color> nameColors = new Dictionary<Character, Color>();
    Dictionary<Character, string> nameStrings = new Dictionary<Character, string>();

    public UI_DialogBox()
    {
        ApplyNameColors();
        ApplyNameStrings();
    }

    public Tween SetUp(Character character)
    {
        nameText.text = nameStrings[character].ToUpper();
        nameText.faceColor = nameColors[character];

        nameBox.SetActive(character == Character.Narrator ? false : true);
        tmpText.maxVisibleCharacters = 0;
        prompt.enabled = true;
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

    void ApplyNameColors()
    {
        nameColors[Character.Null] = Color.white;
        nameColors[Character.Mother] = Helper.RGBToColor(117, 36, 36);  // Maroon
        nameColors[Character.Father] = Helper.RGBToColor(0, 7, 135);    // Dark blue
        nameColors[Character.Sister] = Helper.RGBToColor(252, 186, 3);  // Orange
        nameColors[Character.Grandfather] = Helper.RGBToColor(50, 168, 82); // Mild green
        nameColors[Character.Grandmother] = Helper.RGBToColor(110, 0, 95);  // Dark purple
        nameColors[Character.Narrator] = Color.gray;
        nameColors[Character.Manofhole] = Color.red;
    }

    void ApplyNameStrings()
    {
        foreach (Character character in Enum.GetValues(typeof(Character)))
        {
            string name = "";

            if (character == Character.Manofhole)
                name = "Strange man";
            else if (character == Character.Narrator)
                name = "";
            else
                name = character.ToString();

            nameStrings[character] = name.ToUpper();
        }
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