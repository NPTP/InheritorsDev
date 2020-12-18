using UnityEngine;
using UnityEngine.UI;

[CreateAssetMenu]
public class UIResources : ScriptableObject
{
    [Header("Center Buttons")]
    public Sprite Back_Button;
    public Sprite Start_Button;

    [Header("UI Prompts")]
    public Sprite A_Button;
    public Sprite X_Button;
    public Sprite Y_Button;
    public Sprite B_Button;

    [Header("In-world prompts")]
    public Sprite A_Button_Inworld;
    public Sprite Y_Button_Inworld;

    [Header("Items")]
    public Sprite WOOD;
    public Sprite WATER;
    public Sprite PIG;
    public Sprite AGOUTIS;  // This is actually fish!
    public Sprite PAPAYA;
    public Sprite CORN;
    public Sprite FLUTE;
    public Sprite YOPO;
    public Sprite REED;
    public Sprite URUKU;
    public Sprite HERBS;
    public Sprite SEEDS;
    public Sprite DARKFLUTE;

    [Header("Sounds")]
    public AudioClip sound_taskUpdate;

    public Sprite GetItemIcon(ItemType type)
    {
        switch (type)
        {
            case ItemType.Wood:
                return WOOD;

            case ItemType.Water:
                return WATER;

            case ItemType.Pig:
                return PIG;

            case ItemType.Agoutis:
                return AGOUTIS;

            case ItemType.Papaya:
                return PAPAYA;

            case ItemType.Corn:
                return CORN;

            case ItemType.Flute:
                return FLUTE;

            case ItemType.Yopo:
                return YOPO;

            case ItemType.Reed:
                return REED;

            case ItemType.Uruku:
                return URUKU;

            case ItemType.Herbs:
                return HERBS;

            case ItemType.Seeds:
                return SEEDS;

            case ItemType.DarkFlute:
                return DARKFLUTE;

            default:
                return null;
        }
    }
}