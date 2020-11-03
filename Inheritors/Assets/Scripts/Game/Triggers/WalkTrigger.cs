using System;
using UnityEngine;

// Trigger to be placed around any pickup, sends events to InteractManager.
// TODO: make the trigger class support any kind of trigger. Walk triggers
// should support boxes AND spheres and whatever else.
public class WalkTrigger : MonoBehaviour, Trigger
{
    InteractManager interactManager;

    public event EventHandler OnTriggerActivate;

    public bool triggerEnabled = true;
    public bool invisibleTrigger = false;
    public string triggerTag;

    Transform playerTransform;
    Collider triggerCollider;
    Transform itemTransform;
    Vector3 itemLocalScale;
    BoxCollider itemCollider;
    Light l;
    ParticleSystem ps;

    bool triggered = false;

    void Awake()
    {
        interactManager = FindObjectOfType<InteractManager>();
    }

    public void Start()
    {
        playerTransform = GameObject.Find("Player").transform;
        triggerCollider = GetComponent<Collider>();
        itemTransform = transform.GetChild(0);
        itemLocalScale = itemTransform.localScale;
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
        if (!triggered)
        {
            triggerEnabled = true;
            triggerCollider.enabled = true;
            l.enabled = true;
            ps.Play();
        }
    }

    // Disables collider and visual fx but keeps script alive so we can refer to it elsewhere.
    public void Disable()
    {
        triggerEnabled = false;
        triggerCollider.enabled = false;
        l.enabled = false;
        ps.Stop();
    }

    // ONLY call this from another script once that script has finished with the trigger!
    public void Remove()
    {
        Destroy(this.gameObject);
    }

    private void OnTriggerEnter(Collider other)
    {
        if (enabled && !triggered && other.tag == "Player")
        {
            triggered = true;
            OnTriggerActivate?.Invoke(this, EventArgs.Empty);
            Disable();
        }
    }
}
