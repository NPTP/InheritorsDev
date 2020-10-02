using UnityEngine;
using UnityEngine.UI;

public class RewindTextScript : MonoBehaviour
{
    private RewindManagerScript rewindManagerScript;
    private Text rewindText;
    private Image rewindButtonIcon;
    void Start()
    {
        GameObject rewindManager = GameObject.Find("RewindManager");
        rewindManagerScript = rewindManager.GetComponent<RewindManagerScript>();
        rewindText = GetComponent<Text>();

        GameObject icon = GameObject.Find("RewindButtonIcon");
        rewindButtonIcon = icon.GetComponent<Image>();
    }

    void Update()
    {
        if (rewindManagerScript.rewinding)
        {
            rewindButtonIcon.color = Color.red;
            rewindText.fontStyle = FontStyle.Italic;
            rewindText.color = Color.red;
            rewindText.text = "Rewinding...";
        }
        else
        {
            rewindButtonIcon.color = Color.white;
            rewindText.fontStyle = FontStyle.Bold;
            rewindText.color = Color.white;
            rewindText.text = "REWIND";
        }
    }
}
