/* INHERITORS by Nick Perrin (c) 2020 */
using System;
using System.Collections;
using System.Collections.Generic;
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
    ItemType itemType;
    public event EventHandler<PickupArgs> OnPickup;
    public class PickupArgs : EventArgs
    {
        public string tag;
        public PickupManager.Inventory inventory;
    }

    DropoffTrigger dropoffTrigger = null;
    string dropoffTag = null;
    bool dropoffInRange = false;
    ItemType dropoffItemType;
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
                else if (!pickupInRange && dropoffInRange)
                    TryDropoff();
                break;

            case InputManager.Y:
                if (dialogInRange)
                    StartDialog();
                break;

            // case InputManager.X:
            //     if (dropoffInRange)
            //         TryDropoff();
            //     break;

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

    public void AreaEnter(AreaTrigger sender, ref List<Trigger> triggersInside, ItemType toolType)
    {
        areaTrigger = sender;
        areaTag = areaTrigger.GetTag();
        insideArea = true;
        if (areaTrigger.enableTriggersOnEnter)
        {
            foreach (Trigger trigger in triggersInside)
            {
                print(trigger.StartedEnabled());
                if (trigger.StartedEnabled()) { trigger.Enable(); }
            }
        }
        if (toolType != ItemType.Null)
            pickupManager.GetTaskTool(toolType);
        // OnArea?.Invoke(this, new AreaArgs { tag = areaTag, inside = true });
    }

    public void AreaLeave(AreaTrigger sender, ref List<Trigger> triggersInside)
    {
        areaTrigger = null;
        areaTag = null;
        insideArea = false;
        if (sender.enableTriggersOnEnter)
        {
            foreach (Trigger trigger in triggersInside)
            {
                trigger.Disable();
            }
        }
        pickupManager.LoseTaskTool();
        // OnArea?.Invoke(this, new AreaArgs { tag = sender.GetTag(), inside = false });
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
        // Currently not checking against item type & quantity, just allowing all.
        // Make a safety check before allowing pickup.
        if (!pickupInRange || pickupTag is null || pickupTrigger is null)
        {
            print("**** Failed this pickup ****\n" +
            "pickupInRange: " + pickupInRange + "\n" +
            "pickupTag: " + pickupTag + "\n" +
            "pickupTrigger: " + pickupTrigger);
            return;
        }
        stateManager.SetState(State.PickingUp);
        PickupTrigger currentPickup = pickupTrigger;
        currentPickup.GetPickedUp();
        uiManager.ExitRange(currentPickup.transform, "Pickup"); // pickupPrompt.Hide();
        pickupInRange = false;
        StartCoroutine(PickUpItem(currentPickup));
    }

    IEnumerator PickUpItem(PickupTrigger currentPickup)
    {
        bool alreadyHolding = pickupManager.IsHoldingItem();
        Vector3 startPosition = currentPickup.transform.position;
        Quaternion startRotation = currentPickup.transform.localRotation;
        float elapsed = 0f;
        float time = 0.25f;

        while (elapsed < time)
        {
            currentPickup.transform.position = Vector3.Lerp(
                startPosition, GetItemHoldPosition(), Helper.SmoothStep(elapsed / time));
            currentPickup.transform.localRotation = Quaternion.Slerp(
                startRotation, Quaternion.Euler(player.transform.forward), Helper.SmoothStep(elapsed / time));
            if (alreadyHolding)
                currentPickup.transform.localScale *= 1 - (elapsed / time);
            elapsed += Time.deltaTime;
            yield return null;
        }
        currentPickup.transform.position = GetItemHoldPosition();
        currentPickup.transform.rotation = Quaternion.Euler(player.transform.forward);
        stateManager.SetState(State.Holding);

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
            stateManager.SetState(State.DroppingOff);
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
        LookAtTarget(thisDropoff.target);
        Vector3 startPosition = heldItem.transform.position;
        Vector3 endPosition = thisDropoff.target.position;
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
        var dropoffTarget = thisDropoff.target.gameObject.GetComponent<DropoffTarget>();
        if (dropoffTarget != null) dropoffTarget.ReactToDropoff();
        heldItem.Remove();
        stateManager.SetState(State.Normal);

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
        stateManager.SetState(State.Dialog);
        dialogTrigger.Disable();
        dialogInRange = false;
        uiManager.dialogPrompt.Hide();
        if (dialogTrigger.lookAtMyTarget && dialogTrigger.myTarget != null)
        {
            // Halt and face the subject of the dialog!
            player.GetComponent<PlayerMovement>().Halt();
            LookAtTarget(dialogTrigger.myTarget);
        }

        // See if anyone is subscribed to OnDialog.
        // If not, play ONLY the trigger's stored dialog.
        // If so, fire an event to the Day script to handle it, which may play a pre-written dialog or the trigger's stored dialog.
        if (OnDialog == null)
        {
            dialogManager.NewDialog(dialogTrigger.dialog);
        }
        else
        {
            OnDialog.Invoke(this, new DialogArgs
            {
                tag = dialogTrigger.triggerTag,
                dialog = dialogTrigger.dialog
            });
        }

        if (dialogTrigger.dialogPersists)
            StartCoroutine(WaitToResetTrigger());
    }

    void LookAtTarget(Transform target)
    {
        Vector3 lookRotation = Quaternion.LookRotation(
                target.position - player.transform.position,
                Vector3.up).eulerAngles;
        lookRotation.x = 0f;
        lookRotation.z = 0f;
        player.GetComponent<Rigidbody>().DORotate(lookRotation, .4f);
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
        walkTag = thisTrigger.GetTag();
        thisTrigger.Activate();
        OnWalk?.Invoke(this, new WalkArgs { tag = thisTrigger.GetTag() });
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
