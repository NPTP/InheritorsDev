using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace ShadedTechnology.GrassPhysics
{
    /// <summary>
    /// Class that manages custom grass materials physics in Simple Physics Mode
    /// </summary>
    [AddComponentMenu("Grass Physics/For Custom Materials/MeshesSimplePhysics")]
    public class MaterialsSimplePhysics : MonoBehaviour
    {
        [Space]
        public Material[] materials;

        [Space]
        public LimitedSizeArray_GrassActor grassActors;

        private Vector4[] bufferData = new Vector4[GlobalConstants.MAX_GRASS_ACTORS];

        private void Start()
        {
            foreach (Material material in materials)
            {
                material.SetInt("_TargetsCount", grassActors.Length);
            }
        }

        private void FixedUpdate()
        {
            for (int i = 0; i < grassActors.Length && i < GlobalConstants.MAX_GRASS_ACTORS; ++i)
            {
                if (null == grassActors[i])
                {
                    bufferData[i] = Vector4.zero;
                    continue;
                }
                bufferData[i] = grassActors[i].GetVector4();
            }
            foreach (Material material in materials)
            {
                material.SetVectorArray("_TargetsPos", bufferData);
            }
        }
    }
}
