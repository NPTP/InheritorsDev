﻿using System.Collections.Generic;
using UnityEngine;

public class Day4DialogContent : MonoBehaviour, DialogContent
{
    public Dictionary<string, Dialog> content = new Dictionary<string, Dialog>();

    void Awake()
    {
        PackContent();
    }

    public Dialog Get(string key)
    {
        if (content.ContainsKey(key))
            return content[key];
        else
        {
            print("Don't have dialog key: " + key);
            return content["NULL"];
        }
    }

    public bool ContainsKey(string key)
    {
        return content.ContainsKey(key);
    }


    void PackContent()
    {
        content.Add("Day4Opening", new Dialog
        {
            character = Character.Narrator,
            lines = new string[] {
                "Grandmother is waiting for me at the top of the hill, ready to perform the festival of senses."
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
              "Good. Now, hold the reed up to my nose, and slide the ground yopo through it... that's it.",
              "<i>Sniff</i>... <i>Sniff</i>... Aahh...",
              "Imagine this land when there was more than just our family, grandson.",
              "Imagine when there were thousands of Akuntsu here.",
              "We clear the bad blood of we who remain... The ancestors speak... Hear them, see them...",
              "<i>NOW</i>!"
            }
        });

        content.Add("Grandmother_Festival2", new Dialog
        {
            character = Character.Grandmother,
            skippable = false,
            lines = new string[] {
                "Before you were born, we were so many, our roots grew so deep.",
                "Now, grandson, see those roots... \nsee how deep they truly are.",
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
