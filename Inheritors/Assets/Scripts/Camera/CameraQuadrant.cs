﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraQuadrant : MonoBehaviour
{
    CameraManager cameraManager;

    void Start()
    {
        cameraManager = FindObjectOfType<CameraManager>();
    }

    private void OnTriggerEnter(Collider other)
    {
        cameraManager.SendCamTo(this.transform);
    }

    private void OnTriggerExit(Collider other)
    {
        cameraManager.SwitchToPlayerCam();
    }
}