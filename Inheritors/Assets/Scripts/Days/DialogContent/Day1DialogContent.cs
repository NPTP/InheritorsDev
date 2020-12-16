using System.Collections.Generic;
using UnityEngine;

public class Day1DialogContent : MonoBehaviour, DialogContent
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
        content.Add("Day1Opening", new Dialog
        {
            character = Character.Mother,
            lines = new string[] {
                "Good morning, son. You slept well?",
                "Look! Last night, you left a trail behind you. This land has memory.",
                "If you walk a path over and over again, it remembers you. Keep that in mind.",
                "Now, you are old enough to help with the work. Here is what we need today!"
            }
        });

        content.Add("Dialog_TaskExplanation", new Dialog
        {
            character = Character.Mother,
            lines = new string[] {
                "Begin the work in any order you like. Once you have begun a task, finish it before pursuing another.",
                "You already have everything you need for this work.",
                "This is a great help to the family, son. I am proud of you!"
            }
        });

        content.Add("DayOver", new Dialog
        {
            character = Character.Mother,
            lines = new string[] {
                "Thank you, son. That's all the work today!",
                "You have worked so hard, it's time for siesta. Come inside."
            }
        });

        // ████████████████████████████████████████████████████████████████████████
        // ███ MOTHER
        // ████████████████████████████████████████████████████████████████████████

        content.Add("Mother_Start", new Dialog
        {
            character = Character.Mother,
            lines = new string[] {
                "Begin the work in any order you like. Once you have begun a task, finish it before pursuing another.",
                "You already have everything you need for this work.",
                "This is a great help to the family, son. I am proud of you!"
            }
        });

        content.Add("Mother_Other", new Dialog
        {
            character = Character.Mother,
            lines = new string[] {
                "You and father have caught a pig? \nIncredible, son! It will make a beautiful meal.",
                "Place it over the fire. We will cook it later."
            }
        });

        content.Add("Mother_Active", new Dialog
        {
            character = Character.Mother,
            lines = new string[] {
                "Just pour the water into the big grey pot."
            }
        });

        content.Add("Mother_Completed", new Dialog
        {
            character = Character.Mother,
            lines = new string[] {
                "I can see already that it will be a beautiful day. \nHow fine it is that you should grow together with our tribe, son."
            }
        });

        // ████████████████████████████████████████████████████████████████████████
        // ███ FATHER
        // ████████████████████████████████████████████████████████████████████████        

        content.Add("Father_Start", new Dialog
        {
            character = Character.Father,
            lines = new string[] {
                "Son! Finally you are old enough and we have enough men to hunt. I will teach you.",
                "The women tried their hand at it, but only out of necessity, and had not the skill. But you will be great.",
                "Today we hunt wild pig. Aim the bow carefully - do not scare it away. Go, my son."
            }
        });

        content.Add("Father_AfterKill", new Dialog
        {
            character = Character.Father,
            lines = new string[] {
                "Well done, my son!",
                "We are blessed to have wild animals here that we can eat.",
                "There is a plain, through that path just there to the north, leaving the forest. I have seen cows there.",
                "Yes, cows make for good meat, and cows are never found in the forest. But it is not safe out there, so promise me this:",
                "I will teach you the hunt, but you must never leave this forest. Ever. \nUnderstand?",
                "Good. Now, take the meat home. I will see you for siesta."
            }
        });

        content.Add("Father_Other", new Dialog
        {
            character = Character.Father,
            lines = new string[] {
                "Funny boy! That water belongs with your mother, not here! \n Come back after you have brought it to her!"
            }
        });

        content.Add("Father_Active", new Dialog
        {
            character = Character.Father,
            lines = new string[] {
                "I will teach you the hunt, but you must promise to never leave this forest. Ever.",
                "Now take that pig back to the fire pit!"
            }
        });

        content.Add("Father_Completed", new Dialog
        {
            character = Character.Father,
            lines = new string[] {
                "Come back again tomorrow and I will teach you more. When we hunt, we never leave this forest. Understand?",
            }
        });

        // ████████████████████████████████████████████████████████████████████████
        // ███ SISTER
        // ████████████████████████████████████████████████████████████████████████    

        content.Add("Sister_Start", new Dialog
        {
            character = Character.Sister,
            lines = new string[] {
                "Hello little brother! You are working hard today?",
                "Come around tomorrow, I may need your help with something."
            }
        });

    }
}
