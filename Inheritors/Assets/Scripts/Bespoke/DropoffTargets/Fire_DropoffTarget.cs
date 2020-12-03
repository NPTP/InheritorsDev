using UnityEngine;
using DG.Tweening;

public class Fire_DropoffTarget : MonoBehaviour, DropoffTarget
{
    AudioManager audioManager;
    Transform fireParent;
    ParticleSystem sparks;
    ParticleSystem fire;
    ParticleSystem smoke;
    ParticleSystem dark;
    LightFlicker lightFlicker;

    void Awake()
    {
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
        // TODO: also play a sound here. e.g.:
        // audioManager.PlayOneShot("fire-burst");
        float flareTime = 3f;
        fireParent.DOScale(new Vector3(3f, 3f, 3f), flareTime).SetEase(Ease.OutElastic);
        lightFlicker.light.DOIntensity(2f, flareTime).SetEase(Ease.OutElastic);
        sparks.Play();
        smoke.Play();
        dark.Play();
        lightFlicker.minIntensity = 1f;
        lightFlicker.maxIntensity = 2f;
        Destroy(this); // Eliminate this script, won't need this again!
    }
}
