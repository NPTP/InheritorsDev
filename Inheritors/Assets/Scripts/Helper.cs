using UnityEngine;

public class Helper : ScriptableObject
{
    // Handy helper which returns the same color, but with alpha set to input parameter.
    public static Color ChangedAlpha(Color color, float newAlpha)
    {
        Color c = color;
        c.a = newAlpha;
        return c;
    }

    // Takes an input in the interval [0, 1] and returns the smooth-stepped output.
    public static float SmoothStep(float t)
    {
        return t * t * (3f - 2f * t);
    }
}
