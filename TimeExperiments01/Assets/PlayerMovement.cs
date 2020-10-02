using System.Collections;
using UnityEngine;

public class PlayerMovement : MonoBehaviour
{
    private CharacterController characterController;
    public float speed = 6.0F;
    public float jumpSpeed = 8.0F;
    public float gravity = 20.0F;
    private Vector3 moveDirection = Vector3.zero;

    private RewindManagerScript rewindManagerScript;
    void Start()
    {
        GameObject rewindManager = GameObject.Find("RewindManager");
        rewindManagerScript = rewindManager.GetComponent<RewindManagerScript>();

        characterController = GetComponent<CharacterController>();
    }

    void Update()
    {
        if (!rewindManagerScript.rewinding) // Only take inputs and use gravity if we are NOT currently rewinding.
        {
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
    }
}
