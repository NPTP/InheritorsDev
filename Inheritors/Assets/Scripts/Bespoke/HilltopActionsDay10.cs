using UnityEngine;
using DG.Tweening;

public class HilltopActionsDay10 : MonoBehaviour, CameraQuadrantActions
{
    public ParticleSystem hillSteam;
    public VolumetricLight volumetricLight;
    float noiseIntensity;
    float floor = 0.5f;

    void Start()
    {
        noiseIntensity = volumetricLight.NoiseIntensity;
    }

    public void StartAction()
    {
        hillSteam.Play();
        DOTween.To(
            () => volumetricLight.NoiseIntensity,
            x => volumetricLight.NoiseIntensity = x,
            floor,
            1f
        ).SetEase(Ease.InQuad);
    }

    public void StopAction()
    {
        hillSteam.Stop();
        DOTween.To(
            () => volumetricLight.NoiseIntensity,
            x => volumetricLight.NoiseIntensity = x,
            noiseIntensity,
            1f
        ).SetEase(Ease.OutQuad);
    }
}
