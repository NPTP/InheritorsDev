//Custom variables
uniform fixed4 _GrassColorTint;
uniform sampler2D _GrassGlossMap;
uniform float _GrassGlossStren;
uniform float _GrassSpecStren;
uniform fixed4 _GrassSpecColor;

void surf(Input IN, inout SurfaceOutputStandardSpecular o) {
	fixed4 c = tex2D(_MainTex, IN.uv_MainTex) * fixed4(IN.color.rgb,1) * _GrassColorTint;
	o.Albedo = c.rgb;
	o.Alpha = c.a;
	o.Smoothness = tex2D(_GrassGlossMap, IN.uv_MainTex).r * _GrassGlossStren;
	o.Specular = (tex2D(_GrassGlossMap, IN.uv_MainTex) + 0.000001) * _GrassSpecStren * _GrassSpecColor;
	clip(o.Alpha - _Cutoff);
}