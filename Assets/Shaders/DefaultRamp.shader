Shader "Custom/NewSurfaceShader"
{
    Properties
    {
        _Color ("Color", Color) = (1,1,1,1)
        _MainTex ("Albedo (RGB)", 2D) = "white" {}
        _Glossiness ("Smoothness", Range(0,1)) = 0.5
        _Metallic ("Metallic", Range(0,1)) = 0.0
    }
    SubShader
    {
        Tags { "RenderType"="Opaque" }
        LOD 200

        CGPROGRAM
        #pragma surface surf Ramp

        sampler2D _Ramp;

        half4 LightingRamp (SurfaceOutput s, half3 lightDir, half atten) {
            half NdotL = dot (s.Normal, lightDir);
            half diff = NdotL * 0.5 + 0.5;
            half3 ramp = tex2D (_Ramp, diff).rgb;
            half4 c;
            c.rgb = _LightColor0.rgb * ramp * atten;
            c.a = 1;
            return c;
        }

        struct Input {
            float2 uv_MainTex;
        };
        
        sampler2D _MainTex;
        
        void surf (Input IN, inout SurfaceOutput o) {
            o.Albedo = tex2D (_MainTex, IN.uv_MainTex).rgb;
        }
        ENDCG
    }
    FallBack "Diffuse"
}
