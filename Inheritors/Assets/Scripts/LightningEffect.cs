using UnityEngine;
using System.Collections;

public class LightningEffect : MonoBehaviour
{

    [Header("Assignments")]
    public float frequencyBase = 9f;
    public float intensityBase = 2f;
    public float exposureBase = 1f;
    public float rotationSpeed = 5f;
    public float noiseScale = 2f;
    public float timeScale = 0.015f;
    public AudioClip thunderClip;

    [Header("This Scene's Skybox Material")]
    public Material theSkyBox;

    Quaternion startRot;
    Light daLight;

    GameObject soundObj;
    AudioSource audioS;
    IEnumerator Start()
    {

        if (soundObj == null)
        {
            CreateSoundObj();
        }
        startRot = transform.rotation;
        daLight = GetComponent<Light>();
        theSkyBox.EnableKeyword("_Exposure");
        theSkyBox.EnableKeyword("_Rotation");
        exposureBase = theSkyBox.GetFloat("_Exposure");
        intensityBase = daLight.intensity;

        while (true)
        {
            float currentRotation = theSkyBox.GetFloat("_Rotation");
            if (currentRotation >= 360f)
                theSkyBox.SetFloat("_Rotation", 0f);
            else
                theSkyBox.SetFloat("_Rotation", currentRotation + rotationSpeed * Time.deltaTime);

            daLight.intensity = intensityBase;
            yield return new WaitForSeconds(Random.Range(frequencyBase - 4f, frequencyBase + 4f));

            float dist = Random.Range(15f, 100f);
            Vector2 random = Random.insideUnitCircle * dist;
            Vector3 hitPoint = Camera.main.transform.position + new Vector3(random.x, 0f, random.y);

            StartCoroutine(ThunderDelay(dist, hitPoint));

            ///Sets rotation so shadows will fall away from the lightning source. (only Y rotation, not x and z)
            Quaternion rot = transform.rotation;
            rot.y = Quaternion.LookRotation(Camera.main.transform.position - hitPoint, Vector3.up).y;
            transform.rotation = rot;
            float decay = Random.Range(0.1f, 0.5f);
            daLight.intensity = intensityBase + 1f;

            while (true)
            {
                float noise = Mathf.PerlinNoise(Time.time, 0f);
                daLight.intensity *= (noise * noiseScale);
                daLight.intensity = Mathf.Lerp(daLight.intensity, intensityBase, Time.deltaTime * decay);
                theSkyBox.SetFloat("_Exposure", exposureBase + daLight.intensity / 8f);
                if (daLight.intensity <= intensityBase + 0.1f)
                {
                    daLight.intensity = intensityBase;
                    theSkyBox.SetFloat("_Exposure", exposureBase);

                    ///Resets rotation for the original sun's rotation
                    transform.rotation = startRot;
                    StartCoroutine("Start");
                    yield break;
                }
                yield return null;
            }
        }
    }

    ///Moves the soundObject to the virtual lightning strike point, and plays it back delayed by distance.
    IEnumerator ThunderDelay(float dist, Vector3 hitPoint)
    {
        yield return new WaitForSeconds(dist * timeScale);

        soundObj.transform.position = hitPoint;

        float volume = ((100f / dist) / 4f);
        audioS.volume = volume / 4f;
        audioS.Play();

        while (true)
        {
            yield return null;
            if (!audioS.isPlaying)
            {
                yield break;
            }
            yield return null;
        }
    }


    void CreateSoundObj()
    {
        soundObj = new GameObject();
        audioS = soundObj.AddComponent<AudioSource>();
        audioS.rolloffMode = AudioRolloffMode.Linear;
        audioS.spatialBlend = 1f;
        audioS.clip = thunderClip;
    }

}