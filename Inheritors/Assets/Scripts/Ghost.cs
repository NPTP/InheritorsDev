using System.Collections;
using UnityEngine;
using DG.Tweening;

public class Ghost : MonoBehaviour
{
    Renderer rend;
    SampleBuffer sampleBuffer;
    [Header("Animator controller")]
    [SerializeField] Animator animator;
    [Header("Materials")]
    [SerializeField] Material mainMaterial;
    [SerializeField] Material dissolveMaterial;
    [Header("Particles")]
    [SerializeField] ParticleSystem enterParticles;
    [SerializeField] ParticleSystem activeParticles;
    [SerializeField] ParticleSystem exitParticles;
    [SerializeField] ParticleSystem poofParticles;
    [Header("Light")]
    [SerializeField] Light thisLight;
    [Header("Audio")]
    [SerializeField] AudioSource audioSource;

    float volumeScale = 1f;
    [SerializeField] AudioClip enterSound;
    [SerializeField] AudioClip loopSound;
    [SerializeField] AudioClip[] exitSounds;

    float fadeInTime = 3f;
    float fadeOutTime = 4f;

    float savedRange;
    float savedIntensity;
    float savedVolume;
    bool playing = false;
    int frame = 0;

    void FixedUpdate()
    {
        if (!playing) return;

        if (frame < sampleBuffer.Length)
        {
            Playback();
            frame++;
        }
        else
        {
            playing = false;
            StartCoroutine(GhostExit());
        }
    }

    void Playback()
    {
        Sample sample = sampleBuffer[frame];

        animator.SetBool("Grounded", sample.isGrounded);
        transform.position = sample.position;
        transform.rotation = sample.rotation;
        Vector3 direction = sample.direction;
        if (direction != Vector3.zero)
        {
            animator.SetFloat("MoveSpeed", direction.magnitude);
        }
    }

    public void InitializeGhost(SampleBuffer buffer)
    {
        sampleBuffer = buffer;
        savedRange = thisLight.range;
        savedIntensity = thisLight.intensity;
        savedVolume = audioSource.volume;

        // Set up first frame
        if (frame < sampleBuffer.Length)
        {
            Playback();
            frame++;
        }

        rend = transform.GetChild(1).gameObject.GetComponent<Renderer>();
        StartCoroutine("GhostEnter");
    }

    public void EndGhost()
    {
        if (playing) { playing = false; }
        StopCoroutine("GhostEnter");
        StartCoroutine(GhostExit(true));
    }

    void RestartSelf()
    {
        frame = 0;
        if (frame < sampleBuffer.Length)
        {
            Playback();
            frame++;
        }
        StartCoroutine("GhostEnter");
    }

    IEnumerator GhostEnter()
    {
        animator.speed = 0;
        thisLight.intensity = 0;
        audioSource.volume = 0;

        enterParticles.Play();
        audioSource.PlayOneShot(enterSound, volumeScale);

        rend.enabled = true;
        rend.materials = new Material[2] { dissolveMaterial, dissolveMaterial };

        for (int i = 0; i < rend.materials.Length; i++)
        {
            rend.materials[i].SetFloat("_DissolveAmount", 1);
            int mat = i;
            DOTween.To(
                () => rend.materials[mat].GetFloat("_DissolveAmount"),
                x => rend.materials[mat].SetFloat("_DissolveAmount", x),
                0,
                fadeInTime
            );
        }

        FadeLight(true);
        Tween t = audioSource.DOFade(savedVolume, fadeInTime);
        yield return t.WaitForCompletion();

        // Reset shader to better-looking fresnel effect
        rend.materials = new Material[2] { mainMaterial, mainMaterial };

        poofParticles.Play();
        PoofLight();

        animator.speed = 1;
        activeParticles.Play();
        playing = true;
    }

    IEnumerator GhostExit(bool destroyOnFinish = false)
    {
        activeParticles.Stop();
        poofParticles.Play();
        exitParticles.Play();
        PoofLight();
        audioSource.PlayOneShot(exitSounds[Random.Range(0, exitSounds.Length)], volumeScale);
        animator.speed = 0;

        yield return new WaitForSeconds(0.2f);

        rend.materials = new Material[2] { dissolveMaterial, dissolveMaterial };

        for (int i = 0; i < rend.materials.Length; i++)
        {
            rend.materials[i].SetFloat("_DissolveAmount", 0);
            int mat = i;
            DOTween.To(
                () => rend.materials[mat].GetFloat("_DissolveAmount"),
                x => rend.materials[mat].SetFloat("_DissolveAmount", x),
                1,
                fadeOutTime
            ).From(0);
        }

        FadeLight(false);
        Tween t = audioSource.DOFade(0, fadeOutTime).SetEase(Ease.InOutQuad);
        yield return t.WaitForCompletion();

        // Ensure all particles have finished
        yield return new WaitForSeconds(1);
        rend.enabled = false;

        // Destroy this ghost after exiting, if we've been told to stop.
        if (destroyOnFinish) { Destroy(this.gameObject); }

        // Otherwise, wait an interval before restarting.
        float minResetTime = 5.0f;
        float maxResetTime = 15.0f;
        yield return new WaitForSeconds(Random.Range(minResetTime, maxResetTime));
        RestartSelf();
    }

    void FadeLight(bool fadeIn)
    {
        if (fadeIn)
        {
            DOTween.To(
                () => thisLight.range,
                x => thisLight.range = x,
                savedRange, fadeInTime
            );
            thisLight.DOIntensity(savedIntensity, fadeOutTime);
        }
        else
        {
            DOTween.To(
                () => thisLight.range,
                x => thisLight.range = x,
                0, fadeOutTime
            );
            thisLight.DOIntensity(0, fadeOutTime);
        }
    }

    void PoofLight()
    {
        Sequence s = DOTween.Sequence();
        s.Append(
           DOTween.To(() => thisLight.range, x => thisLight.range = x, savedRange * 1.25f, 0.15f)
        ).Append(
            DOTween.To(() => thisLight.range, x => thisLight.range = x, savedRange, 0.2f).SetEase(Ease.OutQuint)
        );
    }
}
