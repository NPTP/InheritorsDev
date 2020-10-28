using System;
using System.Collections;
using UnityEngine;
using DG.Tweening;
using TMPro;
using UnityEngine.UI;


public class InteractManager : MonoBehaviour
{
    GameObject player;
    DayManager dayManager;
    InputManager inputManager;
    // Will need taskmanager and all that

    PickupTrigger[] pickupTriggers;
    GameObject item = null;
    bool pickupInRange = false;
    bool holdingItem = false;

    GameObject interactPrompt;
    RectTransform interactPromptRectTransform;
    Image interactPromptImage;
    public UIPromptImages uiPromptImages;

    void Start()
    {
        player = GameObject.Find("Player");
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

        interactPrompt = GameObject.Find("InteractPrompt");
        interactPromptRectTransform = interactPrompt.GetComponent<RectTransform>();
        interactPromptImage = interactPrompt.GetComponent<Image>();
        interactPromptImage.enabled = false;
    }

    // Subscribe a newly added pickup.
    void AddNewPickup(PickupTrigger pt)
    {
        pt.OnPickupEnterRange += HandlePickupEnterRange;
        pt.OnPickupLeaveRange += HandlePickupLeaveRange;
    }

    private void HandleInputEvent(object sender, InputManager.ButtonArgs args)
    {
        if (dayManager.state == DayManager.State.Normal)
        {
            if (args.buttonCode == InputManager.A)
            {
                if (pickupInRange && !holdingItem)
                    StartCoroutine(PickUpItem());
                else if (holdingItem)
                    StartCoroutine(PutDownItem());
            }
        }
    }

    IEnumerator PickUpItem()
    {
        inputManager.allow_AButton = false;
        interactPromptImage.enabled = false;
        item.GetComponent<PickupTrigger>().GetPickedUp();
        float t = 0f;
        while (t < 1f)
        {
            item.transform.position = Vector3.Lerp(item.transform.position, GetItemHoldPosition(), t);
            t += Time.deltaTime;
            if (t > .8f) break;
            yield return null;
        }
        item.transform.position = GetItemHoldPosition();
        pickupInRange = false;
        holdingItem = true;
        item.transform.SetParent(player.transform);
        inputManager.allow_AButton = true;
        // TODO: set UI showing you have an item/button to put item back down
    }

    IEnumerator PutDownItem()
    {
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
        interactPromptImage.enabled = true;
        interactPromptImage.sprite = uiPromptImages.A_Button;
        StartCoroutine(AlignPrompt());
    }

    IEnumerator AlignPrompt()
    {
        interactPromptImage.color = Helper.ChangedAlpha(interactPromptImage.color, 0f);
        DOTween.To(() => interactPromptImage.color, x => interactPromptImage.color = x, Helper.ChangedAlpha(interactPromptImage.color, 1f), .25f);
        interactPromptImage.CrossFadeAlpha(1f, .25f, false);
        while (pickupInRange)
        {
            Vector3 pos = Camera.main.WorldToScreenPoint(item.transform.position);
            pos.y += 100f;
            interactPromptRectTransform.position = pos;
            yield return null;
        }
    }

    void HandlePickupLeaveRange(object sender, EventArgs args)
    {
        pickupInRange = false;
        item = null;
        DOTween.To(() => interactPromptImage.color, x => interactPromptImage.color = x, Helper.ChangedAlpha(interactPromptImage.color, 0f), .25f);
        // interactPromptImage.enabled = false;
        // TODO: a coroutine here that handles keeping the prompt aligned while also fading it out, then disables it
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
