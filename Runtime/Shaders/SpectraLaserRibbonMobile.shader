Shader "SpectraOverdrive/Mobile/LaserRibbon"
{
    Properties
    {
        _BaseColor ("Fallback Color", Color) = (1,0,0,1)
        _LaserSegments ("Segments", Range(1,32)) = 8
        _LaserPower ("Power", Range(0,4)) = 1
        _LaserSpeed ("Scan Speed", Range(0,4)) = 1
        _LaserSpread ("Spread", Range(0,1)) = 0.5
        _LaserJitter ("Jitter", Range(0,1)) = 0
        _LineWidth ("Line Width", Range(0.001,0.2)) = 0.02
    }

    SubShader
    {
        Tags
        {
            "Queue"="Transparent+10"
            "RenderType"="Transparent"
        }

        Blend One One
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

            float4 _BaseColor;
            float _LaserSegments;
            float _LaserPower;
            float _LaserSpeed;
            float _LaserSpread;
            float _LaserJitter;
            float _LineWidth;

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

            float hash(float n)
            {
                return frac(sin(n) * 43758.5453);
            }

            v2f vert(appdata v)
            {
                v2f o;
                float3 p = v.vertex.xyz;

                float segment = floor(v.uv.x * max(1.0, _LaserSegments));
                float phase = _SpectraShowTime * _LaserSpeed + segment;
                float wobble = sin(phase * 1.7) * _LaserSpread;
                wobble += (hash(segment * 17.0) - 0.5) * _LaserJitter;

                p.x += wobble * p.z;
                p.y += cos(phase * 1.3) * _LaserSpread * 0.5 * p.z;

                o.pos = UnityObjectToClipPos(float4(p,1));
                o.uv = v.uv;
                return o;
            }

            fixed4 frag(v2f i) : SV_Target
            {
                float dimmer = SpectraFixtureDimmer();
                float strobe = SpectraFixtureStrobeMask();
                float3 dmxColor = SpectraFixtureColor();
                float presence = step(0.001, dot(dmxColor, 1.0));
                float3 color = lerp(_BaseColor.rgb, dmxColor, presence);

                float lane = abs(frac(i.uv.x * _LaserSegments) - 0.5);
                float line = 1.0 - smoothstep(_LineWidth, _LineWidth * 2.0, lane);

                float intensity = line * dimmer * strobe * _LaserPower * SpectraLaserMultiplier();
                return fixed4(color * intensity, intensity);
            }
            ENDCG
        }
    }

    Fallback Off
}
