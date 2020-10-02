using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerRewind : MonoBehaviour
{
    private RewindManagerScript rewindManagerScript;
    private List<Transform> transformRecord;
    void Start()
    {
        GameObject rewindManager = GameObject.Find("RewindManager");
        rewindManagerScript = rewindManager.GetComponent<RewindManagerScript>();
    }

    // Update is called once per frame
    void Update()
    {
        if (rewindManagerScript.rewinding)
        {
            // If we're rewinding, moving back through the positions, while INPUT IS DISABLED
        }
        else
        {
            // If we ain't rewinding, we're RECORDING!
            // Create new vector3 POS with values of transform position x, y, z,
            // Create new vector3 ROT with values of transform rotation x, y, z
            // Create a new tuple (POS, ROT)
            // Put (POS, ROT) into List
        }
    }
}
