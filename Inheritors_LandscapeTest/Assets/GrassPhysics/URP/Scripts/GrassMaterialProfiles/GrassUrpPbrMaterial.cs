using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace ShadedTechnology.GrassPhysics
{
    /// <summary>
    /// Class stores specular grass material settings
    /// </summary>
    [CreateAssetMenu(fileName = "GrassUrpPbrMaterial", menuName = "GrassPhysics/GrassMaterialProfile/GrassUrpPbrMaterial")]
    [GrassMaterial(name = "URP PBR Material", fileName = "GrassUrpPbrMaterial", fragmentShader = "Assets/GrassPhysics/URP/Shaders/GrassSurfaces/GrassUrpPbrSurface.cginc")]
    public class GrassUrpPbrMaterial : GrassMaterialProfile
    {
        [LowEndPlatformWarning("Some platforms doesn't support PBR materials, so if you getting bad results try changing Grass Material.")]
        public Color grassTint = Color.white;
        public Texture glossMap;
        [Range(0f, 1f)]
        public float smoothness;
        [Range(0f, 1f)]
        public float metallic;
        public Color emission = Color.black;

        /// <summary>
        /// Sets grass material data to global grass shader variables
        /// </summary>
        public override void SetMaterialToGrass()
        {
            Shader.SetGlobalColor("_GrassColorTint", grassTint);
            Shader.SetGlobalTexture("_GrassGlossMap", glossMap);
            Shader.SetGlobalFloat("_GrassSmoothness", smoothness);
            Shader.SetGlobalFloat("_GrassMetallic", metallic);
            Shader.SetGlobalColor("_GrassEmission", emission);
        }
    }
}
