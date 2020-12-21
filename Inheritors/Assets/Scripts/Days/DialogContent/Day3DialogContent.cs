using System.Collections.Generic;
using UnityEngine;

public class Day3DialogContent : MonoBehaviour, DialogContent
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
        content.Add("Day3Opening_1", new Dialog
        {
            character = Character.Father,
            lines = new string[] {
                "Your grandfather and I worked very hard to put the bridge up over the river.",
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
                "I cannot imagine how things would be now if your father and I had not come together between tribes.",
                "How is grandmother doing? She is the keeper of this forest, and has always made her own way. You are headstrong like her."
            }
        });

        content.Add("Mother_Other", new Dialog
        {
            character = Character.Mother,
            lines = new string[] {
                "I am so proud of you, the way you help everyone in our family. Your grandparents must be grateful."
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

        content.Add("DayOver", new Dialog
        {
            character = Character.Mother,
            lines = new string[] {
                "That is all the work for today, son! \nIt is getting late. Come home to sleep."
            }
        });

        // ████████████████████████████████████████████████████████████████████████
        // ███ FATHER
        // ████████████████████████████████████████████████████████████████████████

        content.Add("Father_Start", new Dialog
        {
            character = Character.Father,
            lines = new string[] {
                "I killed a tapir for us. I must take a rest for now. My scars will not heal, and are causing me pain again.",
                "Once I finish cleaning the meat, I will hang it up over the fire overnight to cook through.",
                "I look forward to you leading the hunt one day, son. I know my mother and father are so happy to see you grow up."
            }
        });

        content.Add("Father_Other", new Dialog
        {
            character = Character.Father,
            lines = new string[] {
                "What are you up to now, boy? Go finish that work before you can come back home."
            }
        });

        content.Add("Father_HuntEnd", new Dialog
        {
            character = Character.Father,
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
            character = Character.Sister,
            lines = new string[] {
                "I've gone to see the ancient tree, little brother. It is near grandfather's maloca. I have never seen one so tall.",
                "Please gather <color=blue>four ears of corn</color> from the garden, and bring them back. They look delicious."
            }
        });

        content.Add("Sister_Other", new Dialog
        {
            character = Character.Sister,
            lines = new string[] {
                "Well, little brother, you are doing it all today! \nCome back when you are less occupied, we have new growth!"
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

        // ████████████████████████████████████████████████████████████████████████
        // ███ GRANDFATHER
        // ████████████████████████████████████████████████████████████████████████

        content.Add("Grandfather_Start", new Dialog
        {
            character = Character.Grandfather,
            lines = new string[] {
                "Ahh, my grandson. You have grown.",
                "I am not just your grandfather. I am also the last shaman of the Akuntsu, and the chief of our tribe.",
                "This flute is the <color=blue>matété</color>. We Akuntsu have played this instrument for as long as I can remember.",
                "I have made a <color=blue>matété</color> for you. Retrieve it, and we will play together."
            }
        });

        content.Add("Grandfather_Other", new Dialog
        {
            character = Character.Grandfather,
            lines = new string[] {
                "Oh, my grandson. So nice to see you helping our tribe. Please return when you are finished your current work."
            }
        });

        content.Add("Grandfather_Active", new Dialog
        {
            character = Character.Grandfather,
            lines = new string[] {
                "Go ahead, pick up the matete flute leaning against the rock there. \nI made it for you, and only you."
            }
        });

        content.Add("Grandfather_StartTask", new Dialog
        {
            character = Character.Grandfather,
            lines = new string[] {
                "I will play an old melody for you. \nListen carefully."
            }
        });

        content.Add("Grandfather_ContinueTask", new Dialog
        {
            character = Character.Grandfather,
            lines = new string[] {
                "Now you try, grandson. Control your breath, it is the most valuable thing you possess."
            }
        });

        content.Add("Grandfather_FinishTask", new Dialog
        {
            character = Character.Grandfather,
            lines = new string[] {
                "You are a fast learner. It is a joy to be able to teach you, as my old scars prevent me from playing as often as I used to.",
                "We Akuntsu each have our own matété, and we keep it all our lives. Treat it well!",
                "Please come back again another day to play with me, grandson."
            }
        });

        content.Add("Grandfather_Completed", new Dialog
        {
            character = Character.Grandfather,
            lines = new string[] {
                "We Akuntsu each have our own matété, and we keep it all our lives. Treat it well!",
                "Please come back again another day to play with me, grandson."
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
              "Check the meadow north of grandfather, check just south of here, and check the other side of the bridge.",
              "I will tell you soon what they are for. Please, bring them back to me now."
            }
        });

        content.Add("Grandmother_Other", new Dialog
        {
            character = Character.Grandmother,
            lines = new string[] {
              "The youngest Akuntsu - and the most active. I have something important for you to do when you are available."
            }
        });

        content.Add("Grandmother_Active", new Dialog
        {
            character = Character.Grandmother,
            lines = new string[] {
              "I need <color=blue>three beans of the yopo tree</color>.",
              "Check the meadow north of grandfather's maloca. Check just south of here. And check the other side of the bridge."
            }
        });

        content.Add("Grandmother_FinishTask", new Dialog
        {
            character = Character.Grandmother,
            lines = new string[] {
              "Well done, youngest one. The yopo bean is a powerful medicine that opens our minds and bodies to the spirit world.",
              "I will grind the beans for the <color=red>festival of senses</color> and clear our tribe of bad blood.",
              "To receive the full wisdom of the Akuntsu, I ask you return in the small hours of twilight for the <color=red>festival</color>.",
              "Meet me at the very top of the hill just south of here! The rocks will be cleared to let you ascend the path. And youngest one...",
              "Do <i>not</i> tell your mother."
            }
        });

        content.Add("Grandmother_Completed", new Dialog
        {
            character = Character.Grandmother,
            lines = new string[] {
              "Return in the small hours of twilight for the <color=red>festival of senses</color>.",
              "The rocks blocking the path up the hill will be cleared. Meet me at the very top of the hill! And youngest one...",
              "Do <i>not</i> tell your mother."
            }
        });

    }
}
