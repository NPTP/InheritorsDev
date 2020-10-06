using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AffectedTiles : MonoBehaviour
{
    // Start is called before the first frame update
    void Start()
    {

    }

    // Update is called once per frame
    void Update()
    {

    }

    void OnCollisionEnter(Collision other)
    {
        if (other.collider.tag == "Player")
        {
            Debug.Log("collided with player");
            this.gameObject.GetComponent<Material>().color = Color.green;
        }
    }
}
