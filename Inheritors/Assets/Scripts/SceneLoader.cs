using System.Collections;
using System.Collections.Generic;
using System;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;
using DG.Tweening;

// For use in menus
public class SceneLoader : MonoBehaviour
{
    CanvasGroup canvasGroup;
    Image image;
    List<Tuple<AudioSource, float>> audioSourceVolumePairs;

    [Header("Fade options")]
    public bool fadeAudioWithScreen = true;
    public bool fadeInOnSceneStart = true;
    public bool fadeOutOnSceneEnd = true;
    public float inFadeDuration = 1f;
    public float outFadeDuration = 1f;

    [Header("Color picker")]
    public Color startSceneColor = Color.black;
    public Color endSceneColor = Color.black;

    void Awake()
    {
        GameObject SLI = GameObject.Find("SceneLoaderCanvasElement");
        canvasGroup = SLI.GetComponent<CanvasGroup>();
        image = SLI.GetComponent<Image>();
        AudioSource[] audioSources = FindObjectsOfType<AudioSource>();
        audioSourceVolumePairs = new List<Tuple<AudioSource, float>>();
        foreach (AudioSource audioSource in audioSources)
        {
            print(audioSource.gameObject.name);
            audioSourceVolumePairs.Add(new Tuple<AudioSource, float>(audioSource, audioSource.volume));
        }
    }

    void Start()
    {
        if (fadeInOnSceneStart)
        {
            canvasGroup.alpha = 1f;
            image.color = startSceneColor;
            StartCoroutine(SceneStartProcess());
        }
        else
        {
            canvasGroup.alpha = 0f;
        }
    }

    IEnumerator SceneStartProcess()
    {
        yield return null;
        canvasGroup.DOFade(0f, inFadeDuration).SetEase(Ease.InCubic);
        if (fadeAudioWithScreen)
        {
            foreach (Tuple<AudioSource, float> pair in audioSourceVolumePairs)
            {
                pair.Item1.volume = 0f;
                pair.Item1.DOFade(pair.Item2, inFadeDuration).SetEase(Ease.InCubic);
            }
        }
    }

    /// <summary>
    /// Calling this will fade to color and load the named scene.
    /// </summary>
    public void LoadSceneByName(string name)
    {
        StartCoroutine(LoadNextSceneProcess(name));
    }

    /// <summary>
    /// Calling this will fade to color and load the next scene in the build order.
    /// </summary>
    public void LoadNextScene()
    {
        StartCoroutine(LoadNextSceneProcess());
    }

    IEnumerator LoadNextSceneProcess(string name = "")
    {
        if (fadeOutOnSceneEnd)
        {
            image.color = endSceneColor;
            Tween t = canvasGroup.DOFade(1f, outFadeDuration).SetEase(Ease.InCubic);
            if (fadeAudioWithScreen)
            {
                foreach (Tuple<AudioSource, float> pair in audioSourceVolumePairs)
                {
                    pair.Item1.DOFade(0f, outFadeDuration).SetEase(Ease.InOutCubic);
                }
            }
            yield return new WaitWhile(() => t != null && t.IsPlaying());
        }
        print("name: " + name);
        if (name == "")
            SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex + 1);
        else
            SceneManager.LoadScene(name);
    }
}
