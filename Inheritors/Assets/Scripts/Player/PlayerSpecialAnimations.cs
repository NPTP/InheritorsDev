using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerSpecialAnimations : MonoBehaviour
{
    AudioManager audioManager;
    PlayerMovement playerMovement;
    Animator animator;

    [Header("Bow")]
    [SerializeField] string bowAnimation;

    [Header("Flute")]
    [SerializeField] string fluteAnimation;

    bool animationImportantPoint = false;

    void Awake()
    {
        audioManager = FindObjectOfType<AudioManager>();
        playerMovement = GetComponent<PlayerMovement>();
        animator = GetComponent<Animator>();
    }

    public void PlayBowAnimation()
    {
        animator.Play(bowAnimation);
    }

    public void PlayFluteAnimation()
    {
        animator.Play(fluteAnimation);
    }

    public void StopFluteAnimation()
    {
        animator.SetBool("Playing", false);
    }

    public void SetImportantPoint()
    {
        animationImportantPoint = true;
    }

    public void ResetImportantPoint()
    {
        animationImportantPoint = true;
    }

}
