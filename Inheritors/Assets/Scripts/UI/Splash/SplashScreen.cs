using UnityEngine;

public class SplashScreen : MonoBehaviour
{
    SceneLoader sceneLoader;

    void Awake()
    {
        sceneLoader = FindObjectOfType<SceneLoader>();
    }

    public void HandleEndOfAnimation()
    {
        sceneLoader.LoadSceneByName("MainMenu");
    }
}
