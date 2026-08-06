Shader "SpectraOverdrive/Demo/FixtureEmissive"
{
    Properties
    {
        _BaseColor ("Base Color", Color) = (1,1,1,1)
        _EmissionPower ("Emission Power", Range(0,8)) = 2
    }

    SubShader
    {
        Tags { "Queue"="Geometry" "RenderType"="Opaque" }
        Pass
        {
            CGPROGRAM
            #pragma target 3.0
            #pragma vertex vert
            #pragma fragment frag
            #include "UnityCG.cginc"
            #include "Includes/SpectraCommon.cginc"

            float4 _BaseColor;
            float _EmissionPower;
            float4 _SpectraGroupColor;
            float4 _SpectraGroupMotion;
            float4 _SpectraGroupOptics;
            float4 _SpectraGroupEffects;

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
                float zoom = _SpectraGroupOptics.w < 0.0 ? 0.5 : saturate(_SpectraGroupOptics.w);
                float3 p = v.vertex.xyz;
                p.xy *= lerp(1.2, 0.65, zoom);
                o.pos = UnityObjectToClipPos(float4(p, 1));
                o.uv = v.uv;
                return o;
            }

            fixed4 frag(v2f i) : SV_Target
            {
                float intensity = max(0.0, _SpectraGroupMotion.w) * SpectraEffectiveMaster();
                float prism = saturate(_SpectraGroupOptics.z);
                float stripe = 0.75 + 0.25 * sin((i.uv.x + i.uv.y) * (8.0 + prism * 24.0));
                float3 color = _BaseColor.rgb * _SpectraGroupColor.rgb;
                color = lerp(color, color.brg, prism * 0.35);
                return fixed4(color * intensity * _EmissionPower * stripe, 1);
            }
            ENDCG
        }
    }
    Fallback "Unlit/Color"
}
