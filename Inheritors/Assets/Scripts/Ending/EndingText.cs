using System.Collections;
using System.Collections.Generic;
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
    SceneLoader sceneLoader;

    string[] endingText;
    bool creditsOver = false;

    public float textHoldTime = 4f;
    public float textFadeTime = 1f;
    public float waitBetweenTextTime = 0.25f;

    void Awake()
    {
        GameObject textObject = GameObject.Find("Text");
        textCanvasGroup = textObject.GetComponent<CanvasGroup>();
        text = textObject.GetComponent<TMP_Text>();
        sceneLoader = FindObjectOfType<SceneLoader>();
    }

    void Start()
    {
        InitializeEndingText();
        StartCoroutine("Ending");
    }

    IEnumerator Ending()
    {
        textCanvasGroup.alpha = 0f;
        yield return new WaitForSeconds(sceneLoader.inFadeDuration);

        for (int page = 0; page < endingText.Length; page++)
        {
            Tween t;
            text.text = endingText[page];
            t = textCanvasGroup.DOFade(1f, 1f);
            yield return t.WaitForCompletion();

            yield return new WaitForSecondsRealtime(textHoldTime);
            t = textCanvasGroup.DOFade(0f, 1f);

            yield return t.WaitForCompletion();

            yield return new WaitForSecondsRealtime(waitBetweenTextTime);
        }

        sceneLoader.LoadSceneByName("MainMenu");
    }

    void InitializeEndingText()
    {
        endingText = new string[] {
            "The real-life Akuntsu are the isolated remnants of a tribe living \n" +
            "in the Rio Omerê Indigenous Territory of Rondônia, Brazil. \n\n" +

            "The details of this game are based on research on the Akuntsu and \n" +
            "other indigenous groups like the Kanoe, living in the Rio Omerê.",

            "Illegal logging, farming and mining encroached on former Akuntsu territory \n" +
            "and their people were killed en masse. The survivors hold tenuously to their \n" +
            "lives and land, and as of 2020 only four remain living. \n\n" +

            "They once numbered in the thousands.",

            "No others speak their unique language, and they will not marry or integrate \n" +
            "with outside tribes. There is no \"somewhere new\" for the Akuntsu, no inheritors \n" +
            "to the Akuntsu culture. When the last of them and others in the Rio Omerê are gone, \n" +
            "their protected land is likely to be developed.",

            "Their language, their customs, and their traditions will all be lost to time. \n\n" +
            "A human way of being will go extinct. We will never know their whole story.",

            "It is too late to save the Akuntsu. But there are many other tribes like them. \n" +
            "Many for whom the struggle to preserve their way of life continues. \n\n" +
            "For these tribes, time may be running out, but it is not too late.",

            "To find out more, visit <b>survivalinternational.org</b> \n\n" +
            "<size=75%>By Nick Perrin \nSpecial thanks to Steve Engels & the playtesters</size>"

            // Final elements appear, smaller. At the bottom left, “By Nick Perrin \n Special thanks to: Steve Engels \n The playtesters”.
            // At the bottom right, a prompt to “Finish” the game. Press the button, and we fade to black from the white.
            // The black fades back out into the post-completion version of the main menu.
        };
    }

}
