using System.Collections;
using UnityEngine;

public class WaterPickupFX : MonoBehaviour, PickupFX
{
    AudioManager audioManager;
    [SerializeField] AudioClip splashSound;
    float splashSoundVolumeScale = 0.4f;
    ParticleSystem ps;

    void Start()
    {
        audioManager = FindObjectOfType<AudioManager>();
        ps = GetComponent<ParticleSystem>();
    }

    public void PlayFX()
    {
        StartCoroutine(PlayFXCoroutine());
    }

    IEnumerator PlayFXCoroutine()
    {
        audioManager.PlayOneShot(splashSound, splashSoundVolumeScale);
        ps.Play(true);

        yield return new WaitForSeconds(Mathf.Max(splashSound.length, ps.main.startLifetime.constantMax));
        Destroy(this);
    }
}
