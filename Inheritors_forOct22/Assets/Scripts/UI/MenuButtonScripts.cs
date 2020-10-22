using UnityEngine;
using UnityEngine.SceneManagement;

public class MenuButtonScripts : MonoBehaviour
{
    SceneLoader sceneLoader;

    void Start()
    {
        sceneLoader = GameObject.Find("SceneLoader").GetComponent<SceneLoader>();
    }

    public void BeginGame()
    {
        sceneLoader.LoadNextScene();
    }

    public void ExitGame()
    {
        Application.Quit();
#if UNITY_EDITOR
        UnityEditor.EditorApplication.isPlaying = false;
#endif
    }
}
