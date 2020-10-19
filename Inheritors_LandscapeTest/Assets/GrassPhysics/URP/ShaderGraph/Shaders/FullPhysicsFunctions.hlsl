#define PHYSICS_FULL 1
#include "Assets/GrassPhysics/Shaders/GrassPhysics.cginc"

void GrassFullPhysics_float(float3 vertex, out float3 vertOut, out float deformAmount)
{
	float4 vert = float4(vertex, 1);
	deformAmount = GrassFullPhysics(vert);
	vertOut = vert.xyz;
}