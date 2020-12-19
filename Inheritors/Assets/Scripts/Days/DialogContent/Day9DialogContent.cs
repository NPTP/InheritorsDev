using System.Collections.Generic;
using UnityEngine;

public class Day9DialogContent : MonoBehaviour, DialogContent
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
        content.Add("Day9Opening_1", new Dialog
        {
            character = Character.Mother,
            lines = new string[] {
                "Grandmother's passing has not been easy to take. I know you cared for her, son.",
                "She was not my mother, but she was family all the same.",
                "Do not be sad. \nShe was very old, and it was her time.",
                "Now, the sky pours down, giving us all the water we could ever need. But we have nothing to eat, nothing to grow.",
                "I need you to check on everyone today.",
                "Time is running out."
            }
        });

        content.Add("Day9Opening_2", new Dialog
        {
            character = Character.Mother,
            lines = new string[] {
                "You <i>must</i> go and find your father today. \nWhen you do, bring him back. \nAnd I want your sister with us again.",
                "Your grandfather is our chief. He has lead the tribe for a long time. He can help us, \nso see him as soon as you can.",
                "I am counting on you, my son."
            }
        });

        content.Add("DayOver", new Dialog
        {
            character = Character.Narrator,
            lines = new string[] {
                "I do not hear mother's voice calling me.",
                "I should return home and wait for her."
            }
        });

        // ████████████████████████████████████████████████████████████████████████
        // ███ MOTHER
        // ████████████████████████████████████████████████████████████████████████

        content.Add("Mother_Start", new Dialog
        {
            character = Character.Mother,
            lines = new string[] {
                "Our water is overflowing, yet we have no meat, no fruit, no vegetables.",
                "Please, check on everyone as soon as you can. I don't want us all to have to leave our home...",
                "Not again."
            }
        });

        content.Add("Mother_Default", new Dialog
        {
            character = Character.Mother,
            lines = new string[] {
                "Our water is overflowing, yet we have no meat, no fruit, no vegetables.",
                "Please, check on everyone as soon as you can. I don't want us all to have to leave our home...",
                "Not again."
            }
        });

        content.Add("Mother_Other", new Dialog
        {
            character = Character.Mother,
            lines = new string[] {
                "Why do you have your grandfather's matété? Why have you brought it here? \nHe would never let it go...",
                "What are you saying? \nThe chief of the Akuntsu... why is he not here, holding it himself?",
                "Son, take it to him, and ask for his help! \nThe two of you will save us from this misfortune..."
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
            character = Character.Narrator,
            lines = new string[] {
                "These logs have all been cut from our forest.",
                "I cannot reach father.",
                "It has been two days since he left the forest.",
                "I do not think he is ever coming back."
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
            character = Character.Narrator,
            lines = new string[] {
                "I cannot reach sister.",
                "Did she leave the forest? \nDid she find a new garden?",
                "I don't know if I will ever see her again."
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
            character = Character.Narrator,
            lines = new string[] {
                "Grandfather is asleep.",
                "He does not hold his matété. \nMaybe he will wake, if I bring it to him."
            }
        });

        content.Add("Grandfather_Other", new Dialog
        {
            character = Character.Grandfather,
            lines = new string[] {
                "<UNUSED>"
            }
        });

        content.Add("Grandfather_Active", new Dialog
        {
            character = Character.Grandfather,
            lines = new string[] {
                "<UNUSED>"
            }
        });

        content.Add("Grandfather_FinishTask", new Dialog
        {
            character = Character.Narrator,
            lines = new string[] {
                "Now he holds the matété.",
                "But I see he has lost an ear. \nThat must be why he cannot hear me.",
                "He will not wake up."
            }
        });

        content.Add("Grandfather_Completed", new Dialog
        {
            character = Character.Grandfather,
            lines = new string[] {
                "<UNUSED>"
            }
        });

        // ████████████████████████████████████████████████████████████████████████
        // ███ GRANDMOTHER
        // ████████████████████████████████████████████████████████████████████████

        content.Add("Grandmother_Start", new Dialog
        {
            character = Character.Narrator,
            lines = new string[] {
              "< UNUSED >"
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
