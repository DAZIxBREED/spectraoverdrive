#ifndef SPECTRA_AUDIOLINK_INCLUDED
#define SPECTRA_AUDIOLINK_INCLUDED

sampler2D _SpectraAudioTexture;
float4 _SpectraAudioSettings;
float4 _SpectraAudioFallbackBands;
float _SpectraAudioFallbackOverall;
float _SpectraUseColorChord;
float _SpectraUseThemeColors;
float4 _SpectraAudioUv0;
float4 _SpectraAudioUv1;
float4 _SpectraAudioUv2;
float4 _SpectraAudioUv3;

inline float2 SpectraBandUv(int band)
{
    if (band == 0) return _SpectraAudioUv0.xy;
    if (band == 1) return _SpectraAudioUv0.zw;
    if (band == 2) return _SpectraAudioUv1.xy;
    if (band == 3) return _SpectraAudioUv1.zw;
    return _SpectraAudioUv2.xy;
}

inline float SpectraSampleAudioBand(int band)
{
    float2 uv = SpectraBandUv(band);
    float sampled = tex2Dlod(_SpectraAudioTexture, float4(uv, 0, 0)).r;

    float fallback = 0.0;
    if (band == 0) fallback = _SpectraAudioFallbackBands.x;
    else if (band == 1) fallback = _SpectraAudioFallbackBands.y;
    else if (band == 2) fallback = _SpectraAudioFallbackBands.z;
    else if (band == 3) fallback = _SpectraAudioFallbackBands.w;
    else fallback = _SpectraAudioFallbackOverall;

    return saturate(max(sampled, fallback) * _SpectraAudioSettings.x);
}

inline float3 SpectraSampleAudioColor()
{
    float3 sampled = tex2Dlod(_SpectraAudioTexture, float4(_SpectraAudioUv2.zw, 0, 0)).rgb;
    float presence = step(0.001, dot(sampled, 1.0));
    return lerp(float3(1,1,1), sampled, presence);
}

inline float3 SpectraSampleThemeColor()
{
    float3 sampled = tex2Dlod(_SpectraAudioTexture, float4(_SpectraAudioUv3.xy, 0, 0)).rgb;
    float presence = step(0.001, dot(sampled, 1.0));
    return lerp(float3(1,1,1), sampled, presence);
}

#endif
