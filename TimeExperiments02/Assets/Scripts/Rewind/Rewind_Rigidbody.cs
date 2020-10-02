using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

public class Rewind_Rigidbody : MonoBehaviour
{
    private RewindManagerScript rewindManagerScript;
    private List<Vector3> positionRecord;
    private List<Quaternion> rotationRecord;
    private List<Vector3> velocityRecord;
    private List<Vector3> angularVelocityRecord;
    private int lastIndex = 0;
    private Rigidbody rb;

    private Vector3 savedVelocity;
    private Vector3 savedAngularVelocity;
    private bool useSaved = false;

    void Start()
    {
        GameObject rewindManager = GameObject.Find("RewindManager");
        rewindManagerScript = rewindManager.GetComponent<RewindManagerScript>();
        positionRecord = new List<Vector3>();
        rotationRecord = new List<Quaternion>();
        velocityRecord = new List<Vector3>();
        angularVelocityRecord = new List<Vector3>();

        rb = GetComponent<Rigidbody>();
    }

    void Update()
    {
        if (rewindManagerScript.rewinding && positionRecord.Count >= 1)
            Rewind();
        else
            Record();
    }

    void Rewind()
    {
        rb.isKinematic = true;
        lastIndex = positionRecord.Count - 1;

        // Apply position, rotation, velocity.
        // transform.position = positionRecord[lastIndex];
        // transform.rotation = rotationRecord[lastIndex];
        rb.position = positionRecord[lastIndex];
        rb.rotation = rotationRecord[lastIndex];
        savedVelocity = velocityRecord[lastIndex];
        savedAngularVelocity = angularVelocityRecord[lastIndex];
        useSaved = true;

        // Remove the frame information from the record.
        positionRecord.RemoveAt(lastIndex);
        rotationRecord.RemoveAt(lastIndex);
        velocityRecord.RemoveAt(lastIndex);
        angularVelocityRecord.RemoveAt(lastIndex);
    }

    void Record()
    {
        if (useSaved)
        {
            Debug.Log(savedVelocity);
            rb.velocity = savedVelocity;
            rb.angularVelocity = savedAngularVelocity;
            useSaved = false;
        }

        rb.isKinematic = false;
        // positionRecord.Add(transform.position);
        // rotationRecord.Add(transform.rotation);
        positionRecord.Add(rb.position);
        rotationRecord.Add(rb.rotation);
        velocityRecord.Add(rb.velocity);
        angularVelocityRecord.Add(rb.angularVelocity);
    }


}
