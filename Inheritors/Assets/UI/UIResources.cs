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

    public Sprite GetItemIcon(ItemType type)
    {
        switch (type)
        {
            case ItemType.Wood:
                return WOOD;

            default:
                return null;
        }
    }
}