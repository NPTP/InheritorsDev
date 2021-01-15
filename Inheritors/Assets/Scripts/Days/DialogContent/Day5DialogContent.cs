﻿using System.Collections.Generic;
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
            character = Character.Father,
            lines = new string[] {
                "Good morning, son... were you sneaking around last night?",
                "It is important to get your sleep for the day's work!",
                "We are having a feast later, so finish these tasks and come home quickly!"
            }
        });

        content.Add("Day5Opening_2", new Dialog
        {
            character = Character.Mother,
            lines = new string[] {
                "Ghosts, you say? No, I do not see them! \nOh my son, you imagine so much. We are the only people on this land.",
                "If I did not know better, I would have thought maybe other Kanoê found this place.",
                "But there are none... oh, that is, none who would come this way."
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
                "Tell me, son, what do these ghosts look like?",
                "Like you? And there are how many? \nI can hardly handle one boy with so much energy already!"
            }
        });

        content.Add("Mother_Other", new Dialog
        {
            character = Character.Mother,
            lines = Get("Mother_Start").lines
        });

        content.Add("Mother_Active", new Dialog
        {
            character = Character.Mother,
            lines = new string[] {
                "<UNUSED>"
            }
        });

        content.Add("Mother_FinishTask", new Dialog
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
                "The south of our forest has changed... did some trees burn down?"
            }
        });

        // ████████████████████████████████████████████████████████████████████████
        // ███ FATHER
        // ████████████████████████████████████████████████████████████████████████

        content.Add("Father_Start", new Dialog
        {
            character = Character.Father,
            lines = new string[] {
                "What a catch I had! We are blessed by the ancestors today, my boy. They smile on us.",
                "You have seen them? Little boys like you? Well, there are many ancestors on this land. We were once thousands...",
                "I would not be surprised to hear that some of my elders are still with us.",
            }
        });

        content.Add("Father_Other", new Dialog
        {
            character = Character.Father,
            lines = Get("Father_Start").lines
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
                "I not do this... \n<i>they</i> do this... \nthey, from outside.",
                "Your people... few. \nMine... gone. \nMe... last one.",
                "Akuntsu... talk 'hope.' \nNot real. \nAkuntsu not learn.",
                "Instead... be fear. Not safe. \nAlways they... hunting us, the outside. \nWant... our forest.",
                "No believe?",
                "Ask mother... why Kanoê not enemy Akuntsu, now.",
                "Ask elder... what happen at crush maloca.",
                "Ask father, grandfather... how get scars on back. What running from?",
                "My home, burned. Again... I run. \nFind new place. \nWhile can.",
                "Go tell sister. \nFruit is gone. \nNot grow back."
            }
        });

        content.Add("Manofhole_Completed", new Dialog
        {
            character = Character.Manofhole,
            lines = new string[] {
                "Ask mother... why Kanoê close to Akuntsu now.",
                "Ask elder... what happen at crush maloca.",
                "Ask father, grandfather... how get scars on back. What running from?",
                "My home, burned. Again... I run. Find new place. While can.",
                "Go tell sister. \nFruit is gone. \nNot grow back."
            }
        });

        // --------------------------------------------------------------------

        content.Add("Sister_Start", new Dialog
        {
            character = Character.Sister,
            lines = new string[] {
                "Have you seen the garden? It is growing more full every day. We will have so much for our family.",
                "Please find <color=blue>three papayas</color> for me at the trees south of mother's maloca, where you found them before."
            }
        });

        content.Add("Sister_Other", new Dialog
        {
            character = Character.Sister,
            lines = new string[] {
                "Come back when you are free, brother. I will need your help again!"
            }
        });

        content.Add("Sister_Active", new Dialog
        {
            character = Character.Sister,
            lines = new string[] {
                "Soon the garden will have plentiful manioc and corn. But first we need <color=blue>3 papayas</color>, which only grow south of mother's maloca."
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
                "Now now, don't try to hide it. I will not tell your mother. But now you are seeing your own ghosts of the past.",
                "Be not alarmed. To see the past clearly is important. But still you miss the full picture.",
                "Play the matété again with me, grandson, and serenade the spirits."
            }
        });

        content.Add("Grandfather_Other", new Dialog
        {
            character = Character.Grandfather,
            lines = new string[] {
                "Grandson, return when you are able, and not burdened by the task you presently pursue."
            }
        });

        content.Add("Grandfather_Active", new Dialog
        {
            character = Character.Grandfather,
            lines = new string[] {
                "Please, pick up your matété, and I will teach you another traditional melody."
            }
        });

        content.Add("Grandfather_StartTask", new Dialog
        {
            character = Character.Grandfather,
            lines = new string[] {
                "Just as before, grandson. Listen carefully, then you will try."
            }
        });

        content.Add("Grandfather_ContinueTask", new Dialog
        {
            character = Character.Grandfather,
            lines = new string[] {
                "Feel the forest through the wood of the matété. Let the ancestors speak through you."
            }
        });

        content.Add("Grandfather_FinishTask", new Dialog
        {
            character = Character.Grandfather,
            lines = new string[] {
                "You play beautifully, so full of life. It is a gift that soothes old pains and scars.",
                "Soon you will be ready for us to play together, in a full duet that shall rouse the ancestors.",
                "Farewell for now, grandson!"
            }
        });

        content.Add("Grandfather_Completed", new Dialog
        {
            character = Character.Grandfather,
            lines = new string[] {
                "Soon you will be ready for us to play together, in a full duet that shall rouse the ancestors.",
                "Farewell for now, grandson!"
            }
        });

        // ████████████████████████████████████████████████████████████████████████
        // ███ GRANDMOTHER
        // ████████████████████████████████████████████████████████████████████████


        content.Add("Grandmother_Start", new Dialog
        {
            character = Character.Grandmother,
            lines = new string[] {
              "How are you feeling, youngest one? The yopo should be fading... but you see more clearly now, do you not?",
              "It is one thing to see one's roots, another to carry them out. Today you will grind uruku seeds, like our ancestors.",
              "These seeds are what gives my hair its bright red color.",
              "I have some ready in that bag - go on, make the dye from them."
            }
        });

        content.Add("Grandmother_Other", new Dialog
        {
            character = Character.Grandmother,
            lines = new string[] {
              "With every step you take, all the work you do, they seem to shadow your every move, don't they?",
              "The ancestors push and pull at the forest itself to help you expand into the fullness of an Akuntsu future...",
              "I will connect you even closer to them when you are finished what you are doing right now."
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
              "Oh, <i>she</i> used to dye her hair every day... \n<i>she</i> would have loved you so much. \nIf she were still here.",
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
