using System;
using UnityEngine;

namespace SpectraOverdrive
{
    [Serializable]
    public class SpectraAutomationKey
    {
        [Range(0f, 1f)] public float normalizedTime;
        public Vector4 value = Vector4.one;
        public SpectraAutomationInterpolation interpolation = SpectraAutomationInterpolation.Smooth;
    }

    [Serializable]
    public class SpectraCueBlock
    {
        public string id;
        public string name = "Cue";
        public bool enabled = true;
        public SpectraTimingMode timingMode;
        [Min(0f)] public float startSeconds;
        [Min(0f)] public float durationSeconds = 1f;
        public SpectraMusicalPosition startMusical = new SpectraMusicalPosition(1, 1, 0f);
        [Min(0f)] public float durationBeats = 1f;
        [Min(0f)] public float fadeIn;
        [Min(0f)] public float fadeOut;
        public SpectraCueEasing easing;
        public SpectraCueValueType valueType;
        public SpectraCueBlendMode blendMode;
        public SpectraAutomationMode automationMode;
        [Tooltip("Normalized, editor-authored value envelope. Keys are flattened by the show compiler.")]
        public SpectraAutomationKey[] automationKeys = new SpectraAutomationKey[0];
        [Header("Deterministic procedural modulation")]
        public SpectraModulationWaveform modulationWaveform;
        public SpectraModulationTimeBase modulationTimeBase = SpectraModulationTimeBase.Beats;
        public SpectraAutomationMode modulationMode = SpectraAutomationMode.Multiply;
        [Min(0.0001f)] public float modulationCycleLength = 1f;
        public float modulationPhase;
        [Range(0.01f, 0.99f)] public float modulationDutyCycle = 0.5f;
        [Range(0, 32)] public int modulationQuantizeSteps;
        public Vector4 modulationOffset = Vector4.one;
        public Vector4 modulationDepth = Vector4.zero;
        [Header("Synchronized performance macro")]
        [Range(-1, 3)] public int performanceMacroIndex = -1;
        public SpectraAutomationMode performanceMacroMode = SpectraAutomationMode.Multiply;
        public Vector4 performanceMacroMinimum = Vector4.one;
        public Vector4 performanceMacroMaximum = Vector4.one;
        [Header("Deterministic rhythm gate")]
        public SpectraCueGatePattern gatePattern;
        public SpectraModulationTimeBase gateTimeBase = SpectraModulationTimeBase.Beats;
        [Min(0.0001f)] public float gateStepLength = 0.25f;
        [Range(1, 32)] public int gateStepCount = 8;
        [Range(1, 32)] public int gateActiveSteps = 4;
        [Range(0.01f, 0.99f)] public float gateDutyCycle = 0.72f;
        [Range(0f, 0.49f)] public float gateAttack = 0.02f;
        [Range(0f, 0.49f)] public float gateRelease = 0.06f;
        public float gatePhase;
        [Tooltip("32-bit step mask used by Custom Mask. Bit 0 is step 1. -1 enables all steps.")]
        public int gateCustomMask = -1;
        public bool gateInvert;
        [Header("Dynamic color palette")]
        [Range(-1, 15)] public int paletteIndex = -1;
        public SpectraPalettePlaybackMode paletteMode;
        public SpectraModulationTimeBase paletteTimeBase = SpectraModulationTimeBase.Beats;
        [Min(0.0001f)] public float paletteStepLength = 1f;
        public float palettePhase;
        [Range(0, 15)] public int palettePrimaryIndex;
        [Range(0, 15)] public int paletteSecondaryIndex = 1;
        [Range(-1, 3)] public int paletteMacroIndex = -1;
        [Range(0f, 1f)] public float paletteBlend = 1f;
        [Header("Deterministic cue condition")]
        public SpectraCueConditionMode conditionMode;
        public SpectraModulationTimeBase conditionTimeBase = SpectraModulationTimeBase.Beats;
        [Min(0.0001f)] public float conditionCycleLength = 1f;
        public float conditionPhase;
        [Range(0f, 1f)] public float conditionProbability = 0.5f;
        [Range(1, 32)] public int conditionEveryN = 2;
        [Range(0, 31)] public int conditionCycleOffset;
        [Range(-1, 3)] public int conditionMacroIndex = -1;
        public SpectraAudioBand conditionAudioBand = SpectraAudioBand.Bass;
        [Range(0f, 1f)] public float conditionThreshold = 0.5f;
        public bool conditionInvert;
        [Header("Synchronized variation group")]
        public SpectraVariationSelectionMode variationMode;
        [Range(-1, 15)] public int variationGroup = -1;
        [Range(0, 7)] public int variationOption;
        [Range(2, 8)] public int variationOptionCount = 2;
        public SpectraModulationTimeBase variationTimeBase = SpectraModulationTimeBase.Bars;
        [Min(0.0001f)] public float variationCycleLength = 1f;
        public float variationPhase;
        public int variationSeed;
        [Range(-1, 3)] public int variationMacroIndex = -1;
        [Header("Cue layer and deterministic arbitration")]
        [Range(-1, 15)] public int layerIndex = -1;
        public SpectraCueArbitrationMode arbitrationMode;
        [Range(-1, 15)] public int arbitrationGroup = -1;
        public SpectraModulationTimeBase arbitrationTimeBase = SpectraModulationTimeBase.Bars;
        [Min(0.0001f)] public float arbitrationCycleLength = 1f;
        public float arbitrationPhase;
        public int arbitrationSeed;
        public int priority;
        public Color color = Color.white;
        [Range(0f, 2f)] public float intensity = 1f;
        [Range(-1f, 1f)] public float pan;
        [Range(-1f, 1f)] public float tilt;
        [Range(0f, 8f)] public float movementSpeed = 1f;
        public SpectraMovementPatternKind movementPattern;
        [Range(0f, 2f)] public float movementAmplitude = 1f;
        [Range(0f, 2f)] public float movementSpread = 1f;
        [Range(-8f, 8f)] public float movementPhase;
        [Range(-1f, 1f)] public float movementDirection = 1f;
        [Range(0f, 1f)] public float movementSmoothing = 0.5f;
        [Range(0f, 30f)] public float strobeHz;
        [Min(0)] public int goboIndex;
        [Range(-8f, 8f)] public float goboRotation;
        [Range(0f, 1f)] public float prismAmount;
        [Range(0f, 1f)] public float zoom = 0.5f;
        [Range(0f, 1f)] public float focus = 0.5f;
        public SpectraAudioBand audioBand = SpectraAudioBand.Bass;
        [Range(-2f, 2f)] public float audioAmount = 0.5f;
        [Range(0f, 1f)] public float audioFloor;
        [Min(0)] public int eventChannel;
        public bool eventOnce = true;
        public bool boolValue = true;
        public int randomSeed;
        public SpectraPlatformFallback questFallback = SpectraPlatformFallback.Simplified;
        public SpectraPlatformFallback iosFallback = SpectraPlatformFallback.EmissiveOnly;
        public SpectraPlatformFallback androidFallback = SpectraPlatformFallback.EmissiveOnly;
        public SpectraCapabilityFallback capabilityFallback = SpectraCapabilityFallback.EmissiveApproximation;
        public bool accessibilitySafe;
        [TextArea] public string notes;

        public float ResolveStartSeconds(SpectraBeatGrid grid)
        {
            return timingMode == SpectraTimingMode.Musical && grid != null
                ? (float)grid.MusicalToSeconds(startMusical) : Mathf.Max(0f, startSeconds);
        }

        public float ResolveDurationSeconds(SpectraBeatGrid grid)
        {
            if (timingMode != SpectraTimingMode.Musical || grid == null) return Mathf.Max(0f, durationSeconds);
            double startBeat = grid.SecondsToBeat(ResolveStartSeconds(grid));
            return (float)Math.Max(0d, grid.BeatToSeconds(startBeat + durationBeats) - grid.BeatToSeconds(startBeat));
        }

        public float EvaluateWeight(float showTime, float resolvedStart, float resolvedDuration)
        {
            if (showTime < resolvedStart || showTime > resolvedStart + resolvedDuration) return 0f;
            float elapsed = showTime - resolvedStart;
            float remaining = resolvedStart + resolvedDuration - showTime;
            float weight = 1f;
            if (fadeIn > 0f) weight = Mathf.Min(weight, elapsed / fadeIn);
            if (fadeOut > 0f) weight = Mathf.Min(weight, remaining / fadeOut);
            return ApplyEasing(Mathf.Clamp01(weight));
        }

        private float ApplyEasing(float t)
        {
            if (easing == SpectraCueEasing.SmoothStep || easing == SpectraCueEasing.EaseInOut)
                return t * t * (3f - 2f * t);
            if (easing == SpectraCueEasing.EaseIn) return t * t;
            if (easing == SpectraCueEasing.EaseOut) return 1f - (1f - t) * (1f - t);
            return t;
        }
    }

    [Serializable]
    public class SpectraTimelineTrack
    {
        public string id;
        public string name = "Track";
        public SpectraTrackType trackType;
        public string fixtureGroupId;
        public bool muted;
        public bool locked;
        public bool collapsed;
        public Color displayColor = new Color(0.55f, 0.2f, 0.95f, 1f);
        public SpectraCueBlock[] cues = new SpectraCueBlock[0];
    }

    [Serializable]
    public class SpectraTimelineMarker
    {
        public string id;
        public string name;
        public SpectraMarkerKind kind;
        public SpectraTimingMode timingMode;
        [Min(0f)] public float timeSeconds;
        public SpectraMusicalPosition musicalPosition = new SpectraMusicalPosition(1, 1, 0f);
        public Color color = new Color(1f, 0.35f, 0.85f, 1f);
        [Header("Live performance hot cue")]
        public bool hotCue;
        public SpectraHotCueQuantization hotCueQuantization = SpectraHotCueQuantization.Bar;
        [Range(0f, 4f)] public float transitionSeconds = 0.35f;
        [Header("Scene stack")]
        public bool scene;
        [Range(0, 7)] public int sceneBank;
        [Min(0)] public int sceneOrder;
        public bool sceneAutoAdvance;

        public float ResolveSeconds(SpectraBeatGrid grid)
        {
            return timingMode == SpectraTimingMode.Musical && grid != null
                ? (float)grid.MusicalToSeconds(musicalPosition) : Mathf.Max(0f, timeSeconds);
        }
    }

    [Serializable]
    public class SpectraLoopRegion
    {
        public string id;
        public string name;
        [Min(0f)] public float startSeconds;
        [Min(0f)] public float endSeconds;
        public bool enabled;
        public int repeatCount;
        public bool quantizeExitToBar = true;
        public Color color = new Color(0.2f, 0.85f, 1f, 0.22f);
    }
}
