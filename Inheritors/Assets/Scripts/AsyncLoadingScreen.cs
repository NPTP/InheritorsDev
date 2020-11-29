using System.Collections;
using UnityEngine;
using UnityEngine.UI;
using DG.Tweening;
using UnityEngine.SceneManagement;

public class AsyncLoadingScreen : MonoBehaviour
{
    AudioSource audioSource;
    CanvasGroup textContainerCG;
    CanvasGroup dayTextCG;
    Text dayText;
    CanvasGroup loadingTextCG;
    Text loadingText;
    CanvasGroup finishedTextCG;
    Text finishedText;

    [Header("Options")]
    public float dayTextFadeUpTime = 1f;
    public float timeBetweenDayAndLoading = 1f;
    public float loadingTextFadeUpTime = 1f;
    public float minimumHoldTime = 2f;
    public float allFadeOutTime = 1f;

    [Space]

    [Header("Audio")]
    public AudioClip confirmSound;

    void Start()
    {
        audioSource = GetComponent<AudioSource>();

        textContainerCG = GameObject.Find("TextContainer").GetComponent<CanvasGroup>();

        GameObject dt = GameObject.Find("DayText");
        dayTextCG = dt.GetComponent<CanvasGroup>();
        dayText = dt.GetComponent<Text>();

        GameObject lt = GameObject.Find("LoadingText");
        loadingTextCG = lt.GetComponent<CanvasGroup>();
        loadingText = lt.GetComponent<Text>();

        GameObject ft = GameObject.Find("FinishedText");
        finishedTextCG = ft.GetComponent<CanvasGroup>();
        finishedText = ft.GetComponent<Text>();

        PlayerPrefs.SetInt("currentDayNumber", 0);
        StartCoroutine(LoadingProcess());
    }

    IEnumerator LoadingProcess()
    {
        dayTextCG.alpha = 0;
        loadingTextCG.alpha = 0;
        finishedTextCG.alpha = 0;

        string nextDayNumberString = (PlayerPrefs.GetInt("currentDayNumber", -101) + 1).ToString();
        dayText.text = "day " + nextDayNumberString;

        yield return new WaitForSeconds(1f);

        dayTextCG.DOFade(1f, dayTextFadeUpTime);

        yield return new WaitForSeconds(timeBetweenDayAndLoading);

        loadingTextCG.DOFade(1f, loadingTextFadeUpTime);

        // Begin async load
        AsyncOperation asyncOperation = SceneManager.LoadSceneAsync("Day" + nextDayNumberString);
        asyncOperation.allowSceneActivation = false;

        bool finishedLoading = false;
        while (!finishedLoading)
        {
            loadingText.text = "Loading: " + ((asyncOperation.progress / 0.9) * 100) + "%";

            if (asyncOperation.progress >= 0.9f)
            {
                loadingTextCG.alpha = 0;
                finishedTextCG.alpha = 1;
                if (Input.GetButtonDown("A"))
                {
                    audioSource.PlayOneShot(confirmSound);
                    finishedLoading = true;
                }
            }
            yield return null;
        }

        // Fade out and allow async operation to complete.
        finishedTextCG.DOFade(0f, allFadeOutTime * 0.25f);
        Tween t = textContainerCG.DOFade(0f, allFadeOutTime);
        yield return t.WaitForCompletion();

        asyncOperation.allowSceneActivation = true;
    }


}
