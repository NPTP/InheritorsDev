﻿using System.Collections.Generic;
using UnityEngine;

public class Day2DialogContent : MonoBehaviour, DialogContent
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
        content.Add("Day2Opening_1", new Dialog
        {
            character = Character.Mother,
            lines = new string[] {
                "It is so nice to see you running around, making your own path.",
                "I could not have imagined it. When I was young, the Kanoê and the Akuntsu would never...",
                "Well, maybe that is a story for when you are older.",
                "Here is what I need you to do today."
            }
        });

        content.Add("Day2Opening_2", new Dialog
        {
            character = Character.Mother,
            lines = new string[] {
                "Your father is fishing today. He will be beside the river, waiting for you to come help.",
                "And this will be your first time helping your sister - ask her what she needs.",
                "Off you go now, son!"
            }
        });

        content.Add("DayOver", new Dialog
        {
            character = Character.Mother,
            lines = new string[] {
                "Thank you, son. That is all your work for today!",
                "Come back home now."
            }
        });

        // ████████████████████████████████████████████████████████████████████████
        // ███ MOTHER
        // ████████████████████████████████████████████████████████████████████████    

        content.Add("Mother_Start", new Dialog
        {
            character = Character.Mother,
            lines = new string[] {
                "There are so few of us now, but we are together. That is what matters.",
                "Son, you still have work to do!"
            }
        });

        content.Add("Mother_Other", new Dialog
        {
            character = Character.Mother,
            lines = new string[] {
                "Look at you, my boy, running to help everyone. I am so proud of you."
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
            character = Character.Father,
            lines = new string[] {
                "Hello Son! You did well yesterday. Today we are catching fish from the River Omerê.",
                "I will teach you. We extend the net, and let them come naturally.",
                "You cannot force the fish from the water on its own accord. First, the net must surround it...",
                "Then it has nowhere to go.",
                "I will show you - be ready to catch whatever I pull out of the river!"
            }
        });

        content.Add("Father_Other", new Dialog
        {
            character = Character.Father,
            lines = new string[] {
                "Hello, my boy. You are working on something else now, but when you are done, I will show you how to catch fish."
            }
        });

        content.Add("Father_HaveFish", new Dialog
        {
            character = Character.Father,
            lines = new string[] {
                "Good catch, son! Put the fish into the bucket. It is the first of hopefully many to come."
            }
        });

        content.Add("Father_Active", new Dialog
        {
            character = Character.Father,
            lines = new string[] {
                "Drop the fish into the bucket just there, son. You cannot carry it around all day!"
            }
        });

        content.Add("Father_HuntEnd", new Dialog
        {
            character = Character.Father,
            lines = new string[] {
                "Thank you. You need not bring any fish home now. One fish is too few, so I will stay to catch more.",
                "Your grandfather and I will be placing the bridge over the river - right here - tomorrow.",
                "Once we have done so, your grandfather would like to see you.",
                "Bye for now, my boy."
            }
        });

        content.Add("Father_Completed", new Dialog
        {
            character = Character.Father,
            lines = new string[] {
                "Tomorrow we will have the bridge over the river again. Your grandfather has been waiting to see you."
            }
        });

        // ████████████████████████████████████████████████████████████████████████
        // ███ SISTER
        // ████████████████████████████████████████████████████████████████████████    

        content.Add("Sister_Start", new Dialog
        {
            character = Character.Sister,
            lines = new string[] {
                "There is a tree on the other side of the river that is hundreds of years old. I wonder how deep are its roots...",
                "But here, I have been growing fruit! \nWe need to pick <color=blue>three papayas</color>.",
                "I saw them growing on some small trees south of mother's maloca.",
                "Come back when you have <color=blue>three</color> and drop them in the bucket."
            }
        });

        content.Add("Sister_Other", new Dialog
        {
            character = Character.Sister,
            lines = new string[] {
                "Oh, little brother. Finish your work for father first, then come back!"
            }
        });

        content.Add("Sister_Active", new Dialog
        {
            character = Character.Sister,
            lines = new string[] {
                "You should be able to find <color=blue>three papayas</color> to the south of mother's maloca.",
                "When you have them all, just drop them in the bucket over there!"
            }
        });

        content.Add("Sister_Completed", new Dialog
        {
            character = Character.Sister,
            lines = new string[] {
                "Thank you little brother! That is all we need!",
                "Come back tomorrow. I have started growing the garden - we will make our first harvest."
            }
        });
    }
}
