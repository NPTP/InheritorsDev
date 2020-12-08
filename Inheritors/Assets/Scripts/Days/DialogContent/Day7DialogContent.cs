using System.Collections.Generic;
using UnityEngine;

public class Day7DialogContent : MonoBehaviour
{
    public Dictionary<string, Dialog> content = new Dictionary<string, Dialog>();

    void Awake()
    {
        PackContent();
    }

    void PackContent()
    {
        content.Add("Day7Opening_1", new Dialog
        {
            character = Character.Mother,
            lines = new string[] {
                "For decades I have watched the sun set over the Omere, over the forest.",
                "For decades this beautiful land has been a part of all families who call it home.",
                "If we never give up on it, I know that it will never give up on <i>us</i>.",
                "Go with love today, son."
            }
        });

        content.Add("DayOver", new Dialog
        {
            character = Character.Mother,
            lines = new string[] {
                "You have done well, son! Come home now and rest. Tomorrow will be another beautiful day."
            }
        });

        // ████████████████████████████████████████████████████████████████████████
        // ███ MOTHER
        // ████████████████████████████████████████████████████████████████████████

        content.Add("Mother_Start", new Dialog
        {
            character = Character.Mother,
            lines = new string[] {
                "Things will get better. Things <i>are</i> getting better.",
                "Our tribe is strong and united, and you and your sister are smart and capable."
            }
        });

        content.Add("Mother_Active", new Dialog
        {
            character = Character.Mother,
            lines = new string[] {
                "Firewood onto the fire pit..."
            }
        });

        content.Add("Mother_Completed", new Dialog
        {
            character = Character.Mother,
            lines = new string[] {
                "Well, the fire is ready, but we still need meat to cook..."
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
                "We cannot hunt here anymore. It has all been razed. I do not see, hear or smell a single animal nearby.",
                "I cannot let us down. I must clear the rocks, leave the forest and bring back meat from the pastures.",
                "It will be alright, son. I will be back soon. I will be quick so they do not see me. Do not dare follow me. ",
                "Tomorrow, we will be having a feast again, I promise you. Meet me here when I return, and help me carry the meat."
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
                "I removed the spears, planted new papaya trees in the pits, and tore down the hut.",
                "Our seeds from yesterday are sprouting. \nThe soil is now ready for four more!"
            }
        });

        content.Add("Sister_Active", new Dialog
        {
            character = Character.Sister,
            lines = new string[] {
                "You only need plant the seeds in <color=blue>four spots</color>."
            }
        });

        content.Add("Sister_FinishTask", new Dialog
        {
            character = Character.Sister,
            lines = new string[] {
                "In time, this spot will grow richer even than before it was burned, and we will have more than we can possibly eat.",
                "Tomorrow, meet me at my home - we will have a grand harvest from all that has grown in my garden!"
            }
        });

        content.Add("Sister_Completed", new Dialog
        {
            character = Character.Sister,
            lines = new string[] {
                "In time this spot will grow richer than before it burned, and we will have more than we can possibly eat.",
                "Tomorrow, meet me at my home - we will have a grand harvest from all that has grown in my garden!"
            }
        });


        // ████████████████████████████████████████████████████████████████████████
        // ███ GRANDFATHER
        // ████████████████████████████████████████████████████████████████████████

        content.Add("Grandfather_Start", new Dialog
        {
            character = Character.Grandfather,
            lines = new string[] {
                "Grandson, there is no need to fear.",
                "You have been working so hard. Your sister's gardens grow strong, and your grandmother is getting better.",
                "Please, take a rest in my <color=blue>hammock</color>, in the meadow to the northeast of here.",
                "Have a brief, but restorative sleep. The meadow will rejuvenate you."
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
                "Come rest in my hammock any time you wish, grandson."
            }
        });

        // ████████████████████████████████████████████████████████████████████████
        // ███ GRANDMOTHER
        // ████████████████████████████████████████████████████████████████████████

        content.Add("Grandmother_Start", new Dialog
        {
            character = Character.Grandmother,
            lines = new string[] {
              "Ahh, the youngest one is back again, always running back to help, eager to be of service.",
              "The herbs you brought me yesterday helped immensely, but I am not through yet. I will need more.",
              "There is a particular leaf that I have seen growing near your <i>other</i> sister's maloca.",
              "Please bring me the <color=blue>herbs</color> from there as soon as you can."
            }
        });

        content.Add("Grandmother_Active", new Dialog
        {
            character = Character.Grandmother,
            lines = new string[] {
              "The <color=blue>herbs</color> I need today grow near your <i>other</i> sister's maloca.",
              "When you have them, drop them in the bucket there."
            }
        });

        content.Add("Grandmother_FinishTask", new Dialog
        {
            character = Character.Grandmother,
            lines = new string[] {
              "Thank you, brave little one. This should be all I need to recover fully.",
              "I was afraid the remedies which have always served our tribe might fail against the outside illness.",
              "But there is no obstacle we Akuntsu cannot overcome together.",
              "I must go inside and rest now, collect my strength. Come see me tomorrow when I have awoken!"
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
