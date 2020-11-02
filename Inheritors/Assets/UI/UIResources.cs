using UnityEngine;
using UnityEngine.UI;

[CreateAssetMenu]
public class UIResources : ScriptableObject
{
    [Header("Center Buttons")]
    public Sprite Back_Button;
    public Sprite Start_Button;

    [Header("Face Buttons")]
    public Sprite A_Button;
    public Sprite X_Button;
    public Sprite Y_Button;
    public Sprite B_Button;

    [Header("Items")]
    public Sprite WOOD;

    public Sprite GetItemIcon(PickupManager.ItemTypes type)
    {
        switch (type)
        {
            case PickupManager.ItemTypes.WOOD:
                return WOOD;

            default:
                return null;
        }
    }
}