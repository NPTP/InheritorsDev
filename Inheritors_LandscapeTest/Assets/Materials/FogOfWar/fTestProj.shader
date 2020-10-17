// Upgrade NOTE: replaced '_Projector' with 'unity_Projector'

Shader "Custom/fTestProj" {
    Properties {
        _Color ("MainColor", Color) = (1,1,1,1)
        _MainTex ("Base (RGB)", 2D) = "white" {}
        _AlphaTex ("Alpha (RGB)", 2D) = "white" {}
    }
    SubShader {
        Tags {"Queue"="Transparent" "IgnoreProjector"="False" "RenderType"="Transparent"}
        LOD 100
        Blend SrcAlpha OneMinusSrcAlpha

        CGPROGRAM
        #pragma surface surf Lambert vertex:fProjUVs
        #include "UnityCG.cginc"

        float4 _Color;
        sampler2D _MainTex;
        sampler2D _AlphaTex;
        uniform float4x4 unity_Projector;

        struct Input {
            float2 uv_MainTex;
            float4 posProj : TEXCOORD0; // position in projector space
        };

        void fProjUVs (inout appdata_full v, out Input o) {
            UNITY_INITIALIZE_OUTPUT(Input,o);
            o.posProj = mul(unity_Projector, v.vertex);
        }

        void surf (Input IN, inout SurfaceOutput o) {
            float4 projCol = tex2D(_MainTex , float2(IN.posProj.xy) / IN.posProj.w) * _Color;
            float projAlpha = tex2D(_AlphaTex , float2(IN.posProj.xy) / IN.posProj.w);

            o.Emission = projCol.rgb;
            o.Alpha = projAlpha.r;
        }
    ENDCG
    }
}