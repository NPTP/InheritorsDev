using UnityEngine;

public class HilltopActions : MonoBehaviour, CameraQuadrantActions
{
    public ParticleSystem hillSteam;

    public void StartAction()
    {
        hillSteam.Play();
    }

    public void StopAction()
    {
        hillSteam.Stop();
    }
}
