using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TallGrass : MonoBehaviour
{
    public AudioSource audioSource;

    void Start()
    {
        audioSource = GetComponent<AudioSource>();
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.tag == "Player")
        {
            // transform.localScale = new Vector3(transform.localScale.x, transform.localScale.y * .15f, transform.localScale.z);
            audioSource.Play();
            Destroy(this.gameObject);
        }
    }
}
