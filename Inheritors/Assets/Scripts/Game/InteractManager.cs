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
    AudioManager audioManager;

    [SerializeField] AudioClip pickupSound;
    [SerializeField] float pickupSoundVolumeScale = 0.2f;
    [SerializeField] AudioClip dropoffSound;
    [SerializeField] float dropoffSoundVolumeScale = 0.2f;
    [SerializeField] AudioClip dialogSound;
    [SerializeField] float dialogSoundVolumeScale = 0.2f;

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
    Vector3 pickupItemLocalScale = Vector3.one; // Used for skeleton items
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

    public void AreaEnter(AreaTrigger sender, ItemType toolType)
    {
        areaTrigger = sender;
        areaTag = areaTrigger.GetTag();
        insideArea = true;
        sender.TurnOnTriggers();
        if (toolType != ItemType.Null)
            pickupManager.GetSkeletonItem(toolType);
        // OnArea?.Invoke(this, new AreaArgs { tag = areaTag, inside = true });
    }

    public void AreaLeave(AreaTrigger sender)
    {
        areaTrigger = null;
        areaTag = null;
        insideArea = false;
        sender.TurnOffTriggers();
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

        audioManager.PlayOneShot(pickupSound, pickupSoundVolumeScale);

        bool isSkeletonItem = IsSkeletonItem(currentPickup.itemType);
        if (isSkeletonItem) { pickupItemLocalScale = currentPickup.itemTransform.localScale; }
        print("Localscale of the the item: " + "(" + pickupItemLocalScale.x + ", " + pickupItemLocalScale.y + ", " + pickupItemLocalScale.z + ")");

        Vector3 startPosition = currentPickup.itemTransform.position;
        Vector3 startForwardHeading = currentPickup.itemTransform.forward;
        float acquireTime = 0.25f;
        float elapsed = 0f;

        while (elapsed < acquireTime)
        {
            float t = elapsed / acquireTime;

            currentPickup.itemTransform.position = Vector3.Lerp(
                startPosition, GetItemHoldPosition(), Helper.SmoothStep(t));
            currentPickup.itemTransform.forward = Vector3.Lerp(
                startForwardHeading, player.transform.forward, t);

            if (alreadyHolding || isSkeletonItem) // Shrink the item (ease in) if we're already holding or is attached to rig
                currentPickup.itemTransform.localScale *= 1 - (t * t);

            elapsed += Time.deltaTime;
            yield return null;
        }

        currentPickup.itemTransform.position = GetItemHoldPosition();
        currentPickup.itemTransform.forward = player.transform.forward;

        if (isSkeletonItem)
        {
            currentPickup.itemTransform.localScale = Vector3.zero;
            if (!alreadyHolding)
                pickupManager.GetSkeletonItem(currentPickup.itemType);
        }

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

    bool IsSkeletonItem(ItemType type)
    {
        if (type == ItemType.Papaya ||
            type == ItemType.Corn ||
            type == ItemType.Yopo ||
            type == ItemType.Herbs ||
            type == ItemType.DarkFlute)
            return true;
        else
            return false;
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
        PickupTrigger heldPickupTrigger = pickupManager.GetHeldItem();
        DropoffTrigger thisDropoff = dropoffTrigger;
        pickupManager.DropOff(thisDropoff.takeFullInventory);

        PlayerMovement playerMovement = player.GetComponent<PlayerMovement>();
        playerMovement.Halt();
        playerMovement.LookAtTarget(thisDropoff.target);

        audioManager.PlayOneShot(dropoffSound, dropoffSoundVolumeScale);
        Animator playerAnimator = player.GetComponent<Animator>();
        playerAnimator.SetTrigger("Pickup");

        bool isSkeletonItem = IsSkeletonItem(heldPickupTrigger.itemType);
        if (isSkeletonItem) { pickupManager.LoseTaskTool(); }

        Transform heldItemTransform = heldPickupTrigger.itemTransform;
        // DEBUG
        if (isSkeletonItem)
        {
            heldItemTransform.localScale = pickupItemLocalScale;
        }
        print("Localscale of the the item: " + "(" + heldItemTransform.localScale.x + ", " + heldItemTransform.localScale.y + ", " + heldItemTransform.localScale.z + ")");
        // DEBUG
        Vector3 startPosition = heldItemTransform.position;
        Vector3 endPosition = thisDropoff.target.position;
        float elapsed = 0f;
        float dropTime = 0.5f;
        while (elapsed < dropTime)
        {
            float timeRatio = elapsed / dropTime;

            heldItemTransform.position = Vector3.Lerp(
                startPosition, endPosition, Helper.SmoothStep(timeRatio));

            if (isSkeletonItem)
            {
                heldItemTransform.localScale = (1 - timeRatio) * pickupItemLocalScale;
            }

            elapsed += Time.deltaTime;
            yield return null;
        }
        heldItemTransform.position = endPosition;

        var dropoffTarget = thisDropoff.target.gameObject.GetComponent<DropoffTarget>();
        thisDropoff.Disable();
        heldPickupTrigger.Remove();
        pickupManager.LoseTaskTool();
        playerAnimator.ResetTrigger("Pickup");

        if (dropoffTarget != null)
        {
            dropoffTarget.ReactToDropoff();
            yield return new WaitUntil(dropoffTarget.DoneReaction);
        }

        stateManager.SetState(State.Normal);
        OnDropoff?.Invoke(this, new DropoffArgs { tag = thisDropoff.GetTag() });

        yield return new WaitForSeconds(1);
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
        dialogTrigger.InteractDisappear();
        dialogInRange = false;
        uiManager.dialogPrompt.Hide();

        audioManager.PlayOneShot(dialogSound, dialogSoundVolumeScale);

        if (dialogTrigger.lookAtMyTarget && dialogTrigger.myTarget != null)
        {
            // Halt and face the subject of the dialog!
            PlayerMovement playerMovement = player.GetComponent<PlayerMovement>();
            playerMovement.Halt();
            playerMovement.LookAtTarget(dialogTrigger.myTarget);
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

    IEnumerator WaitToResetTrigger()
    {
        yield return new WaitUntil(dialogManager.IsDialogAnimationFinished);
        dialogTrigger.InteractAppear();
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
        audioManager = FindObjectOfType<AudioManager>();
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
