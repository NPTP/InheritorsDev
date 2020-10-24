using System.Collections;
using System.Collections.Generic;
using UnityEngine;

// Again, hacky for now. Fix later.
public class PlayerItemInteract : MonoBehaviour
{
    public bool carryingItem = false;
    public bool inPickupRange = false;
    public bool inPutdownRange = false;
    PickupTrigger pickupTrigger;
    PutdownTrigger putdownTrigger;
    GameObject interactPrompt;
    Animator animator;
    PlayerMovement playerMovement;

    void Start()
    {
        interactPrompt = GameObject.Find("InteractPrompt");
        putdownTrigger = GameObject.Find("PutdownTrigger").GetComponent<PutdownTrigger>();
        animator = GetComponent<Animator>();
        playerMovement = GameObject.Find("Player").GetComponent<PlayerMovement>();
        transform.GetChild(3).gameObject.GetComponent<Renderer>().enabled = false;
    }

    void Update()
    {
        if (inPickupRange)
        {
            foreach (Transform child in interactPrompt.transform)
            {
                child.gameObject.SetActive(true);
            }
            if (Input.GetButtonDown("Fire1"))
            {
                carryingItem = true;
                inPickupRange = false;
                animator.Play("Pickup");
                StartCoroutine(InteractWait());
            }
        }
        else if (inPutdownRange)
        {
            foreach (Transform child in interactPrompt.transform)
            {
                child.gameObject.SetActive(true);
            }
            if (Input.GetButtonDown("Fire1"))
            {
                putdownTrigger.GetPutdown();
                carryingItem = false;
                inPutdownRange = false;
                animator.Play("Pickup");
                transform.GetChild(3).gameObject.GetComponent<Renderer>().enabled = false;
                StartCoroutine(InteractWait());
            }
        }
        else
        {
            foreach (Transform child in interactPrompt.transform)
            {
                child.gameObject.SetActive(false);
            }
        }
    }

    IEnumerator InteractWait()
    {
        playerMovement.isInteracting = true;
        yield return new WaitForSeconds(1.5f);
        if (carryingItem)
        {
            pickupTrigger.GetPickedUp();
            putdownTrigger.EnablePutdownTrigger();
            transform.GetChild(3).gameObject.GetComponent<Renderer>().enabled = true;
            playerMovement.isInteracting = false;
        }
    }

    void PickUpItem(GameObject item)
    {
        inPickupRange = false;
        carryingItem = true;
        // heldItem.GetPickedUp();
        // heldItem.transform.position = transform.position + (.5f * transform.forward) + (-.2f * transform.up);
        // heldItem.transform.SetParent(transform); // (player.GetComponent<Transform>());
    }

    void DropItem()
    {
        // heldItem.GetPutDown(transform.forward);
        // heldItem.transform.SetParent(dayManager.currentDay.transform);
        carryingItem = false;
    }

    // Called when *I* enter a trigger collider.
    void OnTriggerEnter(Collider other)
    {
        if (other.tag == "Pickup")
        {
            inPickupRange = true;
            pickupTrigger = other.gameObject.GetComponent<PickupTrigger>();
        }
        else if (other.tag == "Putdown" && carryingItem)
        {
            inPutdownRange = true;
            pickupTrigger = other.gameObject.GetComponent<PickupTrigger>();
        }
    }

    // Called when *I* leave a trigger collider.
    void OnTriggerExit(Collider other)
    {
        if (other.tag == "Pickup")
        {
            inPickupRange = false;
            pickupTrigger = null;
        }
        else if (other.tag == "Putdown")
        {
            inPutdownRange = false;
        }
    }

}
