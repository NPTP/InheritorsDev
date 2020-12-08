using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Animate : MonoBehaviour
{
    // Start is called before the first frame update
    void Start()
    {
        GetComponent<Animation>().Play();
    }


}
