using System.Collections.Generic;
using UnityEngine;

public class Day3DialogContent : MonoBehaviour
{
    public Dictionary<string, Dialog> content = new Dictionary<string, Dialog>();

    void Awake()
    {
        PackContent();
    }

    void PackContent()
    {
        content.Add("Day3Opening_1", new Dialog
        {
            character = Character.Mother,
            lines = new string[] {
                "Father and grandfather worked very hard to put the bridge up over the river.",
                "Your grandparents will want to see you today.",
                "You have some work on this side of the river to finish, but be sure to visit them."
            }
        });

        content.Add("Day3Opening_2", new Dialog
        {
            character = Character.Mother,
            lines = new string[] {
                "Our family is strong, by necessity. Kanoe and Akuntsu, once divided, now united.",
                "And the work you are doing is helping us to grow once again, little one.",
                "There is much to do, so be quick and return before sundown!",
            }
        });

        // ████████████████████████████████████████████████████████████████████████
        // ███ MOTHER
        // ████████████████████████████████████████████████████████████████████████

        content.Add("Mother_Start", new Dialog
        {
            character = Character.Mother,
            lines = new string[] {
                "Before, your father would have been other, enemy. But our common enemy was worse, so we came together."
            }
        });

        content.Add("Mother_Active", new Dialog
        {
            character = Character.Mother,
            lines = new string[] {
                "Fresh water, into the grey pot, son."
            }
        });

        content.Add("Mother_Completed", new Dialog
        {
            character = Character.Mother,
            lines = new string[] {
                "How is grandmother doing? She is the keeper of this forest, and has always made her own way. You are headstrong like her."
            }
        });

        // ████████████████████████████████████████████████████████████████████████
        // ███ FATHER
        // ████████████████████████████████████████████████████████████████████████

        content.Add("Father_Start", new Dialog
        {
            character = Character.Father,
            lines = new string[] {
                "Boy! No time, be quiet - the tapir will flee if startled. Ready your bow."
            }
        });

        content.Add("Father_HuntEnd", new Dialog
        {
            character = Character.Father,
            lines = new string[] {
                "Beautiful, my son. Our hunts have been bountiful and it has been a joy to teach you.",
                "You will surely lead the hunt one day, but my scars are causing me too much pain to carry the <color=blue>meat</color>.",
                "I trust you to return it to mother!"
            }
        });

        content.Add("Father_Completed", new Dialog
        {
            character = Character.Father,
            lines = new string[] {
                "These scars seem never to heal...",
                "Never mind that. You are quickly becoming a man of the tribe, son."
            }
        });

        // ████████████████████████████████████████████████████████████████████████
        // ███ SISTER
        // ████████████████████████████████████████████████████████████████████████

        content.Add("Sister_Start", new Dialog
        {
            character = Character.Sister,
            lines = new string[] {
                "I've gone to see the ancient tree, little brother. It is near grandfather's maloca. I have never seen one so tall.",
                "Please gather <color=blue>four ears of corn</color> from the garden, and bring them back. They look delicious."
            }
        });

        content.Add("Sister_Active", new Dialog
        {
            character = Character.Sister,
            lines = new string[] {
                "All of the corn we need is in my garden."
            }
        });

        content.Add("Sister_Completed", new Dialog
        {
            character = Character.Sister,
            lines = new string[] {
                "More corn should grow here soon, as we work the soil to make it richer.",
                "See you tomorrow, little brother!"
            }
        });

        content.Add("DayOver", new Dialog
        {
            character = Character.Mother,
            lines = new string[] {
                "Thank you, son. That is all your work for today!",
                "It is getting late, so come back for sleep as soon as you can."
            }
        });

        // ████████████████████████████████████████████████████████████████████████
        // ███ GRANDFATHER
        // ████████████████████████████████████████████████████████████████████████

        content.Add("Grandfather_Start", new Dialog
        {
            character = Character.Grandfather,
            lines = new string[] {
                "G"
            }
        });

        content.Add("Grandfather_Active", new Dialog
        {
            character = Character.Grandfather,
            lines = new string[] {
              "G"
            }
        });

        content.Add("Grandfather_Completed", new Dialog
        {
            character = Character.Grandfather,
            lines = new string[] {
              "G"
            }
        });


        // ████████████████████████████████████████████████████████████████████████
        // ███ GRANDMOTHER
        // ████████████████████████████████████████████████████████████████████████



    }
}
