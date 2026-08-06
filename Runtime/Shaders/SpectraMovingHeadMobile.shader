Shader "SpectraOverdrive/Mobile/MovingHeadBeam"
{
    Properties
    {
        _MainTex ("Beam Noise", 2D) = "white" {}
        _GoboAtlas ("Gobo Atlas", 2D) = "white" {}
        _BaseColor ("Fallback Color", Color) = (1,1,1,1)
        _BeamPower ("Beam Power", Range(0,4)) = 1
        _EdgePower ("Edge Power", Range(0.25,8)) = 2
        _DitherStrength ("Dither Strength", Range(0,1)) = 0.65
        _GoboColumns ("Gobo Columns", Float) = 4
        _GoboRows ("Gobo Rows", Float) = 2
        _NoiseScroll ("Noise Scroll", Vector) = (0,0.1,0,0)
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
            #include "Includes/SpectraFixtureControl.cginc"

            sampler2D _MainTex;
            float4 _MainTex_ST;
            sampler2D _GoboAtlas;
            float4 _GoboAtlas_ST;
            float4 _BaseColor;
            float _BeamPower;
            float _EdgePower;
            float _DitherStrength;
            float _GoboColumns;
            float _GoboRows;
            float4 _NoiseScroll;

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
                UNITY_SETUP_INSTANCE_ID(v);
                UNITY_INITIALIZE_VERTEX_OUTPUT_STEREO(o);

                float2 panTilt = SpectraFixturePanTilt01();
                float panRadians = radians((panTilt.x - 0.5) * _SpectraMovementCalibration.x);
                float tiltRadians = radians((panTilt.y - 0.5) * _SpectraMovementCalibration.y);

                float zoom = SpectraFixtureZoom();
                float3 p = v.vertex.xyz;
                p.x *= lerp(1.15, 0.35, zoom);
                p.y *= lerp(1.15, 0.35, zoom);

                p = RotateX(p, tiltRadians);
                p = RotateY(p, panRadians);

                o.pos = UnityObjectToClipPos(float4(p, 1.0));
                o.uv = TRANSFORM_TEX(v.uv, _MainTex);
                o.screenPos = ComputeScreenPos(o.pos);
                return o;
            }

            fixed4 frag(v2f i) : SV_Target
            {
                float dimmer = SpectraFixtureDimmer();
                float strobe = SpectraFixtureStrobeMask();
                float3 dmxColor = SpectraFixtureColor();
                float colorPresence = step(0.001, dot(dmxColor, 1.0));
                float3 color = lerp(_BaseColor.rgb, dmxColor, colorPresence);

                float2 animatedUv = i.uv + (_NoiseScroll.xy * _SpectraShowTime);
                float noise = tex2D(_MainTex, animatedUv).r;

                float goboIndex = SpectraFixtureGoboIndex();
                float goboColumns = max(1.0, _GoboColumns);
                float goboRows = max(1.0, _GoboRows);
                float cellX = fmod(goboIndex, goboColumns);
                float cellY = floor(goboIndex / goboColumns);

                float angle = SpectraFixtureGoboRotation();
                float2 centered = i.uv - 0.5;
                float s = sin(angle);
                float c = cos(angle);
                centered = float2(c * centered.x - s * centered.y, s * centered.x + c * centered.y);
                float2 rotatedUv = centered + 0.5;

                float2 goboUv = float2(
                    (rotatedUv.x + cellX) / goboColumns,
                    (rotatedUv.y + cellY) / goboRows
                );

                float gobo = tex2D(_GoboAtlas, goboUv).r;
                float prism = SpectraFixturePrism();
                if (prism > 0.33 && _SpectraShaderQualityTier >= 2.0)
                {
                    float2 prismUvA = frac(goboUv + float2(0.07, 0.0));
                    float2 prismUvB = frac(goboUv + float2(-0.07, 0.0));
                    gobo = max(gobo, max(tex2D(_GoboAtlas, prismUvA).r, tex2D(_GoboAtlas, prismUvB).r));
                }
                if (prism > 0.66 && _SpectraShaderQualityTier >= 3.0)
                {
                    float2 prismUvC = frac(goboUv + float2(0.0, 0.07));
                    float2 prismUvD = frac(goboUv + float2(0.0, -0.07));
                    gobo = max(gobo, max(tex2D(_GoboAtlas, prismUvC).r, tex2D(_GoboAtlas, prismUvD).r));
                }
                float center = saturate(1.0 - abs(i.uv.x * 2.0 - 1.0));
                float edge = pow(center, _EdgePower);

                float intensity = dimmer * strobe * _BeamPower * _SpectraFixtureCalibration.y * SpectraBeamMultiplier();
                float alpha = saturate(edge * noise * gobo * intensity);

                float2 pixel = (i.screenPos.xy / max(i.screenPos.w, 0.0001)) * _ScreenParams.xy;
                float threshold = SpectraBayer4x4(pixel);
                clip(alpha - lerp(0.02, threshold, _DitherStrength));

                return fixed4(color * intensity * alpha, alpha);
            }
            ENDCG
        }
    }

    Fallback Off
}
