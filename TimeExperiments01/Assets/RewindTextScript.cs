using UnityEngine;
using UnityEngine.UI;

public class RewindTextScript : MonoBehaviour
{
    private RewindManagerScript rewindManagerScript;
    private Text rewindText;
    void Start()
    {
        GameObject rewindManager = GameObject.Find("RewindManager");
        rewindManagerScript = rewindManager.GetComponent<RewindManagerScript>();
        rewindText = GetComponent<Text>();
    }

    void Update()
    {
        if (rewindManagerScript.rewinding)
        {
            rewindText.text = "rewinding...";
        }
        else
        {
            rewindText.text = "REWIND";
        }
    }
}
