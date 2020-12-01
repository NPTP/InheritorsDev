using System;
using System.Collections;
using UnityEngine;
using DG.Tweening;

// Trigger for walking into, works with InteractManager.
public class WalkTrigger : MonoBehaviour, Trigger
{
    InteractManager interactManager;

    public bool triggerEnabled = true;
    public string triggerTag;

    [Header("Walk-specific Options")]
    public bool invisibleTrigger = false;
    public bool haltPlayerOnActivate = true;

    Transform playerTransform;
    Collider triggerCollider;
    Transform itemTransform;
    Vector3 itemLocalScale;
    BoxCollider itemCollider;
    Light l;
    float originalIntensity;
    ParticleSystem ps;
    TriggerProjector triggerProjector;
    ParticleSystem activateParticles;

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
        activateParticles = transform.GetChild(2).gameObject.GetComponent<ParticleSystem>();
        triggerProjector = transform.GetChild(3).GetComponent<TriggerProjector>();

        if (triggerEnabled) Enable();
        else Disable();

        if (invisibleTrigger)
        {
            ps.Stop();
            l.enabled = false;
            triggerProjector.Disable();
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
            triggerProjector.Enable();
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
        triggerProjector.Disable();
    }

    public void Activate()
    {
        if (haltPlayerOnActivate)
            StartCoroutine(HaltPlayerProcess());

        if (!invisibleTrigger)
            StartCoroutine(ActivateAnimation());
        else
            Remove();
    }

    IEnumerator HaltPlayerProcess()
    {
        yield return new WaitForSeconds(0.25f);
        FindObjectOfType<PlayerMovement>().Halt();
    }

    IEnumerator ActivateAnimation()
    {
        if (!invisibleTrigger)
        {
            triggerEnabled = false;
            triggerCollider.enabled = false;
            ps.Stop();
            triggerProjector.Disable();
            activateParticles.Play();
            Tween t = l.DOIntensity(3 * originalIntensity, .25f).SetEase(Ease.OutQuad);
            yield return t.WaitForCompletion();
            t = l.DOIntensity(0f, 0.5f).SetEase(Ease.InQuad);
            yield return t.WaitForCompletion();
        }

        // Delay, then kill
        yield return new WaitForSeconds(2);
        Remove();
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
