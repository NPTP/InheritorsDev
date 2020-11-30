using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Cinemachine;

public class CameraQuadrant : MonoBehaviour
{
    CameraManager cameraManager;
    CinemachineVirtualCamera cmVCam = null;
    bool hasOwnCam = false;

    CameraQuadrantActions cameraQuadrantActions;
    bool hasActions = false;


    void Start()
    {
        cameraManager = FindObjectOfType<CameraManager>();
        cameraQuadrantActions = GetComponent<CameraQuadrantActions>();
        if (cameraQuadrantActions != null) hasActions = true;

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

        if (hasActions)
            cameraQuadrantActions.StartAction();
    }

    private void OnTriggerExit(Collider other)
    {
        cameraManager.SwitchToCam("Player");

        if (hasActions)
            cameraQuadrantActions.StopAction();
    }
}
