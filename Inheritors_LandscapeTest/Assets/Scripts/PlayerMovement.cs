using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerMovement : MonoBehaviour
{
    [SerializeField]
    float moveSpeed = 4f;
    public GameObject playerGhost;

    private Vector3 forward, right;
    private CharacterController characterController;
    private List<Vector3> moveVectors;
    private List<Vector3> headings;
    private Vector3 initialPosition;
    private Quaternion initialRotation;
    private RecordManagerScript recordManagerScript;
    private bool justFinishedRecording = false;
    private bool initialFrame = true;

    void Start()
    {
        forward = Camera.main.transform.forward;
        forward.y = 0;
        forward = Vector3.Normalize(forward);
        right = Quaternion.Euler(new Vector3(0, 90, 0)) * forward;
        initialPosition = Vector3.zero;
        initialRotation = Quaternion.identity;

        characterController = GetComponent<CharacterController>();

        GameObject recordManager = GameObject.Find("RecordManager");
        recordManagerScript = recordManager.GetComponent<RecordManagerScript>();
        moveVectors = new List<Vector3>();
        headings = new List<Vector3>();
    }

    void Update()
    {
        Move();
    }

    void Move()
    {
        Vector3 moveRight = right * moveSpeed * Time.deltaTime * Input.GetAxis("Horizontal");
        Vector3 moveForward = forward * moveSpeed * Time.deltaTime * Input.GetAxis("Vertical");
        Vector3 moveVector = moveRight + moveForward;
        Vector3 heading = Vector3.zero;

        if (moveVector != Vector3.zero)
        {
            heading = Vector3.Normalize(moveVector);
            transform.forward = heading;
        }
        moveVector.y += Physics.gravity.y * Time.deltaTime;
        characterController.Move(moveVector);

        if (recordManagerScript.recording)
        {
            RecordPlayer(moveVector, heading);
            justFinishedRecording = true;
        }
        else if (justFinishedRecording)
        {
            CreatePlaybackGhost();
            moveVectors = new List<Vector3>();
            justFinishedRecording = false;
            initialFrame = true;
        }
    }

    void RecordPlayer(Vector3 moveVector, Vector3 heading)
    {
        Vector3 moveVectorCopy = new Vector3(moveVector.x, moveVector.y, moveVector.z);
        moveVectors.Add(moveVector);
        headings.Add(heading);
        if (initialFrame)
        {
            initialPosition = transform.position;
            initialRotation = transform.rotation;
            initialFrame = false;
        }
    }

    void CreatePlaybackGhost()
    {
        Debug.Log("CreatePlaybackGhost called.");
        GameObject newGhost = Instantiate(playerGhost, initialPosition, initialRotation);
        newGhost.GetComponent<GhostMovement>().PassRecord(moveVectors, headings);
    }

}
