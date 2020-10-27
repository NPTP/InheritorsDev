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
}
