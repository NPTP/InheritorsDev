using UnityEngine;
using UnityEngine.SceneManagement;

public class MenuButtonScripts : MonoBehaviour
{
    Scene activeScene;

    void Start()
    {
        activeScene = SceneManager.GetActiveScene();
    }

    public void BeginGame()
    {
        SceneManager.LoadScene(activeScene.buildIndex + 1);
    }

    public void ExitGame()
    {
        Application.Quit();
#if UNITY_EDITOR
        UnityEditor.EditorApplication.isPlaying = false;
#endif
    }
}
