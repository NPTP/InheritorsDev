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

    Trigger[] triggers;

    DialogTrigger dialogTrigger = null;
    string dialogTag = null;
    bool dialogInRange = false;
    GameObject speaker; // Subject of the dialog

    PickupTrigger pickupTrigger = null;
    string pickupTag = null;
    bool pickupInRange = false;
    // string itemType = null;
    PickupManager.ItemTypes itemType;
    public int itemQuantity = 0;

    // TODO: implement putdown triggers and these, its attributes.
    // PutdownTrigger putdownTrigger = null;
    // string putdownTag = null;
    bool putdownInRange = false;
    // PickupManager.ItemTypes putdownItemType;

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
                // TODO: check dropoffInRange, call a dropoff function.
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
            print("pickupInRange: " + pickupInRange + "\n" +
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
    // ███ DROPOFF
    // ████████████████████████████████████████████████████████████████████████


    IEnumerator PutDownItem()
    {
        stateManager.SetState(StateManager.State.Normal);
        uiManager.pickupPrompt.Hide();
        uiManager.pickupPrompt.SetSize(1f, 1f, 1f);
        // holdingItem = false; // --> replace with pickup manager functionality
        // Put item down here --> used to be GetPutDown call in the trigger object
        pickupTrigger.transform.SetParent(null);
        yield return null;
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
