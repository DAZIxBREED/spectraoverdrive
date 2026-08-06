Shader "SpectraOverdrive/Mobile/MultiZoneReceiver"
{
    Properties
    {
        _MainTex ("Base Texture", 2D) = "white" {}
        _BaseColor ("Base Color", Color) = (1,1,1,1)
        _SpectraReceiverZones ("Zones", Vector) = (0,1,2,3)
        _SpectraReceiverWeights ("Weights", Vector) = (1,0,0,0)
        _ReceiverStrength ("Receiver Strength", Range(0,4)) = 1
    }

    SubShader
    {
        Tags { "RenderType"="Opaque" }
        LOD 120

        Pass
        {
            CGPROGRAM
            #pragma target 3.0
            #pragma vertex vert
            #pragma fragment frag
            #include "UnityCG.cginc"
            #include "Includes/SpectraCommon.cginc"

            sampler2D _MainTex;
            float4 _MainTex_ST;
            float4 _BaseColor;
            float4 _SpectraReceiverZones;
            float4 _SpectraReceiverWeights;
            float _ReceiverStrength;

            float4 _SpectraZone0;
            float4 _SpectraZone1;
            float4 _SpectraZone2;
            float4 _SpectraZone3;
            float4 _SpectraZone4;
            float4 _SpectraZone5;
            float4 _SpectraZone6;
            float4 _SpectraZone7;

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

            float4 GetZone(float index)
            {
                if (index < 0.5) return _SpectraZone0;
                if (index < 1.5) return _SpectraZone1;
                if (index < 2.5) return _SpectraZone2;
                if (index < 3.5) return _SpectraZone3;
                if (index < 4.5) return _SpectraZone4;
                if (index < 5.5) return _SpectraZone5;
                if (index < 6.5) return _SpectraZone6;
                return _SpectraZone7;
            }

            v2f vert(appdata v)
            {
                v2f o;
                o.pos = UnityObjectToClipPos(v.vertex);
                o.uv = TRANSFORM_TEX(v.uv, _MainTex);
                return o;
            }

            fixed4 frag(v2f i) : SV_Target
            {
                fixed4 baseSample = tex2D(_MainTex, i.uv) * _BaseColor;

                float4 a = GetZone(_SpectraReceiverZones.x) * _SpectraReceiverWeights.x;
                float4 b = GetZone(_SpectraReceiverZones.y) * _SpectraReceiverWeights.y;
                float4 c = GetZone(_SpectraReceiverZones.z) * _SpectraReceiverWeights.z;
                float4 d = GetZone(_SpectraReceiverZones.w) * _SpectraReceiverWeights.w;

                float3 lighting = (a.rgb + b.rgb + c.rgb + d.rgb) * _ReceiverStrength;
                return fixed4(baseSample.rgb + lighting, baseSample.a);
            }
            ENDCG
        }
    }

    Fallback Off
}
