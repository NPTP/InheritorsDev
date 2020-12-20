using System.Collections;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;
using DG.Tweening;

public class PauseMenu : MonoBehaviour
{
    PauseManager pauseManager;
    TransitionManager transitionManager;
    AudioManager audioManager;

    [SerializeField] Image background;
    [SerializeField] CanvasGroup buttonsCG;
    [SerializeField] GameObject defaultSelectedButton;
    [SerializeField] AudioClip buttonSound;

    float buttonSoundVolumeScale = .3f;
    float fadeTime = .25f;
    float betweenFadesTime = .125f;
    float backgroundAlpha;

    bool allowPress = false;

    void Awake()
    {
        pauseManager = FindObjectOfType<PauseManager>();
        transitionManager = FindObjectOfType<TransitionManager>();
        audioManager = FindObjectOfType<AudioManager>();

        backgroundAlpha = background.color.a;
        background.color = Helper.ChangedAlpha(background.color, 0);
        buttonsCG.alpha = 0;
    }

    public void Activate()
    {
        StartCoroutine(ActivateCoroutine());
    }

    IEnumerator ActivateCoroutine()
    {
        allowPress = false;
        Sequence seq = DOTween.Sequence()
            .Append(background.DOFade(backgroundAlpha, fadeTime))
            .AppendInterval(betweenFadesTime)
            .Append(buttonsCG.DOFade(1f, fadeTime));
        yield return seq.WaitForCompletion();
        allowPress = true;

        Time.timeScale = 0;
    }

    public void Deactivate()
    {
        Time.timeScale = 1;
        DOTween.Sequence()
            .Append(buttonsCG.DOFade(0f, fadeTime))
            .AppendInterval(betweenFadesTime)
            .Append(background.DOFade(0f, fadeTime))
            .OnComplete(pauseManager.SetPauseMenuInactive);
    }

    void Update()
    {
        if (EventSystem.current.currentSelectedGameObject == null)
        {
            EventSystem.current.SetSelectedGameObject(defaultSelectedButton);
        }
    }

    public void Resume()
    {
        if (allowPress)
        {
            allowPress = false;
            audioManager.PlayOneShot(buttonSound, buttonSoundVolumeScale);
            Deactivate();
        }
    }

    public void RestartDay()
    {
        if (allowPress)
        {
            allowPress = false;
            audioManager.PlayOneShot(buttonSound, buttonSoundVolumeScale);
            StartCoroutine(RestartDayCoroutine());
        }
    }

    IEnumerator RestartDayCoroutine()
    {
        Time.timeScale = 1;
        transitionManager.SetColor(Color.black);
        Tween t = transitionManager.Show(.5f);
        yield return t.WaitForCompletion();
        Helper.RestartScene();
    }

    public void SaveAndQuit()
    {
        if (allowPress)
        {
            allowPress = false;
            audioManager.PlayOneShot(buttonSound, buttonSoundVolumeScale);
            StartCoroutine(SaveAndQuitCoroutine());
        }
    }

    IEnumerator SaveAndQuitCoroutine()
    {
        Time.timeScale = 1;
        transitionManager.SetColor(Color.black);
        Tween t = transitionManager.Show(.5f);
        yield return t.WaitForCompletion();
        Helper.LoadScene("MainMenu");
    }
}
