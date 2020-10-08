using UnityEngine;
using UnityEngine.UI;

public class RecordScript : MonoBehaviour
{
    private RecordManagerScript rm;
    private Text recordText;

    void Start()
    {
        GameObject rmObject = GameObject.Find("RecordManager");
        rm = rmObject.GetComponent<RecordManagerScript>();
        recordText = GetComponent<Text>();
    }

    void Update()
    {
        if (rm.recording)
        {
            recordText.color = Color.red;
            recordText.fontStyle = FontStyle.Bold;
            recordText.text = "Press R to STOP recording";
        }
        else
        {
            recordText.color = Color.white;
            recordText.fontStyle = FontStyle.Normal;
            recordText.text = "Press R to start recording";
        }
    }
}
