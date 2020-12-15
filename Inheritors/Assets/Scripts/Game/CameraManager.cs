/* INHERITORS by Nick Perrin (c) 2020 */
using System;
using System.Collections;
using UnityEngine;
using UnityEngine.Rendering.PostProcessing;
using DG.Tweening;
using Cinemachine;

// Handle all camera events
public class CameraManager : MonoBehaviour
{
    StateManager stateManager;
    InputManager inputManager;
    PostProcessVolume postProcessVolume;
    DepthOfField postProcessDOF;
    float dofDefaultAperture;
    float dofDefaultFocalLength;

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

        postProcessVolume = GameObject.Find("PostProcessVolume").GetComponent<PostProcessVolume>();
        postProcessDOF = null;
        postProcessVolume.profile.TryGetSettings(out postProcessDOF);
        dofDefaultAperture = postProcessDOF.aperture.value;
        dofDefaultFocalLength = postProcessDOF.focalLength.value;

        // postProcessDOF.active = false;
        // postProcessDOF.focusDistance.value = .1f;
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
        presentCam.Priority = 0; // In case this is a bespoke cam
        cmPlayerCam.Priority = 0;
        cmQuadrantCam.Priority = 0;
        cmOtherCam1.Priority = 0;
        cmOtherCam2.Priority = 0;
    }

    /// <summary>
    /// Switches to the prefab virtual camera that goes by camName, or,
    /// if camName is the name of a gameObject containing a virtual camera
    /// that is not in the prefabs, switch to that.
    /// </summary>
    public void SwitchToCam(string camName)
    {
        lastCam = presentCam;
        SetAllCamsZeroPriority();
        CinemachineVirtualCamera switchedtoCam = GetCameraByName(camName);

        if (switchedtoCam == cmDialogCam)
            SetDialogCamDOF();
        else
            ResetDOF();

        presentCam = switchedtoCam;
        presentCam.Priority = 1;
    }

    void ResetDOF()
    {
        if (postProcessDOF is null) return;

        DOTween.To(
            () => postProcessDOF.aperture.value,
            x => postProcessDOF.aperture.value = x,
            20f,
            cmBrain.m_DefaultBlend.m_Time
        );
        DOTween.To(
            () => postProcessDOF.focalLength.value,
            y => postProcessDOF.focalLength.value = y,
            300f,
            cmBrain.m_DefaultBlend.m_Time
        );
    }

    void SetDialogCamDOF()
    {
        if (postProcessDOF is null) return;

        DOTween.To(
            () => postProcessDOF.aperture.value,
            x => postProcessDOF.aperture.value = x,
            18f,
            cmBrain.m_DefaultBlend.m_Time
        );
        DOTween.To(
            () => postProcessDOF.focalLength.value,
            y => postProcessDOF.focalLength.value = y,
            190f,
            cmBrain.m_DefaultBlend.m_Time
        );
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
                return cmPlayerCam;

            case "Dialog":
                return cmDialogCam;

            case "Other":
            case "Other1":
                return cmOtherCam1;

            case "Other2":
                return cmOtherCam2;

            case "Quadrant":
                return cmQuadrantCam;

            default:
                CinemachineVirtualCamera bespokeVCam = GameObject.Find(camName).GetComponent<CinemachineVirtualCamera>();
                if (bespokeVCam != null)
                {
                    cmBrain.m_DefaultBlend.m_Time = defaultBlendSpeed;
                    return bespokeVCam;
                }
                else
                {
                    print("Unknown camera name given to camera manager. Returning player cam.");
                    return cmPlayerCam;
                }
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
