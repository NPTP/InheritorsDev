using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RewindManagerScript : MonoBehaviour
{
    public bool rewinding = false;
    public int targetFPS = 60;
    public int numRewindFrames = 240;
    public int numFramesRewound = 0;

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
            if (numFramesRewound < numRewindFrames)
            {
                rewinding = true;
                numFramesRewound++;
                // StartCoroutine(StartRewindingCoroutine());
            }
            else
            {
                rewinding = false;
            }
        }
        else if (Input.GetKeyUp(KeyCode.R))
        {
            rewinding = false;
            if (numFramesRewound > 0)
                numFramesRewound--;
        }
        else if (rewinding)
        {
            if (numFramesRewound < numRewindFrames)
            {
                numFramesRewound++;
            }
            else
            {
                rewinding = false;
            }
        }
        else if (!rewinding && numFramesRewound > 0)
        {
            numFramesRewound--;
        }

    }


    IEnumerator StartRewindingCoroutine()
    {
        Time.timeScale = 0;

        //yield on a new YieldInstruction that waits for 5 seconds.
        yield return new WaitForSeconds(0.5f);

        Time.timeScale = 1;
    }
}
