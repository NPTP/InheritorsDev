/* INHERITORS by Nick Perrin (c) 2020 */
using System;
using System.Collections;
using UnityEngine;
using DG.Tweening;
using Cinemachine;

// Handle all camera events
public class CameraManager : MonoBehaviour
{
    StateManager stateManager;
    InputManager inputManager;

    CinemachineVirtualCamera cmPlayerCam;
    CinemachineVirtualCamera cmQuadrantCam; // "QuadrantCam"
    CinemachineVirtualCamera cmOtherCam;
    CinemachineVirtualCamera cmLookCam;

    CinemachineVirtualCamera presentCam;
    CinemachineVirtualCamera lastCam;

    float defaultOrthoSize = 10f;
    float defaultFov = 40f;

    void Awake()
    {
        cmPlayerCam = GameObject.Find("CMPlayerCam").GetComponent<CinemachineVirtualCamera>();
        cmQuadrantCam = GameObject.Find("CMQuadrantCam").GetComponent<CinemachineVirtualCamera>();
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
        lastCam = presentCam;
        presentCam = cmOtherCam;
        cmPlayerCam.Priority = 0;
        cmOtherCam.Priority = 1;
        // cmLookCam.Priority = 0;
    }

    public void SwitchToCam(string camName)
    {
        lastCam = presentCam;
        if (camName == "QuadrantCam")
        {
            presentCam = cmQuadrantCam;
            cmQuadrantCam.Priority = 1;
            cmPlayerCam.Priority = 0;
            cmOtherCam.Priority = 0;
            // cmLookCam.Priority = 0;
        }
    }

    public void SwitchToLastCam()
    {
        cmQuadrantCam.Priority = 0;
        cmPlayerCam.Priority = 0;
        cmOtherCam.Priority = 0;
        // cmLookCam.Priority = 0;

        CinemachineVirtualCamera temp = presentCam;
        presentCam = lastCam;
        lastCam = temp;
        presentCam.Priority = 1;
    }

    public void FocusOtherCamOn(Transform transform)
    {
        cmOtherCam.Follow = transform;
        cmOtherCam.LookAt = transform;
    }

    public void FocusCamOn(CinemachineVirtualCamera cam, Transform transform)
    {
        cam.Follow = transform;
        cam.LookAt = transform;
    }

    public void SendCamTo(Transform transform)
    {
        FocusOtherCamOn(transform);
        SwitchToOtherCam();
    }

    public void QuadrantCamActivate(Transform transform)
    {
        FocusCamOn(cmQuadrantCam, transform);
        SwitchToCam("QuadrantCam");
    }

    public void ZoomToSizeOrtho(float orthographicSize, float duration = 0f)
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

    public void ResetSizeOrtho(float duration = 0f)
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

    public void Zoom(float fov, float duration = 0f)
    {
        DOTween.To(
            () => cmOtherCam.m_Lens.FieldOfView,
            x => cmOtherCam.m_Lens.FieldOfView = x,
            fov,
            duration
        );
        DOTween.To(
            () => cmPlayerCam.m_Lens.FieldOfView,
            x => cmPlayerCam.m_Lens.FieldOfView = x,
            fov,
            duration
        );
    }

    public void ResetZoom(float duration = 0f)
    {
        DOTween.To(
              () => cmOtherCam.m_Lens.FieldOfView,
              x => cmOtherCam.m_Lens.FieldOfView = x,
              defaultFov,
              duration
          );
        DOTween.To(
            () => cmPlayerCam.m_Lens.FieldOfView,
            x => cmPlayerCam.m_Lens.FieldOfView = x,
            defaultFov,
            duration
        );
    }

}
