using System;
using UnityEngine;
using DG.Tweening;

// Trigger to be placed around any pickup, sends events to InteractManager.
public class PickupTrigger : MonoBehaviour, Trigger
{
    PickupManager pickupManager;

    public event EventHandler OnPickupEnterRange;
    public event EventHandler OnPickupLeaveRange;
    public event EventHandler OnTriggerActivate;

    public bool triggerEnabled = true;
    public string triggerTag;
    public string pickupType;

    Transform playerTransform;
    Collider sphereCollider;
    Transform itemTransform;
    Vector3 itemLocalScale;
    BoxCollider itemCollider;
    Light l;
    ParticleSystem ps;

    bool pickedUp = false; // Used for drop triggers to know if the item has been dropped inside yet.

    void Awake()
    {
        pickupManager = FindObjectOfType<PickupManager>();
    }

    void Start()
    {
        playerTransform = GameObject.Find("Player").transform;
        sphereCollider = GetComponent<Collider>();
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

    public void Enable()
    {
        triggerEnabled = true;
        sphereCollider.enabled = true;
        itemCollider.enabled = true;
        l.enabled = true;
        ps.Play();
    }

    public void Disable()
    {
        triggerEnabled = false;
        sphereCollider.enabled = false;
        itemCollider.enabled = false;
        l.enabled = false;
        ps.Stop();
    }

    public void Remove()
    {
        Destroy(this.gameObject);
    }

    public void GetPickedUp()
    {
        pickedUp = true;
        OnTriggerActivate?.Invoke(this, EventArgs.Empty);
        itemTransform.DOScale(itemLocalScale, .25f);
        Disable();
    }

    // public void GetPutDown()
    // {
    //     transform.DOMoveY(playerTransform.position.y, .25f);
    //     Enable();
    // }

    public void GetPlacedInDropTrigger()
    {
        Debug.Log("Item placed in drop trigger!");
    }

    private void OnTriggerEnter(Collider other)
    {
        if (triggerEnabled && !pickupManager.IsHoldingItem() && other.tag == "Player")
        {
            OnPickupEnterRange?.Invoke(this, EventArgs.Empty);
            itemTransform.DOScale(itemLocalScale * 1.15f, .25f);
        }
    }

    private void OnTriggerExit(Collider other)
    {
        if (triggerEnabled && !pickupManager.IsHoldingItem() && other.tag == "Player")
        {
            OnPickupLeaveRange?.Invoke(this, EventArgs.Empty);
            itemTransform.DOScale(itemLocalScale, .25f);
        }
    }
}
