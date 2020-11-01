using System;
using System.Collections;
using UnityEngine;
using DG.Tweening;
using TMPro;

// TODO: make prefabs of pickup zone fx that we can spawn in
// TODO: consider breaking up interact and pickup, just like dialog is already broken off from interact
public class InteractManager : MonoBehaviour
{
    GameObject player;
    StateManager stateManager;
    InputManager inputManager;
    UIManager uiManager;

    DialogTrigger[] dialogTriggers;
    bool dialogInRange = false;
    GameObject speaker; // = null; // Subject of the dialog
    public event EventHandler<LocalDialogArgs> OnLocalDialog;
    public class LocalDialogArgs : EventArgs
    {
        public string[] lines;
        public float speed;
    }

    PickupTrigger[] pickupTriggers;
    public GameObject item = null; // Public so other scripts can see what the player is carrying
    bool pickupInRange = false;
    bool holdingItem = false;

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

    // Subscribe a newly added pickup.
    void AddNewPickup(PickupTrigger pt)
    {
        pt.OnPickupEnterRange += HandlePickupEnterRange;
        pt.OnPickupLeaveRange += HandlePickupLeaveRange;
    }

    // Subscribe a newly added dialog.
    void AddNewDialog(DialogTrigger dt)
    {
        dt.OnDialogEnterRange += HandleDialogEnterRange;
        dt.OnDialogLeaveRange += HandleDialogLeaveRange;
    }

    private void HandleInputEvent(object sender, InputManager.ButtonArgs args)
    {
        switch (stateManager.state)
        {
            // NORMAL STATE
            case StateManager.State.Normal:
                if (args.buttonCode == InputManager.A && pickupInRange && !holdingItem)
                    StartCoroutine(PickUpItem());
                else if (args.buttonCode == InputManager.Y && dialogInRange)
                    Debug.Log("Dialog should start here."); // TODO: start the dialog
                break;

            // HOLDING STATE
            case StateManager.State.Holding:
                if (args.buttonCode == InputManager.X && holdingItem)
                    StartCoroutine(PutDownItem());
                break;
            default:
                break; // Don't take inputs while picking up, inert, etc.
        }
    }

    private void HandleState(object sender, StateManager.StateArgs args)
    {
        switch (args.state)
        {
            case StateManager.State.Dialog:
                pickupInRange = false;
                break;
            default:
                break;
        }
    }

    // TODO: This is where we can break off the pickupmanager/simple inventory system.
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
        item.transform.SetParent(player.transform);
        stateManager.SetState(StateManager.State.Holding);
        uiManager.HoldingItem(item.transform);
    }

    void StartDialog(string[] lines, float speed)
    {
        OnLocalDialog?.Invoke(this, new LocalDialogArgs { lines = lines, speed = speed });
        speaker.GetComponent<DialogTrigger>().StartDialog();
        uiManager.pickupPrompt.Hide();
        Vector3 lookRotation = Quaternion.LookRotation(speaker.transform.position - player.transform.position, Vector3.up).eulerAngles;
        lookRotation.x = 0f;
        lookRotation.z = 0f;
        player.GetComponent<Rigidbody>().DORotate(lookRotation, .25f);
    }

    // TODO
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
        item.GetComponent<PickupTrigger>().GetPutDown();
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
        stateManager = GameObject.FindObjectOfType<StateManager>();
        stateManager.OnState += HandleState;
        inputManager = GameObject.FindObjectOfType<InputManager>();
        inputManager.OnButtonDown += HandleInputEvent;
        uiManager = GameObject.FindObjectOfType<UIManager>();
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
