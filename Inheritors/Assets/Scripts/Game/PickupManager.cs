/* INHERITORS by Nick Perrin (c) 2020 */
using System;
using System.Collections;
using UnityEngine;
using DG.Tweening;

public enum ItemType
{
    Null,
    Wood,
    Jug,
    Water,
    Bow,
    Pig,
    Agoutis,
    Papaya,
    Corn,
    Flute,
    Yopo,
    Reed,
    Uruku
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
        public GameObject taskTool = null;
        public bool haveTaskTool = false;
        public bool toolIsAttached = false;
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

    // For just emptying stuff out/debug.
    public void LoseItems()
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
    // ███ TASK TOOL
    // ████████████████████████████████████████████████████████████████████████

    // Resources load version (string name)
    public void GetTaskTool(ItemType itemType)
    {
        GameObject tool = null;
        switch (itemType)
        {
            case ItemType.Jug:
                tool = ResourcesTool("Jug");
                inventory.toolIsAttached = false;
                break;

            case ItemType.Bow:
                tool = ActivatePlayerSkeletonItem(itemType);
                inventory.toolIsAttached = true;
                break;

            default:
                print("Unknown item type in GetTaskTool, pickup manager.");
                return;
        }
        inventory.haveTaskTool = true;
        tool.transform.DOScale(1f, 0.25f).From(0f);
        inventory.taskTool = tool;
    }

    GameObject ResourcesTool(string resourceName)
    {
        return GameObject.Instantiate(
            Resources.Load<GameObject>(resourceName),
            GetItemHoldPosition(),
            player.transform.rotation,
            player.transform
        );
    }

    GameObject ActivatePlayerSkeletonItem(ItemType type)
    {
        GameObject item = FindObjectOfType<PlayerSkeletonItems>().GetItem(type);
        item.SetActive(true);
        return item;
    }

    public void LoseTaskTool()
    {
        if (!inventory.haveTaskTool) { return; }
        StartCoroutine(KillTaskTool());
    }

    IEnumerator KillTaskTool()
    {
        GameObject toolReference = inventory.taskTool;
        inventory.taskTool = null;
        inventory.haveTaskTool = false;
        Tween t = toolReference.transform.DOScale(0f, 0.25f);
        yield return t.WaitForCompletion();

        if (inventory.toolIsAttached)
            toolReference.SetActive(false);
        else
            Destroy(toolReference);

        inventory.toolIsAttached = false;
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
