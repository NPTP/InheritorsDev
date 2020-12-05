using System.Collections.Generic;
using UnityEngine;

public class Day2DialogContent : MonoBehaviour
{
    public Dictionary<string, Dialog> content = new Dictionary<string, Dialog>();

    void Start()
    {
        PackContent();
    }

    void PackContent()
    {
        content.Add("Day2Opening_1", new Dialog
        {
            character = Character.Mother,
            lines = new string[] {
                "It is so nice to see you running around, making your own path.",
                "I could not have imagined it. When I was young, the Kanoe and the Akuntsu would never...",
                "Well, maybe that is a story for when you are older.",
                "Here is what I need you to do today."
            }
        });

        content.Add("Day2Opening_2", new Dialog
        {
            character = Character.Mother,
            lines = new string[] {
                "You already know where to find your father, and the firewood.",
                "But this will be your first time helping your sister - ask her what she needs."
            }
        });

        content.Add("Mother_Start", new Dialog
        {
            character = Character.Mother,
            lines = new string[] {
                "There are so few of us now, but we are together. That is what matters.",
                "Son, you still have work to do!"
            }
        });

        content.Add("Mother_Active", new Dialog
        {
            character = Character.Mother,
            lines = new string[] {
                "Once you have all the wood, drop it onto the fire pit."
            }
        });

        content.Add("Mother_Completed", new Dialog
        {
            character = Character.Mother,
            lines = new string[] {
                "You are doing well, son. I am proud of you. Keep going!"
            }
        });

        content.Add("Father_Start", new Dialog
        {
            character = Character.Father,
            lines = new string[] {
                "Hello Son! You did well yesterday. Today we are catching fish from the River Omere.",
                "Extend your net and let them come naturally. Do not force them out of the water."
            }
        });

        content.Add("Father_HuntEnd", new Dialog
        {
            character = Character.Father,
            lines = new string[] {
                "Well done.",
                "You need not bring any fish home now. One fish is too few, so I will stay to catch more.",
                "Your grandfather and I will be placing the bridge over the river right here, tomorrow.",
                "Once we have done so, your grandfather would like to see you."
            }
        });

        content.Add("Father_Completed", new Dialog
        {
            character = Character.Father,
            lines = new string[] {
                "Tomorrow we will have the bridge over the river again. Your grandfather has been waiting to see you."
            }
        });

        content.Add("Sister_Start", new Dialog
        {
            character = Character.Sister,
            lines = new string[] {
                "There is a tree on the other side of the river that is hundreds of years old. I wonder how deep are its roots...",
                "But here, I have been growing fruit! We need to pick <color=blue>six papayas</color>, one for each of the family.",
                "Gather from my garden to start. If you need more, check the <color=green>south entrance</color> to this forest.",
                "Come back when you have <color=blue>six</color> and drop them in the bucket."
            }
        });

        content.Add("Sister_Active", new Dialog
        {
            character = Character.Sister,
            lines = new string[] {
                "Have you found all the papayas yet? We need <color=blue>six</color>!",
                "There are 3 in my garden, and 3 near the <color=green>south entrance</color> to this forest."
            }
        });

        content.Add("Sister_Completed", new Dialog
        {
            character = Character.Sister,
            lines = new string[] {
                "Thank you little brother! That is all we need!",
                "Come back tomorrow, and we will harvest again."
            }
        });

        content.Add("DayOver", new Dialog
        {
            character = Character.Mother,
            lines = new string[] {
                "Thank you, son. That is all your work for today!",
                "Let us take a siesta. Come inside with me."
            }
        });
    }
}
