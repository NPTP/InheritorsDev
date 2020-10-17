Shader "Interactive Grass/GPU Grass" {
	Properties {
		[Header(Shading)]
		_MainTex             ("Base", 2D) = "white" {}
		_TopColor            ("Top Color", Color) = (1, 1, 1, 1)
		_BottomColor         ("Bottom Color", Color) = (1, 1, 1, 1)
		_TranslucentGain     ("Translucent Gain", Range(0, 1)) = 0.5
		[Space]
		_TessellationUniform ("Tessellation Uniform", Range(1, 64)) = 1
		[Header(Blades)]
		_BladeWidth          ("Blade Width", Float) = 0.05
		_BladeWidthRandom    ("Blade Width Random", Float) = 0.02
		_BladeHeight         ("Blade Height", Float) = 0.5
		_BladeHeightRandom   ("Blade Height Random", Float) = 0.3
		_BendRotationRandom  ("Bend Rotation Random", Range(0, 1)) = 0.2
		_BladeForward        ("Blade Forward Amount", Float) = 0.38
		[Header(Wind)]
		_WindDistortionMap   ("Wind Distortion Map", 2D) = "white" {}
		_WindStrength        ("Wind Strength", Float) = 1
		_WindFrequency       ("Wind Frequency", Vector) = (0.05, 0.05, 0, 0)
		[Header(Force)]
		_ForceCenter         ("Force Center", Vector) = (0, 0, 0, 0)
		_ForceRange          ("Force Range", Float) = 2
		_ForceIntensity      ("Force Intensity", Float) = 2
		[Header(Force2)]
		_ForceCenter2        ("Force Center 2", Vector) = (0, 0, 0, 0)
		_ForceRange2         ("Force Range 2", Float) = 2
		_ForceIntensity2     ("Force Intensity 2", Float) = 2
		[Header(Trail)]
		[Toggle(TRAIL)] _    ("Trail Enable", Float) = 1
		_TrailTex            ("Trail", 2D) = "bump" {}
		_TrailPress          ("TrailPress", Range(0, 4)) = 3
	}

	CGINCLUDE
	#include "UnityCG.cginc"
	#include "Autolight.cginc"
	#include "Lighting.cginc"
	
	float _TessellationUniform;
	float4 _TopColor, _BottomColor;
	float _TranslucentGain;
	float _BladeHeight, _BladeHeightRandom, _BladeWidthRandom, _BladeWidth, _BladeForward, _BendRotationRandom;
	sampler2D _WindDistortionMap, _MainTex, _TrailTex;
	float4 _WindDistortionMap_ST;
	float _WindStrength;
	float2 _WindFrequency;
	float4 _ForceCenter;
	float4 _ForceCenter2;
	float _ForceRange, _ForceIntensity, _TrailPress;
	float _ForceRange2, _ForceIntensity2;
	
	struct vertexInput
	{
		float4 vertex : POSITION;
		float3 normal : NORMAL;
		float4 tangent : TANGENT;
		float2 texcoord : TEXCOORD0;
	};
	struct vertexOutput
	{
		float4 vertex : SV_POSITION;
		float3 normal : NORMAL;
		float4 tangent : TANGENT;
		float2 texcoord : TEXCOORD0;
	};
	struct geometryOutput
	{
		float4 pos : SV_POSITION;
		float3 normal : NORMAL;
		float2 uv : TEXCOORD0;
		unityShadowCoord4 _ShadowCoord : TEXCOORD1;
		float2 uvOrig : TEXCOORD2;
	};

	// tessellation part of shader ///////////////////////////////////////////////////////////////////////////////////////////////////////////
	struct TessellationFactors 
	{
		float edge[3] : SV_TessFactor;
		float inside : SV_InsideTessFactor;
	};
	TessellationFactors patchConstantFunction (InputPatch<vertexInput, 3> patch)
	{
		TessellationFactors f;
		f.edge[0] = _TessellationUniform;
		f.edge[1] = _TessellationUniform;
		f.edge[2] = _TessellationUniform;
		f.inside = _TessellationUniform;
		return f;
	}
	[UNITY_domain("tri")]
	[UNITY_outputcontrolpoints(3)]
	[UNITY_outputtopology("triangle_cw")]
	[UNITY_partitioning("integer")]
	[UNITY_patchconstantfunc("patchConstantFunction")]
	vertexInput hull (InputPatch<vertexInput, 3> patch, uint id : SV_OutputControlPointID)
	{
		return patch[id];
	}
	[UNITY_domain("tri")]
	vertexOutput domain (TessellationFactors factors, OutputPatch<vertexInput, 3> patch, float3 barycentricCoordinates : SV_DomainLocation)
	{
		vertexInput v;

		#define GRASS_DOMAIN_INTERPOLATE(fieldName) v.fieldName = \
			patch[0].fieldName * barycentricCoordinates.x + \
			patch[1].fieldName * barycentricCoordinates.y + \
			patch[2].fieldName * barycentricCoordinates.z;

		GRASS_DOMAIN_INTERPOLATE(vertex)
		GRASS_DOMAIN_INTERPOLATE(normal)
		GRASS_DOMAIN_INTERPOLATE(tangent)
		GRASS_DOMAIN_INTERPOLATE(texcoord)

		vertexOutput o;
		o.vertex = v.vertex;
		o.normal = v.normal;
		o.tangent = v.tangent;
		o.texcoord = v.texcoord;
		return o;
	}
	// tessellation part of shader ///////////////////////////////////////////////////////////////////////////////////////////////////////////
	float rand (float3 v) { return frac(sin(dot(v.xyz, float3(12.9898, 78.233, 53.539))) * 43758.5453); }
	float3x3 AngleAxis3x3 (float angle, float3 axis)
	{
		float c, s;
		sincos(angle, s, c);
		float t = 1 - c;
		float x = axis.x;
		float y = axis.y;
		float z = axis.z;
		return float3x3(
			t * x * x + c, t * x * y - s * z, t * x * z + s * y,
			t * x * y + s * z, t * y * y + c, t * y * z - s * x,
			t * x * z - s * y, t * y * z + s * x, t * z * z + c);
	}
	vertexInput vert (vertexInput v) { return v; }
	[maxvertexcount(3)]
	void geo (point vertexOutput IN[1], inout TriangleStream<geometryOutput> triStream)
	{
		float3 pos = IN[0].vertex.xyz;

		// build tangent to local matrix
		float3 nor = IN[0].normal;
		float4 tgt = IN[0].tangent;
		float3 bin = cross(nor, tgt) * tgt.w;
		float3x3 t2l = float3x3(
			tgt.x, bin.x, nor.x,
			tgt.y, bin.y, nor.y,
			tgt.z, bin.z, nor.z);

		// config size
		float height = (rand(pos.zyx) * 2 - 1) * _BladeHeightRandom + _BladeHeight;
		float width = (rand(pos.xzy) * 2 - 1) * _BladeWidthRandom + _BladeWidth;
		float forward = rand(pos.yyz) * _BladeForward;

		// wind animation
		float2 uv = pos.xz * _WindDistortionMap_ST.xy + _WindDistortionMap_ST.zw + _WindFrequency * _Time.y;
		float2 s = (tex2Dlod(_WindDistortionMap, float4(uv, 0, 0)).xy * 2 - 1) * _WindStrength;
		float3 wind = normalize(float3(s.x, s.y, 0));
		float3x3 windMat = AngleAxis3x3(UNITY_PI * s, wind);

		// transform
		float3x3 m1 = AngleAxis3x3(rand(pos) * UNITY_TWO_PI, float3(0, 0, 1));
		float3x3 m2 = AngleAxis3x3(rand(pos.zzx) * _BendRotationRandom * UNITY_PI * 0.5, float3(-1, 0, 0));
		float3x3 mCompAll = mul(mul(mul(t2l, windMat), m1), m2);
		float3x3 mFacing = mul(t2l, m1);
		
		// position after above calculation
		float3 pos1 = pos + mul(mFacing, float3(width, 0, 0));
		float3 pos2 = pos + mul(mFacing, float3(-width, 0, 0));
		float3 pos3 = pos + mul(mCompAll, float3(0, 0, height));

#if TRAIL
		fixed4 trail = tex2Dlod(_TrailTex, float4(IN[0].texcoord, 0, 0));
		trail.xy = (trail.xy * 2.0) - 1.0;
		pos3.xz += (trail.xy * 2.0);
		pos3.y -= length(trail.xy * _TrailPress);
#else
		// interactive force
		float3 center = _ForceCenter.xyz;
		float3 wldpos = mul((half3x3)unity_ObjectToWorld, pos3);
		float3 dist = distance(center, wldpos);
		float3 circle = 1 - saturate(dist / _ForceRange);
		float3 disp = (wldpos - center) * circle;
		pos3.xz += disp.xz * _ForceIntensity;

		// interactive force 2
		float3 center2 = _ForceCenter2.xyz;
		float3 wldpos2 = mul((half3x3)unity_ObjectToWorld, pos3);
		float3 dist2 = distance(center2, wldpos2);
		float3 circle2 = 1 - saturate(dist2 / _ForceRange2);
		float3 disp2 = (wldpos2 - center2) * circle2;
		pos3.xz += disp2.xz * _ForceIntensity2;
#endif

		geometryOutput o;
		o.pos = UnityObjectToClipPos(pos1);
		o.uv = float2(0, 0);
		o.normal = UnityObjectToWorldNormal(mul(mFacing, normalize(float3(0, -1, forward))));
		o._ShadowCoord = ComputeScreenPos(o.pos);
		o.uvOrig = IN[0].texcoord;
		triStream.Append(o);
		
		o.pos = UnityObjectToClipPos(pos2);
		o.uv = float2(1, 0);
		o.normal = UnityObjectToWorldNormal(mul(mFacing, normalize(float3(0, -1, forward))));
		o._ShadowCoord = ComputeScreenPos(o.pos);
		o.uvOrig = IN[0].texcoord;
		triStream.Append(o);
		
		o.pos = UnityObjectToClipPos(pos3);
		o.uv = float2(0, 1);
		o.normal = UnityObjectToWorldNormal(mul(mCompAll, normalize(float3(0, -1, forward))));
		o._ShadowCoord = ComputeScreenPos(o.pos);
		o.uvOrig = IN[0].texcoord;
		triStream.Append(o);
	}
	ENDCG
	SubShader {
		Cull Off
		Pass {
			Tags { "RenderType" = "Opaque" "LightMode" = "ForwardBase" }
			CGPROGRAM
			#pragma vertex vert
			#pragma geometry geo
			#pragma fragment frag
			#pragma hull hull
			#pragma domain domain
			#pragma target 4.6
			#pragma multi_compile_fwdbase
			#pragma shader_feature TRAIL
			float4 frag (geometryOutput i,  fixed facing : VFACE) : SV_Target
			{
				float3 nor = facing > 0 ? i.normal : -i.normal;
				float shadow = SHADOW_ATTENUATION(i);
				float ndl = saturate(saturate(dot(nor, _WorldSpaceLightPos0)) + _TranslucentGain) * shadow;
				float4 lit = ndl * _LightColor0 + float4(ShadeSH9(float4(nor, 1.0)), 1.0);
				float4 albedo = tex2D(_MainTex, i.uv);
				return lerp(albedo * _BottomColor, albedo * _TopColor * lit, i.uv.y);
			}
			ENDCG
		}
		Pass {
			Tags { "LightMode" = "ShadowCaster" }
			CGPROGRAM
			#pragma vertex vert
			#pragma geometry geo
			#pragma fragment frag
			#pragma hull hull
			#pragma domain domain
			#pragma target 4.6
			#pragma multi_compile_shadowcaster
			float4 frag (geometryOutput i) : SV_Target
			{
				SHADOW_CASTER_FRAGMENT(i)
			}
			ENDCG
		}
	}
}
