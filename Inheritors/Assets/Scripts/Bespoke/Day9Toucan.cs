using UnityEngine;

// Call functions from animation events.
[RequireComponent(typeof(AudioSource))]
public class Day9Toucan : MonoBehaviour
{
    AudioSource audioSource;
    [SerializeField] AudioClip[] chirpSounds;

    float chirpVolumeScale = 0.25f;
    int RRIndex = 0;

    void Awake()
    {
        audioSource = GetComponent<AudioSource>();
    }

    public void ChirpSound()
    {
        audioSource.PlayOneShot(chirpSounds[RRIndex], chirpVolumeScale);
        RRIndex++;
        if (RRIndex >= chirpSounds.Length) { RRIndex = 0; }
    }
}
