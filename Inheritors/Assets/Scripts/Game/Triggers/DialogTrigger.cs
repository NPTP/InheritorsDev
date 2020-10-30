using System;
using UnityEngine;

// Trigger to be placed around any pickup, sends events to InteractManager.
public class DialogTrigger : MonoBehaviour
{
    public event EventHandler OnDialogEnterRange;
    public event EventHandler OnDialogLeaveRange;

    Transform playerTransform;
    SphereCollider sphereCollider;

    void Start()
    {
        playerTransform = GameObject.Find("Player").transform;
        sphereCollider = GetComponent<SphereCollider>();
    }

    public void StartDialog()
    {
        sphereCollider.enabled = false;
    }

    public void EndDialog()
    {
        sphereCollider.enabled = true;
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
