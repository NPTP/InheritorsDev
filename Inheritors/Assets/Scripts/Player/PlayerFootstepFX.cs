using System;
using UnityEngine;

[RequireComponent(typeof(AudioSource))]
public class PlayerFootstepFX : MonoBehaviour
{
    AudioSource aSource;
    PlayerTerrainInteract playerTerrainInteract;
    ParticleSystem footStepParticles;
    Animator animator;

    [SerializeField] FootstepData footstepData;
    int RRIndex = 0;

    void Awake()
    {
        aSource = GetComponent<AudioSource>();
        footStepParticles = GameObject.Find("FootstepParticles").GetComponent<ParticleSystem>();
        animator = GetComponent<Animator>();
    }

    public void PlayFX(float[] textures)
    {
        footStepParticles.Play();

        for (int i = 0; i < textures.Length; i++)
        {
            if (i != (int)TerrainManager.Layers.Trail)
            {
                aSource.PlayOneShot(
                    GetFootstepSoundByLayer(i),
                    textures[i]
                );
            }
        }
    }

    public void TestFootstep()
    {
        aSource.PlayOneShot(footstepData.tester);
    }

    AudioClip GetFootstepSoundByLayer(int layer)
    {
        if (RRIndex == footstepData.clipArrayLength)
            RRIndex = 0;

        AudioClip clip;
        switch (layer)
        {
            case (int)TerrainManager.Layers.GrassLight:
            case (int)TerrainManager.Layers.GrassDark:
                clip = footstepData.grass[RRIndex];
                break;

            case (int)TerrainManager.Layers.DirtLight:
            case (int)TerrainManager.Layers.Dust:
            case (int)TerrainManager.Layers.Farm:
                clip = footstepData.gravel[RRIndex];
                break;

            case (int)TerrainManager.Layers.DirtDark:
                clip = footstepData.mud[RRIndex];
                break;

            case (int)TerrainManager.Layers.LeavesBrown:
            case (int)TerrainManager.Layers.LeavesGreen:
            case (int)TerrainManager.Layers.LeavesYellow:
                clip = footstepData.leaves[RRIndex];
                break;

            case (int)TerrainManager.Layers.Wood:
                clip = footstepData.wood[RRIndex];
                break;

            case (int)TerrainManager.Layers.Water:
                clip = footstepData.water[RRIndex];
                break;

            case (int)TerrainManager.Layers.AshDark:
            case (int)TerrainManager.Layers.Trail:
                clip = footstepData.sand[RRIndex];
                break;

            default:
                clip = footstepData.grass[RRIndex];
                break;
        }

        RRIndex++;
        return clip;
    }
}
