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
    public GameObject item = null; // Public so other scripts can see what the player is carrying
    bool pickupInRange = false;
    bool holdingItem = false;

    GameObject interactPrompt;
    RectTransform interactPromptRectTransform;
    Image interactPromptImage;
    TMP_Text interactPromptText;
    public UIPromptImages uiPromptImages;

    void Start()
    {
        player = GameObject.Find("Player");
        dayManager = GameObject.Find("DayManager").GetComponent<DayManager>();
        dayManager.OnState += HandleState;
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
        interactPromptText = GameObject.Find("InteractPromptText").GetComponent<TMP_Text>();
        interactPromptText.enabled = false;
    }

    // Subscribe a newly added pickup.
    void AddNewPickup(PickupTrigger pt)
    {
        pt.OnPickupEnterRange += HandlePickupEnterRange;
        pt.OnPickupLeaveRange += HandlePickupLeaveRange;
    }

    private void HandleInputEvent(object sender, InputManager.ButtonArgs args)
    {
        switch (dayManager.state)
        {
            case DayManager.State.Normal:
                if (args.buttonCode == InputManager.A && pickupInRange && !holdingItem)
                    StartCoroutine(PickUpItem());
                break;
            case DayManager.State.Holding:
                if (args.buttonCode == InputManager.X && holdingItem)
                    StartCoroutine(PutDownItem());
                break;
            default:
                break; // Don't take inputs while picking up, inert, etc.
        }
    }

    private void HandleState(object sender, DayManager.StateArgs args)
    {
        switch (args.state)
        {
            case DayManager.State.Dialog:
                pickupInRange = false;
                break;
            default:
                break;
        }
    }

    IEnumerator PickUpItem()
    {
        dayManager.SetState(DayManager.State.PickingUp);
        interactPromptImage.enabled = false;
        item.GetComponent<PickupTrigger>().GetPickedUp();
        float t = 0f;
        while (t < 1f)
        {
            item.transform.position = Vector3.Lerp(item.transform.position, GetItemHoldPosition(), t);
            t += Time.deltaTime;
            if (t > .25f) break;
            yield return null;
        }
        item.transform.position = GetItemHoldPosition();
        pickupInRange = false;
        holdingItem = true;
        item.transform.SetParent(player.transform);
        dayManager.SetState(DayManager.State.Holding);
        StartCoroutine(HoldingItemUI());
    }

    IEnumerator HoldingItemUI()
    {
        interactPromptRectTransform.localScale = new Vector3(.6f, .6f, 1f);
        interactPromptImage.enabled = true;
        interactPromptText.enabled = true;
        interactPromptImage.sprite = uiPromptImages.X_Button;
        while (holdingItem)
        {
            Vector3 pos = Camera.main.WorldToScreenPoint(player.transform.position);
            pos.y -= 25f;
            interactPromptRectTransform.position = pos;
            yield return null;
        }
    }

    IEnumerator PutDownItem()
    {
        dayManager.SetState(DayManager.State.Normal);
        interactPromptText.enabled = false;
        interactPromptRectTransform.localScale = new Vector3(1f, 1f, 1f);
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
        interactPromptImage.color = Helper.ChangedAlpha(interactPromptImage.color, 0f);
        DOTween.To(() => interactPromptImage.color, x => interactPromptImage.color = x, Helper.ChangedAlpha(interactPromptImage.color, 1f), .25f);
        StartCoroutine(AlignPromptInRange());
    }

    IEnumerator AlignPromptInRange()
    {
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
        StartCoroutine(AlignPromptOutOfRange(item.transform.position));
        item = null;
    }

    IEnumerator AlignPromptOutOfRange(Vector3 itemPos)
    {
        Tween t = DOTween.To(() => interactPromptImage.color, x => interactPromptImage.color = x, Helper.ChangedAlpha(interactPromptImage.color, 0f), .25f);
        while (t.IsPlaying())
        {
            Vector3 pos = Camera.main.WorldToScreenPoint(itemPos);
            pos.y += 100f;
            interactPromptRectTransform.position = pos;
            yield return null;
        }
        interactPromptImage.enabled = false;
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
