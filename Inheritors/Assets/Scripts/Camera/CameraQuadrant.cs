using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Cinemachine;

public class CameraQuadrant : MonoBehaviour
{
    CameraManager cameraManager;
    CinemachineVirtualCamera cmVCam = null;
    bool hasOwnCam = false;

    void Start()
    {
        cameraManager = FindObjectOfType<CameraManager>();

        if (transform.childCount > 0)
            cmVCam = transform.GetChild(0).gameObject.GetComponent<CinemachineVirtualCamera>();
        if (cmVCam != null)
            hasOwnCam = true;
    }

    private void OnTriggerEnter(Collider other)
    {
        if (hasOwnCam)
            cameraManager.SwitchToCam(cmVCam.gameObject.name);
        else
            cameraManager.QuadrantCamActivate(this.transform);
    }

    private void OnTriggerExit(Collider other)
    {
        cameraManager.SwitchToCam("Player");
    }
}
