﻿using System;
using System.Collections;
using UnityEngine;
using DG.Tweening;

// Trigger to be placed around any pickup, coupled with InteractManager.
public class PickupTrigger : MonoBehaviour, Trigger
{
    InteractManager interactManager;

    public bool triggerEnabled = true;
    public string triggerTag;
    public PickupManager.ItemTypes itemType;

    Transform originalParent;
    Collider triggerCollider;
    Transform itemTransform;
    Vector3 itemLocalScale;
    BoxCollider itemCollider;
    Light l;
    ParticleSystem ps;

    bool pickedUp = false;
    bool droppedOff = false;

    void Awake()
    {
        interactManager = FindObjectOfType<InteractManager>();
    }

    void Start()
    {
        originalParent = transform.parent;
        triggerCollider = GetComponent<Collider>();
        itemTransform = transform.GetChild(0);
        itemLocalScale = itemTransform.localScale;
        itemCollider = itemTransform.gameObject.GetComponent<BoxCollider>();
        l = transform.GetChild(1).gameObject.GetComponent<Light>();
        ps = transform.GetChild(2).gameObject.GetComponent<ParticleSystem>();

        if (triggerEnabled) Enable();
        else Disable();
    }

    public string GetTag()
    {
        return triggerTag;
    }

    public void ResetParent()
    {
        transform.parent = originalParent;
    }

    public void Enable()
    {
        if (!pickedUp)
        {
            triggerEnabled = true;
            triggerCollider.enabled = true;
            itemCollider.enabled = true;
            l.enabled = true;
            ps.Play();
        }
    }

    public void Disable()
    {
        triggerEnabled = false;
        triggerCollider.enabled = false;
        itemCollider.enabled = false;
        l.enabled = false;
        ps.Stop();
    }

    // ONLY call this from another script once that script has finished with the trigger!
    public void Remove()
    {
        Destroy(this.gameObject);
    }

    public void GetPickedUp()
    {
        pickedUp = true;
        itemTransform.DOScale(itemLocalScale, .25f);
        Disable();
    }

    public void GetDroppedOff()
    {
        Debug.Log("Item placed in drop trigger!");
        droppedOff = true;
        Tween t = itemTransform.DOScale(0f, .5f).SetEase(Ease.InQuart);
    }

    private void OnTriggerEnter(Collider other)
    {
        if (triggerEnabled && other.tag == "Player")
        {
            interactManager.PickupEnterRange(this);
            itemTransform.DOScale(itemLocalScale * 1.15f, .25f);
        }
    }

    private void OnTriggerExit(Collider other)
    {
        if (triggerEnabled && other.tag == "Player")
        {
            interactManager.PickupExitRange(this);
            itemTransform.DOScale(itemLocalScale, .25f);
        }
    }
}
