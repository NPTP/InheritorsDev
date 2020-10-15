using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class PickupUI : MonoBehaviour
{
    PlayerPickup playerPickup;
    RawImage rawImage;

    void Start()
    {
        playerPickup = GameObject.Find("Player").GetComponent<PlayerPickup>();
        rawImage = GetComponent<RawImage>();
    }

    // Update is called once per frame
    void Update()
    {
        if (playerPickup.inPickupRange)
            rawImage.enabled = true;
        else
            rawImage.enabled = false;
    }
}
