#ifndef SPECTRA_DMX_DECODE_INCLUDED
#define SPECTRA_DMX_DECODE_INCLUDED

sampler2D _SpectraDmxTexture;
float4 _SpectraDmxLayout;
float4 _SpectraDmxLayoutFlags;
float4 _SpectraDmxUvFlags;
float4 _SpectraDmxSignal;

inline float2 SpectraApplyUvFlags(float2 uv)
{
    if (_SpectraDmxUvFlags.x > 0.5) uv.x = 1.0 - uv.x;
    if (_SpectraDmxUvFlags.y > 0.5) uv.y = 1.0 - uv.y;
    return uv;
}

inline float2 SpectraDmxUvHorizontal(int universe, int channel)
{
    float width = max(1.0, _SpectraDmxLayout.z);
    float height = max(1.0, _SpectraDmxLayout.w);
    float offset = _SpectraDmxUvFlags.z > 0.5 ? 0.5 : 0.0;
    return SpectraApplyUvFlags(float2((channel - 1.0 + offset) / width, (universe - 1.0 + offset) / height));
}

inline float2 SpectraDmxUvVertical(int universe, int channel)
{
    float width = max(1.0, _SpectraDmxLayout.z);
    float height = max(1.0, _SpectraDmxLayout.w);
    float offset = _SpectraDmxUvFlags.z > 0.5 ? 0.5 : 0.0;
    return SpectraApplyUvFlags(float2((universe - 1.0 + offset) / width, (channel - 1.0 + offset) / height));
}

inline float2 SpectraDmxUvLegacy(int universe, int channel)
{
    float width = max(1.0, _SpectraDmxLayout.z);
    float height = max(1.0, _SpectraDmxLayout.w);
    float sectorWidth = max(1.0, _SpectraDmxLayoutFlags.y);
    float sectorHeight = max(1.0, _SpectraDmxLayoutFlags.z);
    float sectorsPerRow = max(1.0, _SpectraDmxLayoutFlags.w);

    float zeroChannel = max(0.0, channel - 1.0);
    float sectorIndex = floor(zeroChannel / (sectorWidth * sectorHeight));
    float localIndex = fmod(zeroChannel, sectorWidth * sectorHeight);
    float sectorX = fmod(sectorIndex, sectorsPerRow);
    float sectorY = floor(sectorIndex / sectorsPerRow);

    float localX = fmod(localIndex, sectorWidth);
    float localY = floor(localIndex / sectorWidth);

    float2 pixel = float2(
        sectorX * sectorWidth + localX + 0.5,
        (universe - 1.0) * sectorHeight + sectorY * sectorHeight + localY + 0.5
    );

    return SpectraApplyUvFlags(pixel / float2(width, height));
}

inline float2 SpectraDmxUv(int universe, int channel)
{
    float mode = _SpectraDmxLayout.x;
    if (mode < 0.5) return SpectraDmxUvHorizontal(universe, channel);
    if (mode < 1.5) return SpectraDmxUvVertical(universe, channel);
    if (mode < 2.5) return SpectraDmxUvLegacy(universe, channel);
    return SpectraDmxUvHorizontal(universe, channel);
}

inline float SpectraSelectPackedChannel(float4 sampleValue, int universe)
{
    float packing = _SpectraDmxLayoutFlags.x;

    if (packing < 0.5)
    {
        return sampleValue.r;
    }

    if (packing < 1.5)
    {
        return sampleValue.r;
    }

    int packedIndex = (universe - 1) % 3;
    if (packedIndex == 0) return sampleValue.r;
    if (packedIndex == 1) return sampleValue.g;
    return sampleValue.b;
}

inline float SpectraSampleDmxChannel(int universe, int channel)
{
    if (channel < 1)
    {
        return 0.0;
    }

    float2 uv = SpectraDmxUv(universe, channel);
    float4 sampleValue = tex2Dlod(_SpectraDmxTexture, float4(uv, 0, 0));
    return saturate(SpectraSelectPackedChannel(sampleValue, universe) * _SpectraDmxSignal.y);
}

inline float SpectraSampleDmx16(int universe, int coarseChannel, int fineChannel)
{
    float coarse = SpectraSampleDmxChannel(universe, coarseChannel);
    float fine = SpectraSampleDmxChannel(universe, fineChannel);
    return saturate(coarse + fine / 255.0);
}

#endif
