using System;
using UnityEngine;

public class DialogTrigger : MonoBehaviour, Trigger
{
    InteractManager interactManager;

    public event EventHandler OnDialogEnterRange;
    public event EventHandler OnDialogLeaveRange;
    public event EventHandler OnTriggerActivate;

    public bool triggerEnabled = true;
    public string triggerTag;

    Collider triggerCollider;

    void Awake()
    {
        interactManager = FindObjectOfType<InteractManager>();
    }

    void Start()
    {
        triggerCollider = GetComponent<Collider>();
    }

    public string GetTag()
    {
        return triggerTag;
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
            OnDialogEnterRange?.Invoke(this, EventArgs.Empty);
        }
    }

    private void OnTriggerExit(Collider other)
    {
        if (other.tag == "Player")
        {
            OnDialogLeaveRange?.Invoke(this, EventArgs.Empty);
        }
    }
}
