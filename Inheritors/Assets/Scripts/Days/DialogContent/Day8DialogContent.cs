﻿using System.Collections.Generic;
using UnityEngine;

public class Day8DialogContent : MonoBehaviour, DialogContent
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
        content.Add("Day8Opening_1", new Dialog
        {
            character = Character.Mother,
            lines = new string[] {
                "It is a cold morning.",
                "They have built across our land. \nThe damage is irreparable.",
                "Your sister... she is not well. I need you to talk to her. Perhaps you can brighten her spirits.",
                "Check on your father, too. He should be returning with fresh meat soon.",
                "Do not forget your grandmother - she must be better by now, so please wake her.",
            }
        });

        content.Add("Day8Opening_2", new Dialog
        {
            character = Character.Mother,
            lines = new string[] {
                "I am sorry I never told you about your other sister. We loved her very much. You would have loved her, too.",
                "We have little meat, and our harvest was ruined. We are running out of food.",
                "We cannot live on water alone, but it will have to do for now. Please bring some."
            }
        });

        content.Add("DayOver", new Dialog
        {
            character = Character.Mother,
            lines = new string[] {
                "It has been a difficult day for the tribe, son. Come back immediately."
            }
        });

        // ████████████████████████████████████████████████████████████████████████
        // ███ MOTHER
        // ████████████████████████████████████████████████████████████████████████

        content.Add("Mother_Start", new Dialog
        {
            character = Character.Mother,
            lines = new string[] {
                "We need more water. But we are running out of meat. And our harvest is ruined.",
                "We cannot live on water alone. \nSomething must be done."
            }
        });

        content.Add("Mother_Other", new Dialog
        {
            character = Character.Mother,
            lines = new string[] {
                "You work so hard. You help us so much.",
                "I wish that your oldest sister hadn't left us so early, and could help you in times like these."
            }
        });

        content.Add("Mother_Active", new Dialog
        {
            character = Character.Mother,
            lines = new string[] {
                "As much water as you can bring, son, please."
            }
        });

        content.Add("Mother_Completed", new Dialog
        {
            character = Character.Mother,
            lines = new string[] {
                "I hope your father comes back soon, and the garden can be regrown...",
                "We need to feed our family."
            }
        });

        // ████████████████████████████████████████████████████████████████████████
        // ███ FATHER
        // ████████████████████████████████████████████████████████████████████████

        content.Add("Father_Start", new Dialog
        {
            character = Character.Narrator,
            lines = new string[] {
                "Father is not here yet.",
                "I should come back tomorrow. \nHe will return. \nHe always does."
            }
        });

        content.Add("Father_Other", new Dialog
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
                "<i>GO AWAY!</i>",
                ". . .    ",
                "Oh, it is you, little brother? Sorry. I cannot do this anymore. I cannot take it.",
                "Everything we worked towards, everything we grew, they just... built over it.",
                "We were so close to a giant harvest, and now we have nothing.",
                "Thank you for coming here, brother, but please leave me be alone inside for today... \ncome back tomorrow."
            }
        });

        content.Add("Sister_Other", new Dialog
        {
            character = Character.Sister,
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
            character = Character.Grandfather,
            lines = new string[] {
                "The events of this day fill me with sorrow. I weep for our land, and our people.",
                "But know this, grandson. We have been through worse, and we survived.",
                "Many Akuntsu gave up hope too early - \nand we miss them every day. \nBut our family never gave it up.",
                "I am the shaman, I am your chief, I will protect this land from outsiders. \nAnd <i>you</i> will inherit it.",
                "Come, take your matété, let us call the ancestors with a rousing duet, and clear all bad blood from this place."
            }
        });

        content.Add("Grandfather_Other", new Dialog
        {
            character = Character.Grandfather,
            lines = new string[] {
                "You always bring me hope, grandson. Come back to play your matété when you have a chance."
            }
        });

        content.Add("Grandfather_Active", new Dialog
        {
            character = Character.Grandfather,
            lines = new string[] {
                "Pick up your matété and play with me. The ancestors will hear us!"
            }
        });

        content.Add("Grandfather_FinishTask", new Dialog
        {
            character = Character.Grandfather,
            lines = new string[] {
                "The ancestors have heard our duet, grandson. And they watch over this land still.",
                "Remember that I am your chief. \nAs long as you and I are here, we will <i>never</i> leave this land.",
                "This is our home, it has always been, and always will be!"
            }
        });

        content.Add("Grandfather_Completed", new Dialog
        {
            character = Character.Grandfather,
            lines = new string[] {
                "Should you lose hope, child, know that as long as you and I are here, we will <i>never</i> leave this land.",
            }
        });

        // ████████████████████████████████████████████████████████████████████████
        // ███ GRANDMOTHER
        // ████████████████████████████████████████████████████████████████████████

        content.Add("Grandmother_Start", new Dialog
        {
            character = Character.Narrator,
            lines = new string[] {
              ". . .       ",
              "Grandmother won't wake up.",
              "The outside illness has put her to sleep."
            }
        });

        content.Add("Grandmother_Other", new Dialog
        {
            character = Character.Grandmother,
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
