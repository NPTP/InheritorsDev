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

    CinemachineBrain cmBrain;

    CinemachineVirtualCamera cmPlayerCam;
    CinemachineVirtualCamera cmDialogCam;
    CinemachineVirtualCamera cmQuadrantCam;
    CinemachineVirtualCamera cmOtherCam1;
    CinemachineVirtualCamera cmOtherCam2;

    CinemachineVirtualCamera presentCam;
    CinemachineVirtualCamera lastCam;

    float defaultOrthoSize = 10f;
    float defaultFov = 40f;
    float dialogZoomMultiplier = 0.75f;
    float defaultBlendSpeed;
    bool isBlending = false;

    void Awake()
    {
        cmBrain = FindObjectOfType<CinemachineBrain>();
        defaultBlendSpeed = cmBrain.m_DefaultBlend.m_Time;
        cmPlayerCam = GameObject.Find("CMPlayerCam").GetComponent<CinemachineVirtualCamera>();
        cmDialogCam = GameObject.Find("CMDialogCam").GetComponent<CinemachineVirtualCamera>();
        cmQuadrantCam = GameObject.Find("CMQuadrantCam").GetComponent<CinemachineVirtualCamera>();
        cmOtherCam1 = GameObject.Find("CMOtherCam1").GetComponent<CinemachineVirtualCamera>();
        cmOtherCam2 = GameObject.Find("CMOtherCam2").GetComponent<CinemachineVirtualCamera>();
    }

    void Start()
    {
        presentCam = cmPlayerCam;
    }

    public bool IsSwitching()
    {
        // return cmBrain.IsBlending;
        return CinemachineCore.Instance.IsLive(lastCam);
    }

    void SetAllCamsZeroPriority()
    {
        cmPlayerCam.Priority = 0;
        cmQuadrantCam.Priority = 0;
        cmOtherCam1.Priority = 0;
        cmOtherCam2.Priority = 0;
    }

    public void SwitchToCam(string camName)
    {
        lastCam = presentCam;
        SetAllCamsZeroPriority();
        CinemachineVirtualCamera switchedtoCam = GetCameraByName(camName);
        presentCam = switchedtoCam;
        presentCam.Priority = 1;
    }

    public void SwitchToLastCam()
    {
        SetAllCamsZeroPriority();
        CinemachineVirtualCamera temp = presentCam;
        presentCam = lastCam;
        lastCam = temp;
        presentCam.Priority = 1;
    }

    public void FocusCamOn(string camName, Transform transform)
    {
        CinemachineVirtualCamera cam = GetCameraByName(camName);
        cam.Follow = transform;
        cam.LookAt = transform;
    }

    public void SendCamTo(Transform transform)
    {
        if (presentCam != cmOtherCam1)
        {
            FocusCamOn("Other1", transform);
            SwitchToCam("Other1");
        }
        else
        {
            FocusCamOn("Other2", transform);
            SwitchToCam("Other2");
        }
    }

    public void QuadrantCamActivate(Transform transform)
    {
        FocusCamOn("Quadrant", transform);
        SwitchToCam("Quadrant");
    }


    CinemachineVirtualCamera GetCameraByName(string camName)
    {
        switch (camName)
        {
            case "Player":
                cmBrain.m_DefaultBlend.m_Time = defaultBlendSpeed;
                return cmPlayerCam;

            case "Dialog":
                cmBrain.m_DefaultBlend.m_Time = 1f;
                return cmDialogCam;

            case "Other":
            case "Other1":
                cmBrain.m_DefaultBlend.m_Time = defaultBlendSpeed;
                return cmOtherCam1;

            case "Other2":
                cmBrain.m_DefaultBlend.m_Time = defaultBlendSpeed;
                return cmOtherCam2;

            case "Quadrant":
                cmBrain.m_DefaultBlend.m_Time = defaultBlendSpeed;
                return cmQuadrantCam;

            default:
                print("Unknown camera name given to camera manager. Returning player cam.");
                return cmPlayerCam;
        }
    }

    public void ZoomToSizeOrtho(float orthographicSize, float duration = 0f)
    {
        DOTween.To(
            () => cmOtherCam1.m_Lens.OrthographicSize,
            x => cmOtherCam1.m_Lens.OrthographicSize = x,
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
            () => cmOtherCam1.m_Lens.OrthographicSize,
            x => cmOtherCam1.m_Lens.OrthographicSize = x,
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

    public void Zoom(float multiplier, float duration = 0f)
    {
        float value = defaultFov * multiplier;
        DOTween.To(
            () => cmOtherCam1.m_Lens.FieldOfView,
            x => cmOtherCam1.m_Lens.FieldOfView = x,
            value,
            duration
        );
        DOTween.To(
            () => cmPlayerCam.m_Lens.FieldOfView,
            x => cmPlayerCam.m_Lens.FieldOfView = x,
            value,
            duration
        );
    }

    // TODO: There needs to be a dialog cam with a different angle.
    public void ZoomDialog()
    {
        Zoom(dialogZoomMultiplier, .25f);
    }

    public void ResetZoom(float duration = 0f)
    {
        DOTween.To(
              () => cmOtherCam1.m_Lens.FieldOfView,
              x => cmOtherCam1.m_Lens.FieldOfView = x,
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
