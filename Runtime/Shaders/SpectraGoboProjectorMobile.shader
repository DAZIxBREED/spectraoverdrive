Shader "SpectraOverdrive/Mobile/GoboProjector"
{
    Properties
    {
        _GoboAtlas ("Gobo Atlas", 2D) = "white" {}
        _BaseColor ("Fallback Color", Color) = (1,1,1,1)
        _GoboColumns ("Gobo Columns", Float) = 4
        _GoboRows ("Gobo Rows", Float) = 2
        _ProjectionPower ("Projection Power", Range(0,4)) = 1
        _Softness ("Edge Softness", Range(0.001,0.5)) = 0.1
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

            sampler2D _GoboAtlas;
            float4 _BaseColor;
            float _GoboColumns;
            float _GoboRows;
            float _ProjectionPower;
            float _Softness;

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

            v2f vert(appdata v)
            {
                v2f o;
                float2 panTilt = SpectraFixturePanTilt01();
                float3 p = v.vertex.xyz;
                p.xy += (panTilt - 0.5) * 0.5;
                o.pos = UnityObjectToClipPos(float4(p,1));
                o.uv = v.uv;
                return o;
            }

            fixed4 frag(v2f i) : SV_Target
            {
                float index = SpectraFixtureGoboIndex();
                float columns = max(1.0, _GoboColumns);
                float rows = max(1.0, _GoboRows);

                float cellX = fmod(index, columns);
                float cellY = floor(index / columns);

                float angle = SpectraFixtureGoboRotation();
                float2 centered = i.uv - 0.5;
                float s = sin(angle);
                float c = cos(angle);
                centered = float2(c * centered.x - s * centered.y, s * centered.x + c * centered.y);
                float2 uv = centered + 0.5;

                float2 atlasUv = float2(
                    (uv.x + cellX) / columns,
                    (uv.y + cellY) / rows
                );

                float gobo = tex2D(_GoboAtlas, atlasUv).r;
                float2 edgeDistance = abs(i.uv - 0.5) * 2.0;
                float edge = 1.0 - smoothstep(1.0 - _Softness, 1.0, max(edgeDistance.x, edgeDistance.y));

                float dimmer = SpectraFixtureDimmer();
                float3 dmxColor = SpectraFixtureColor();
                float colorPresence = step(0.001, dot(dmxColor, 1.0));
                float3 color = lerp(_BaseColor.rgb, dmxColor, colorPresence);

                float alpha = gobo * edge * dimmer * _ProjectionPower * SpectraProjectionMultiplier();
                return fixed4(color * alpha, alpha);
            }
            ENDCG
        }
    }

    Fallback Off
}
