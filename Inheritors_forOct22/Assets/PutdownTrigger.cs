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

    public void GetPutdown()
    {
        Debug.Log("Player put item down into me.");
        ps.Stop();
        foreach (Transform child in transform)
            Destroy(child.gameObject);
        GameObject.Find("TaskManager").GetComponent<TaskManager>().CompleteTask(2);
        GameObject.Find("TaskManager").GetComponent<TaskManager>().CompleteTask(3);
        Destroy(gameObject);
    }
}
