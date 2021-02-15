using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SteveTempScript : MonoBehaviour
{
    Animator animator;
    
    void Start()
    {
        animator = GetComponent<Animator>();    
    }

    void Update()
    {
        if (Input.GetKeyDown(KeyCode.Space))
        {
            animator.SetTrigger("Wave");
        }
    }
}
