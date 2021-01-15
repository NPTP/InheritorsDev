using System.Collections.Generic;
using UnityEngine;

public class Day7DialogContent : MonoBehaviour, DialogContent
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
        content.Add("Day7Opening_1", new Dialog
        {
            character = Character.Mother,
            lines = new string[] {
                "For decades I have watched the sun set over the Omerê, over this forest.",
                "For decades this beautiful land has been a part of all the families who call it home.",
                "If we never give up on it, I know that it will never give up on <i>us</i>.",
            }
        });

        content.Add("Day7Opening_2", new Dialog
        {
            character = Character.Mother,
            lines = new string[] {
                "Things will get better. Things <i>are</i> getting better.",
                "Our tribe is strong and united, and you and your sister are smart and capable.",
                "If we keep working, if we keep building, we will keep the forest. Do not stop, my son."
            }
        });

        content.Add("DayOver", new Dialog
        {
            character = Character.Mother,
            lines = new string[] {
                "You have done well, son! Come home now."
            }
        });

        // ████████████████████████████████████████████████████████████████████████
        // ███ MOTHER
        // ████████████████████████████████████████████████████████████████████████

        content.Add("Mother_Start", new Dialog
        {
            character = Character.Mother,
            lines = new string[] {
                "We have the wood to start a fire, but we need meat to cook..."
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
                "I can't believe it...",
                "It has all been razed. \nWe cannot hunt here anymore. I do not see, hear or smell a single animal.",
                "I will not let us down. I must clear the rocks, leave the forest and bring back meat from the pastures.",
                "It will be alright, son. I will be back soon. \nI will be quick so they do not see me. \nDo not dare follow me. ",
                "We will be having a feast again, I promise you. Meet me here, tomorrow, when I return, and help me carry the meat."
            }
        });

        content.Add("Father_Other", new Dialog
        {
            character = Character.Father,
            lines = new string[] {
                "Finish what you are doing and come back to me, my boy. I must tell you where I am going."
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
                "I promise to be quick, and inconspicuous. They will not see me take my prize.",
                "You will need to be ready for my return tomorrow - we will have to move fast, but then, we will eat well!"
            }
        });

        // ████████████████████████████████████████████████████████████████████████
        // ███ SISTER
        // ████████████████████████████████████████████████████████████████████████

        content.Add("Sister_Start", new Dialog
        {
            character = Character.Sister,
            lines = new string[] {
                "It is amazing, brother! \nEverything grows so well here!",
                "I removed the spears, planted new papaya in the pits, and tore down the hut.",
                "My seeds are already sprouting. \nPlease plant <color=blue>four seeds</color> of manioc in the dirt!"
            }
        });

        content.Add("Sister_Other", new Dialog
        {
            character = Character.Sister,
            lines = new string[] {
                "Oh, I'm so excited, little brother! Come back when you are done the task at hand, and I will show you!"
            }
        });

        content.Add("Sister_Active", new Dialog
        {
            character = Character.Sister,
            lines = new string[] {
                "You only need plant <color=blue>four seeds</color> in the dirt here."
            }
        });

        content.Add("Sister_FinishTask", new Dialog
        {
            character = Character.Sister,
            lines = new string[] {
                "In time, this spot will grow richer even than before it was burned, and we will have more than we can possibly eat.",
                "Tomorrow, meet me at my home - we will make a grand harvest from all that has grown in my garden!"
            }
        });

        content.Add("Sister_Completed", new Dialog
        {
            character = Character.Sister,
            lines = Get("Sister_FinishTask").lines
        });


        // ████████████████████████████████████████████████████████████████████████
        // ███ GRANDFATHER
        // ████████████████████████████████████████████████████████████████████████

        content.Add("Grandfather_Start", new Dialog
        {
            character = Character.Grandfather,
            lines = new string[] {
                "I knew it would come to such questions, but I did not expect them to come from encountering the Man of the Hole.",
                "Grandson, you are lucky to have even seen him - for he is the last of his tribe.",
                "Even I do not know what happened to the rest of them. But his solitude <i>now</i> is his own choice.",
                "That fear does not serve us. My scars are the same as your father's. In our former lands, they came, and they shot, and we ran...",
                "But they did not kill us all. And I am your chief, always leading our tribe. There is no need to fear.",
                "Your sister's gardens grow strong, and your grandmother is getting better. You have been working so hard.",
                "Please, take a rest in my <color=blue>hammock</color>, in the meadow to the northeast of here.",
                "Have a brief, restorative sleep. The meadow will rejuvenate you."
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
                "Grandson, go to my <color=blue>hammock</color> in the meadow just northeast of here. The short rest will restore you!"
            }
        });

        content.Add("Grandfather_FinishTask", new Dialog
        {
            character = Character.Grandfather,
            lines = new string[] {
                "How do you feel?",
                "Loss is part of life. This land and the future is yours, and with hope, we will overcome any fear.",
                "When we are rested in mind and body, when we do not run from fear, but instead, we follow hope...",
                "That is when the Akuntsu will thrive again. That time is now."
            }
        });

        content.Add("Grandfather_Completed", new Dialog
        {
            character = Character.Grandfather,
            lines = new string[] {
                "When we are rested in mind and body, when we do not run from fear, but instead, we follow hope...",
                "That is when the Akuntsu will thrive again. That time is now.",
            }
        });

        // ████████████████████████████████████████████████████████████████████████
        // ███ GRANDMOTHER
        // ████████████████████████████████████████████████████████████████████████

        content.Add("Grandmother_Start", new Dialog
        {
            character = Character.Grandmother,
            lines = new string[] {
              "The herbs you brought me yesterday helped immensely, but I am not through yet. I will need rest.",
              "You have seen the past made manifest after our festival. Today you will evoke the past again.",
              "Just like our ancestors did, you will grind the uruku seeds. These seeds are what give my hair its bright red color.",
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
                "Oh my, don't you look like a real Akuntsu, my grandson!",
                "I am getting quite old now, so you must carry on this tradition. Can you do that for me?",
                "Oh, how <i>she</i> would have loved you... Your sister, who passed, she used to dye her hair just like that...",
                "I must rest to overcome this outside sickness. You give me so much strength, little one.",
                "Come back to see me tomorrow when I have awoken!"
            }
        });

        content.Add("Grandmother_Completed", new Dialog
        {
            character = Character.Grandmother,
            lines = new string[] {
              "Oh, how <i>she</i> would have loved you... Your sister, who passed, she used to dye her hair just like that...",
              "Come back to see me tomorrow when I have awoken!"
            }
        });

    }
}
