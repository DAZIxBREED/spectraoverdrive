using System;
using UnityEngine;

namespace SpectraOverdrive.Editor
{
    public static class SpectraAutomationAuthoring
    {
        public static void ApplyPulseEnvelope(SpectraCueBlock cue)
        {
            if (cue == null) throw new ArgumentNullException("cue");
            cue.automationMode = SpectraAutomationMode.Multiply;
            cue.automationKeys = new[]
            {
                Key(0f, Vector4.zero, SpectraAutomationInterpolation.Smooth),
                Key(0.12f, Vector4.one, SpectraAutomationInterpolation.Smooth),
                Key(0.72f, Vector4.one, SpectraAutomationInterpolation.Smooth),
                Key(1f, Vector4.zero, SpectraAutomationInterpolation.Smooth)
            };
        }

        public static void ApplyRiserEnvelope(SpectraCueBlock cue)
        {
            if (cue == null) throw new ArgumentNullException("cue");
            cue.automationMode = SpectraAutomationMode.Multiply;
            cue.automationKeys = new[]
            {
                Key(0f, new Vector4(0.05f, 0.05f, 0.05f, 1f), SpectraAutomationInterpolation.Smooth),
                Key(0.5f, new Vector4(0.45f, 0.45f, 0.45f, 1f), SpectraAutomationInterpolation.Smooth),
                Key(0.85f, new Vector4(0.82f, 0.82f, 0.82f, 1f), SpectraAutomationInterpolation.Smooth),
                Key(1f, Vector4.one, SpectraAutomationInterpolation.Linear)
            };
        }

        public static void ApplyFourBeatGate(SpectraCueBlock cue)
        {
            if (cue == null) throw new ArgumentNullException("cue");
            cue.automationMode = SpectraAutomationMode.Multiply;
            cue.automationKeys = new[]
            {
                Key(0f, Vector4.one, SpectraAutomationInterpolation.Step),
                Key(0.125f, Vector4.zero, SpectraAutomationInterpolation.Step),
                Key(0.25f, Vector4.one, SpectraAutomationInterpolation.Step),
                Key(0.375f, Vector4.zero, SpectraAutomationInterpolation.Step),
                Key(0.5f, Vector4.one, SpectraAutomationInterpolation.Step),
                Key(0.625f, Vector4.zero, SpectraAutomationInterpolation.Step),
                Key(0.75f, Vector4.one, SpectraAutomationInterpolation.Step),
                Key(0.875f, Vector4.zero, SpectraAutomationInterpolation.Step),
                Key(1f, Vector4.one, SpectraAutomationInterpolation.Step)
            };
        }

        public static void Clear(SpectraCueBlock cue)
        {
            if (cue == null) throw new ArgumentNullException("cue");
            cue.automationMode = SpectraAutomationMode.Disabled;
            cue.automationKeys = new SpectraAutomationKey[0];
        }

        private static SpectraAutomationKey Key(
            float normalizedTime,
            Vector4 value,
            SpectraAutomationInterpolation interpolation)
        {
            return new SpectraAutomationKey
            {
                normalizedTime = Mathf.Clamp01(normalizedTime),
                value = value,
                interpolation = interpolation
            };
        }
    }
}
