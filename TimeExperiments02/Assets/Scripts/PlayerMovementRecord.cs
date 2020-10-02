using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

public class PlayerMovementRecord : MonoBehaviour
{
    public GameObject playerGhost;
    public GameObject testObject;
    private RecordManagerScript recordManagerScript;
    private bool justFinishedRecording = false;
    private List<Tuple<Vector3, Quaternion, Vector3, bool>> posRotMoveShockRecord;
    private CharacterController characterController;
    public float speed;
    public float jumpSpeed;
    public float gravity;
    private Vector3 moveDirection = Vector3.zero;

    public float shockwavePower;
    public float shockwaveRadius;
    public float shockwaveUpwardsForce;
    private bool shockwaveThisFrame = false;

    private int ghostCount = 0;
    public int maxGhosts = 5;

    void Start()
    {
        GameObject recordManager = GameObject.Find("RecordManager");
        recordManagerScript = recordManager.GetComponent<RecordManagerScript>();
        posRotMoveShockRecord = new List<Tuple<Vector3, Quaternion, Vector3, bool>>();

        characterController = GetComponent<CharacterController>();
    }

    void Update()
    {
        TakeInput();

        if (recordManagerScript.recording)
        {
            Record();
            justFinishedRecording = true;
        }
        else if (justFinishedRecording)
        {
            if (ghostCount < maxGhosts)
                CreatePlaybackGhost();
            posRotMoveShockRecord = new List<Tuple<Vector3, Quaternion, Vector3, bool>>();
            justFinishedRecording = false;
        }
    }

    void TakeInput()
    {
        if (Input.GetKeyDown(KeyCode.Return))
        {
            Shockwave();
            shockwaveThisFrame = true;
        }
        else
        {
            shockwaveThisFrame = false;
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
        Tuple<Vector3, Quaternion, Vector3, bool> posRotMoveShock = new Tuple<Vector3, Quaternion, Vector3, bool>(
            transform.position, transform.rotation, moveDirection, shockwaveThisFrame
        );
        posRotMoveShockRecord.Add(posRotMoveShock);
    }

    void CreatePlaybackGhost()
    {
        Vector3 firstRecordedPosition = posRotMoveShockRecord[0].Item1;
        Quaternion firstRecordedRotation = posRotMoveShockRecord[0].Item2;
        GameObject newGhost = Instantiate(playerGhost, firstRecordedPosition, transform.rotation);
        newGhost.GetComponent<GhostActions>().PassRecord(posRotMoveShockRecord);
        ghostCount++;

        // Create random spheres everywhere
        // GameObject testObj = Instantiate(testObject, transform.position, transform.rotation);
        // testObj.GetComponent<TestScript>().Fly(new Vector3(UnityEngine.Random.Range(-10.0f, 10.0f), UnityEngine.Random.Range(0.0f, 10.0f), UnityEngine.Random.Range(-10.0f, 10.0f)));
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


