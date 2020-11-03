using System;
using UnityEngine;

public class PickupManager : MonoBehaviour
{
    InteractManager interactManager;
    UIManager uiManager;

    public enum ItemTypes
    {
        NULL,
        WOOD,
        JUG
    }

    Inventory inventory = new Inventory();
    public class Inventory
    {
        public PickupTrigger heldItem;
        public bool holdingItem;
        public PickupManager.ItemTypes itemType;
        public int itemQuantity;
    }

    // ████████████████████████████████████████████████████████████████████████
    // ███ ACTIONS
    // ████████████████████████████████████████████████████████████████████████

    public void PickUp(PickupTrigger currentPickup)
    {
        inventory.heldItem = currentPickup;
        inventory.holdingItem = true;
        inventory.itemType = currentPickup.itemType;
        inventory.itemQuantity++;
        uiManager.UpdateInventory(inventory);
    }

    // A dropoff ALWAYS empties the inventory completely.
    public void DropOff()
    {
        inventory.heldItem.ResetParent();
        inventory.heldItem.GetDroppedOff();
        inventory.heldItem = null;
        inventory.holdingItem = false;
        inventory.itemType = ItemTypes.NULL;
        inventory.itemQuantity = 0;
        uiManager.UpdateInventory(inventory);
    }

    // ████████████████████████████████████████████████████████████████████████
    // ███ GETTERS & SETTERS
    // ████████████████████████████████████████████████████████████████████████

    public bool IsHoldingItem()
    {
        return inventory.holdingItem;
    }

    public int GetItemQuantity()
    {
        return inventory.itemQuantity;
    }

    public PickupTrigger GetHeldItem()
    {
        return inventory.heldItem;
    }

    public PickupManager.Inventory GetInventory()
    {
        return inventory;
    }

    // ████████████████████████████████████████████████████████████████████████
    // ███ INITIALIZERS
    // ████████████████████████████████████████████████████████████████████████

    void Awake()
    {
        InitializeReferences();
    }

    void Start()
    {
        inventory.heldItem = null;
        inventory.holdingItem = false;
        inventory.itemQuantity = 0;
    }

    void InitializeReferences()
    {
        interactManager = FindObjectOfType<InteractManager>();
        uiManager = FindObjectOfType<UIManager>();
    }


}
