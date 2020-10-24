using UnityEngine;

// Keep the audio listener aligned on the X/Z axis to conform with the camera perspective.
public class PlayerAudioListener : MonoBehaviour
{
    void LateUpdate()
    {
        transform.rotation = Quaternion.identity;
    }
}
