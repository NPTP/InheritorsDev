using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerPickup : MonoBehaviour
{
    public bool carryingItem = false;
    public bool inPickupRange = false;
    GameObject itemInRange;
    PickupItem heldItem;
    DayManager dayManager;

    void Start()
    {
        // TODO: item types that let you hold multiple of them
        dayManager = GameObject.Find("DayManager").GetComponent<DayManager>();
    }

    void Update()
    {
        if (Input.GetKeyDown(KeyCode.E))
        {
            if (carryingItem)
                DropItem();
            else if (inPickupRange)
                PickUpItem(itemInRange);
        }
    }

    void PickUpItem(GameObject item)
    {
        heldItem = item.GetComponent<PickupItem>();
        inPickupRange = false;
        carryingItem = true;
        heldItem.GetPickedUp();
        heldItem.transform.position = transform.position + (.5f * transform.forward) + (-.2f * transform.up);
        heldItem.transform.SetParent(transform); // (player.GetComponent<Transform>());
        Debug.Log("Picked up a " + heldItem.name);
    }

    void DropItem()
    {
        heldItem.GetPutDown(transform.forward);
        heldItem.transform.SetParent(dayManager.currentDay.transform);
        carryingItem = false;
    }

    // Called when *I* enter a trigger collider.
    void OnTriggerEnter(Collider other)
    {
        if (other.tag == "Pickup")
        {
            inPickupRange = true;
            itemInRange = other.gameObject.transform.parent.gameObject;
        }
    }

    // Called when *I* leave a trigger collider.
    void OnTriggerExit(Collider other)
    {
        if (other.tag == "Pickup")
        {
            inPickupRange = false;
            itemInRange = null;
        }
    }

    // Called when something hits ME
    void OnCollisionEnter(Collision collision)
    {

    }

    // Called when *I* hit something
    void OnControllerColliderHit(ControllerColliderHit hit)
    {

    }


}
