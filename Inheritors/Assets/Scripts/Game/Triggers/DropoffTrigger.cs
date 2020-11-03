using System;
using UnityEngine;
using DG.Tweening;

// Trigger to be placed around any pickup, sends events to InteractManager.
public class DropoffTrigger : MonoBehaviour, Trigger
{
    InteractManager interactManager;

    public event EventHandler OnTriggerActivate;

    public bool triggerEnabled = true;
    public string triggerTag;
    public Transform targetTransform;

    Transform playerTransform;
    Collider triggerCollider;
    Light l;
    ParticleSystem ps;
    bool dropoffDone = false;

    void Awake()
    {
        interactManager = FindObjectOfType<InteractManager>();
    }

    void Start()
    {
        playerTransform = GameObject.Find("Player").transform;
        triggerCollider = GetComponent<Collider>();
        l = transform.GetChild(0).gameObject.GetComponent<Light>();
        ps = transform.GetChild(1).gameObject.GetComponent<ParticleSystem>();

        if (triggerEnabled) Enable();
        else Disable();
    }

    public string GetTag()
    {
        return triggerTag;
    }

    public void Enable()
    {
        if (!dropoffDone)
        {
            triggerEnabled = true;
            triggerCollider.enabled = true;
            l.enabled = true;
            ps.Play();
        }
    }

    public void Disable()
    {
        triggerEnabled = false;
        triggerCollider.enabled = false;
        l.enabled = false;
        ps.Stop();
    }

    // ONLY call this from the Day when you have used its information already.
    public void Remove()
    {
        Destroy(this.gameObject);
    }

    // Called by interact manager to close out the trigger.
    public void CompleteDropoff()
    {
        dropoffDone = true;
        OnTriggerActivate?.Invoke(this, EventArgs.Empty); // TODO: maybe move this trigger
        // to the end of the dropoff process, so we can control task completion, animation,
        // Day sequences after things have been fully dropped off, not just when you PRESS
        // drop off.
    }

    private void OnTriggerEnter(Collider other)
    {
        if (triggerEnabled && !dropoffDone && other.tag == "Player")
        {
            interactManager.DropoffEnterRange(this);
        }
    }

    private void OnTriggerExit(Collider other)
    {
        if (triggerEnabled && !dropoffDone && other.tag == "Player")
        {
            interactManager.DropoffExitRange(this);
        }
    }

}
