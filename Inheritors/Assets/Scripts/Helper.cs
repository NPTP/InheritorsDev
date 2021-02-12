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
    /// Returns a new RGB Color object from values in the 0-255 range (optional alpha in 0.0-1.0 range).
    /// </summary>
    public static Color RGBToColor(float r, float g, float b, float a = 1.0f)
    {
        return new Color(r / 255f, g / 255f, b / 255f, a);
    }

    /// <summary> 
    /// Takes an input in the interval [0, 1] and returns the smooth-stepped output.
    /// </summary>
    public static float SmoothStep(float t)
    {
        return t * t * (3f - 2f * t);
    }

    /// <summary>
    /// Takes a positive integer "odds" and returns a 1-in-"odds" random gamble
    /// </summary>
    public static bool Chance(int odds)
    {
        if (Random.Range(0, odds) == odds - 1)
            return true;
        else
            return false;
    }

    public static void LoadScene(string name)
    {
        SceneManager.LoadScene(name);
    }

    public static void RestartScene()
    {
        SceneManager.LoadScene(SceneManager.GetActiveScene().name);
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
