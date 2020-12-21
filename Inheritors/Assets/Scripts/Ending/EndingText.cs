using System.Collections;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using DG.Tweening;

// Sceneloader handles audio fade in/out
public class EndingText : MonoBehaviour
{
    float textSpeed = 0.03f;//0.02f;
    CanvasGroup textCanvasGroup;
    TMP_Text text;
    Image prompt;
    SceneLoader sceneLoader;
    AudioSource audioSource;

    string[] endingText;
    bool creditsOver = false;

    public float textHoldTime = 4f;
    public float textFadeTime = 1f;
    public float waitBetweenTextTime = 0.25f;

    bool buttonDown = false;

    void Awake()
    {
        GameObject textObject = GameObject.Find("Text");
        textCanvasGroup = textObject.GetComponent<CanvasGroup>();
        text = textObject.GetComponent<TMP_Text>();
        prompt = GameObject.Find("Prompt").GetComponent<Image>();
        sceneLoader = FindObjectOfType<SceneLoader>();
        audioSource = GetComponent<AudioSource>();
    }

    void Start()
    {
        PlayerPrefs.SetInt("CompletedGame", 1);

        Cursor.visible = false;
        prompt.color = Helper.ChangedAlpha(prompt.color, 0f);
        InitializeEndingText();
        StartCoroutine("Ending");
    }

    void Update()
    {
        if (Input.GetButtonDown("A"))
        {
            buttonDown = true;
        }
    }

    IEnumerator Ending()
    {
        textCanvasGroup.alpha = 0f;
        yield return new WaitForSeconds(sceneLoader.inFadeDuration + 1);

        for (int page = 0; page < endingText.Length; page++)
        {
            // Bring up the page.
            Tween t;
            text.text = endingText[page];
            t = textCanvasGroup.DOFade(1f, 1f);
            yield return t.WaitForCompletion();

            // Hold the page before allowing to go to next.
            yield return new WaitForSecondsRealtime(textHoldTime);

            // Prompt appears
            buttonDown = false;
            t = prompt.DOFade(1f, 0.5f);
            yield return new WaitUntil(() => buttonDown || t.IsComplete());

            // Prompt yoyo fade animation
            Sequence sequence = DOTween.Sequence();
            sequence.Append(prompt.DOFade(0f, 1f));
            sequence.SetLoops(-1, LoopType.Yoyo);
            yield return new WaitUntil(() => buttonDown);

            audioSource.Play();

            // Prompt disappears
            sequence.Kill();
            t.Kill();
            prompt.DOFade(0f, 0.25f);
            t = textCanvasGroup.DOFade(0f, 1f);
            yield return t.WaitForCompletion();

            // Wait before showing next page
            yield return new WaitForSecondsRealtime(waitBetweenTextTime);
        }

        sceneLoader.LoadSceneByName("MainMenu");
    }

    void InitializeEndingText()
    {
        endingText = new string[] {
            "The real-life Akuntsu are the isolated remnants of a tribe living in the Rio Omerê Indigenous Territory of Rondônia, Brazil. \n\n" +
            "The events of this game are based on research into the Akuntsu and other indigenous groups like the Kanoe, living in the Rio Omerê.",

            "Illegal logging, farming and mining encroached on former Akuntsu territory and their people were killed en masse. The survivors hold tenuously to what remains -  as of 2020, only four remain living. \n\n" +
            "They once numbered in the thousands.",

            "No others speak their unique language, and they will not marry or integrate with outside tribes. There is no \"somewhere new\" for the Akuntsu, no inheritors to the Akuntsu culture. When the last of them and others in the Rio Omerê are gone, their protected land is likely to be developed.",

            "Their language, their customs, and their traditions will all be lost to time. \n\n" +
            "An entire way of being will go extinct. We will never know their whole story.",

            "It is too late to save the Akuntsu. But there are many other tribes like them. Many for whom the struggle to preserve their way of life continues. For these tribes, time may be running out, but it is not too late.",

            "To find out more, visit <b>survivalinternational.org</b> \n\n" +
            "<size=75%><b>Inheritors</b> By Nick Perrin \nSpecial thanks to Steve Engels & the playtesters</size>"
        };
    }

}
