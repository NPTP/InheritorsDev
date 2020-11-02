using System;
using System.Collections;
using UnityEngine;
using DG.Tweening;
using TMPro;

public class InteractManager_OLD : MonoBehaviour
{
    GameObject player;
    StateManager stateManager;
    InputManager inputManager;
    UIManager uiManager;

    DialogTrigger[] dialogTriggers;
    bool dialogInRange = false;
    GameObject speaker; // Subject of the dialog

    PickupTrigger[] pickupTriggers;
    public GameObject item = null; // Public so other scripts can see what the player is carrying
    bool pickupInRange = false;
    bool holdingItem = false;
    string heldItemType = null;
    public int itemQuantity = 0;
    bool putdownInRange = false;

    void Start()
    {
        InitializeReferences();
        SubscribeToEvents();
    }

    public bool IsPickupInRange()
    {
        return pickupInRange;
    }

    public bool IsDialogInRange()
    {
        return dialogInRange;
    }

    private void HandleInputEvent(object sender, InputManager.ButtonArgs args)
    {
        switch (stateManager.state)
        {
            case StateManager.State.Normal:
                if (args.buttonCode == InputManager.A && pickupInRange && !holdingItem)
                    StartCoroutine(PickUpItem());
                else if (args.buttonCode == InputManager.Y && dialogInRange)
                    Debug.Log("Dialog should start here.");
                break;

            case StateManager.State.Holding:
                if (args.buttonCode == InputManager.X && holdingItem && putdownInRange)
                    StartCoroutine(PutDownItem());
                break;
            default:
                break; // Don't take inputs while picking up, inert, etc.
        }
    }

    IEnumerator PickUpItem()
    {
        stateManager.SetState(StateManager.State.PickingUp);
        uiManager.pickupPrompt.Hide();
        item.GetComponent<PickupTrigger>().GetPickedUp();
        Vector3 startPosition = new Vector3(item.transform.position.x, item.transform.position.y, item.transform.position.z);
        float elapsed = 0f;
        float time = 0.25f;
        while (elapsed < time)
        {
            item.transform.position = Vector3.Lerp(startPosition, GetItemHoldPosition(), Helper.SmoothStep(elapsed / time));
            elapsed += Time.deltaTime;
            yield return null;
        }
        item.transform.position = GetItemHoldPosition();
        pickupInRange = false;
        holdingItem = true;
        string itemType = item.GetComponent<PickupTrigger>().pickupType;
        heldItemType = itemType;
        item.transform.SetParent(player.transform);
        stateManager.SetState(StateManager.State.Holding);
        if (itemType == heldItemType)
            itemQuantity++;
    }

    void StartDialog(string[] lines, float speed)
    {
        // OnLocalDialog?.Invoke(this, new LocalDialogArgs { lines = lines, speed = speed });
        speaker.GetComponent<DialogTrigger>().StartDialog();
        uiManager.pickupPrompt.Hide();
        Vector3 lookRotation = Quaternion.LookRotation(speaker.transform.position - player.transform.position, Vector3.up).eulerAngles;
        lookRotation.x = 0f;
        lookRotation.z = 0f;
        player.GetComponent<Rigidbody>().DORotate(lookRotation, .25f);
    }

    public void EndDialog()
    {
        speaker.GetComponent<DialogTrigger>().EndDialog();
    }

    IEnumerator PutDownItem()
    {
        stateManager.SetState(StateManager.State.Normal);
        uiManager.pickupPrompt.Hide();
        uiManager.pickupPrompt.SetSize(1f, 1f, 1f);
        holdingItem = false;
        // Put item down here --> used to be GetPutDown call in the trigger object
        item.transform.SetParent(null);
        yield return null;
    }

    Vector3 GetItemHoldPosition()
    {
        return player.transform.position + player.transform.up + (.5f * player.transform.forward);
    }

    void HandlePickupEnterRange(object sender, EventArgs args)
    {
        pickupInRange = true;
        item = ((PickupTrigger)sender).gameObject;
        uiManager.EnterRange(item.transform, "Pickup");
    }

    void HandleDialogEnterRange(object sender, EventArgs args)
    {
        dialogInRange = true;
        speaker = ((DialogTrigger)sender).gameObject;
        uiManager.EnterRange(speaker.transform, "Dialog");
    }

    void HandlePickupLeaveRange(object sender, EventArgs args)
    {
        pickupInRange = false;
        uiManager.ExitRange(item.transform, "Pickup");
        item = null;
    }

    void HandleDialogLeaveRange(object sender, EventArgs args)
    {
        dialogInRange = false;
        uiManager.ExitRange(speaker.transform, "Dialog");
        speaker = null;
    }

    // Unsubscribe from all events
    void OnDestroy()
    {
        foreach (PickupTrigger pt in pickupTriggers)
        {
            pt.OnPickupEnterRange -= HandlePickupEnterRange;
            pt.OnPickupLeaveRange -= HandlePickupLeaveRange;
        }
        foreach (DialogTrigger dt in dialogTriggers)
        {
            dt.OnDialogEnterRange -= HandleDialogEnterRange;
            dt.OnDialogLeaveRange -= HandleDialogLeaveRange;
        }
    }

    void InitializeReferences()
    {
        player = GameObject.Find("Player");
        stateManager = FindObjectOfType<StateManager>();
        inputManager = FindObjectOfType<InputManager>();
        inputManager.OnButtonDown += HandleInputEvent;
        uiManager = FindObjectOfType<UIManager>();
    }

    void SubscribeToEvents()
    {


        // Subscribe to all existing dialog triggers.
        dialogTriggers = GameObject.FindObjectsOfType<DialogTrigger>();
        foreach (DialogTrigger dt in dialogTriggers)
        {
            dt.OnDialogEnterRange += HandleDialogEnterRange;
            dt.OnDialogLeaveRange += HandleDialogLeaveRange;
        }

        // Subscribe to all existing pickup triggers.
        pickupTriggers = GameObject.FindObjectsOfType<PickupTrigger>();
        foreach (PickupTrigger pt in pickupTriggers)
        {
            pt.OnPickupEnterRange += HandlePickupEnterRange;
            pt.OnPickupLeaveRange += HandlePickupLeaveRange;
        }
    }
}
