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

    PickupTrigger pickupTrigger = null;
    string pickupTag = null;
    bool pickupInRange = false;
    // string itemType = null;
    PickupManager.ItemTypes itemType;

    // TODO: you might need a HELD pickup trigger storage attribute here, to differentiate
    // when you run over another pickuptrigger zone and the one you're holding gets forgotten!

    DropoffTrigger dropoffTrigger = null;
    string dropoffTag = null;
    bool dropoffInRange = false;
    PickupManager.ItemTypes dropoffItemType;

    DialogTrigger dialogTrigger = null;
    string dialogTag = null;
    bool dialogInRange = false;
    GameObject speaker; // Subject of the dialog

    WalkTrigger walkTrigger = null;
    string walkTag = null;

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

            case InputManager.Y:
                if (dialogInRange)
                    StartDialog();
                break;

            case InputManager.X:
                if (dropoffInRange)
                    TryDropoff();
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
        print("Tried pickup of " + pickupTag);
        // Currently not checking against item type & quantity, just allow all.
        // Make a safety check before allowing pickup.
        if (!pickupInRange || pickupTag is null || pickupTrigger is null)
        {
            print("Failed this pickup:\n" +
            "pickupInRange: " + pickupInRange + "\n" +
            "pickupTag: " + pickupTag + "\n" +
            "pickupTrigger: " + pickupTrigger);
            return;
        }
        stateManager.SetState(StateManager.State.PickingUp);
        uiManager.pickupPrompt.Hide();
        pickupTrigger.GetPickedUp();
        pickupInRange = false;
        StartCoroutine(PickUpItem());
    }

    IEnumerator PickUpItem()
    {
        Vector3 startPosition = pickupTrigger.transform.position;
        float elapsed = 0f;
        float time = 0.25f;
        while (elapsed < time)
        {
            pickupTrigger.transform.position = Vector3.Lerp(
                startPosition, GetItemHoldPosition(), Helper.SmoothStep(elapsed / time));
            elapsed += Time.deltaTime;
            yield return null;
        }
        pickupTrigger.transform.position = GetItemHoldPosition();
        pickupTrigger.transform.SetParent(player.transform);
        stateManager.SetState(StateManager.State.Holding);

        // Hand it off here to the PickupManager.
        pickupManager.PickUp(pickupTrigger, itemType);
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
        stateManager.SetState(StateManager.State.DroppingOff);
        uiManager.pickupPrompt.Hide();
        dropoffTrigger.CompleteDropoff();
        dropoffInRange = false;
        StartCoroutine(DropOffItem());
    }

    IEnumerator DropOffItem()
    {
        pickupTrigger.transform.SetParent(null); // Do we need to remember the prev. parent?
        pickupTrigger.GetDroppedOff();
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
        dropoffTrigger.Remove();
        stateManager.SetState(StateManager.State.Normal);

        // Hand it off here to the PickupManager.
        pickupManager.DropOff(pickupTrigger, itemType);
    }

    // ████████████████████████████████████████████████████████████████████████
    // ███ DIALOG
    // ████████████████████████████████████████████████████████████████████████

    public bool IsDialogInRange()
    {
        return dialogInRange;
    }

    public void HandleDialogEnterRange(object sender, EventArgs args)
    {
        dialogTrigger = (DialogTrigger)sender;
        dialogInRange = true;
        uiManager.EnterRange(dialogTrigger.transform, "Dialog");
    }

    public void HandleDialogLeaveRange(object sender, EventArgs args)
    {
        dialogTrigger = (DialogTrigger)sender;
        dialogInRange = false;
        uiManager.ExitRange(dialogTrigger.transform, "Dialog");
        dialogTrigger = null;
    }

    void StartDialog()
    {
        speaker.GetComponent<DialogTrigger>().Disable();
        uiManager.pickupPrompt.Hide();
        Vector3 lookRotation = Quaternion.LookRotation(speaker.transform.position - player.transform.position, Vector3.up).eulerAngles;
        lookRotation.x = 0f;
        lookRotation.z = 0f;
        player.GetComponent<Rigidbody>().DORotate(lookRotation, .25f);
    }

    // TODO
    public void EndDialog()
    {
        speaker.GetComponent<DialogTrigger>().Enable();
    }

    // ████████████████████████████████████████████████████████████████████████
    // ███ WALK
    // ████████████████████████████████████████████████████████████████████████

    public void WalkEnter(WalkTrigger sender)
    {
        walkTrigger = sender;
        walkTag = walkTrigger.GetTag();
        // TODO: invoke an event from here.
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
