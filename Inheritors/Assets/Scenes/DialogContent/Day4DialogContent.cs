using System.Collections.Generic;
using UnityEngine;

public class Day4DialogContent : MonoBehaviour
{
    public Dictionary<string, Dialog> content = new Dictionary<string, Dialog>();

    void Awake()
    {
        PackContent();
    }

    void PackContent()
    {

        // TODO
        content.Add("Day4Opening", new Dialog
        {
            character = Character.Narrator,
            lines = new string[] {
                "Grandmother is waiting at the top of the hill to perform the festival of senses."
            }
        });

        // ████████████████████████████████████████████████████████████████████████
        // ███ GRANDMOTHER
        // ████████████████████████████████████████████████████████████████████████

        content.Add("Grandmother_Start", new Dialog
        {
            character = Character.Grandmother,
            lines = new string[] {
              "Welcome youngest one. The <color=red>festival of senses</color> is about to begin.",
              "The ritual begins with the inhalation of the yopo. Pick up the <color=blue>reed</color> behind you and bring it to me."
            }
        });

        content.Add("Grandmother_Festival1", new Dialog
        {
            character = Character.Grandmother,
            lines = new string[] {
              "Good, grandson. Now, hold the reed up to my nose, and slide the ground yopo through it... that's it.",
              "<i>Sniff</i>... <i>Sniff</i>... Aahh...",
              "Much bad blood to clear... The ancestors speak... Hear them, see them...",
              "Now!"
            }
        });

        content.Add("Grandmother_Festival2", new Dialog
        {
            character = Character.Grandmother,
            skippable = false,
            lines = new string[] {
                "Before you were born, we were so many, our roots grew so deep.",
                "Now, grandson, see those roots... see how deep they truly are.",
            }
        });

        content.Add("Grandmother_Festival3", new Dialog
        {
            character = Character.Grandmother,
            lines = new string[] {
                "Ohhh...",
                "We have shared a special moment. Thank you, child. The festival is complete. You should go home now.",
                "But keep your eyes open - you will see how deep the roots go..."
            }
        });

        content.Add("Grandmother_Completed", new Dialog
        {
            character = Character.Grandmother,
            lines = new string[] {
                "We have shared a special moment. Thank you, child. The festival is complete. You should go home now.",
                "But keep your eyes open... You will see."
            }
        });

    }
}
