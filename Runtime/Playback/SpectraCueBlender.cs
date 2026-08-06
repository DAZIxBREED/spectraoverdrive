using UnityEngine;

namespace SpectraOverdrive
{
    public static class SpectraCueBlender
    {
        public static float BlendFloat(float current, float value, float weight, SpectraCueBlendMode mode)
        {
            weight = Mathf.Clamp01(weight);
            if (mode == SpectraCueBlendMode.Add) return current + value * weight;
            if (mode == SpectraCueBlendMode.Multiply) return current * Mathf.Lerp(1f, value, weight);
            if (mode == SpectraCueBlendMode.Maximum) return Mathf.Max(current, value * weight);
            if (mode == SpectraCueBlendMode.Minimum) return Mathf.Min(current, Mathf.Lerp(current, value, weight));
            return Mathf.Lerp(current, value, weight);
        }

        public static Color BlendColor(Color current, Color value, float weight, SpectraCueBlendMode mode)
        {
            weight = Mathf.Clamp01(weight);
            if (mode == SpectraCueBlendMode.Add) return current + value * weight;
            if (mode == SpectraCueBlendMode.Multiply) return current * Color.Lerp(Color.white, value, weight);
            if (mode == SpectraCueBlendMode.Maximum)
                return new Color(Mathf.Max(current.r, value.r * weight), Mathf.Max(current.g, value.g * weight), Mathf.Max(current.b, value.b * weight), 1f);
            if (mode == SpectraCueBlendMode.Minimum)
                return new Color(Mathf.Min(current.r, value.r), Mathf.Min(current.g, value.g), Mathf.Min(current.b, value.b), 1f);
            return Color.Lerp(current, value, weight);
        }
    }
}
