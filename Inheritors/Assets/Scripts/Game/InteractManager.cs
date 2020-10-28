using System;
using System.Collections;
using UnityEngine;
using DG.Tweening;
using TMPro;
using UnityEngine.UI;


public class InteractManager : MonoBehaviour
{
    DayManager dayManager;
    InputManager inputManager;
    // Will need taskmanager and all that

    PickupTrigger[] pickupTriggers;
    GameObject item;
    bool pickupInRange = false;

    void Start()
    {
        dayManager = GameObject.Find("DayManager").GetComponent<DayManager>();
        inputManager = GameObject.Find("InputManager").GetComponent<InputManager>();
        inputManager.OnButtonDown += HandleInputEvent;

        // Subscribe to all existing pickups.
        pickupTriggers = GameObject.FindObjectsOfType<PickupTrigger>();
        foreach (PickupTrigger pt in pickupTriggers)
        {
            pt.OnPickupEnterRange += HandlePickupEnterRange;
            pt.OnPickupLeaveRange += HandlePickupLeaveRange;
        }
    }

    private void HandleInputEvent(object sender, InputManager.ButtonArgs args)
    {
        if (dayManager.state == DayManager.State.Normal)
        {
            if (args.buttonCode == InputManager.A)
            {
                if (pickupInRange)
                {
                    Debug.Log("Hit pickup button in pickup range");
                }
                else
                {
                    Debug.Log("Tried to pickup outside of range");
                }
            }
        }
    }

    // Subscribe a newly added pickup.
    void AddNewPickup(PickupTrigger pt)
    {
        pt.OnPickupEnterRange += HandlePickupEnterRange;
        pt.OnPickupLeaveRange += HandlePickupLeaveRange;
    }

    void HandlePickupEnterRange(object sender, EventArgs args)
    {
        pickupInRange = true;
        item = ((PickupTrigger)sender).gameObject;
        // TODO: set UI appropriately
    }

    void HandlePickupLeaveRange(object sender, EventArgs args)
    {
        pickupInRange = false;
        item = null;
        // TODO: set UI appropriately
    }

    void PickUpItem()
    {
        pickupInRange = false;
        item.GetComponent<PickupTrigger>().GetPickedUp();
    }

    // Unsubscribe from all events
    void OnDestroy()
    {
        foreach (PickupTrigger pt in pickupTriggers)
        {
            pt.OnPickupEnterRange -= HandlePickupEnterRange;
            pt.OnPickupLeaveRange -= HandlePickupLeaveRange;
        }
    }
}
