Shader "SpectraOverdrive/Mobile/StaticFixture"
{
    Properties
    {
        _BaseColor ("Fallback Color", Color) = (1,1,1,1)
        _MainTex ("Lens Texture", 2D) = "white" {}
        _EmissionPower ("Emission Power", Range(0,8)) = 1
        _AudioBand ("Audio Band", Range(0,4)) = 4
        _AudioAmount ("Audio Amount", Range(0,2)) = 0
    }

    SubShader
    {
        Tags { "RenderType"="Opaque" }
        LOD 100

        Pass
        {
            CGPROGRAM
            #pragma target 3.0
            #pragma vertex vert
            #pragma fragment frag
            #include "UnityCG.cginc"
            #include "Includes/SpectraCommon.cginc"
            #include "Includes/SpectraFixtureControl.cginc"
            #include "Includes/SpectraAudioLink.cginc"

            sampler2D _MainTex;
            float4 _MainTex_ST;
            float4 _BaseColor;
            float _EmissionPower;
            float _AudioBand;
            float _AudioAmount;

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
                o.pos = UnityObjectToClipPos(v.vertex);
                o.uv = TRANSFORM_TEX(v.uv, _MainTex);
                return o;
            }

            fixed4 frag(v2f i) : SV_Target
            {
                float dimmer = SpectraFixtureDimmer();
                float strobe = SpectraFixtureStrobeMask();
                float3 dmxColor = SpectraFixtureColor();
                float colorPresence = step(0.001, dot(dmxColor, 1.0));
                float3 color = lerp(_BaseColor.rgb, dmxColor, colorPresence);

                float audio = SpectraSampleAudioBand((int)_AudioBand);
                float intensity = dimmer * strobe * _EmissionPower * lerp(1.0, audio, _AudioAmount);
                fixed4 tex = tex2D(_MainTex, i.uv);

                return fixed4(tex.rgb * color * intensity * SpectraEffectiveMaster(), tex.a);
            }
            ENDCG
        }
    }

    Fallback Off
}
