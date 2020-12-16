using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

// For debug only.
public class DebugSceneLoader : MonoBehaviour
{
    public bool debugEnabled = true;

    void Start()
    {
        if (!enabled)
            Destroy(this.gameObject);
    }

    void Update()
    {
        if (Input.GetKeyDown(KeyCode.Escape))
        {
            Go("MainMenu");
        }
        else if (Input.GetKeyDown(KeyCode.BackQuote))
        {
            Go("Day0");
        }
        else if (Input.GetKeyDown(KeyCode.Alpha1))
        {
            Go("Day1");
        }
        else if (Input.GetKeyDown(KeyCode.Alpha2))
        {
            Go("Day2");
        }
        else if (Input.GetKeyDown(KeyCode.Alpha3))
        {
            Go("Day3");
        }
        else if (Input.GetKeyDown(KeyCode.Alpha4))
        {
            Go("Day4");
        }
        else if (Input.GetKeyDown(KeyCode.Alpha5))
        {
            Go("Day5");
        }
        else if (Input.GetKeyDown(KeyCode.Alpha6))
        {
            Go("Day6");
        }
        else if (Input.GetKeyDown(KeyCode.Alpha7))
        {
            Go("Day7");
        }
        else if (Input.GetKeyDown(KeyCode.Alpha8))
        {
            Go("Day8");
        }
        else if (Input.GetKeyDown(KeyCode.Alpha9))
        {
            Go("Day9");
        }
        else if (Input.GetKeyDown(KeyCode.Alpha0))
        {
            Go("Day10");
        }
        else if (Input.GetKeyDown(KeyCode.Backspace))
        {
            Go(SceneManager.GetActiveScene().name);
        }
    }

    void Go(string name)
    {
        if (Application.CanStreamedLevelBeLoaded(name))
            SceneManager.LoadScene(name);
        else
            print("Scene '" + name + "' could not be found.");
    }

}
