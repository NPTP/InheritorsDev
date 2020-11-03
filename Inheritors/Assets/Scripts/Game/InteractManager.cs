﻿using System;
using System.Collections;
using UnityEngine;
using DG.Tweening;
using TMPro;

public class InteractManager : MonoBehaviour
{
    GameObject player;
    StateManager stateManager;
    InputManager inputManager;
    UIManager uiManager;
    DialogManager dialogManager;
    PickupManager pickupManager;

    PickupTrigger pickupTrigger = null;
    string pickupTag = null;
    bool pickupInRange = false;
    PickupManager.ItemTypes itemType;
    public event EventHandler<PickupArgs> OnPickup;
    public class PickupArgs : EventArgs
    {
        public string tag;
        public PickupManager.Inventory inventory;
    }

    DropoffTrigger dropoffTrigger = null;
    string dropoffTag = null;
    bool dropoffInRange = false;
    PickupManager.ItemTypes dropoffItemType;
    public event EventHandler<DropoffArgs> OnDropoff;
    public class DropoffArgs : EventArgs
    {
        public string tag;
    }

    DialogTrigger dialogTrigger = null;
    string dialogTag = null;
    bool dialogInRange = false;
    public event EventHandler<DialogArgs> OnDialog;
    public class DialogArgs : EventArgs
    {
        public string tag;
        public Dialog dialog;
    }

    WalkTrigger walkTrigger = null;
    string walkTag = null;
    public event EventHandler<WalkArgs> OnWalk;
    public class WalkArgs : EventArgs
    {
        public string tag;
    }

    void Awake()
    {
        InitializeReferences();
        SubscribeToEvents();
    }

    private void HandleInputEvent(object sender, InputManager.ButtonArgs args)
    {
        switch (args.buttonCode)
        {
            case InputManager.A:
                if (pickupInRange)
                    TryPickUp();
                break;

            case InputManager.X:
                if (dropoffInRange)
                    TryDropoff();
                break;

            case InputManager.Y:
                if (dialogInRange)
                    StartDialog();
                break;

            default:
                break;
        }
    }

    // ████████████████████████████████████████████████████████████████████████
    // ███ PICKUP
    // ████████████████████████████████████████████████████████████████████████

    public bool IsPickupInRange()
    {
        return pickupInRange;
    }

    public void PickupEnterRange(PickupTrigger sender)
    {
        pickupTrigger = sender;
        pickupTag = pickupTrigger.GetTag();
        pickupInRange = true;
        itemType = pickupTrigger.itemType;
        uiManager.EnterRange(pickupTrigger.transform, "Pickup");
    }

    public void PickupExitRange(PickupTrigger sender)
    {
        pickupTrigger = sender;
        pickupTag = null;
        pickupInRange = false;
        uiManager.ExitRange(pickupTrigger.transform, "Pickup");
        pickupTrigger = null;
    }

    void TryPickUp()
    {
        // Currently not checking against item type & quantity, just allow all.
        // Make a safety check before allowing pickup.
        if (!pickupInRange || pickupTag is null || pickupTrigger is null)
        {
            print("**** Failed this pickup ****\n" +
            "pickupInRange: " + pickupInRange + "\n" +
            "pickupTag: " + pickupTag + "\n" +
            "pickupTrigger: " + pickupTrigger);
            return;
        }
        stateManager.SetState(StateManager.State.PickingUp);
        PickupTrigger currentPickup = pickupTrigger;
        currentPickup.GetPickedUp();
        uiManager.pickupPrompt.Hide();
        pickupInRange = false;
        StartCoroutine(PickUpItem(currentPickup));
    }

    IEnumerator PickUpItem(PickupTrigger currentPickup)
    {
        Vector3 startPosition = currentPickup.transform.position;
        float elapsed = 0f;
        float time = 0.25f;
        while (elapsed < time)
        {
            currentPickup.transform.position = Vector3.Lerp(
                startPosition, GetItemHoldPosition(), Helper.SmoothStep(elapsed / time));
            elapsed += Time.deltaTime;
            yield return null;
        }
        currentPickup.transform.position = GetItemHoldPosition();
        currentPickup.transform.SetParent(player.transform);
        stateManager.SetState(StateManager.State.Holding);

        // Hand it off here to the PickupManager, updates the inventory.
        pickupManager.PickUp(currentPickup);

        // Send a pickup event.
        OnPickup?.Invoke(this, new PickupArgs
        {
            tag = currentPickup.GetTag(),
            inventory = pickupManager.GetInventory()
        });
    }

    Vector3 GetItemHoldPosition()
    {
        return player.transform.position + player.transform.up + (.5f * player.transform.forward);
    }

    // ████████████████████████████████████████████████████████████████████████
    // ███ DROPOFF
    // ████████████████████████████████████████████████████████████████████████

    public bool IsDropoffInRange()
    {
        return dropoffInRange;
    }

    public void DropoffEnterRange(DropoffTrigger sender)
    {
        dropoffTrigger = sender;
        dropoffTag = dropoffTrigger.GetTag();
        dropoffInRange = true;
        uiManager.EnterRange(dropoffTrigger.transform, "Dropoff");
    }

    public void DropoffExitRange(DropoffTrigger sender)
    {
        dropoffTrigger = sender;
        dropoffTag = null;
        dropoffInRange = false;
        uiManager.ExitRange(dropoffTrigger.transform, "Dropoff");
        dropoffTrigger = null;
    }

    void TryDropoff()
    {
        if (pickupManager.IsHoldingItem())
        {
            stateManager.SetState(StateManager.State.DroppingOff);
            uiManager.pickupPrompt.Hide();
            dropoffTrigger.CompleteDropoff();
            dropoffInRange = false;
            StartCoroutine(DropOffItem());
        }
    }

    IEnumerator DropOffItem()
    {
        PickupTrigger heldItem = pickupManager.GetHeldItem();
        pickupManager.DropOff();

        Vector3 startPosition = pickupTrigger.transform.position;
        Vector3 endPosition = dropoffTrigger.targetTransform.position;
        float elapsed = 0f;
        float time = 0.5f;
        while (elapsed < time)
        {
            pickupTrigger.transform.position = Vector3.Lerp(
                startPosition, endPosition, Helper.SmoothStep(elapsed / time));
            elapsed += Time.deltaTime;
            yield return null;
        }
        pickupTrigger.transform.position = endPosition;
        var dropoffTarget = dropoffTrigger.targetTransform.gameObject.GetComponent<DropoffTarget>();
        if (dropoffTarget != null) dropoffTarget.ReactToDropoff();
        heldItem.Remove();
        stateManager.SetState(StateManager.State.Normal);

        // TODO: call event for dropoff here, then remove the trigger.

        dropoffTrigger.Remove();
    }

    // ████████████████████████████████████████████████████████████████████████
    // ███ DIALOG
    // ████████████████████████████████████████████████████████████████████████

    public bool IsDialogInRange()
    {
        return dialogInRange;
    }

    public void DialogEnterRange(DialogTrigger sender)
    {
        dialogTrigger = sender;
        dialogTag = dialogTrigger.GetTag();
        dialogInRange = true;
        uiManager.EnterRange(dialogTrigger.transform, "Dialog");
    }

    public void DialogExitRange(DialogTrigger sender)
    {
        dialogTrigger = null;
        dialogTag = null;
        dialogInRange = false;
        uiManager.ExitRange(sender.transform, "Dialog");
    }

    void StartDialog()
    {
        stateManager.SetState(StateManager.State.Dialog);
        dialogTrigger.Disable();
        dialogInRange = false;
        uiManager.dialogPrompt.Hide();
        if (dialogTrigger.lookAtMyTarget && dialogTrigger.myTarget != null)
        {
            Vector3 lookRotation = Quaternion.LookRotation(
                dialogTrigger.myTarget.position - player.transform.position,
                Vector3.up).eulerAngles;
            lookRotation.x = 0f;
            lookRotation.z = 0f;

            // TODO: get this facing-dialog-target-rotation working later (for polish).
            player.transform.DORotate(lookRotation, .25f);
            player.GetComponent<Rigidbody>().rotation = Quaternion.Euler(lookRotation);
        }

        // Send dialog event to Day, which will display it and run any connected events.
        OnDialog.Invoke(this, new DialogArgs
        {
            tag = dialogTrigger.tag,
            dialog = dialogTrigger.dialog
        });

        if (dialogTrigger.dialogPersists)
            StartCoroutine(WaitToResetTrigger());
    }

    IEnumerator WaitToResetTrigger()
    {
        yield return new WaitUntil(dialogManager.IsDialogFinished);
        dialogTrigger.Enable();
    }

    // ████████████████████████████████████████████████████████████████████████
    // ███ WALK
    // ████████████████████████████████████████████████████████████████████████

    public void WalkEnter(WalkTrigger sender)
    {
        // Consider a small wait for nicer feel.
        WalkTrigger thisTrigger = sender;
        StartCoroutine(WalkTriggerActivate(thisTrigger));
    }

    IEnumerator WalkTriggerActivate(WalkTrigger thisTrigger)
    {
        yield return new WaitForSeconds(0.3f);
        walkTag = thisTrigger.GetTag();
        thisTrigger.Disable();
        OnWalk?.Invoke(this, new WalkArgs { tag = thisTrigger.GetTag() });
        thisTrigger.Remove();
    }

    // ████████████████████████████████████████████████████████████████████████
    // ███ INITIALIZERS & DESTROYERS
    // ████████████████████████████████████████████████████████████████████████

    void InitializeReferences()
    {
        player = GameObject.Find("Player");
        stateManager = FindObjectOfType<StateManager>();
        inputManager = FindObjectOfType<InputManager>();
        inputManager.OnButtonDown += HandleInputEvent;
        uiManager = FindObjectOfType<UIManager>();
        dialogManager = FindObjectOfType<DialogManager>();
        pickupManager = FindObjectOfType<PickupManager>();
    }

    void SubscribeToEvents()
    {

    }

    void OnDestroy()
    {
        // Unsubscribe from all events
    }

}
