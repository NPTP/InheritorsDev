using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

public class PlayerMovementRewind : MonoBehaviour
{

    private RewindManagerScript rewindManagerScript;
    private List<Tuple<Vector3, Vector3, Vector3>> posRotMoveRecord;
    private CharacterController characterController;
    public float speed;
    public float jumpSpeed;
    public float gravity;
    private Vector3 moveDirection = Vector3.zero;

    public float shockwavePower;
    public float shockwaveRadius;
    public float shockwaveUpwardsForce;

    void Start()
    {
        GameObject rewindManager = GameObject.Find("RewindManager");
        rewindManagerScript = rewindManager.GetComponent<RewindManagerScript>();
        posRotMoveRecord = new List<Tuple<Vector3, Vector3, Vector3>>();

        characterController = GetComponent<CharacterController>();
    }

    void Update()
    {
        if (!rewindManagerScript.rewinding || posRotMoveRecord.Count < 1) // Only take inputs and use gravity if we are NOT currently rewinding.
        {
            TakeInput();
            Record();
        }
        else
        {
            Rewind();
        }
    }

    void TakeInput()
    {
        if (Input.GetKeyDown(KeyCode.Return))
        {
            Shockwave();
        }

        // is the characterController on the ground?
        if (characterController.isGrounded)
        {
            // CharacterController characterController = GetComponent<CharacterController>();
            //Feed moveDirection with input.
            moveDirection = new Vector3(Input.GetAxis("Horizontal"), 0, Input.GetAxis("Vertical"));
            moveDirection = transform.TransformDirection(moveDirection);
            //Multiply it by speed.
            moveDirection *= speed;
            //Jumping
            if (Input.GetButton("Jump"))
                moveDirection.y = jumpSpeed;

        }

        //Applying gravity to the characterController
        moveDirection.y -= gravity * Time.deltaTime;
        //Making the character move
        characterController.Move(moveDirection * Time.deltaTime);

    }

    // Record position, rotation and moveDirection of the player.
    void Record()
    {
        Tuple<Vector3, Vector3, Vector3> posRotMovedir = new Tuple<Vector3, Vector3, Vector3>(
            transform.position, transform.rotation.eulerAngles, moveDirection
        );
        posRotMoveRecord.Add(posRotMovedir);
    }

    void Rewind()
    {
        // Retrieve position, rotation, moveDirection.
        int lastIndex = posRotMoveRecord.Count - 1;
        Tuple<Vector3, Vector3, Vector3> recordTuple = posRotMoveRecord[lastIndex];
        Vector3 oldPosition = recordTuple.Item1;
        Vector3 oldRotation = recordTuple.Item2;
        Vector3 oldMoveDirection = recordTuple.Item3;

        // Apply position, rotation, moveDirection.
        transform.position = oldPosition;
        transform.rotation = Quaternion.Euler(oldRotation.x, oldRotation.y, oldRotation.z); // World. Use 'localRotation' for local
        moveDirection = oldMoveDirection;

        // Remove the frame information from the record.
        posRotMoveRecord.RemoveAt(lastIndex);
    }

    void Shockwave()
    {
        Vector3 explosionPosition = transform.position;
        Collider[] colliders = Physics.OverlapSphere(explosionPosition, shockwaveRadius);
        foreach (Collider hit in colliders)
        {
            Rigidbody rb = hit.GetComponent<Rigidbody>();
            if (rb != null)
            {
                rb.AddExplosionForce(shockwavePower, explosionPosition, shockwaveRadius, shockwaveUpwardsForce, ForceMode.Impulse);
            }
        }
    }

}


