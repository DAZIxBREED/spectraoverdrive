Shader "SpectraOverdrive/PC/MovingHeadBeam"
{
    Properties
    {
        _MainTex ("Beam Noise", 2D) = "white" {}
        _GoboAtlas ("Gobo Atlas", 2D) = "white" {}
        _BaseColor ("Fallback Color", Color) = (1,1,1,1)
        _BeamPower ("Beam Power", Range(0,8)) = 2
        _Density ("Density", Range(0.01,4)) = 1
        _SoftEdge ("Soft Edge", Range(0.01,1)) = 0.25
        _GoboColumns ("Gobo Columns", Float) = 4
        _GoboRows ("Gobo Rows", Float) = 2
        _NoiseScroll ("Noise Scroll", Vector) = (0,0.05,0,0)
    }

    SubShader
    {
        Tags
        {
            "Queue"="Transparent+20"
            "RenderType"="Transparent"
        }

        Blend SrcAlpha One
        ZWrite Off
        Cull Off

        Pass
        {
            CGPROGRAM
            #pragma target 4.5
            #pragma vertex vert
            #pragma fragment frag
            #include "UnityCG.cginc"
            #include "Includes/SpectraCommon.cginc"
            #include "Includes/SpectraFixtureControl.cginc"

            sampler2D _MainTex;
            sampler2D _GoboAtlas;
            float4 _BaseColor;
            float _BeamPower;
            float _Density;
            float _SoftEdge;
            float _GoboColumns;
            float _GoboRows;
            float4 _NoiseScroll;

            struct appdata
            {
                float4 vertex : POSITION;
                float2 uv : TEXCOORD0;
            };

            struct v2f
            {
                float4 pos : SV_POSITION;
                float2 uv : TEXCOORD0;
                float3 objectPos : TEXCOORD1;
            };

            v2f vert(appdata v)
            {
                v2f o;
                float2 panTilt = SpectraFixturePanTilt01();
                float3 p = v.vertex.xyz;
                p.x *= lerp(1.3, 0.25, SpectraFixtureZoom());
                p.y *= lerp(1.3, 0.25, SpectraFixtureZoom());

                float pan = radians((panTilt.x - 0.5) * _SpectraMovementCalibration.x);
                float tilt = radians((panTilt.y - 0.5) * _SpectraMovementCalibration.y);

                float sp = sin(pan);
                float cp = cos(pan);
                p = float3(cp * p.x + sp * p.z, p.y, -sp * p.x + cp * p.z);

                float st = sin(tilt);
                float ct = cos(tilt);
                p = float3(p.x, ct * p.y - st * p.z, st * p.y + ct * p.z);

                o.objectPos = p;
                o.pos = UnityObjectToClipPos(float4(p, 1));
                o.uv = v.uv;
                return o;
            }

            fixed4 frag(v2f i) : SV_Target
            {
                float dimmer = SpectraFixtureDimmer();
                float strobe = SpectraFixtureStrobeMask();
                float3 dmxColor = SpectraFixtureColor();
                float colorPresence = step(0.001, dot(dmxColor, 1.0));
                float3 color = lerp(_BaseColor.rgb, dmxColor, colorPresence);

                float radial = abs(i.uv.x * 2.0 - 1.0);
                float edge = 1.0 - smoothstep(1.0 - _SoftEdge, 1.0, radial);
                float noise = tex2D(_MainTex, i.uv + _NoiseScroll.xy * _SpectraShowTime).r;

                float goboIndex = SpectraFixtureGoboIndex();
                float columns = max(1.0, _GoboColumns);
                float rows = max(1.0, _GoboRows);
                float angle = SpectraFixtureGoboRotation();
                float2 centered = i.uv - 0.5;
                float sinAngle = sin(angle);
                float cosAngle = cos(angle);
                centered = float2(
                    cosAngle * centered.x - sinAngle * centered.y,
                    sinAngle * centered.x + cosAngle * centered.y
                );
                float2 rotatedUv = centered + 0.5;
                float2 goboUv = float2(
                    (rotatedUv.x + fmod(goboIndex, columns)) / columns,
                    (rotatedUv.y + floor(goboIndex / columns)) / rows
                );
                float gobo = tex2D(_GoboAtlas, goboUv).r;
                float prism = SpectraFixturePrism();
                if (prism > 0.33 && _SpectraShaderQualityTier >= 2.0)
                {
                    gobo = max(gobo, tex2D(_GoboAtlas, frac(goboUv + float2(0.06, 0.0))).r);
                    gobo = max(gobo, tex2D(_GoboAtlas, frac(goboUv - float2(0.06, 0.0))).r);
                }
                if (prism > 0.66 && _SpectraShaderQualityTier >= 3.0)
                {
                    gobo = max(gobo, tex2D(_GoboAtlas, frac(goboUv + float2(0.0, 0.06))).r);
                    gobo = max(gobo, tex2D(_GoboAtlas, frac(goboUv - float2(0.0, 0.06))).r);
                }

                float depthFade = saturate(1.0 - abs(i.objectPos.z) * 0.05);
                float alpha = edge * noise * gobo * depthFade * dimmer * strobe * _Density;
                float intensity = alpha * _BeamPower * SpectraBeamMultiplier();

                return fixed4(color * intensity, alpha);
            }
            ENDCG
        }
    }

    Fallback Off
}
