using UnityEngine;
using UnityEngine.UI;

public class RecordTextScript : MonoBehaviour
{
    private RecordManagerScript recordManagerScript;
    private Text recordText;
    private Image recordButtonIcon;
    void Start()
    {
        GameObject recordManager = GameObject.Find("RecordManager");
        recordManagerScript = recordManager.GetComponent<RecordManagerScript>();
        recordText = GetComponent<Text>();
        recordText.fontStyle = FontStyle.Bold;

        GameObject icon = GameObject.Find("RecordButtonIcon");
        recordButtonIcon = icon.GetComponent<Image>();
    }

    void Update()
    {
        if (recordManagerScript.recording)
        {
            recordButtonIcon.color = Color.red;
            recordText.color = Color.red;
            recordText.text = "STOP RECORDING";
        }
        else
        {
            recordButtonIcon.color = Color.white;
            recordText.color = Color.white;
            recordText.text = "START RECORDING";
        }
    }
}
