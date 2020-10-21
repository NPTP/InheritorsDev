
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class SceneTransitions : MonoBehaviour
{
    public Animator dipToBlackTransition;
    public float dipToBlackTransitionTime = 1.5f;

    // Loads the next scene in the build order.
    public void LoadNextScene()
    {
        int activeSceneBuildIndex = SceneManager.GetActiveScene().buildIndex;
        if (activeSceneBuildIndex + 1 < SceneManager.sceneCountInBuildSettings)
            StartCoroutine(LoadScene(SceneManager.GetActiveScene().buildIndex + 1));
        else
            Debug.Log("SceneLoader: Reached end of build order, no scene with greater build index.");
    }

    public void LoadSceneByName(string name)
    {
        SceneManager.LoadScene(name);
    }

    IEnumerator LoadScene(int levelIndex)
    {
        dipToBlackTransition.SetTrigger("Start");
        yield return new WaitForSeconds(dipToBlackTransitionTime);
        SceneManager.LoadScene(levelIndex);
    }

}
