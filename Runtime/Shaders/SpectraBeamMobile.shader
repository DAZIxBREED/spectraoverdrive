Shader "SpectraOverdrive/Mobile/Beam"
{
    Properties
    {
        _Color ("Beam Color", Color) = (1,1,1,1)
        _MainTex ("Noise / Gobo", 2D) = "white" {}
        _Intensity ("Intensity", Range(0,4)) = 1
        _EdgePower ("Edge Power", Range(0.25,8)) = 2
        _DitherStrength ("Dither Strength", Range(0,1)) = 0.65
        _ScrollSpeed ("Noise Scroll", Vector) = (0,0.15,0,0)
    }

    SubShader
    {
        Tags
        {
            "Queue"="AlphaTest+20"
            "RenderType"="TransparentCutout"
            "IgnoreProjector"="True"
        }

        Cull Off
        ZWrite Off
        Blend One OneMinusSrcAlpha
        AlphaToMask On

        Pass
        {
            CGPROGRAM
            #pragma target 3.0
            #pragma vertex vert
            #pragma fragment frag
            #pragma multi_compile_instancing
            #include "UnityCG.cginc"
            #include "Includes/SpectraCommon.cginc"

            sampler2D _MainTex;
            float4 _MainTex_ST;
            float4 _Color;
            float _Intensity;
            float _EdgePower;
            float _DitherStrength;
            float4 _ScrollSpeed;

            struct appdata
            {
                float4 vertex : POSITION;
                float2 uv : TEXCOORD0;
                UNITY_VERTEX_INPUT_INSTANCE_ID
            };

            struct v2f
            {
                float4 pos : SV_POSITION;
                float2 uv : TEXCOORD0;
                float4 screenPos : TEXCOORD1;
                UNITY_VERTEX_OUTPUT_STEREO
            };

            v2f vert(appdata v)
            {
                v2f o;
                UNITY_SETUP_INSTANCE_ID(v);
                UNITY_INITIALIZE_VERTEX_OUTPUT_STEREO(o);
                o.pos = UnityObjectToClipPos(v.vertex);
                o.uv = TRANSFORM_TEX(v.uv, _MainTex);
                o.screenPos = ComputeScreenPos(o.pos);
                return o;
            }

            fixed4 frag(v2f i) : SV_Target
            {
                float2 animatedUv = i.uv + (_ScrollSpeed.xy * _SpectraShowTime);
                float noise = tex2D(_MainTex, animatedUv).r;

                float center = saturate(1.0 - abs(i.uv.x * 2.0 - 1.0));
                float edge = pow(center, _EdgePower);
                float intensity = _Intensity * SpectraBeamMultiplier();
                float alpha = saturate(edge * noise * intensity);

                float2 pixel = (i.screenPos.xy / max(i.screenPos.w, 0.0001)) * _ScreenParams.xy;
                float threshold = SpectraBayer4x4(pixel);
                clip(alpha - lerp(0.02, threshold, _DitherStrength));

                fixed3 rgb = _Color.rgb * intensity * edge;
                return fixed4(rgb * alpha, alpha);
            }
            ENDCG
        }
    }

    Fallback Off
}
