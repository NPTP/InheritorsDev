using System;
using System.Collections;
using UnityEngine;
using DG.Tweening;
using Cinemachine;

// Handle all camera events
public class CameraManager : MonoBehaviour
{
    CinemachineVirtualCamera cmPlayerCam;
    CinemachineVirtualCamera cmOtherCam;
    CinemachineVirtualCamera cmLookCam;
    StateManager stateManager;
    InputManager inputManager;

    void Start()
    {
        cmPlayerCam = GameObject.Find("CMPlayerCam").GetComponent<CinemachineVirtualCamera>();
        cmOtherCam = GameObject.Find("CMOtherCam").GetComponent<CinemachineVirtualCamera>();
        // TODO: implement the look cam with an invisible object (right mouse/right stick scroll map)
        // cmLookCam = GameObject.Find("CMLookCam").GetComponent<CinemachineVirtualCamera>();
    }

    public void UseLookCam()
    {
        if (stateManager.GetState() == StateManager.State.Normal)
        {
            // Do the look stuff:
            // - set state to "Looking" (zero axes, disallow other inputs)
            // - align lookcam to thingy
        }
    }

    public void SwitchToPlayerCam()
    {
        cmPlayerCam.Priority = 1;
        cmOtherCam.Priority = 0;
        // cmLookCam.Priority = 0;
    }

    public void SwitchToOtherCam()
    {
        cmPlayerCam.Priority = 0;
        cmOtherCam.Priority = 1;
        // cmLookCam.Priority = 0;
    }

    public void FocusOtherCamOn(Transform transform)
    {
        cmOtherCam.Follow = transform;
        cmOtherCam.LookAt = transform;
    }

    public void SendCamTo(Transform transform)
    {
        FocusOtherCamOn(transform);
        SwitchToOtherCam();
    }

}
