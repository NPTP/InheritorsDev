using System.Collections.Generic;
using UnityEngine;

public class Day0DialogContent : MonoBehaviour, DialogContent
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
        string delay = DialogManager.Tools.DELAY;

        content.Add("Opening", new Dialog
        {
            character = Character.Narrator,
            skippable = false,
            lines = new string[] {
                "The Omerê is our home." + delay,
                "Our people have lived here for hundreds of years." + delay,
                "Your mother is of <b>Kanoê</b>." + delay + "\nYour father, of <b>Akuntsu</b>." + delay,
                "You are young, <b>son</b>." + delay + "\nYou are the inheritor of this land." + delay + "\nThe inheritor of our tradition." + delay,
                "You will bring us hope." + delay + delay
            }
        });

        content.Add("Firepit", new Dialog
        {
            character = Character.Narrator,
            lines = new string[] {
                "This fire is dying.",
                "Fetch <color=blue>3 logs</color> from our pile of dried wood, so that it might find new life."
            }
        });

        content.Add("Wood1", new Dialog
        {
            character = Character.Narrator,
            lines = new string[] {
                "The Cumaru wood. Tough, ancient, everlasting."
            }
        });

        content.Add("Wood2", new Dialog
        {
            character = Character.Narrator,
            lines = new string[] {
                "Massaranduba tree. Hard-won, deep, rich, and beautiful."
            }
        });

        content.Add("Wood3", new Dialog
        {
            character = Character.Narrator,
            lines = new string[] {
                "The wood of the Ipê. Life-giving, sturdy, ever-present... and coveted.",
                "That is enough wood for now. Return to the fire."
            }
        });

        content.Add("Maloca", new Dialog
        {
            character = Character.Mother,
            lines = new string[] {
                "Well done, son! I see the shadows dancing on the inside of the maloca.",
                "It is late. Come join me inside. Tomorrow is an important day."
            }
        });

        content.Add("SisterSleep", new Dialog
        {
            character = Character.Sister,
            lines = new string[] {
                "...",
                "Hn... huh? Oh, you woke me up!",
                "Brother, what are you doing here so late? Go to sleep.",
                "Always up to mischief. Mother must be so worried..."
            }
        });
    }
}
