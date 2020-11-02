using System;
using UnityEngine;
using DG.Tweening;

// Trigger to be placed around any pickup, sends events to InteractManager.
public class DropTrigger : MonoBehaviour, Trigger
{
    public event EventHandler OnTriggerActivate;
    // public event EventHandler OnPickupEnterRange;
    // public event EventHandler OnPickupLeaveRange;

    public bool triggerEnabled = true;
    public string triggerTag;

    // Transform playerTransform;
    // SphereCollider sphereCollider;
    // Transform itemTransform;
    // Vector3 itemLocalScale;
    // BoxCollider itemCollider;
    // Light l;
    // ParticleSystem ps;

    // void Start()
    // {
    //     playerTransform = GameObject.Find("Player").transform;
    //     sphereCollider = GetComponent<SphereCollider>();
    //     itemTransform = transform.GetChild(0);
    //     itemLocalScale = itemTransform.localScale;
    //     itemCollider = itemTransform.gameObject.GetComponent<BoxCollider>();
    //     l = transform.GetChild(1).gameObject.GetComponent<Light>();
    //     ps = transform.GetChild(2).gameObject.GetComponent<ParticleSystem>();
    // }

    public string GetTag()
    {
        return triggerTag;
    }

    public void Enable()
    {

    }

    public void Disable()
    {

    }

    public void Remove()
    {
        Destroy(this.gameObject);
    }

    // public void GetPickedUp()
    // {
    //     itemTransform.DOScale(itemLocalScale, .25f);
    //     sphereCollider.enabled = false;
    //     itemCollider.enabled = false;
    //     l.enabled = false;
    //     ps.Stop();
    // }

    // public void GetPutDown()
    // {
    //     transform.DOMoveY(playerTransform.position.y, .25f); // TODO: this will cause problems on height changes. We'll need collision later.
    //     sphereCollider.enabled = true;
    //     itemCollider.enabled = true;
    //     l.enabled = true;
    //     ps.Play();
    // }

    // private void OnTriggerEnter(Collider other)
    // {
    //     if (other.tag == "Player")
    //     {
    //         OnPickupEnterRange?.Invoke(this, EventArgs.Empty);
    //         itemTransform.DOScale(itemLocalScale * 1.15f, .25f);
    //     }
    // }

    // private void OnTriggerExit(Collider other)
    // {
    //     if (other.tag == "Player")
    //     {
    //         OnPickupLeaveRange?.Invoke(this, EventArgs.Empty);
    //         itemTransform.DOScale(itemLocalScale, .25f);
    //     }
    // }
}
