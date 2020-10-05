using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerMovement : MonoBehaviour
{
    [SerializeField]
    float moveSpeed = 4f;
    [SerializeField]
    float jumpSpeed = 4f;

    private Vector3 forward, right;
    private CharacterController characterController;

    // Start is called before the first frame update
    void Start()
    {
        forward = Camera.main.transform.forward;
        forward.y = 0;
        forward = Vector3.Normalize(forward);
        right = Quaternion.Euler(new Vector3(0, 90, 0)) * forward;

        characterController = GetComponent<CharacterController>();
    }

    // Update is called once per frame
    void Update()
    {
        Move();
    }

    void Move()
    {
        Vector3 moveRight = right * moveSpeed * Time.deltaTime * Input.GetAxis("Horizontal");
        Vector3 moveForward = forward * moveSpeed * Time.deltaTime * Input.GetAxis("Vertical");
        Vector3 moveVector = moveRight + moveForward;

        if (moveVector != Vector3.zero)
        {
            Vector3 heading = Vector3.Normalize(moveVector);
            transform.forward = heading;
        }

        moveVector.y += Physics.gravity.y * Time.deltaTime;

        characterController.Move(moveVector);
    }
}
