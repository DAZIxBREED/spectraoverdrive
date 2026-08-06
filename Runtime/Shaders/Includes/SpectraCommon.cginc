#ifndef SPECTRA_COMMON_INCLUDED
#define SPECTRA_COMMON_INCLUDED

float4 _SpectraGlobalState;
float4 _SpectraAccessibility;
float _SpectraDisableStrobes;
float _SpectraReducedMotion;
float _SpectraShowTime;
float _SpectraShaderQualityTier;
float _SpectraAudioReactiveUpdateDivider;

inline float SpectraEffectiveMaster()
{
    return saturate(_SpectraGlobalState.x) * saturate(_SpectraAccessibility.x);
}

inline float SpectraBeamMultiplier()
{
    return SpectraEffectiveMaster() * saturate(_SpectraAccessibility.y);
}

inline float SpectraProjectionMultiplier()
{
    return SpectraEffectiveMaster() * saturate(_SpectraAccessibility.z);
}

inline float SpectraLaserMultiplier()
{
    return SpectraEffectiveMaster() * saturate(_SpectraAccessibility.w);
}

inline float SpectraBayer4x4(float2 pixel)
{
    int2 p = int2(fmod(pixel.x, 4.0), fmod(pixel.y, 4.0));
    const float matrix[16] = {
        0.0, 8.0, 2.0, 10.0,
        12.0, 4.0, 14.0, 6.0,
        3.0, 11.0, 1.0, 9.0,
        15.0, 7.0, 13.0, 5.0
    };
    return (matrix[p.y * 4 + p.x] + 0.5) / 16.0;
}

#endif
