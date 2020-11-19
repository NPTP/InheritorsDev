using UnityEngine;

// Keep the audio listener aligned on the X/Z axis to conform with the camera perspective.
public class FixedDirAudioListener : MonoBehaviour
{
    void LateUpdate()
    {
        transform.rotation = Quaternion.identity;
    }
}
