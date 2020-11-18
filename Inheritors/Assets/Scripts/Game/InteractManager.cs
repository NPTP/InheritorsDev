/* INHERITORS by Nick Perrin (c) 2020 */
using System;
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

    AreaTrigger areaTrigger = null;
    string areaTag = null;
    bool insideArea = false;
    public event EventHandler<AreaArgs> OnArea;
    public class AreaArgs : EventArgs
    {
        public string tag;
        public bool inside;
    }

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
    // ███ AREA
    // ████████████████████████████████████████████████████████████████████████

    public bool IsInsideArea()
    {
        return insideArea;
    }

    public void AreaEnter(AreaTrigger sender)
    {
        areaTrigger = sender;
        areaTag = areaTrigger.GetTag();
        insideArea = true;
        print("Inside area.");
        OnArea?.Invoke(this, new AreaArgs { tag = areaTag, inside = true });
    }

    public void AreaLeave(AreaTrigger sender)
    {
        areaTrigger = null;
        areaTag = null;
        insideArea = false;
        print("Outside area.");
        OnArea?.Invoke(this, new AreaArgs { tag = sender.GetTag(), inside = false });
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
        bool alreadyHolding = pickupManager.IsHoldingItem();
        Vector3 startPosition = currentPickup.transform.position;
        float elapsed = 0f;
        float time = 0.25f;

        while (elapsed < time)
        {
            currentPickup.transform.position = Vector3.Lerp(
                startPosition, GetItemHoldPosition(), Helper.SmoothStep(elapsed / time));
            if (alreadyHolding)
                currentPickup.transform.localScale *= 1 - (elapsed / time);
            elapsed += Time.deltaTime;
            yield return null;
        }
        currentPickup.transform.position = GetItemHoldPosition();
        stateManager.SetState(StateManager.State.Holding);

        // Hand it off here to the PickupManager, updates the inventory.
        pickupManager.PickUp(currentPickup);

        // Send a pickup event.
        OnPickup?.Invoke(this, new PickupArgs
        {
            tag = currentPickup.GetTag(),
            inventory = pickupManager.GetInventory()
        });

        // Don't show the player holding the pickup if we already have something in hand.
        if (!alreadyHolding)
            currentPickup.transform.SetParent(player.transform);
        else
            Destroy(currentPickup.gameObject);
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

    public void DropoffEnterRange(DropoffTrigger sender, string promptText)
    {
        dropoffTrigger = sender;
        dropoffTag = dropoffTrigger.GetTag();
        dropoffInRange = true;
        uiManager.EnterRange(dropoffTrigger.transform, "Dropoff", promptText);
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

        DropoffTrigger thisDropoff = dropoffTrigger;
        Vector3 startPosition = heldItem.transform.position;
        Vector3 endPosition = thisDropoff.targetTransform.position;
        float elapsed = 0f;
        float time = 0.5f;
        while (elapsed < time)
        {
            heldItem.transform.position = Vector3.Lerp(
                startPosition, endPosition, Helper.SmoothStep(elapsed / time));
            elapsed += Time.deltaTime;
            yield return null;
        }
        heldItem.transform.position = endPosition;
        var dropoffTarget = thisDropoff.targetTransform.gameObject.GetComponent<DropoffTarget>();
        if (dropoffTarget != null) dropoffTarget.ReactToDropoff();
        heldItem.Remove();
        stateManager.SetState(StateManager.State.Normal);

        OnDropoff?.Invoke(this, new DropoffArgs { tag = thisDropoff.GetTag() });

        thisDropoff.Remove();
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
            tag = dialogTrigger.triggerTag,
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
        uiManager = FindObjectOfType<UIManager>();
        dialogManager = FindObjectOfType<DialogManager>();
        pickupManager = FindObjectOfType<PickupManager>();
    }

    void SubscribeToEvents()
    {
        inputManager.OnButtonDown += HandleInputEvent;
    }

    void OnDestroy()
    {
        inputManager.OnButtonDown -= HandleInputEvent;
    }

}
