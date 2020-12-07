using System.Collections.Generic;
using UnityEngine;

public class Day6DialogContent : MonoBehaviour
{
    public Dictionary<string, Dialog> content = new Dictionary<string, Dialog>();

    void Awake()
    {
        PackContent();
    }

    void PackContent()
    {
        content.Add("Day6Opening_1", new Dialog
        {
            character = Character.Mother,
            lines = new string[] {
                "Son, I have told you already. Your father and I had no choice but to come together.",
                "The Kanoe and Akuntsu had been enemies, yes, but now we are together.",
                "Forget what the strange man said. We are stronger together, all of us, the tribe, the family.",
                "Now, what do you need to do today?"
            }
        });

        content.Add("Day6Opening_2", new Dialog
        {
            character = Character.Mother,
            lines = new string[] {
                "Keep your eyes to the future, my son. The past need not haunt us. Do well!"
            }
        });

        content.Add("DayOver", new Dialog
        {
            character = Character.Mother,
            lines = new string[] {
                "It has been a hard day, but our tribe has survived a long time, and so we will continue to do.",
                "Come inside now and rest. You have done all you can!"
            }
        });

        // ████████████████████████████████████████████████████████████████████████
        // ███ MOTHER
        // ████████████████████████████████████████████████████████████████████████

        content.Add("Mother_Start", new Dialog
        {
            character = Character.Mother,
            lines = new string[] {
                "Seeing ghosts and talking to strange men from the woods... my son is already a shaman!"
            }
        });

        content.Add("Mother_Active", new Dialog
        {
            character = Character.Mother,
            lines = new string[] {
                "Water into the grey pot, as always."
            }
        });

        content.Add("Mother_Completed", new Dialog
        {
            character = Character.Mother,
            lines = new string[] {
                "Imagine if we had still been enemies, your father and I? Then you would never have known life.",
                "Since our union, I no longer understand the drive to dominance over the other. It never helped either of us."
            }
        });

        // ████████████████████████████████████████████████████████████████████████
        // ███ FATHER
        // ████████████████████████████████████████████████████████████████████████

        content.Add("Father_Start", new Dialog
        {
            character = Character.Father,
            lines = new string[] {
                "Son, these scars are from a past life, before you, who made all life anew. Do not worry yourself about them!",
                "The future is what belongs to you, not the past! And look, I see the ancestors now as well, just like you!",
                "Their shadows are in the trees, waiting, watching over our hunt! We are sure to succeed! Let us catch the pig!"
            }
        });

        content.Add("Father_HuntEnd", new Dialog
        {
            character = Character.Father,
            lines = new string[] {
                "I do not know... what that was.",
                "Some kind of fire and smoke and noise thrown from the forest. Every animal in sight has fled.",
                "We won't have another chance to kill anything for at least a day! Bad fortune...",
                "We will have to live without meat today, son, but no matter. You are so skilled now, we will catch something tomorrow."
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
                "Before I had these scars, we lived in a place where everything was plentiful. Now we rely on luck.",
                "But do not fret, my boy. Come again tomorrow and we will surely catch enough meat for the tribe."
            }
        });

        // ████████████████████████████████████████████████████████████████████████
        // ███ SISTER
        // ████████████████████████████████████████████████████████████████████████

        content.Add("Sister_Start", new Dialog
        {
            character = Character.Sister,
            lines = new string[] {
                "You changed your hair, little brother. The uruku dye?",
                "So it is not so bad after all. The garden at home is flourishing, and even this blackened spot will be able to sprout anew.",
                "Take and plant these manioc seeds in <color=blue>three spots</color> in the ash."
            }
        });

        content.Add("Sister_Active", new Dialog
        {
            character = Character.Sister,
            lines = new string[] {
                "You only need plant the seeds in <color=blue>three spots</color> on the ash."
            }
        });

        content.Add("Sister_FinishTask", new Dialog
        {
            character = Character.Sister,
            lines = new string[] {
                "Very good, little brother. Soon this spot will be green again, and will provide for us a rich harvest.",
                "You see? Nothing to be worried about. To burn down and start again is the cycle of all things from the earth.",
                "Come here again tomorrow - I will need your help."
            }
        });

        content.Add("Sister_Completed", new Dialog
        {
            character = Character.Sister,
            lines = new string[] {
                "The garden at my home is flourishing, brother, it does not need any help now.",
                "So come here to the ashen ground again tomorrow, where we will establish our second garden!",
            }
        });


        // ████████████████████████████████████████████████████████████████████████
        // ███ GRANDFATHER
        // ████████████████████████████████████████████████████████████████████████

        content.Add("Grandfather_Start", new Dialog
        {
            character = Character.Grandfather,
            lines = new string[] {
                "I knew it would come to such questions, but I did not expect them to come from encountering the man of the hole.",
                "Grandson, you are lucky to have even seen him - for he is the last of his tribe.",
                "Even I cannot say what happened to the rest of them. But his solitude <i>now</i> is his own choice, when we are here with open arms.",
                "Fear does not serve us. My scars are the same as your father's. In our former lands, they came, and they shot, and we ran...",
                "But they did not kill me. And I am your chief, always present, always leading our tribe and family toward hope.",
                "Right now, your grandmother is sick. I have done every ritual that will help her. The next step is yours. Go to her.",
                "With your help, she can only improve in health."
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
              "Ohh... I am so grateful for you. Youngest one, I have an illness I have never encountered before.",
              "Men from outside the forest came in the night and brought it with them.",
              "I have tried all the herbs and medicines we have, but for one which grows at the top of the hill.",
              "I am too old and tired to retrieve it myself. Please, go atop the hill, and bring the <color=blue>herbs</color> to me.",
              "Then I will answer what you have been asking, what the man of the hole told you about, the crushed maloca."
            }
        });

        content.Add("Grandmother_Active", new Dialog
        {
            character = Character.Grandmother,
            lines = new string[] {
              "The <color=blue>herbs</color> I need only grow at the very top of the hill, where we performed the festival of senses.",
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
              "Please come back again tomorrow, these herbs will only last a day."
            }
        });

        content.Add("Grandmother_Completed", new Dialog
        {
            character = Character.Grandmother,
            lines = new string[] {
              "You never met your other sister, but I see a part of her in you. Her spirit and yours are intertwined.",
              "Please come back again tomorrow, these herbs will only last a day."
            }
        });

    }
}
