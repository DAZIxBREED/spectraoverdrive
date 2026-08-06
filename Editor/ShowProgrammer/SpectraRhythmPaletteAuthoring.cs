using UnityEngine;

namespace SpectraOverdrive.Editor
{
    public static class SpectraRhythmPaletteAuthoring
    {
        public static void ApplyEuclideanGate(SpectraCueBlock cue, int pulses, int steps)
        {
            if (cue == null) return;
            cue.gatePattern = SpectraCueGatePattern.Euclidean;
            cue.gateTimeBase = SpectraModulationTimeBase.Beats;
            cue.gateStepLength = 0.25f;
            cue.gateStepCount = Mathf.Clamp(steps, 1, 32);
            cue.gateActiveSteps = Mathf.Clamp(pulses, 1, cue.gateStepCount);
            cue.gateDutyCycle = 0.7f;
            cue.gateAttack = 0.015f;
            cue.gateRelease = 0.06f;
            cue.gatePhase = 0f;
            cue.gateInvert = false;
        }

        public static void ApplySyncopatedMask(SpectraCueBlock cue)
        {
            if (cue == null) return;
            cue.gatePattern = SpectraCueGatePattern.CustomMask;
            cue.gateTimeBase = SpectraModulationTimeBase.Beats;
            cue.gateStepLength = 0.25f;
            cue.gateStepCount = 16;
            cue.gateActiveSteps = 9;
            cue.gateCustomMask = 0x5B4D;
            cue.gateDutyCycle = 0.64f;
            cue.gateAttack = 0.01f;
            cue.gateRelease = 0.05f;
            cue.gatePhase = 0f;
            cue.gateInvert = false;
        }

        public static void ApplySeededGate(SpectraCueBlock cue)
        {
            if (cue == null) return;
            cue.gatePattern = SpectraCueGatePattern.SeededRandom;
            cue.gateTimeBase = SpectraModulationTimeBase.Beats;
            cue.gateStepLength = 0.25f;
            cue.gateStepCount = 16;
            cue.gateActiveSteps = 7;
            cue.gateDutyCycle = 0.72f;
            cue.gateAttack = 0.02f;
            cue.gateRelease = 0.07f;
            if (cue.randomSeed == 0) cue.randomSeed = 130130;
        }

        public static void ClearGate(SpectraCueBlock cue)
        {
            if (cue == null) return;
            cue.gatePattern = SpectraCueGatePattern.Disabled;
            cue.gateTimeBase = SpectraModulationTimeBase.Beats;
            cue.gateStepLength = 0.25f;
            cue.gateStepCount = 8;
            cue.gateActiveSteps = 4;
            cue.gateDutyCycle = 0.72f;
            cue.gateAttack = 0.02f;
            cue.gateRelease = 0.06f;
            cue.gatePhase = 0f;
            cue.gateCustomMask = -1;
            cue.gateInvert = false;
        }

        public static void ApplyPaletteStep(SpectraCueBlock cue, int paletteIndex)
        {
            if (cue == null) return;
            cue.valueType = SpectraCueValueType.Color;
            cue.paletteIndex = Mathf.Max(0, paletteIndex);
            cue.paletteMode = SpectraPalettePlaybackMode.Step;
            cue.paletteTimeBase = SpectraModulationTimeBase.Beats;
            cue.paletteStepLength = 1f;
            cue.palettePhase = 0f;
            cue.palettePrimaryIndex = 0;
            cue.paletteSecondaryIndex = 1;
            cue.paletteMacroIndex = -1;
            cue.paletteBlend = 1f;
        }

        public static void ApplyPaletteMacroMorph(SpectraCueBlock cue, int paletteIndex, int macroIndex)
        {
            if (cue == null) return;
            cue.valueType = SpectraCueValueType.Color;
            cue.paletteIndex = Mathf.Max(0, paletteIndex);
            cue.paletteMode = SpectraPalettePlaybackMode.MacroMorph;
            cue.palettePrimaryIndex = 0;
            cue.paletteSecondaryIndex = 1;
            cue.paletteMacroIndex = Mathf.Clamp(macroIndex, 0, 3);
            cue.paletteBlend = 1f;
        }

        public static void ClearPalette(SpectraCueBlock cue)
        {
            if (cue == null) return;
            cue.paletteIndex = -1;
            cue.paletteMode = SpectraPalettePlaybackMode.Disabled;
            cue.paletteTimeBase = SpectraModulationTimeBase.Beats;
            cue.paletteStepLength = 1f;
            cue.palettePhase = 0f;
            cue.palettePrimaryIndex = 0;
            cue.paletteSecondaryIndex = 1;
            cue.paletteMacroIndex = -1;
            cue.paletteBlend = 1f;
        }

        public static void EnsureStarterPalettes(SpectraShowAsset show)
        {
            if (show == null || (show.colorPalettes != null && show.colorPalettes.Length > 0)) return;
            show.colorPalettes = new[]
            {
                new SpectraColorPalette
                {
                    name = "Overdrive Neon",
                    description = "High-contrast magenta, ultraviolet, cyan, and white performance colors.",
                    colors = new[]
                    {
                        new Color(1f, 0.05f, 0.65f, 1f),
                        new Color(0.42f, 0.08f, 1f, 1f),
                        new Color(0.05f, 0.85f, 1f, 1f),
                        Color.white
                    }
                },
                new SpectraColorPalette
                {
                    name = "Industrial Heat",
                    description = "Amber, red, white-hot, and deep violet colors for heavy drops.",
                    colors = new[]
                    {
                        new Color(1f, 0.18f, 0.02f, 1f),
                        new Color(1f, 0.55f, 0.04f, 1f),
                        new Color(1f, 0.95f, 0.72f, 1f),
                        new Color(0.22f, 0.02f, 0.4f, 1f)
                    }
                }
            };
        }
    }
}
