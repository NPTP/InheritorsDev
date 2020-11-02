﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DG.Tweening;

public class AudioManager : MonoBehaviour
{
    AudioSource audioSource;
    void Awake()
    {
        audioSource = GetComponent<AudioSource>();
    }

    public void SetVolume(float volume)
    {
        audioSource.volume = volume;
    }

    public void Play(bool looping = false)
    {
        audioSource.Play();
        audioSource.loop = looping;
    }

    public void FadeTo(float volume, float duration, Ease ease = Ease.Linear)
    {
        audioSource.DOFade(volume, duration).SetEase(ease);
    }
}