using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PickupItem : MonoBehaviour
{
    Rigidbody rb;
    ParticleSystem ps;

    void Start()
    {
        rb = GetComponent<Rigidbody>();
        ps = GetComponent<ParticleSystem>();
    }

    public void GetPickedUp()
    {
        rb.isKinematic = true;
        ps.Stop();
        transform.GetChild(0).gameObject.SetActive(false);
        transform.GetChild(1).gameObject.SetActive(false);
    }

    public void GetPutDown(Vector3 dropDirection)
    {
        rb.isKinematic = false;
        ps.Play();
        transform.GetChild(0).gameObject.SetActive(true);
        transform.GetChild(1).gameObject.SetActive(true);
    }

    public void GetUsedCorrectly()
    {
        // Spitballing the next functionality here.
    }
}
