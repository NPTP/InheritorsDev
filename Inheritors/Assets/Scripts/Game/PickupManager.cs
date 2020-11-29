/* INHERITORS by Nick Perrin (c) 2020 */
using System;
using UnityEngine;
using DG.Tweening;

public enum ItemType
{
    Null,
    Wood,
    Jug
}

public class PickupManager : MonoBehaviour
{
    GameObject player;
    InteractManager interactManager;
    UIManager uiManager;

    Inventory inventory = new Inventory();
    public class Inventory
    {
        public PickupTrigger heldItem = null;
        public bool holdingItem = false;
        public ItemType itemType = ItemType.Null;
        public int itemQuantity = 0;
        public bool haveReadyItem = false;
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
        inventory.itemType = ItemType.Null;
        inventory.itemQuantity = 0;
        uiManager.UpdateInventory(inventory);
    }

    // ████████████████████████████████████████████████████████████████████████
    // ███ AUTO-GET
    // ████████████████████████████████████████████████████████████████████████

    public void AutoGetItem(string resourceName = "INVALID")
    {
        inventory.haveReadyItem = true;
        GameObject item = GameObject.Instantiate(
            Resources.Load<GameObject>(resourceName),
            GetItemHoldPosition(),
            player.transform.rotation,
            player.transform
        );
        item.transform.DOScale(1f, 0.25f).From(0f);
    }

    Vector3 GetItemHoldPosition()
    {
        return player.transform.position + (.5f * player.transform.up) + (.5f * player.transform.forward);
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

    public ItemType GetHeldItemType()
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
        player = GameObject.FindWithTag("Player");
        interactManager = FindObjectOfType<InteractManager>();
        uiManager = FindObjectOfType<UIManager>();
    }


}
