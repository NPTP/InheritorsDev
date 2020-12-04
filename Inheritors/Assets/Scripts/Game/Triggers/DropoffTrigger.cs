﻿using UnityEngine;

public class DropoffTrigger : MonoBehaviour, Trigger
{
    InteractManager interactManager;

    public bool triggerEnabled = true;
    public string triggerTag;
    bool destroyed = false;

    Collider triggerCollider;
    Light l;
    ParticleSystem ps;
    TriggerProjector triggerProjector;
    Transform projectorTransform;

    [Header("Dropoff-Specific Properties")]
    public Transform target;
    public string promptText;
    bool dropoffDone = false;

    public bool StartedEnabled()
    {
        return triggerEnabled;
    }

    void Awake()
    {
        interactManager = FindObjectOfType<InteractManager>();
        triggerCollider = GetComponent<Collider>();
        l = transform.GetChild(0).gameObject.GetComponent<Light>();
        ps = transform.GetChild(1).gameObject.GetComponent<ParticleSystem>();
        triggerProjector = transform.GetChild(2).GetComponent<TriggerProjector>();

        if (triggerEnabled) Enable();
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

    public void Enable()
    {
        if (!dropoffDone)
        {
            // triggerEnabled = true;
            triggerCollider.enabled = true;
            l.enabled = true;
            ps.Play();
            triggerProjector.Enable();
        }
    }

    public void Disable()
    {
        if (!destroyed)
        {
            // triggerEnabled = false;
            triggerCollider.enabled = false;
            l.enabled = false;
            ps.Stop();
            triggerProjector.Disable();
        }
    }

    void OnDestroy()
    {
        destroyed = true;
    }

    public void Remove()
    {
        Destroy(this.gameObject);
    }

    // Called by interact manager to close out the trigger.
    public void CompleteDropoff()
    {
        dropoffDone = true;
    }

    private void OnTriggerEnter(Collider other)
    {
        // if (triggerEnabled && !dropoffDone && other.tag == "Player")
        if (!dropoffDone && other.tag == "Player")
        {
            interactManager.DropoffEnterRange(this, promptText);
        }
    }

    private void OnTriggerExit(Collider other)
    {
        // if (triggerEnabled && !dropoffDone && other.tag == "Player")
        if (!dropoffDone && other.tag == "Player")
        {
            interactManager.DropoffExitRange(this);
        }
    }

}
