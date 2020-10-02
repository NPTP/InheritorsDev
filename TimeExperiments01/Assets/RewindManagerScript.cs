using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RewindManagerScript : MonoBehaviour
{
    public bool rewinding = false;
    public int targetFPS = 60;
    public int numRewindFrames = 240;

    void Awake()
    {
        QualitySettings.vSyncCount = 0;  // VSync disabled for targetFrameRate to work
        Application.targetFrameRate = targetFPS;
    }
    void Start()
    {

    }

    void Update()
    {
        if (Input.GetKeyDown(KeyCode.R))
        {
            rewinding = true;
        }
        else if (Input.GetKeyUp(KeyCode.R))
        {
            rewinding = false;
        }
    }
}
