using System.Collections.Generic;
using UnityEngine;

public class Day10DialogContent : MonoBehaviour
{
    public Dictionary<string, Dialog> content = new Dictionary<string, Dialog>();

    void Awake()
    {
        PackContent();
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
                "I have to find mother."
            }
        });

        content.Add("Day10Opening_2", new Dialog
        {
            character = Character.Mother,
            lines = new string[] {
                "She must be in the forest. She would never leave me.",
                "I will check everywhere."
            }
        });

        // Check the watering hole

        content.Add("WateringHole", new Dialog
        {
            character = Character.Narrator,
            skippable = false,
            lines = new string[] {
                "The water is thick and muddy now. We cannot drink this.",
                "She is not here."
            }
        });

        // Check sister's

        content.Add("Sister", new Dialog
        {
            character = Character.Narrator,
            skippable = false,
            lines = new string[] {
                "There is nothing beyond the fence. Everything is burning.",
                "She is not here."
            }
        });

        // Check the hunting forest

        content.Add("Father", new Dialog
        {
            character = Character.Narrator,
            skippable = false,
            lines = new string[] {
                "I wondered if she came here to wait for father, but no.",
                "She is not here."
            }
        });

        // Check grandfather's

        content.Add("Grandfather", new Dialog
        {
            character = Character.Narrator,
            skippable = false,
            lines = new string[] {
                "The heat is unbearable, and I can hardly breathe in the smoke.",
                "She is nowhere to be seen."
            }
        });

        // Check grandmother's

        content.Add("Grandmother", new Dialog
        {
            character = Character.Narrator,
            skippable = false,
            lines = new string[] {
                "Did she find what grandmother left behind? I see no trace.",
                "She is not here..."
            }
        });

        // Talk to the man of the hole.

        content.Add("Manofhole", new Dialog
        {
            character = Character.Narrator,
            skippable = false,
            lines = new string[] {
                "The boy... you still here!",
                "Before you, before boy, I saw... this happen before.",
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
            character = Character.Narrator,
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
            lines = new string[] {
                "Our water is overflowing, yet we have no meat, no fruit, no vegetables.",
                "Please, check on everyone as soon as you can. I don't want us all to have to leave our home...",
                "Not again."
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
                "<UNUSED>"
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
