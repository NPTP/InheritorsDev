using System.Collections;
using UnityEngine;
using DG.Tweening;

public class Fire_DropoffTarget : MonoBehaviour, DropoffTarget
{
    bool doneReaction = false;
    AudioManager audioManager;
    Transform fireParent;
    ParticleSystem sparks;
    ParticleSystem fire;
    ParticleSystem smoke;
    ParticleSystem dark;
    LightFlicker lightFlicker;

    [SerializeField] AudioSource fireSource;
    [SerializeField] AudioClip flareSound;
    float flareSoundVolume = 0.6f;

    public bool DoneReaction()
    {
        return doneReaction;
    }

    void Awake()
    {
        audioManager = FindObjectOfType<AudioManager>();
        fireParent = GameObject.Find("FirePSParent").transform;
        sparks = fireParent.GetChild(0).GetComponent<ParticleSystem>();
        fire = fireParent.GetChild(1).GetComponent<ParticleSystem>();
        smoke = fireParent.GetChild(2).GetComponent<ParticleSystem>();
        dark = fireParent.GetChild(3).GetComponent<ParticleSystem>();
        lightFlicker = fireParent.GetChild(4).GetComponent<LightFlicker>();
    }

    void Start()
    {
        fireParent.localScale = new Vector3(1f, 1f, 1f);
        sparks.Stop();
        smoke.Stop();
        dark.Stop();
        lightFlicker.minIntensity = 0.5f;
        lightFlicker.maxIntensity = 1f;
    }

    public void ReactToDropoff()
    {
        StartCoroutine(DropoffAnimation());
    }

    IEnumerator DropoffAnimation()
    {
        float flareTime = 3f;
        audioManager.PlayOneShot(flareSound, flareSoundVolume);
        fireParent.DOScale(new Vector3(3f, 3f, 3f), flareTime).SetEase(Ease.OutElastic);
        lightFlicker.light.DOIntensity(2f, flareTime).SetEase(Ease.OutElastic);
        fireSource.DOFade(1f, flareTime);
        sparks.Play();
        smoke.Play();
        dark.Play();
        lightFlicker.minIntensity = 1f;
        lightFlicker.maxIntensity = 2f;

        doneReaction = true;

        yield return null;
        Destroy(this);
    }
}
