using System;
using UnityEngine;

public class PickupManager : MonoBehaviour
{
    InteractManager interactManager;
    UIManager uiManager;
    public event EventHandler<PickupArgs> OnPickup;
    public class PickupArgs : EventArgs
    {
        public bool holdingItem;
        public PickupManager.ItemTypes itemType;
        public int itemQuantity;
    }

    public PickupManager.ItemTypes types;
    public enum ItemTypes
    {
        WOOD,
        JUG
    }

    bool holdingItem = false;
    PickupManager.ItemTypes itemType;
    int itemQuantity = 0;

    // ████████████████████████████████████████████████████████████████████████
    // ███ ACTIONS
    // ████████████████████████████████████████████████████████████████████████

    public void PickUp(PickupTrigger pickupTrigger, PickupManager.ItemTypes type)
    {
        holdingItem = true;
        itemType = type;
        itemQuantity++;
        uiManager.UpdateHolding(type, itemQuantity);
        OnPickup?.Invoke(
            this, new PickupArgs
            {
                holdingItem = this.holdingItem,
                itemType = this.itemType,
                itemQuantity = this.itemQuantity
            }
        );
    }

    public void DropOff(PickupTrigger pickupTrigger, PickupManager.ItemTypes type)
    {
        holdingItem = false;
        itemType = type;
        itemQuantity = 0;
        uiManager.UpdateHolding(type, itemQuantity);
    }

    // ████████████████████████████████████████████████████████████████████████
    // ███ INITIALIZERS
    // ████████████████████████████████████████████████████████████████████████

    void Awake()
    {
        InitializeReferences();
    }

    void InitializeReferences()
    {
        interactManager = FindObjectOfType<InteractManager>();
        uiManager = FindObjectOfType<UIManager>();
    }

    // ████████████████████████████████████████████████████████████████████████
    // ███ GETTERS & SETTERS
    // ████████████████████████████████████████████████████████████████████████

    public int GetItemQuantity()
    {
        return itemQuantity;
    }

}
