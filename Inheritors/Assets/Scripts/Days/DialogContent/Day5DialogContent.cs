using System.Collections.Generic;
using UnityEngine;

public class Day5DialogContent : MonoBehaviour, DialogContent
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
        content.Add("Day5Opening_1", new Dialog
        {
            character = Character.Mother,
            lines = new string[] {
                "Good morning, son... were you sneaking around last night?",
                "It is important to get your sleep for the day's work!"
            }
        });

        content.Add("Day5Opening_2", new Dialog
        {
            character = Character.Mother,
            lines = new string[] {
                "There is much to get done today. Good luck, my son!"
            }
        });

        content.Add("DayOver", new Dialog
        {
            character = Character.Mother,
            lines = new string[] {
                "Come back inside now, son."
            }
        });

        // ████████████████████████████████████████████████████████████████████████
        // ███ MOTHER
        // ████████████████████████████████████████████████████████████████████████

        content.Add("Mother_Start", new Dialog
        {
            character = Character.Mother,
            lines = new string[] {
                "Ghosts, you say? Oh my son, you imagine so much. We are the only people on this land.",
                "If I did not know better, I would have thought maybe other Kanoe found this place.",
                "But there are none... oh, that is, none who would come this way.",
            }
        });

        content.Add("Mother_Active", new Dialog
        {
            character = Character.Mother,
            lines = new string[] {
                "Logs onto the fire pit, son. Place them carefully."
            }
        });

        content.Add("Mother_FinishTask", new Dialog
        {
            character = Character.Mother,
            lines = new string[] {
                "Good work. We had left the fire burning all night to cook the tapir. A feast is coming."
            }
        });

        content.Add("Mother_Completed", new Dialog
        {
            character = Character.Mother,
            lines = new string[] {
                "The southern entrance to the forest has changed... did some of the trees burn down?"
            }
        });

        // ████████████████████████████████████████████████████████████████████████
        // ███ FATHER
        // ████████████████████████████████████████████████████████████████████████

        content.Add("Father_Start", new Dialog
        {
            character = Character.Father,
            lines = new string[] {
                "The river flows again today in a way I haven't seen for years. It is incredible!",
                "We will surely catch many fish in this condition, so put your net into the water."
            }
        });

        content.Add("Father_HuntEnd", new Dialog
        {
            character = Character.Father,
            lines = new string[] {
                "What a catch! We are blessed by the ancestors today, my boy. They smile on us.",
                "You have seen their shadows? Well, there are many on this land. We were once thousands...",
                "I would not be surprised to hear that some are still with us."
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
                "Now I am certain our ancestors are still with us. They certainly helped our catch today."
            }
        });

        // ████████████████████████████████████████████████████████████████████████
        // ███ SISTER
        // ████████████████████████████████████████████████████████████████████████

        // Manofhole ----------------------------------------------------------

        content.Add("Manofhole_Start", new Dialog
        {
            character = Character.Manofhole,
            lines = new string[] {
                "I not do this... they do this... they, from outside.",
                "Your people... few. Mine... gone. Me... last one.",
                "Akuntsu... talk 'hope.' Not real. Akuntsu not learn.",
                "Instead... be fear. Not safe. Always they... hunting us, the outside. Want... our forest.",
                "No believe?",
                "Ask mother... why Kanoe not enemy Akuntsu, now.",
                "Ask elder... what happen at crush maloca.",
                "Ask father, grandfather... how get scars on back. What running from?",
                "My home, burned. Again... I run. Find new place. While can."
            }
        });

        content.Add("Manofhole_Completed", new Dialog
        {
            character = Character.Manofhole,
            lines = new string[] {
                "Ask mother... why Kanoe close to Akuntsu now.",
                "Ask elder... what happen at crush maloca.",
                "Ask father, grandfather... how get scars on back. What running from?",
                "My home, burned. Again... I run. Find new place. While can."
            }
        });

        // --------------------------------------------------------------------

        content.Add("Sister_Start", new Dialog
        {
            character = Character.Sister,
            lines = new string[] {
                "Have you seen the garden? It is growing more full every day. We will have so much for our family.",
                "Please find <color=blue>three papayas</color> for me at the southern entrance to this forest, brother."
            }
        });

        content.Add("Sister_Active", new Dialog
        {
            character = Character.Sister,
            lines = new string[] {
                "Soon the garden will have plentiful manioc and corn. But I need <color=blue>three papayas</color>, which only grow in the wild, south of the garden."
            }
        });

        content.Add("Sister_FinishTask", new Dialog
        {
            character = Character.Sister,
            lines = new string[] {
                "<i>WHAT?</i>",
                "Oh no. Those trees will no longer bear fruit if there has been a fire.",
                "Do not be afraid, little brother. Your big sister will figure something out.",
                "Find me tomorrow at that same burned patch of forest - we will do something to repair the damage.",
                "Hopefully the strange man has left by then. What did he say to you?"
            }
        });

        content.Add("Sister_Completed", new Dialog
        {
            character = Character.Sister,
            lines = new string[] {
                "Find me tomorrow at the burned patch of forest. There is always a way to regrow...",
                "Don't take the things that strange man said too seriously, little brother. He was trying to scare you."
            }
        });


        // ████████████████████████████████████████████████████████████████████████
        // ███ GRANDFATHER
        // ████████████████████████████████████████████████████████████████████████

        content.Add("Grandfather_Start", new Dialog
        {
            character = Character.Grandfather,
            lines = new string[] {
                "Ahh, my sweet grandson. I see it in you, in your aura. You have participated in the <color=red>festival</color>.",
                "Now now, don't try to hide it. I will not tell your mother. But now you are seeing the ghosts of the past.",
                "Be not alarmed. To see the past clearly is important. But still you miss the full picture.",
                "Play the matété again with me, grandson, and soothe the ghosts in anguish."
            }
        });

        content.Add("Grandfather_FinishTask", new Dialog
        {
            character = Character.Grandfather,
            lines = new string[] {
                "No history, no scar is too painful to be unsoothed by the magic duet of our matétés.",
                "This is why we Akuntsu never let it go while we still hold breath.",
                "Farewell for now, grandson!"
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
                "No history, no scar is too painful to be unsoothed by the magic duet of our matétés.",
                "This is why we Akuntsu never let it go while we still hold breath."
            }
        });


        // ████████████████████████████████████████████████████████████████████████
        // ███ GRANDMOTHER
        // ████████████████████████████████████████████████████████████████████████


        content.Add("Grandmother_Start", new Dialog
        {
            character = Character.Grandmother,
            lines = new string[] {
              "how are you feeling, youngest one? You see more clearly now, do you not?",
              "It is one thing to see one's roots, another to carry them out. Today you will grind uruku seeds, like our ancestors.",
              "These seeds are what gives my hair its bright red color.",
              "I have some ready in that bag - go on, make the dye from them."
            }
        });

        content.Add("Grandmother_Active", new Dialog
        {
            character = Character.Grandmother,
            lines = new string[] {
              "Go on, grind the uruku seed and extract the dye, and color yourself like the ancestors did."
            }
        });

        content.Add("Grandmother_FinishTask", new Dialog
        {
            character = Character.Grandmother,
            lines = new string[] {
              "Look at you! The ancestors truly live on in you now!",
              "I am getting quite old, young one, so you must carry on this Akuntsu tradition. Can you do that for me?",
              "Oh, <i>she</i> used to dye her hair every day... \n<i>she</i> would have loved you so much, if <i>she</i> were still here.",
            }
        });

        content.Add("Grandmother_Completed", new Dialog
        {
            character = Character.Grandmother,
            lines = new string[] {
              "The ancestors truly live on in you, grandson!",
              "Oh, how <i>she</i> would have loved you...",
            }
        });

    }
}
