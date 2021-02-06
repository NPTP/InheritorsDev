using System.Collections;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;
using DG.Tweening;

// This script handles the buttons state machine of the main menu.
public class MainMenu : MonoBehaviour
{
    SceneLoader sceneLoader;
    AudioSource audioSource;
    float fadeTime = .25f;
    bool saveExists = false;

    public AnimationClip menuStartAnim;
    public AnimationClip menuQuickstartAnim;
    public Animator UIAnimator;
    public CanvasGroup buttonGroup;
    public GameObject defaultSelectedButton;

    [Header("Main buttons")]
    public CanvasGroup mainButtonCG;
    public GameObject beginButton;
    public GameObject continueButton;
    public GameObject endButton;

    [Header("Are you sure BEGIN buttons")]
    public CanvasGroup beginSureCG;
    public GameObject beginSureYesButton;
    public GameObject beginSureNoButton;

    [Header("Are you sure END buttons")]
    public CanvasGroup endSureCG;
    public GameObject endSureYesButton;
    public GameObject endSureNoButton;

    void Awake()
    {
        audioSource = GetComponent<AudioSource>();
        sceneLoader = GameObject.Find("SceneLoader").GetComponent<SceneLoader>();

        ReturningFromGame returningFromGame = FindObjectOfType<ReturningFromGame>();
        if (returningFromGame)
        {
            UIAnimator.SetTrigger("QuickStart");
            menuStartAnim = menuQuickstartAnim;
            sceneLoader.inFadeDuration = menuStartAnim.length;
            Destroy(returningFromGame);
        }
    }

    void Start()
    {

        buttonGroup.interactable = false;
        beginSureCG.alpha = 0;
        endSureCG.alpha = 0;

        if (PlayerPrefs.HasKey("saveExists"))
            saveExists = true;

        // Change button navigation if continue button is greyed out.
        if (!saveExists)
        {
            Button begin = beginButton.GetComponent<Button>();
            Button end = endButton.GetComponent<Button>();

            Navigation beginNav = begin.navigation;
            beginNav.selectOnDown = end;
            begin.navigation = beginNav;

            Navigation endNav = end.navigation;
            endNav.selectOnUp = begin;
            end.navigation = endNav;
        }

        StartCoroutine(WaitForStartAnimation());
    }

    IEnumerator WaitForStartAnimation()
    {
        float threeQuarters = 0.75f;
        yield return new WaitForSeconds(menuStartAnim.length * threeQuarters);
        buttonGroup.interactable = true;
        mainButtonCG.interactable = true;
        SelectObject(defaultSelectedButton);
    }

    IEnumerator ShowConfirmation(CanvasGroup fromCG, CanvasGroup toCG, GameObject selectedButton)
    {
        fromCG.interactable = false;
        audioSource.Play();
        yield return toCG.DOFade(1f, fadeTime).WaitForCompletion();
        toCG.interactable = true;
        SelectObject(selectedButton);
    }

    IEnumerator HideConfirmation(CanvasGroup fromCG, CanvasGroup toCG, GameObject selectedButton)
    {
        fromCG.interactable = false;
        audioSource.Play();
        yield return fromCG.DOFade(0f, fadeTime).WaitForCompletion();
        toCG.interactable = true;
        SelectObject(selectedButton);
    }

    public void Begin()
    {
        if (saveExists)
            StartCoroutine(ShowConfirmation(mainButtonCG, beginSureCG, beginSureNoButton));
        else
            StartGame();
    }

    public void BeginYes()
    {
        StartGame();
    }

    public void StartGame()
    {
        buttonGroup.interactable = false;
        audioSource.Play();
        UIAnimator.SetTrigger("End");
        sceneLoader.LoadSceneByName("Day0");
    }

    public void BeginNo()
    {
        StartCoroutine(HideConfirmation(beginSureCG, mainButtonCG, beginButton));
    }

    public void ContinueGame()
    {
        buttonGroup.interactable = false;
        audioSource.Play();

        PlayerPrefs.SetInt("continuing", 1);
        PlayerPrefs.Save();
        int savedDayNumber = PlayerPrefs.GetInt("savedDayNumber");
        string sceneName = "Day" + savedDayNumber.ToString();
        sceneLoader.LoadSceneByName(sceneName);
    }

    public void End()
    {
        StartCoroutine(ShowConfirmation(mainButtonCG, endSureCG, endSureNoButton));
    }

    public void EndYes()
    {
        buttonGroup.interactable = false;
        Application.Quit();
#if UNITY_EDITOR
        UnityEditor.EditorApplication.isPlaying = false;
#endif
    }

    public void EndNo()
    {
        StartCoroutine(HideConfirmation(endSureCG, mainButtonCG, endButton));
    }

    void SelectObject(GameObject obj)
    {
        EventSystem.current.SetSelectedGameObject(obj);
    }

}

// void Update()
// {
//     if (EventSystem.current.currentSelectedGameObject == null)
//     {
//         EventSystem.current.SetSelectedGameObject(defaultSelectedButton);
//     }
// }