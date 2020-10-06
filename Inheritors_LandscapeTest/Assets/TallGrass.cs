using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TallGrass : MonoBehaviour
{
    private bool steppedOn = false;
    public AudioSource audioSource;
    // Start is called before the first frame update
    void Start()
    {

    }

    // Update is called once per frame
    void Update()
    {

    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.tag == "Player" && !steppedOn)
        {
            Vector3 scaler = new Vector3(1f, 0.3f, 1f);
            Vector3 scaledGrass = Vector3.Scale(transform.localScale, scaler);
            transform.localScale = Vector3.Slerp(transform.localScale, scaledGrass, 1f);
            audioSource.Play();
            steppedOn = true;
        }
    }

    private void OnTriggerExit(Collider other)
    {
        // Do nothing, G
    }
}
