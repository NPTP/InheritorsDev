using System;
using System.Collections;
using UnityEngine;
using DG.Tweening;

// Trigger to be placed around any pickup, coupled with InteractManager.
public class PickupTrigger : MonoBehaviour, Trigger
{
    InteractManager interactManager;

    public bool startEnabled = true;
    public string triggerTag;
    public ItemType itemType;
    public TaskType taskType;

    bool destroyed = false;

    Transform originalParent;
    Collider triggerCollider;
    Transform itemTransform;
    Vector3 itemLocalScale;
    Light l;
    float originalIntensity;
    ParticleSystem ps;
    TriggerProjector triggerProjector;

    bool pickedUp = false;
    bool droppedOff = false;

    public bool StartedEnabled()
    {
        return startEnabled;
    }

    void Awake()
    {
        interactManager = FindObjectOfType<InteractManager>();
        originalParent = transform.parent;
        triggerCollider = GetComponent<Collider>();
        itemTransform = transform.GetChild(0);
        itemLocalScale = itemTransform.localScale;
        l = transform.GetChild(1).gameObject.GetComponent<Light>();
        originalIntensity = l.intensity;
        ps = transform.GetChild(2).gameObject.GetComponent<ParticleSystem>();
        triggerProjector = transform.GetChild(3).gameObject.GetComponent<TriggerProjector>();
    }

    void Start()
    {
        if (startEnabled) Enable();
        else Disable();
    }

    public string GetTag()
    {
        return triggerTag;
    }

    public Vector3 GetPosition()
    {
        return transform.position;
    }

    public void SetOriginalParent(Transform parent)
    {
        originalParent = parent;
    }

    public void ResetParent()
    {
        transform.parent = originalParent;
    }

    public void Enable()
    {
        if (!pickedUp)
        {
            // startEnabled = true;
            triggerCollider.enabled = true;
            l.enabled = true;
            l.DOIntensity(originalIntensity, .25f).From(0f);
            ps.Play();
            triggerProjector.Enable();
        }
    }

    public void Disable()
    {
        // startEnabled = false;
        if (!destroyed)
        {
            triggerCollider.enabled = false;
            l.DOIntensity(0f, .25f);
            ps.Stop();
            triggerProjector.Disable();
        }
    }

    void OnDestroy()
    {
        destroyed = true;
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
        droppedOff = true;
        Tween t = itemTransform.DOScale(0f, .5f).SetEase(Ease.InQuart);
    }

    private void OnTriggerEnter(Collider other)
    {
        // if (startEnabled && other.tag == "Player")
        if (other.tag == "Player")
        {
            interactManager.PickupEnterRange(this);
            itemTransform.DOScale(itemLocalScale * 1.15f, .25f);
        }
    }

    private void OnTriggerExit(Collider other)
    {
        // if (startEnabled && other.tag == "Player")
        if (other.tag == "Player")
        {
            interactManager.PickupExitRange(this);
            itemTransform.DOScale(itemLocalScale, .25f);
        }
    }

}
