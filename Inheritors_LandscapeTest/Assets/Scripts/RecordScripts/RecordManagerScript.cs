using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RecordManagerScript : MonoBehaviour
{
    public bool recording = false;
    public int targetFPS = 60;
    // public int numRecordFrames = 240;
    private int numFramesRecorded = 0;

    void Awake()
    {
        QualitySettings.vSyncCount = 0;  // VSync disabled for targetFrameRate to work
        Application.targetFrameRate = targetFPS;
    }

    void Update()
    {
        if (recording)
        {
            if (Input.GetKeyDown(KeyCode.R))
            {
                recording = false;
                numFramesRecorded = 0;
            }
            else
            {
                numFramesRecorded++;
            }
        }
        else if (Input.GetKeyDown(KeyCode.R))
        {
            recording = true;
            numFramesRecorded++;
        }
    }

}
