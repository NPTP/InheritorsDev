using System.Collections.Generic;
using UnityEngine;

public class Day10DialogContent : MonoBehaviour, DialogContent
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
        content.Add("Day10Opening_1", new Dialog
        {
            character = Character.Narrator,
            lines = new string[] {
                "Mother?",
                "Where are you?",
                "The air is thick with smoke.",
                "I have to find mother.",
                "Maybe she is looking for the others. \nI will check everyone's place."
            }
        });

        content.Add("Day10Opening_2", new Dialog
        {
            character = Character.Narrator,
            lines = new string[] {
                "She must be somewhere, here, in the forest. \nShe would never leave me.",
            }
        });

        // Check the watering hole

        content.Add("Mother", new Dialog
        {
            character = Character.Narrator,
            skippable = false,
            lines = new string[] {
                "The water is thick and muddy now. \nWe cannot drink this.",
                "She is not here."
            }
        });

        // Check sister's

        content.Add("Sister", new Dialog
        {
            character = Character.Narrator,
            skippable = false,
            lines = new string[] {
                "There is nothing beyond the fence. \nEverything is burning.",
                "She is not here."
            }
        });

        // Check the hunting forest

        content.Add("Father", new Dialog
        {
            character = Character.Narrator,
            skippable = false,
            lines = new string[] {
                "Did she wait for father to return?",
                "She is not here."
            }
        });

        // Check grandfather's

        content.Add("Grandfather", new Dialog
        {
            character = Character.Narrator,
            skippable = false,
            lines = new string[] {
                "The heat is unbearable. \nI can hardly breathe in the smoke.",
                "She is nowhere to be seen."
            }
        });

        // Check grandmother's

        content.Add("Grandmother", new Dialog
        {
            character = Character.Narrator,
            skippable = false,
            lines = new string[] {
                "Did she find what grandmother left behind? \nI see no trace.",
                "She is not here..."
            }
        });

        // Talk to the man of the hole.

        content.Add("Manofhole_Start", new Dialog
        {
            character = Character.Manofhole,
            skippable = false,
            lines = new string[] {
                "The boy... you still here!",
                "Before you, before boy, I saw... \nthis happen before.",
                "Many Akuntsu... had no hope. \nMany... end their own live.",
                "Now, even hope Akuntsu gone. \nAll Akuntsu... all gone.",
                "But...",
                "Kanoe... is like me. \nStill one survive.",
                "A woman. \nA woman... on top of hill.",
                "She run, but I... \nI not run anymore. \nThis, mine... my home.",
                "I stay here. Even if... die."
            }
        });

        content.Add("Manofhole_Repeat", new Dialog
        {
            character = Character.Manofhole,
            lines = new string[] {
                "Kanoe... is like me. \nStill one survive.",
                "A woman. \nA woman... on top of hill.",
                "She run, but I... \nI not run anymore. \nThis, mine... my home.",
                "I stay here. Even if... die."
            }
        });

        content.Add("DayOver", new Dialog
        {
            character = Character.Narrator,
            lines = new string[] {
                "I do not hear mother's voice calling me.",
                "I should return home and wait for her."
            }
        });

        // ████████████████████████████████████████████████████████████████████████
        // ███ MOTHER
        // ████████████████████████████████████████████████████████████████████████

        content.Add("Mother_Start", new Dialog
        {
            character = Character.Mother,
            skippable = false,
            lines = new string[] {
                "My son! You're alive! \nYou have no idea how happy I am to see you...",
                "I feared for you. They took me from our home and... I escaped, and lit the torch for you to see, but...",
                "It does not matter. We are together now. \nIt looks as though the rain washed the dye out of your hair...",
                "What about your father? \nYour sister?",
                ".  .  .        ",
                "We cannot stay. \nIt is time to go.",
                "Maybe we can make a home... somewhere else.",
                "Maybe there is space for us somewhere new, a place we will belong again...",
                "A place for hope.",
                "Say your goodbyes to the Omere, then come with me out of the forest."
            }
        });

        content.Add("Mother_Active", new Dialog
        {
            character = Character.Mother,
            lines = new string[] {
                "<UNUSED>"
            }
        });

        content.Add("Mother_Completed", new Dialog
        {
            character = Character.Mother,
            lines = new string[] {
                "The Akuntsu live on, in you."
            }
        });

        // ████████████████████████████████████████████████████████████████████████
        // ███ FATHER
        // ████████████████████████████████████████████████████████████████████████

        content.Add("Father_Start", new Dialog
        {
            character = Character.Narrator,
            lines = new string[] {
                "<UNUSED>"
            }
        });

        content.Add("Father_Active", new Dialog
        {
            character = Character.Father,
            lines = new string[] {
                "<UNUSED>"
            }
        });

        content.Add("Father_Completed", new Dialog
        {
            character = Character.Father,
            lines = new string[] {
                "<UNUSED>"
            }
        });

        // ████████████████████████████████████████████████████████████████████████
        // ███ SISTER
        // ████████████████████████████████████████████████████████████████████████

        content.Add("Sister_Start", new Dialog
        {
            character = Character.Narrator,
            lines = new string[] {
                "<UNUSED>"
            }
        });

        content.Add("Sister_Active", new Dialog
        {
            character = Character.Sister,
            lines = new string[] {
                "<UNUSED>"
            }
        });

        content.Add("Sister_Completed", new Dialog
        {
            character = Character.Sister,
            lines = new string[] {
                "<UNUSED>"
            }
        });


        // ████████████████████████████████████████████████████████████████████████
        // ███ GRANDFATHER
        // ████████████████████████████████████████████████████████████████████████

        content.Add("Grandfather_Start", new Dialog
        {
            character = Character.Narrator,
            lines = new string[] {
                "<UNUSED>"
            }
        });

        content.Add("Grandfather_Active", new Dialog
        {
            character = Character.Grandfather,
            lines = new string[] {
                "<UNUSED>"
            }
        });

        content.Add("Grandfather_FinishTask", new Dialog
        {
            character = Character.Narrator,
            lines = new string[] {
                "<UNUSED>"
            }
        });

        content.Add("Grandfather_Completed", new Dialog
        {
            character = Character.Grandfather,
            lines = new string[] {
                "<UNUSED>"
            }
        });

        // ████████████████████████████████████████████████████████████████████████
        // ███ GRANDMOTHER
        // ████████████████████████████████████████████████████████████████████████

        content.Add("Grandmother_Start", new Dialog
        {
            character = Character.Narrator,
            lines = new string[] {
              "< UNUSED >"
            }
        });

        content.Add("Grandmother_Active", new Dialog
        {
            character = Character.Grandmother,
            lines = new string[] {
              "< UNUSED >"
            }
        });

        content.Add("Grandmother_Completed", new Dialog
        {
            character = Character.Grandmother,
            lines = new string[] {
              "< UNUSED >"
            }
        });

    }
}
