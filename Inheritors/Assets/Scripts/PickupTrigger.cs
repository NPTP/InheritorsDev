using System;
using UnityEngine;

// Trigger to be placed around any pickup, sends events to InteractManager.
public class PickupTrigger : MonoBehaviour
{
    public event EventHandler OnPickupEnterRange;
    public event EventHandler OnPickupLeaveRange;

    SphereCollider sphereCollider;
    Light l;
    ParticleSystem ps;

    void Start()
    {
        sphereCollider = GetComponent<SphereCollider>();
        l = transform.GetChild(1).gameObject.GetComponent<Light>();
        ps = transform.GetChild(2).gameObject.GetComponent<ParticleSystem>();
    }

    public void GetPickedUp()
    {
        sphereCollider.enabled = false;
        l.enabled = false;
        ps.Stop();
    }

    public void GetPutDown()
    {
        sphereCollider.enabled = true;
        l.enabled = true;
        ps.Play();
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.tag == "Player")
        {
            OnPickupEnterRange?.Invoke(this, EventArgs.Empty);
        }
    }

    private void OnTriggerExit(Collider other)
    {
        if (other.tag == "Player")
        {
            OnPickupLeaveRange?.Invoke(this, EventArgs.Empty);
        }
    }
}
