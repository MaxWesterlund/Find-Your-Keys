Shader "Custom/Comic" {
    Properties {
        _Color ("Color", Color) = (1, 1, 1, 1)
        _MainTex ("Albedo (RGB)", 2D) = "white" {}
        _ShadowColor1 ("Shadow Color 1", Color) = (0.25, 0.25, 0.25, 1)
        _ShadowColor2 ("Shadow Color 2", Color) = (0.5, 0.5, 0.5, 1)
        [Toggle(USE_TEXTURE)] _UseTexture("Use Texture", Float) = 0
        [Toggle(USE_TEXTURE)] _RecieveShadows("Recieve Shadows", Float) = 0
    }
    
    SubShader {
        Tags { "RenderType"="Opaque" }
        LOD 200

        CGPROGRAM
        #pragma surface surf Ramp

        sampler2D _MainTex;
        fixed4 _Color;
        float _UseTexture;

        struct Input {
            float2 uv_MainTex;
        };
        
        void surf (Input IN, inout SurfaceOutput o) {
            fixed4 texSample = tex2D(_MainTex, IN.uv_MainTex);
            o.Albedo = _UseTexture > 0 ? texSample : + _Color;
        }

        fixed4 _ShadowColor1;
        fixed4 _ShadowColor2;
        float _RecieveShadows;

        half4 LightingRamp (SurfaceOutput s, half3 lightDir, half atten) {
            half NdotL = dot (s.Normal, lightDir);
            half diff = NdotL * 0.5 + 0.5;
            fixed4 c = _Color * 0.5;
            c.rgb *= s.Albedo;
            if (diff < 0.25) {
                c = _ShadowColor2;
            }
            else if (diff < 0.5) {
                c *= _ShadowColor1;
            }
            else if (atten == 0 && _RecieveShadows > 0) {
                c = _ShadowColor2;
            }
            else {
                c *= _LightColor0;
            }
            return c;
        }
        ENDCG
    }
    FallBack "Diffuse"
}