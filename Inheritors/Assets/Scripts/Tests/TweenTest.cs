using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using DG.Tweening;

public class TweenTest : MonoBehaviour
{
    public string s;
    public Font font;

    void Start()
    {
        TextGenerator textGen = new TextGenerator();
        TextGenerationSettings textGenSettings = new TextGenerationSettings();
        textGenSettings.font = font;
        textGenSettings.fontSize = 80;
        float textWidth = textGen.GetPreferredWidth(s, textGenSettings);

        Debug.Log(textWidth);

        GetComponent<Text>().DOText(s, 5f);
    }

    // Update is called once per frame
    void Update()
    {

    }
}
