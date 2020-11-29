/* INHERITORS by Nick Perrin (c) 2020 */
using System;
using System.Collections;
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
        public GameObject taskTool = null;
        public bool haveTaskTool = false;
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
    // ███ TASK TOOL
    // ████████████████████████████████████████████████████████████████████████

    // Game object version
    public void GetTaskTool(GameObject toolPrefab)
    {
        inventory.haveTaskTool = true;
        GameObject tool = GameObject.Instantiate(
            toolPrefab,
            GetItemHoldPosition(),
            player.transform.rotation,
            player.transform
        );
        tool.transform.DOScale(1f, 0.25f).From(0f);
        inventory.taskTool = tool;
    }

    // Resources load version (string name)
    public void GetTaskTool(string resourceName = "INVALID")
    {
        inventory.haveTaskTool = true;
        GameObject tool = GameObject.Instantiate(
            Resources.Load<GameObject>(resourceName),
            GetItemHoldPosition(),
            player.transform.rotation,
            player.transform
        );
        tool.transform.DOScale(1f, 0.25f).From(0f);
        inventory.taskTool = tool;
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
        Destroy(toolReference);
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
