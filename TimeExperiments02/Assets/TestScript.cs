using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TestScript : MonoBehaviour
{
    private Vector3 vector;
    void Start()
    {

    }

    // Update is called once per frame
    void Update()
    {

    }

    public void Fly(Vector3 vec)
    {
        transform.position = vec;
    }
}
