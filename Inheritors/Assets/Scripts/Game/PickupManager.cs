/* INHERITORS by Nick Perrin (c) 2020 */
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
        public PickupTrigger heldItem = null;
        public bool holdingItem = false;
        public ItemTypes itemType = ItemTypes.NULL;
        public int itemQuantity = 0;
    }

    // ████████████████████████████████████████████████████████████████████████
    // ███ ACTIONS
    // ████████████████████████████████████████████████████████████████████████

    public void PickUp(PickupTrigger currentPickup)
    {
        if (!IsHoldingItem())
        {
            inventory.heldItem = currentPickup;
            inventory.holdingItem = true;
            inventory.itemType = currentPickup.itemType;
        }
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

    public PickupManager.ItemTypes GetHeldItemType()
    {
        return inventory.itemType;
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

    void InitializeReferences()
    {
        interactManager = FindObjectOfType<InteractManager>();
        uiManager = FindObjectOfType<UIManager>();
    }


}
