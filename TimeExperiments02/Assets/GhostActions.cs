using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

public class GhostActions : MonoBehaviour
{
    private List<Tuple<Vector3, Quaternion, Vector3, bool>> posRotMoveShockRecord;

    void Update()
    {
        if (posRotMoveShockRecord.Count > 0)
        {
            Playback();
        }
        else
        {
            // Destroy(this.gameObject);
        }
    }

    public void PassRecord(List<Tuple<Vector3, Quaternion, Vector3, bool>> record)
    {
        posRotMoveShockRecord = record;
    }

    public void Playback()
    {
        Tuple<Vector3, Quaternion, Vector3, bool> recordTuple = posRotMoveShockRecord[0];
        Vector3 oldPosition = recordTuple.Item1;
        Quaternion oldRotation = recordTuple.Item2;
        Vector3 oldMoveDirection = recordTuple.Item3;
        bool shockwaveAtFrame = recordTuple.Item4;

        transform.position = oldPosition;
        transform.rotation = oldRotation;
        if (shockwaveAtFrame)
            Shockwave();

        posRotMoveShockRecord.RemoveAt(0);
    }

    // TODO: Move shared actions like Shockwave() to a 'PlayerActions' type of script.
    // We would then take an argument like Shockwave(Vector3 worldPosition).
    void Shockwave()
    {
        Vector3 explosionPosition = transform.position;
        Collider[] colliders = Physics.OverlapSphere(explosionPosition, 10f);
        foreach (Collider hit in colliders)
        {
            Rigidbody rb = hit.GetComponent<Rigidbody>();
            if (rb != null)
            {
                rb.AddExplosionForce(5f, explosionPosition, 10f, 5f, ForceMode.Impulse);
            }
        }
    }
}
