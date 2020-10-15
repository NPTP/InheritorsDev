using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PickupManager : MonoBehaviour
{
    PlayerPickup playerPickup;

    void Start()
    {
        playerPickup = GameObject.Find("Player").GetComponent<PlayerPickup>();
    }

    void Update()
    {
        // Do nothing
    }

    void PickUpEvent()
    {

    }
}
