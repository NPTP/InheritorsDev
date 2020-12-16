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

        content.Add("Dialog_HavePig", new Dialog
        {
            character = Character.Mother,
            lines = new string[] {
                "You and father have caught a pig? It is beautiful.",
                "Place it over the fire. We will cook it later."
            }
        });

        content.Add("Dialog_HaveWater", new Dialog
        {
            character = Character.Mother,
            lines = new string[] {
                "Just pour the water into the big grey pot."
            }
        });

        content.Add("Dialog_HuntBegin", new Dialog
        {
            character = Character.Father,
            lines = new string[] {
                "Son! Finally you are old enough and we have enough men to hunt. I will teach you.",
                "The women tried their hand at it, but only out of necessity, and had not the skill. But you will be great.",
                "Today we hunt wild pig. Aim the bow carefully - do not scare it away. Go, my son."
            }
        });

        content.Add("Dialog_HuntEnd", new Dialog
        {
            character = Character.Father,
            lines = new string[] {
                "Well done, my son!",
                "We are blessed to have wild animals here that we can eat.",
                "There is a plain, through that path to the north, leaving the forest. I have seen cows there.",
                "Yes, cows would make for good meat, and cows are not found in the forest. But it not safe out there, so make me a promise.",
                "I will teach you the hunt, but you must promise to never leave this forest. Ever. Understand?",
                "Good. Now, take the meat home. I will see you for siesta."
            }
        });

        content.Add("Dialog_NoHunt", new Dialog
        {
            character = Character.Father,
            lines = new string[] {
                "Funny boy! That water belongs with your mother, not here!"
            }
        });

        content.Add("Dialog_HuntOver", new Dialog
        {
            character = Character.Father,
            lines = new string[] {
                "I will teach you the hunt, but you must promise to never leave this forest. Ever. Understand?",
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

        content.Add("Dialog_Sister", new Dialog
        {
            character = Character.Sister,
            lines = new string[] {
                "Hello little brother! You are working hard today?",
                "Come around tomorrow, I may need your help with something."
            }
        });

    }
}
