#ifndef SPECTRA_FIXTURE_CONTROL_INCLUDED
#define SPECTRA_FIXTURE_CONTROL_INCLUDED

#include "SpectraDmxDecode.cginc"
#include "SpectraAudioLink.cginc"

float _SpectraUniverse;
float _SpectraStartAddress;
float4 _SpectraChannelMap0;
float4 _SpectraChannelMap1;
float4 _SpectraChannelMap2;
float4 _SpectraFixtureCalibration;
float4 _SpectraMovementCalibration;
float4 _SpectraGroupMotion;
float4 _SpectraGroupColor;
float4 _SpectraGroupOptics;
float4 _SpectraGroupEffects;
float4 _SpectraGroupAudio;
float _SpectraGoboCount;

inline float SpectraGroupAudioMultiplier()
{
    if (_SpectraGroupAudio.w < 0.5 || _SpectraGroupAudio.x < 0.0) return 1.0;
    float band = SpectraSampleAudioBand((int)_SpectraGroupAudio.x);
    return saturate(_SpectraGroupAudio.z + band * _SpectraGroupAudio.y);
}

inline float SpectraFixtureDimmer()
{
    float dimmer = SpectraSampleDmxChannel((int)_SpectraUniverse, (int)_SpectraChannelMap0.x);
    return dimmer * _SpectraFixtureCalibration.x * _SpectraGroupMotion.w * SpectraGroupAudioMultiplier();
}

inline float3 SpectraFixtureColor()
{
    float3 rgb = float3(
        SpectraSampleDmxChannel((int)_SpectraUniverse, (int)_SpectraChannelMap0.y),
        SpectraSampleDmxChannel((int)_SpectraUniverse, (int)_SpectraChannelMap0.z),
        SpectraSampleDmxChannel((int)_SpectraUniverse, (int)_SpectraChannelMap0.w)
    );
    return rgb * _SpectraGroupColor.rgb;
}

inline float2 SpectraFixturePanTilt01()
{
    float pan = SpectraSampleDmx16((int)_SpectraUniverse, (int)_SpectraChannelMap1.x, (int)_SpectraChannelMap1.y);
    float tilt = SpectraSampleDmx16((int)_SpectraUniverse, (int)_SpectraChannelMap1.z, (int)_SpectraChannelMap1.w);

    if (_SpectraMovementCalibration.z > 0.5) pan = 1.0 - pan;
    if (_SpectraMovementCalibration.w > 0.5) tilt = 1.0 - tilt;

    pan = saturate((pan - 0.5) * _SpectraGroupMotion.z + 0.5 + _SpectraGroupMotion.x);
    tilt = saturate((tilt - 0.5) * _SpectraGroupMotion.z + 0.5 + _SpectraGroupMotion.y);

    return float2(pan, tilt);
}

inline float SpectraFixtureStrobeMask()
{
    float raw = SpectraSampleDmxChannel((int)_SpectraUniverse, (int)_SpectraChannelMap2.x);
    float groupHz = max(0.0, _SpectraGroupEffects.x);
    if (_SpectraDisableStrobes > 0.5 || (raw <= 0.001 && groupHz <= 0.001))
    {
        return 1.0;
    }

    float hz = groupHz > 0.001 ? groupHz : lerp(1.0, 18.0, raw);
    float pulse = frac(_SpectraShowTime * hz);
    return pulse < 0.5 ? 1.0 : 0.0;
}

inline float SpectraFixtureZoom()
{
    if (_SpectraGroupOptics.w >= 0.0) return saturate(_SpectraGroupOptics.w);
    return SpectraSampleDmxChannel((int)_SpectraUniverse, (int)_SpectraChannelMap2.y);
}

inline float SpectraFixtureGoboIndex()
{
    if (_SpectraGroupOptics.x >= 0.0)
        return min(_SpectraGroupOptics.x, max(0.0, _SpectraGoboCount - 1.0));
    float raw = SpectraSampleDmxChannel((int)_SpectraUniverse, (int)_SpectraChannelMap2.z);
    return floor(raw * max(1.0, _SpectraGoboCount));
}

inline float SpectraFixtureGoboRotation()
{
    if (abs(_SpectraGroupOptics.y) > 0.0001)
        return _SpectraGroupOptics.y * _SpectraShowTime;
    float raw = SpectraSampleDmxChannel((int)_SpectraUniverse, (int)_SpectraChannelMap2.w);
    return (raw * 2.0 - 1.0) * _SpectraShowTime;
}

inline float SpectraFixturePrism()
{
    if (_SpectraGroupOptics.z > 0.001) return saturate(_SpectraGroupOptics.z);
    return SpectraSampleDmxChannel((int)_SpectraUniverse, (int)(_SpectraChannelMap2.w + 1));
}

inline float SpectraFixtureFocus()
{
    return _SpectraGroupEffects.z >= 0.0 ? saturate(_SpectraGroupEffects.z) : 0.5;
}

inline float SpectraFixtureLaserEnabled()
{
    return _SpectraGroupEffects.y;
}

#endif
