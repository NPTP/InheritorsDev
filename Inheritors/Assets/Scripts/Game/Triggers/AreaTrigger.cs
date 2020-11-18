using System;
using UnityEngine;

public class AreaTrigger : MonoBehaviour, Trigger
{
    InteractManager interactManager;

    public event EventHandler OnAreaEnter;
    public event EventHandler OnAreaLeave;
    public event EventHandler OnAreaActivate;

    public bool triggerEnabled = true;
    public string triggerTag;

    [HideInInspector]
    public bool taskHasBegun = false;

    Collider triggerCollider;

    void Awake()
    {
        interactManager = FindObjectOfType<InteractManager>();
    }

    void Start()
    {
        triggerCollider = GetComponent<Collider>();

        if (triggerEnabled) Enable();
        else Disable();
    }

    public string GetTag()
    {
        return triggerTag;
    }

    public void BeginTaskInArea()
    {
        taskHasBegun = true;
    }

    public bool HasTaskBegun()
    {
        return taskHasBegun;
    }

    public void Enable()
    {
        triggerCollider.enabled = true;
    }

    public void Disable()
    {
        triggerCollider.enabled = false;
    }

    public void Remove()
    {
        Destroy(this.gameObject);
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.tag == "Player")
        {
            interactManager.AreaEnter(this);
        }
    }

    private void OnTriggerExit(Collider other)
    {
        if (other.tag == "Player")
        {
            interactManager.AreaLeave(this);
        }
    }
}
