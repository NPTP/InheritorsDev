using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using DG.Tweening;

public class TMPTextTest : MonoBehaviour
{
    public Image buttonPrompt;
    TMP_Text tmpText;
    int index = 0;
    string[] lines;
    bool readyforNextLine = true;
    public float timePerChar = 0.02f;

    void Start()
    {
        tmpText = GetComponent<TMP_Text>();
        string[] l = {
            "Lorem ipsum dolor sit amet, consectetur adipiscing elit, sed do eiusmod tempor incididunt ut labore et dolore magna aliqua.",
            "Ut enim ad minim veniam, quis nostrud exercitation ullamco laboris nisi ut aliquip ex ea commodo consequat.",
            "Duis aute irure dolor in reprehenderit in voluptate velit esse cillum dolore eu fugiat nulla pariatur.",
            "Excepteur sint occaecat cupidatat non proident, sunt in culpa qui officia deserunt mollit anim id est laborum."
        };
        lines = l;
        buttonPrompt.enabled = false;
    }

    void Update()
    {
        if (Input.GetButtonDown("A") && index < lines.Length && readyforNextLine)
        {
            buttonPrompt.enabled = false;
            readyforNextLine = false;
            StartCoroutine(DisplayLine(lines[index]));
            index++;
        }
    }

    IEnumerator DisplayLine(string line)
    {
        tmpText.text = line;
        tmpText.ForceMeshUpdate();
        Debug.Log(tmpText.textInfo.lineCount);

        for (int i = 0; i <= tmpText.text.Length; i++)
        {
            tmpText.maxVisibleCharacters = i;
            yield return new WaitForSeconds(timePerChar);
        }
        buttonPrompt.enabled = true;
        buttonPrompt.color = ReturnColorWithNewAlpha(buttonPrompt.color, 0f);
        buttonPrompt.DOFade(1f, 10 * timePerChar);
        yield return new WaitForSeconds(5 * timePerChar);
        readyforNextLine = true;
    }

    Color ReturnColorWithNewAlpha(Color color, float alpha)
    {
        Color c = color;
        c.a = alpha;
        return c;
    }
}
