using System.Collections;
using System.Collections.Generic;
using UnityEngine;

// Okay, this is gonna be hacky.
// We'll redo pickup systems later.
public class PickupTrigger : MonoBehaviour
{
    ParticleSystem ps;

    void Start()
    {
        ps = transform.GetChild(1).gameObject.GetComponent<ParticleSystem>();
    }

    public void GetPickedUp()
    {
        Debug.Log("Player picked me up.");
        ps.Stop();
        foreach (Transform child in transform)
            Destroy(child.gameObject);
        GameObject.Find("TaskManager").GetComponent<TaskManager>().CompleteTask(1);
        Destroy(gameObject);
    }
}
