using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu]
public class Day2DialogContent : ScriptableObject
{
    public Dictionary<string, Dialog> content = new Dictionary<string, Dialog>();

    void OnEnable()
    {
        PackContent();
    }

    void PackContent()
    {
        content.Add("Day2Opening_1", new Dialog
        {
            character = Character.Mother,
            lines = new string[] {
                "It’s so nice to see you running around making your own path here.",
                "I couldn’t have imagined it. When I was young, the Kanoe and the Akuntsu would never...",
                "Well, maybe that’s a story for when you’re older.",
                "Here’s what I need you to do today."
            }
        });

        content.Add("Day2Opening_2", new Dialog
        {
            character = Character.Mother,
            lines = new string[] {
                "You already know where to find your father, and the firewood.",
                "But this will be your first time helping your sister! Ask her what she needs. Off you go now."
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
                "Once you have all the wood, just drop it onto the firepit."
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
                "Hey! Son! You did good yesterday. Now, today, we’ve got a faster and smaller target than a pig: the agoutis.",
                 "Try to hit one of their group; you’ll have to lead your target. That’ll <i>really</i> teach you how to use a bow."
            }
        });

        content.Add("Father_HuntEnd", new Dialog
        {
            character = Character.Father,
            lines = new string[] {
                "Nice shot!",
                "Don’t worry about bringing any meat home today. One agoutis is too small, so dad is going to catch a few more first.",
                "Hey, me and your grandfather were hoping to get the bridge back up over the river tomorrow.",
                "When we do, you should go across and say hi to him. It’s been a while."
            }
        });

        content.Add("Father_Completed", new Dialog
        {
            character = Character.Father,
            lines = new string[] {
                "Tomorrow we'll have the bridge put up over the river again. Your grandfather's been waiting to see you."
            }
        });

        content.Add("Sister_Start", new Dialog
        {
            character = Character.Sister,
            lines = new string[] {
                "I heard that on the other side of the river, there’s a tree bigger and older than any other.",
                "Grandmother said she climbed it as a child, as her own grandmother did before her.",
                "I've grown a lot of plants, but nothing like that. If we get to cross the river soon, let's mark our names on it!",
                "I wonder how deep its roots go...",
                "But first! Let’s get the <color=blue>papayas</color> we need. 6 in total - one for each of the family.",
                "Gather some from my garden, but it may not be enough. I think I saw some more growing at the <color=green>south entrance</color> to this forest.",
                "Come back when you have <color=blue>6 papayas</color>, and drop them in my bucket. I’m sure I could eat all 6 myself…"
            }
        });

        content.Add("Sister_Active", new Dialog
        {
            character = Character.Sister,
            lines = new string[] {
                "Have you found all the papayas yet? We need 6!",
                "There are 3 in my garden, and 3 near the <color=green>south entrance</color> to this forest."
            }
        });

        content.Add("Sister_Completed", new Dialog
        {
            character = Character.Sister,
            lines = new string[] {
                "Thanks little bro! That's all we need!",
                "Maybe you're not so lazy after all. Come back again tomorrow, and we'll grow something new."
            }
        });

        content.Add("DayOver", new Dialog
        {
            character = Character.Mother,
            lines = new string[] {
                "Thank you, son. That's everything for today!",
                "You've been hard at work, it's time for a siesta. Come on inside."
            }
        });
    }
}
