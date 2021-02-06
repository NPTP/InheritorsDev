using System;
using UnityEngine;

[RequireComponent(typeof(AudioSource))]
public class PlayerFootstepFX : MonoBehaviour
{
    AudioSource aSource;
    PlayerTerrainInteract playerTerrainInteract;
    Animator animator;

    ParticleSystem grassParticles;
    ParticleSystem dirtParticles;
    ParticleSystem gravelParticles;
    ParticleSystem leavesParticles;
    ParticleSystem ashParticles;
    ParticleSystem waterParticles;

    [SerializeField] FootstepData footstepData;
    float[] textures;
    int RRIndex = 0;

    void Awake()
    {
        aSource = GetComponent<AudioSource>();
        animator = GetComponent<Animator>();

        grassParticles = GameObject.Find("FootstepParticles_Grass").GetComponent<ParticleSystem>();
        dirtParticles = GameObject.Find("FootstepParticles_Dirt").GetComponent<ParticleSystem>();
        gravelParticles = GameObject.Find("FootstepParticles_Gravel").GetComponent<ParticleSystem>();
        leavesParticles = GameObject.Find("FootstepParticles_Leaves").GetComponent<ParticleSystem>();
        ashParticles = GameObject.Find("FootstepParticles_Ash").GetComponent<ParticleSystem>();
        waterParticles = GameObject.Find("FootstepParticles_Water").GetComponent<ParticleSystem>();
    }

    public void PlayFX(float[] textures)
    {
        this.textures = textures;
        float max = 0;
        int maxIndex = 0;
        for (int i = 0; i < textures.Length; i++)
        {
            if (i != (int)TerrainManager.Layers.Trail)
            {
                aSource.PlayOneShot(
                    GetFootstepSoundByLayer(i),
                    textures[i]
                );

                if (textures[i] >= max)
                {
                    max = textures[i];
                    maxIndex = i;
                }
            }
        }

        SpawnParticles(maxIndex);
    }

    void SpawnParticles(int layer)
    {
        switch (layer)
        {
            case (int)TerrainManager.Layers.GrassLight:
            case (int)TerrainManager.Layers.GrassDark:
                grassParticles.Play();
                break;

            case (int)TerrainManager.Layers.Dust:
            case (int)TerrainManager.Layers.Farm:
                gravelParticles.Play();
                break;

            case (int)TerrainManager.Layers.DirtLight:
            case (int)TerrainManager.Layers.DirtDark:
            case (int)TerrainManager.Layers.Trail:
                dirtParticles.Play();
                break;

            case (int)TerrainManager.Layers.LeavesBrown:
            case (int)TerrainManager.Layers.LeavesGreen:
            case (int)TerrainManager.Layers.LeavesYellow:
                leavesParticles.Play();
                break;

            case (int)TerrainManager.Layers.Water:
                waterParticles.Play();
                break;

            case (int)TerrainManager.Layers.AshDark:
                ashParticles.Play();
                break;

            case (int)TerrainManager.Layers.Wood:
                break;

            default:
                dirtParticles.Play();
                break;
        }
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

            case (int)TerrainManager.Layers.Dust:
            case (int)TerrainManager.Layers.Farm:
            case (int)TerrainManager.Layers.Trail:
                clip = footstepData.gravel[RRIndex];
                break;

            case (int)TerrainManager.Layers.DirtLight:
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
