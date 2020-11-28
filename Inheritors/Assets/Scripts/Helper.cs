using UnityEngine;
using UnityEngine.SceneManagement;

public class Helper : ScriptableObject
{
    /// <summary>
    /// Returns the same color, but with alpha set to input parameter.
    /// </summary>
    public static Color ChangedAlpha(Color color, float newAlpha)
    {
        Color c = color;
        c.a = newAlpha;
        return c;
    }

    /// <summary> 
    /// Takes an input in the interval [0, 1] and returns the smooth-stepped output.
    /// </summary>
    public static float SmoothStep(float t)
    {
        return t * t * (3f - 2f * t);
    }

    public static void LoadScene(string name)
    {
        SceneManager.LoadScene(name);
    }

    public static void LoadNextSceneInBuildOrder()
    {
        SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex + 1);
    }

    public static void GoToLoadingScreen()
    {
        SceneManager.LoadScene("Loading");
    }
}
