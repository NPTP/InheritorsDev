using System;
using UnityEngine;
using DG.Tweening;

// Trigger for walking into, works with InteractManager.
public class WalkTrigger : MonoBehaviour, Trigger
{
    InteractManager interactManager;

    public bool triggerEnabled = true;
    public bool invisibleTrigger = false;
    public string triggerTag;

    Transform playerTransform;
    Collider triggerCollider;
    Transform itemTransform;
    Vector3 itemLocalScale;
    BoxCollider itemCollider;
    Light l;
    float originalIntensity;
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
        originalIntensity = l.intensity;
        ps = transform.GetChild(1).gameObject.GetComponent<ParticleSystem>();

        if (triggerEnabled) Enable();
        else Disable();

        if (invisibleTrigger)
        {
            ps.Stop();
            l.enabled = false;
        }
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
        if (!triggered)
        {
            triggerEnabled = true;
            triggerCollider.enabled = true;
            l.enabled = true;
            l.DOIntensity(originalIntensity, .25f).From(0f);
            ps.Play();
        }
    }

    // Disables collider and visual fx but keeps script alive so we can refer to it elsewhere.
    public void Disable()
    {
        triggerEnabled = false;
        triggerCollider.enabled = false;
        l.DOIntensity(0f, .25f);
        // l.enabled = false;
        ps.Stop();
    }

    // ONLY call this from another script once that script has finished with the trigger!
    public void Remove()
    {
        Destroy(this.gameObject);
    }

    private void OnTriggerEnter(Collider other)
    {
        if (triggerEnabled && !triggered && other.tag == "Player")
        {
            triggered = true;
            interactManager.WalkEnter(this);
        }
    }

    public void FlagInArea(AreaTrigger area) { }
}
