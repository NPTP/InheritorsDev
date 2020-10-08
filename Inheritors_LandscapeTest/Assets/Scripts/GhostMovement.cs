using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GhostMovement : MonoBehaviour
{
    private CharacterController characterController;
    private List<Vector3> moveVectors;
    private List<Vector3> headings;

    void Start()
    {
        characterController = GetComponent<CharacterController>();
    }

    void Update()
    {
        if (moveVectors.Count > 0 && headings.Count > 0)
            Move();
    }

    void Move()
    {
        transform.forward = headings[0];
        characterController.Move(moveVectors[0]);
        headings.RemoveAt(0);
        moveVectors.RemoveAt(0);
    }

    public void PassRecord(List<Vector3> mvecs, List<Vector3> hds)
    {
        moveVectors = mvecs;
        headings = hds;
        transform.forward = headings[0];
    }

}
