/* INHERITORS by Nick Perrin (c) 2020 */
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DG.Tweening;

public class AudioManager : MonoBehaviour
{
    [Header("Multi-channel Options")]
    [SerializeField] int setNumberOfChannels = 2;
    bool initialized = false;
    int numChannels = 0;
    int currentChannel = 0;
    AudioSource[] audioSources;

    [Header("Other AudioSource Fade Options")]
    [SerializeField] bool fadeOnSceneStart = true;
    [SerializeField] float fadeInTime = 5f;
    AudioSource[] otherSources;

    void Start()
    {
        if (fadeOnSceneStart) { FadeOtherSources("Up", fadeInTime); }
        audioSources = new AudioSource[0];
        NumChannels = setNumberOfChannels;
        initialized = true;
    }

    public void FadeOtherSources(string upDown, float fadeTime)
    {
        otherSources = FindObjectsOfType<AudioSource>();

        foreach (AudioSource otherSource in otherSources)
        {
            if (upDown.ToLower() == "up")
            {
                float savedVolume = otherSource.volume;
                otherSource.DOFade(savedVolume, fadeTime).From(0f).SetEase(Ease.InQuad);
            }
            else // "down"
            {
                otherSource.DOFade(0f, fadeTime).SetEase(Ease.InOutQuad);
            }
        }
    }

    // TODO: a universal fade for everything, saving the original volumes plz
    // public void FadeTo(float volume, float duration, Ease ease = Ease.Linear)
    // {
    //     audioSource.DOFade(volume, duration).SetEase(ease);
    // }

    public int NumChannels
    {
        get
        {
            return _GetNumChannels();
        }
        set
        {
            _SetNumChannels(value);
        }
    }

    int _GetNumChannels()
    {
        return numChannels;
    }

    void _SetNumChannels(int value)
    {
        if (value == 0)
        {
            if (audioSources.Length > 0)
            {
                for (int i = 0; i < audioSources.Length; i++)
                {
                    Destroy(audioSources[i]);
                }
            }

            audioSources = new AudioSource[0];
        }
        else if (value > numChannels)
        {
            int moreChannelsToCreate = value - numChannels;

            AudioSource[] newAudioSources = new AudioSource[audioSources.Length + moreChannelsToCreate];

            for (int i = 0; i < audioSources.Length; i++)
            {
                newAudioSources[i] = audioSources[i];
            }

            for (int i = 0; i < moreChannelsToCreate; i++)
            {
                newAudioSources[i + audioSources.Length] = gameObject.AddComponent<AudioSource>();
            }

            audioSources = newAudioSources;
        }
        else if (value < numChannels)
        {
            AudioSource[] newAudioSources = new AudioSource[value];

            for (int i = 0; i < value; i++)
            {
                newAudioSources[i] = audioSources[i];
            }

            for (int i = value; i < audioSources.Length; i++)
            {
                Destroy(audioSources[i]);
            }
        }

        numChannels = value;
    }


    public bool Initialized
    {
        get
        {
            return initialized;
        }
    }

    public void Stop(int idx)
    {
        foreach (AudioSource audioSource in audioSources)
        {
            audioSource.Stop();
        }
    }

    public void Play(AudioClip audioClip)
    {
        audioSources[currentChannel].clip = audioClip;
        audioSources[currentChannel].Play();

        if (++currentChannel >= numChannels)
        {
            currentChannel = 0;
        }
    }

    public void PlayOneShot(AudioClip audioClip, float volumeScale = 1f)
    {
        audioSources[currentChannel].PlayOneShot(audioClip, volumeScale);

        if (++currentChannel >= numChannels)
        {
            currentChannel = 0;
        }
    }

}
