Shader "SpectraOverdrive/Mobile/MovingWash"
{
    Properties
    {
        _BaseColor ("Fallback Color", Color) = (1,1,1,1)
        _MainTex ("Wash Texture", 2D) = "white" {}
        _WashPower ("Wash Power", Range(0,4)) = 1
        _WashSoftness ("Wash Softness", Range(0.01,1)) = 0.5
    }

    SubShader
    {
        Tags
        {
            "Queue"="Transparent"
            "RenderType"="Transparent"
        }

        Blend SrcAlpha OneMinusSrcAlpha
        ZWrite Off
        Cull Off

        Pass
        {
            CGPROGRAM
            #pragma target 3.0
            #pragma vertex vert
            #pragma fragment frag
            #include "UnityCG.cginc"
            #include "Includes/SpectraCommon.cginc"
            #include "Includes/SpectraFixtureControl.cginc"

            sampler2D _MainTex;
            float4 _MainTex_ST;
            float4 _BaseColor;
            float _WashPower;
            float _WashSoftness;

            struct appdata
            {
                float4 vertex : POSITION;
                float2 uv : TEXCOORD0;
            };

            struct v2f
            {
                float4 pos : SV_POSITION;
                float2 uv : TEXCOORD0;
            };

            float3 RotateY(float3 p, float a)
            {
                float s = sin(a);
                float c = cos(a);
                return float3(c * p.x + s * p.z, p.y, -s * p.x + c * p.z);
            }

            float3 RotateX(float3 p, float a)
            {
                float s = sin(a);
                float c = cos(a);
                return float3(p.x, c * p.y - s * p.z, s * p.y + c * p.z);
            }

            v2f vert(appdata v)
            {
                v2f o;
                float2 panTilt = SpectraFixturePanTilt01();
                float panRadians = radians((panTilt.x - 0.5) * _SpectraMovementCalibration.x);
                float tiltRadians = radians((panTilt.y - 0.5) * _SpectraMovementCalibration.y);

                float zoom = SpectraFixtureZoom();
                float3 p = v.vertex.xyz;
                p.xy *= lerp(1.5, 0.45, zoom);
                p = RotateX(p, tiltRadians);
                p = RotateY(p, panRadians);

                o.pos = UnityObjectToClipPos(float4(p,1));
                o.uv = TRANSFORM_TEX(v.uv, _MainTex);
                return o;
            }

            fixed4 frag(v2f i) : SV_Target
            {
                float2 centered = i.uv * 2.0 - 1.0;
                float radial = length(centered);
                float softness = max(0.01, _WashSoftness);
                float falloff = 1.0 - smoothstep(1.0 - softness, 1.0, radial);

                float dimmer = SpectraFixtureDimmer();
                float strobe = SpectraFixtureStrobeMask();
                float3 dmxColor = SpectraFixtureColor();
                float presence = step(0.001, dot(dmxColor, 1.0));
                float3 color = lerp(_BaseColor.rgb, dmxColor, presence);

                float textureMask = tex2D(_MainTex, i.uv).r;
                float alpha = falloff * textureMask * dimmer * strobe * _WashPower * SpectraProjectionMultiplier();

                return fixed4(color * alpha, alpha);
            }
            ENDCG
        }
    }

    Fallback Off
}
