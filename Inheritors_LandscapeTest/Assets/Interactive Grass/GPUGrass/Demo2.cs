using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace InteractiveGrass
{
    public class Demo2 : MonoBehaviour
    {
        public GameObject m_Grass;
        public GameObject m_Character;
        public KeyCode m_ForwardButton = KeyCode.UpArrow;
        public KeyCode m_BackwardButton = KeyCode.DownArrow;
        public KeyCode m_RightButton = KeyCode.RightArrow;
        public KeyCode m_LeftButton = KeyCode.LeftArrow;
        Renderer m_GrassRd;

        [Header("Grass")]
        [Range(4, 16)] public int m_Tess = 9;
        [Range(0.05f, 1f)] public float m_WindStrength = 0.4f;
        public float m_ForceRange = 1.2f;
        public float m_ForceIntensity = 4f;

        void Start()
        {
            QualitySettings.antiAliasing = 8;
            m_GrassRd = m_Grass.GetComponent<Renderer>();
        }
        void Update()
        {
            // Vector3 dir = Vector3.zero;
            // Move (m_ForwardButton, ref dir, m_Character.transform.forward);
            // Move (m_BackwardButton, ref dir, -m_Character.transform.forward);
            // Move (m_RightButton, ref dir, m_Character.transform.right);
            // Move(m_LeftButton, ref dir, -m_Character.transform.right);
            // m_Character.transform.position += dir * 4f * Time.deltaTime;

            m_GrassRd.material.SetFloat("_TessellationUniform", m_Tess);
            m_GrassRd.material.SetFloat("_WindStrength", m_WindStrength);
            m_GrassRd.material.SetFloat("_ForceRange", m_ForceRange);
            m_GrassRd.material.SetFloat("_ForceIntensity", m_ForceIntensity);
            m_GrassRd.material.SetVector("_ForceCenter", m_Character.transform.position);
        }
        void Move(KeyCode key, ref Vector3 moveTo, Vector3 dir)
        {
            if (Input.GetKey(key))
                moveTo = dir;
        }
    }
}