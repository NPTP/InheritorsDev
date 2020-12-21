using System.Collections.Generic;
using UnityEngine;

public class Day6DialogContent : MonoBehaviour, DialogContent
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
        content.Add("NULL", new Dialog
        {
            character = Character.Mother,
            lines = new string[] {
                "< NULL > "
            }
        });

        content.Add("Day6Opening_1", new Dialog
        {
            character = Character.Mother,
            lines = new string[] {
                "Son, I have told you already. Your father and I had no choice but to come together.",
                "The Kanoe and Akuntsu had been opposed, yes, but if not for a common enemy, <i>you</i> would never have known life.",
                "Forget what the strange man said. We are stronger together, all of us, the tribe, the family.",
                "Now, what do you need to do today?"
            }
        });

        content.Add("Day6Opening_2", new Dialog
        {
            character = Character.Sister,
            lines = new string[] {
                "Things are not so bad. My garden at home is flourishing, and even the burned spot will sprout again.",
                "I am taking care to tend it, so that by tomorrow, you can come help me plant new vegetables!",
                "Keep your eyes to the future, little brother."
            }
        });

        content.Add("DayOver", new Dialog
        {
            character = Character.Mother,
            lines = new string[] {
                "Come inside now and rest, son. You have done all you can!"
            }
        });

        // ████████████████████████████████████████████████████████████████████████
        // ███ MOTHER
        // ████████████████████████████████████████████████████████████████████████

        content.Add("Mother_Start", new Dialog
        {
            character = Character.Mother,
            lines = new string[] {
                "Seeing ghosts and talking to strange men from the woods... my son is already a shaman!",
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
                "Son, these scars are from a life before you. <i>You</i>, who came and made all anew! Do not worry yourself about them.",
                "I see the ancestors now, just as you do! See them in the trees - they are here now.",
                "They must be watching over our hunt. We are sure to succeed! Kill the pig now, son."
            }
        });

        content.Add("Father_Other", new Dialog
        {
            character = Character.Father,
            lines = new string[] {
                "That strange man is not to be trusted, son. Come back after you have finished your task."
            }
        });

        content.Add("Father_Active", new Dialog
        {
            character = Character.Father,
            lines = new string[] {
                "<UNUSED>"
            }
        });


        content.Add("Father_HuntEnd", new Dialog
        {
            character = Character.Father,
            lines = new string[] {
                "I... I do not know... what that was.",
                "The shadows threw their fire and smoke and noise toward the pig and... every animal has fled in fear!",
                "Listen, son... \nthe whole forest has quieted.",
                "We will not have another chance to kill anything for at least a day.",
                "Alright, with our combined skill and effort, we will surely catch something tomorrow. Come back tomorrow.",
            }
        });

        content.Add("Father_Completed", new Dialog
        {
            character = Character.Father,
            lines = new string[] {
                "I wish that I knew what happened here today.",
                "Before I gained these scars, we lived in a place where everything was plentiful.\nNow, we rely on luck.",
                "But do not fret, my boy. Come again tomorrow and we will surely catch enough meat for the whole tribe."
            }
        });

        // ████████████████████████████████████████████████████████████████████████
        // ███ SISTER
        // ████████████████████████████████████████████████████████████████████████

        content.Add("Sister_Start", new Dialog
        {
            character = Character.Sister,
            lines = new string[] {
                "Soon all that burned will be green again, and we will have a rich harvest.",
                "You see? Nothing to be worried about. Some things must burn down to allow greater growth from the ashes.",
                "Meet me there, where the fire was, tomorrow. I will need your help."
            }
        });

        content.Add("Sister_Other", new Dialog
        {
            character = Character.Sister,
            lines = Get("Sister_Start").lines
        });

        // content.Add("Sister_Start", new Dialog
        // {
        //     character = Character.Sister,
        //     lines = new string[] {
        //         "You changed your hair, little brother. The uruku dye?",
        //         "Things are not so bad after all. The garden at home is flourishing, and even this blackened spot will sprout again.",
        //         "Take and plant these manioc seeds in <color=blue>four spots</color> in the ash."
        //     }
        // });

        // content.Add("Sister_Other", new Dialog
        // {
        //     character = Character.Sister,
        //     lines = new string[] {
        //         "Come back when you have finished what you are working on. I am working the ashen soil."
        //     }
        // });

        // content.Add("Sister_Active", new Dialog
        // {
        //     character = Character.Sister,
        //     lines = new string[] {
        //         "You only need plant the seeds in <color=blue>four spots</color> on the ash."
        //     }
        // });

        // content.Add("Sister_FinishTask", new Dialog
        // {
        //     character = Character.Sister,
        //     lines = new string[] {
        //         "Very good, little brother. Soon this spot will be green again, and will provide for us a rich harvest.",
        //         "You see? Nothing to be worried about. Some things must burn down to allow greater growth from the ashes.",
        //         "Come back to this spot again tomorrow - I will need your help."
        //     }
        // });

        // content.Add("Sister_Completed", new Dialog
        // {
        //     character = Character.Sister,
        //     lines = new string[] {
        //         "The garden at my home is flourishing, brother, it does not need any help now.",
        //         "So come here to the ashen ground again tomorrow, where we will establish our second garden!",
        //     }
        // });


        // ████████████████████████████████████████████████████████████████████████
        // ███ GRANDFATHER
        // ████████████████████████████████████████████████████████████████████████

        content.Add("Grandfather_Start", new Dialog
        {
            character = Character.Grandfather,
            lines = new string[] {
                "I knew it would come to such questions, but I did not expect them to come from encountering the Man of the Hole.",
                "Grandson, you are lucky to have even seen him - for he is the last of his tribe.",
                "Even I cannot say what happened to the rest of them. But his solitude <i>now</i> is his own choice, when we are here with open arms.",
                "Fear does not serve us. My scars are the same as your father's. In our former lands, they came, and they shot, and we ran...",
                "But they did not kill me. And I am your chief, always present, always leading our tribe and family toward hope.",
                "Right now, your grandmother is sick. I have done every ritual that will help her. The next step is yours. Go to her.",
                "With your help, she can only improve in health."
            }
        });

        content.Add("Grandfather_Other", new Dialog
        {
            character = Character.Grandfather,
            lines = new string[] {
                "Return when you are finished your present work, and I promise to answer your questions."
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
                "Do not fear, grandson. Loss is part of life. This land and the future is yours, and with hope, we will overcome any fear.",
                "Right now, your grandmother is sick. I have done every ritual that will help her. The next step is yours. Go to her.",
                "With your help, she can only improve in health."
            }
        });

        // ████████████████████████████████████████████████████████████████████████
        // ███ GRANDMOTHER
        // ████████████████████████████████████████████████████████████████████████

        content.Add("Grandmother_Start", new Dialog
        {
            character = Character.Grandmother,
            lines = new string[] {
              "Ohh... youngest one, I am so grateful you came. Have the effects of the festival worn off by now?",
              "I have an illness I have never encountered before. Men from outside the forest came in the night and brought it with them.",
              "I have tried all the herbs and medicines we have, but for one which grows on the side of the hill.",
              "I am too sick and tired to retrieve it myself. Please, ascend the path, and you will see the <color=blue>herbs</color>.",
              "Then I will answer what you have been asking - what the Man of the Hole told you about."
            }
        });

        content.Add("Grandmother_Other", new Dialog
        {
            character = Character.Grandmother,
            lines = new string[] {
                "I see you are working on something, young one, but come back quickly when you are done. I need your help."
            }
        });

        content.Add("Grandmother_Active", new Dialog
        {
            character = Character.Grandmother,
            lines = new string[] {
              "The <color=blue>herbs</color> I need only grow on the hill, on the path to where we performed the festival of senses.",
              "When you have them, drop them in the bucket there."
            }
        });

        content.Add("Grandmother_FinishTask", new Dialog
        {
            character = Character.Grandmother,
            lines = new string[] {
              "Ohh, thank you, youngest one. I am strong enough to prepare the herbs myself - you have done the difficult work.",
              "Now, you asked about the crushed maloca, near the river crossing. Yes, a member of our tribe and family once lived there.",
              "She was your sister. Not the one you know now, but <i>another</i> sister. So much like you, she was. But the forest... claimed her.",
              "Not long after her passing, you were born to us. And so, our family grew again. As it will continue to.",
              "Please come back again tomorrow, these herbs will only last a day. \nUntil then, sweet boy."
            }
        });

        content.Add("Grandmother_Completed", new Dialog
        {
            character = Character.Grandmother,
            lines = new string[] {
              "You never met your other sister, but I see a part of her in you. Her spirit and yours are intertwined.",
              "Please come back again tomorrow."
            }
        });

    }
}
