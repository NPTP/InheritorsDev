
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class SceneLoader : MonoBehaviour
{
    public Animator dipToBlackTransition;
    public float dipToBlackTransitionTime = 1.5f;

    // Loads the next scene in the build order.
    public void LoadNextScene()
    {
        StartCoroutine(LoadScene(SceneManager.GetActiveScene().buildIndex + 1));
    }

    // Loads scene by string name
    public void LoadSceneByName(string name)
    {
        SceneManager.LoadScene(name);
    }

    // Loads scene by int build index
    public void LoadSceneByIndex(int index)
    {
        StartCoroutine(LoadScene(index));
    }

    IEnumerator LoadScene(int levelIndex)
    {
        dipToBlackTransition.SetTrigger("Start");
        yield return new WaitForSeconds(dipToBlackTransitionTime);
        SceneManager.LoadScene(levelIndex);
    }

}
