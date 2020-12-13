using System.Collections;
using UnityEngine;
using DG.Tweening;

public class Ghost : MonoBehaviour
{
    Renderer rend;
    SampleBuffer sampleBuffer;
    [SerializeField] Animator animator;
    [SerializeField] Shader dissolveShader;
    [SerializeField] Shader mainShader;
    [SerializeField] Texture dissolveMap;
    [SerializeField] ParticleSystem enterParticles;
    [SerializeField] ParticleSystem activeParticles;
    [SerializeField] ParticleSystem exitParticles;
    [SerializeField] ParticleSystem poofParticles;
    [SerializeField] Light thisLight;
    [SerializeField] AudioSource audioSource;

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

        if (frame < sampleBuffer.length)
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
        Sample sample = sampleBuffer.Get(frame);

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
        if (frame < sampleBuffer.length)
        {
            Playback();
            frame++;
        }

        rend = transform.GetChild(1).gameObject.GetComponent<Renderer>();
        StartCoroutine(GhostEnter());
    }

    IEnumerator GhostEnter()
    {
        thisLight.intensity = 0;
        audioSource.volume = 0;

        enterParticles.Play();

        for (int i = 0; i < rend.materials.Length; i++)
        {
            rend.materials[i].shader = dissolveShader;
            rend.materials[i].SetTexture("Dissolve Map", dissolveMap);
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
        for (int i = 0; i < rend.materials.Length; i++)
            rend.materials[i].shader = mainShader;

        poofParticles.Play();
        PoofLight();

        activeParticles.Play();
        playing = true;
    }

    IEnumerator GhostExit()
    {
        activeParticles.Stop();
        poofParticles.Play();
        exitParticles.Play();
        PoofLight();

        yield return new WaitForSeconds(0.2f);

        for (int i = 0; i < rend.materials.Length; i++)
        {
            rend.materials[i].shader = dissolveShader;
            rend.materials[i].SetTexture("Dissolve Map", dissolveMap);
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

        Destroy(this.gameObject);
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
