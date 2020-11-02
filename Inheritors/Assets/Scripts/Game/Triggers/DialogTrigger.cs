using System;
using UnityEngine;

public class DialogTrigger : MonoBehaviour, Trigger
{
    public event EventHandler OnDialogEnterRange;
    public event EventHandler OnDialogLeaveRange;
    public event EventHandler OnTriggerActivate;

    public bool triggerEnabled = true;
    public string triggerTag;

    Transform playerTransform;
    SphereCollider sphereCollider;

    void Start()
    {
        playerTransform = GameObject.Find("Player").transform;
        sphereCollider = GetComponent<SphereCollider>();
    }

    public string GetTag()
    {
        return triggerTag;
    }

    public void Enable()
    {
        sphereCollider.enabled = true;
    }

    public void Disable()
    {
        sphereCollider.enabled = false;
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
