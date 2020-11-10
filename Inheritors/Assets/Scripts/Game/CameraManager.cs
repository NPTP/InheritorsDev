/* INHERITORS by Nick Perrin (c) 2020 */
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
    CinemachineVirtualCamera presentCam;
    StateManager stateManager;
    InputManager inputManager;

    float defaultOrthoSize = 10f;

    void Awake()
    {
        cmPlayerCam = GameObject.Find("CMPlayerCam").GetComponent<CinemachineVirtualCamera>();
        cmOtherCam = GameObject.Find("CMOtherCam").GetComponent<CinemachineVirtualCamera>();
        // TODO: implement the look cam with an invisible object (right mouse/right stick scroll map)
        // cmLookCam = GameObject.Find("CMLookCam").GetComponent<CinemachineVirtualCamera>();
    }

    void Start()
    {
        presentCam = cmPlayerCam;
    }

    public void UseLookCam()
    {
        if (stateManager.GetState() == StateManager.State.Normal)
        {
            // Do the look stuff:
            // presentCam = cmOtherCam;
            // - set state to "Looking" (zero axes, disallow other inputs)
            // - align lookcam to thingy
        }
    }

    public void SwitchToPlayerCam()
    {
        presentCam = cmPlayerCam;
        cmPlayerCam.Priority = 1;
        cmOtherCam.Priority = 0;
        // cmLookCam.Priority = 0;
    }

    public void SwitchToOtherCam()
    {
        presentCam = cmOtherCam;
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

    public void ZoomToSize(float orthographicSize, float duration = 0f)
    {
        DOTween.To(
            () => cmOtherCam.m_Lens.OrthographicSize,
            x => cmOtherCam.m_Lens.OrthographicSize = x,
            orthographicSize,
            duration
        );
        DOTween.To(
            () => cmPlayerCam.m_Lens.OrthographicSize,
            x => cmPlayerCam.m_Lens.OrthographicSize = x,
            orthographicSize,
            duration
        );
    }

    public void ResetSize(float duration = 0f)
    {
        DOTween.To(
            () => cmOtherCam.m_Lens.OrthographicSize,
            x => cmOtherCam.m_Lens.OrthographicSize = x,
            defaultOrthoSize,
            duration
        );
        DOTween.To(
            () => cmPlayerCam.m_Lens.OrthographicSize,
            x => cmPlayerCam.m_Lens.OrthographicSize = x,
            defaultOrthoSize,
            duration
        );
    }

}
