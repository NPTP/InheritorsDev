using UnityEngine;
using UnityEngine.SceneManagement;

public class MenuButtonScripts : MonoBehaviour
{
    SceneTransitions sceneTransitions;

    void Start()
    {
        sceneTransitions = GameObject.Find("SceneTransitions").GetComponent<SceneTransitions>();
    }

    public void BeginGame()
    {
        sceneTransitions.LoadNextScene();
    }

    public void ExitGame()
    {
        Application.Quit();
#if UNITY_EDITOR
        UnityEditor.EditorApplication.isPlaying = false;
#endif
    }
}
