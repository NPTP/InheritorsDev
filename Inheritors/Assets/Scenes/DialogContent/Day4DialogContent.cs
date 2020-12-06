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
                "Before, your father would have been the other, the enemy. But our common enemy was worse, so we came together."
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

        content.Add("father_Active", new Dialog
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

        content.Add("Sister_FinishTask", new Dialog
        {
            character = Character.Sister,
            lines = new string[] {
                "More corn should grow here soon, as we work the soil to make it richer.",
                "See you tomorrow, little brother!"
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
                "Ahh, my grandson. You have grown.",
                "I am not just your grandfather. I am also the last shaman of the Akuntsu, and the chief of our tribe.",
                "This flute is the <color=blue>matété</color>. We Akuntsu claim our matété once in youth, then never part with it until we die.",
                "I have made a <color=blue>matété</color> for you from fresh wood. Retrieve it, and we will play together."
            }
        });

        content.Add("Grandfather_StartTask", new Dialog
        {
            character = Character.Grandfather,
            lines = new string[] {
                "You play beautifully, so full of life. Playing with you eases the pain of the old scars I carry.",
                "Please come back again another day to play the matété with me, grandson."
            }
        });

        content.Add("Grandfather_Active", new Dialog
        {
            character = Character.Grandfather,
            lines = new string[] {
                "<UNUSED>"
            }
        });

        content.Add("Grandfather_Completed", new Dialog
        {
            character = Character.Grandfather,
            lines = new string[] {
              "You play beautifully, so full of life. Playing with you eases the pain of the old scars I carry.",
                "Please come back again another day to play the matété with me, grandson."
            }
        });


        // ████████████████████████████████████████████████████████████████████████
        // ███ GRANDMOTHER
        // ████████████████████████████████████████████████████████████████████████


        content.Add("Grandmother_Start", new Dialog
        {
            character = Character.Grandmother,
            lines = new string[] {
              "Hello, my youngest one. You have finally arrived.",
              "I am the eldest of our tribe, older even than your grandfather. The time has come for me to pass on what I know.",
              "As the youngest, you are the receiver of this wisdom.",
              "I need you to find for me <color=blue>three beans of the yopo tree</color>.",
              "They are colored red. You will find some in the meadow north of grandfather's maloca, and some to the south of here.",
              "I will tell you soon what they are for. Please, bring them back to me now."
            }
        });

        content.Add("Grandmother_Active", new Dialog
        {
            character = Character.Grandmother,
            lines = new string[] {
              "I need <color=blue>three beans of the yopo tree</color>.",
              "Look north-east of Grandfather's maloca, south-west of my own, and near the river bridge."
            }
        });

        content.Add("Grandmother_FinishTask", new Dialog
        {
            character = Character.Grandmother,
            lines = new string[] {
              "Well done, youngest one. The yopo bean is a powerful medicine that opens our minds and bodies to the spirit world.",
              "I will grind the beans for the <color=red>festival of senses</color> and clear our tribe of bad blood.",
              "To receive the full wisdom of the Akuntsu, I ask you return in the small hours of twilight for the <color=red>festival</color>.",
              "The path to the south of here will be open - it leads up the hill. Meet me at the very top. And youngest one...",
              "Do <i>not</i> tell your mother."
            }
        });

        content.Add("Grandmother_Completed", new Dialog
        {
            character = Character.Grandmother,
            lines = new string[] {
              "Return in the small hours of twilight for the <color=red>festival of senses</color>.",
              "The path to the south of here will be open - it leads up the hill. Meet me at the very top. Do <i>not</i> tell your mother."
            }
        });

    }
}
