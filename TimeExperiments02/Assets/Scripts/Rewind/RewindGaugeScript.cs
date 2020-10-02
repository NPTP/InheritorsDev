using UnityEngine;
using UnityEngine.UI;

public class RewindGaugeScript : MonoBehaviour
{
    private RewindManagerScript rewindManagerScript;
    private Text rewindGaugeText;
    void Start()
    {
        GameObject rewindManager = GameObject.Find("RewindManager");
        rewindManagerScript = rewindManager.GetComponent<RewindManagerScript>();
        rewindGaugeText = GetComponent<Text>();
    }

    void Update()
    {
        int powerRemaining = rewindManagerScript.numRewindFrames - rewindManagerScript.numFramesRewound;
        rewindGaugeText.text = "Rewind Power remaining: " + powerRemaining.ToString();

        if (rewindManagerScript.rewinding)
            rewindGaugeText.color = Color.red;
        else
            rewindGaugeText.color = Color.white;
    }
}
