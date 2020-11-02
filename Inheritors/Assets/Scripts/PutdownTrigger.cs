using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PutdownTrigger : MonoBehaviour
{
    ParticleSystem ps;
    Light l;

    void Start()
    {
        l = transform.GetChild(0).gameObject.GetComponent<Light>();
        l.enabled = false;
        ps = transform.GetChild(1).gameObject.GetComponent<ParticleSystem>();
        ps.Stop();
    }

    public void EnablePutdownTrigger()
    {
        l.enabled = true;
        ps.Play();
    }


}
