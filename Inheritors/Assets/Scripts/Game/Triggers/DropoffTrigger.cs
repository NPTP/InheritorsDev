using System;
using UnityEngine;
using DG.Tweening;

// Trigger to be placed around any dropoff point, coupled with InteractManager.
public class DropoffTrigger : MonoBehaviour, Trigger
{
    InteractManager interactManager;

    public bool triggerEnabled = true;
    public string triggerTag;

    [Header("Dropoff-Specific Properties")]
    public Transform target;
    public string promptText;
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

    public Vector3 GetPosition()
    {
        return transform.position;
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
    }

    private void OnTriggerEnter(Collider other)
    {
        if (triggerEnabled && !dropoffDone && other.tag == "Player")
        {
            interactManager.DropoffEnterRange(this, promptText);
        }
    }

    private void OnTriggerExit(Collider other)
    {
        if (triggerEnabled && !dropoffDone && other.tag == "Player")
        {
            interactManager.DropoffExitRange(this);
        }
    }

    public void FlagInArea(AreaTrigger area) { }
}
