using System;
using UnityEngine;

namespace SpectraOverdrive
{
    public enum SpectraShowPlaybackState { Stopped, Playing, Paused, Armed, Invalid }
    public enum SpectraTrackType { FixtureGroup, Intensity, Color, Movement, Gobo, Prism, ZoomFocus, Strobe, Laser, AudioReactive, Global, Event }
    public enum SpectraCueValueType
    {
        Intensity,
        Color,
        Movement,
        Strobe,
        LaserEnable,
        Blackout,
        Gobo,
        Prism,
        ZoomFocus,
        AudioReactiveIntensity,
        Event
    }
    public enum SpectraCueBlendMode { Replace, Add, Multiply, Maximum, Minimum, ColorMix, IntensityOnly, MovementOnly, Mask, PriorityOverride }
    public enum SpectraCueEasing { Linear, SmoothStep, EaseIn, EaseOut, EaseInOut }
    public enum SpectraAutomationMode { Disabled, Replace, Add, Multiply }
    public enum SpectraAutomationInterpolation { Step, Linear, Smooth }
    public enum SpectraModulationWaveform
    {
        Disabled,
        Sine,
        Triangle,
        SawUp,
        SawDown,
        Square,
        Pulse,
        SampleAndHold
    }
    public enum SpectraModulationTimeBase { Seconds, Beats, Bars }
    public enum SpectraCueGatePattern { Disabled, Pulse, Alternating, Euclidean, SeededRandom, CustomMask }
    public enum SpectraPalettePlaybackMode { Disabled, Fixed, Step, PingPong, SeededRandom, MacroMorph }
    public enum SpectraCueConditionMode
    {
        Disabled,
        Probability,
        EveryNthCycle,
        MacroAbove,
        MacroBelow,
        AudioAbove,
        AudioBelow
    }
    public enum SpectraVariationSelectionMode { Disabled, Cycle, PingPong, SeededRandom, MacroSelect }
    public enum SpectraTimingMode { Seconds, Musical }
    public enum SpectraFixtureSelection { All, Odd, Even, Alternating, Reverse, CenterOut, SeededRandom }
    public enum SpectraPlatformFallback { Full, Simplified, EmissiveOnly, Disabled }
    public enum SpectraCapabilityFallback { DisableCue, BestEffort, EmissiveApproximation }
    public enum SpectraHotCueQuantization { Immediate, Beat, HalfBar, Bar, TwoBars, FourBars }
    public enum SpectraNetworkSyncStatus { Offline, WaitingForOwner, Synchronized, Recovering, ShowMismatch, InvalidState }
    public enum SpectraOverrideMode { None, Replace, Add, Multiply, Solo, Mute }
    public enum SpectraRecordingAction
    {
        Intensity,
        Color,
        Movement,
        Gobo,
        Prism,
        ZoomFocus,
        Strobe,
        Laser,
        Blackout,
        ClearGroup,
        ClearAll
    }
    public enum SpectraMarkerKind { Generic, Intro, Build, Verse, Chorus, Breakdown, PreDrop, Drop, Bridge, Outro, Recovery, LoopPoint }
    public enum SpectraTimelineSnap { Off, Frame, TenthSecond, QuarterSecond, Beat, HalfBeat, QuarterBeat, EighthBeat, Bar, TwoBars, FourBars, EightBars, SixteenBars }
    public enum SpectraMovementPatternKind
    {
        Static,
        HorizontalSweep,
        VerticalSweep,
        Circle,
        FigureEight,
        Fan,
        ReverseFan,
        CenterOutFan,
        Wave,
        AlternatingWave,
        Bounce,
        Spiral,
        Cross,
        AudienceSweep,
        StageSweep,
        DjFocus,
        Mirrored,
        FollowTheLeader,
        Chase,
        SeededRandom
    }

    [Flags]
    public enum SpectraFixtureCapability
    {
        None = 0,
        Intensity = 1 << 0,
        Color = 1 << 1,
        Movement = 1 << 2,
        Gobo = 1 << 3,
        Prism = 1 << 4,
        ZoomFocus = 1 << 5,
        Strobe = 1 << 6,
        Laser = 1 << 7,
        AudioReactive = 1 << 8,
        WorldEvent = 1 << 9,
        All = Intensity | Color | Movement | Gobo | Prism | ZoomFocus
            | Strobe | Laser | AudioReactive | WorldEvent
    }

    [Serializable]
    public struct SpectraStableId
    {
        [SerializeField] private string value;
        public string Value { get { return value ?? string.Empty; } }
        public bool IsValid { get { Guid parsed; return Guid.TryParse(Value, out parsed); } }

        public SpectraStableId(string id) { value = id; }
        public static SpectraStableId NewId() { return new SpectraStableId(Guid.NewGuid().ToString("N")); }
        public override string ToString() { return Value; }
    }

    [Serializable]
    public struct SpectraMusicalPosition
    {
        [Min(1)] public int bar;
        [Min(1)] public int beat;
        [Range(0f, 0.999999f)] public float beatFraction;

        public SpectraMusicalPosition(int barNumber, int beatNumber, float fraction)
        {
            bar = Mathf.Max(1, barNumber);
            beat = Mathf.Max(1, beatNumber);
            beatFraction = Mathf.Clamp(fraction, 0f, 0.999999f);
        }
    }

    [Serializable]
    public class SpectraValidationIssue
    {
        public bool isError;
        public string path;
        public string message;

        public SpectraValidationIssue(bool error, string issuePath, string issueMessage)
        {
            isError = error;
            path = issuePath;
            message = issueMessage;
        }
    }

    [Serializable]
    public class SpectraPlatformPolicy
    {
        public SpectraPlatformKind platform;
        [Min(1)] public int maximumActiveCues = 128;
        [Range(1, 120)] public int updateRate = 60;
        [Min(1)] public int maximumFixtures = 128;
        [Min(0)] public int maximumTransparentBeams = 64;
        [Range(1, 8)] public int audioReactiveUpdateDivider = 1;
        [Range(0, 3)] public int shaderQualityTier = 3;
        [Range(2, 32)] public int snapshotCapacity = 16;
        public SpectraPlatformFallback defaultFallback = SpectraPlatformFallback.Full;
        [Range(0f, 1f)] public float movementComplexity = 1f;
        [Range(0f, 1f)] public float transparencyBudget = 1f;
        public bool thermalScaling;
        public bool allowStrobes = true;
        public bool allowLasers = true;

        public static SpectraPlatformPolicy CreateDefault(SpectraPlatformKind kind)
        {
            SpectraPlatformPolicy policy = new SpectraPlatformPolicy();
            policy.platform = kind;
            if (kind == SpectraPlatformKind.Quest)
            {
                policy.maximumActiveCues = 48;
                policy.updateRate = 36;
                policy.maximumFixtures = 64;
                policy.maximumTransparentBeams = 20;
                policy.audioReactiveUpdateDivider = 2;
                policy.shaderQualityTier = 2;
                policy.snapshotCapacity = 8;
                policy.defaultFallback = SpectraPlatformFallback.Simplified;
                policy.movementComplexity = 0.65f;
                policy.transparencyBudget = 0.35f;
                policy.thermalScaling = true;
            }
            else if (kind == SpectraPlatformKind.IOS || kind == SpectraPlatformKind.Android)
            {
                policy.maximumActiveCues = 32;
                policy.updateRate = 30;
                policy.maximumFixtures = 48;
                policy.maximumTransparentBeams = kind == SpectraPlatformKind.IOS ? 12 : 10;
                policy.audioReactiveUpdateDivider = 3;
                policy.shaderQualityTier = 1;
                policy.snapshotCapacity = 6;
                policy.defaultFallback = SpectraPlatformFallback.EmissiveOnly;
                policy.movementComplexity = 0.4f;
                policy.transparencyBudget = 0.2f;
                policy.thermalScaling = true;
            }
            return policy;
        }
    }

    [Serializable]
    public class SpectraColorPalette
    {
        public string name = "Color Palette";
        [TextArea] public string description;
        public Color[] colors = new[] { Color.white };
    }

    [Serializable]
    public class SpectraPerformanceMacro
    {
        public string name = "Performance Macro";
        [TextArea] public string description;
        [Range(0f, 1f)] public float defaultValue = 1f;
        [Range(0f, 4f)] public float smoothingSeconds = 0.08f;
        public Color displayColor = new Color(0.85f, 0.2f, 1f, 1f);
    }

    [Serializable]
    public class SpectraPerformanceMacroSnapshot
    {
        public string name = "Macro Snapshot";
        [TextArea] public string description;
        public Color displayColor = new Color(0.2f, 0.85f, 1f, 1f);
        [Tooltip("Targets for performance macros 1 through 4.")]
        public Vector4 values = Vector4.one;
        [Range(0f, 8f)] public float transitionSeconds = 0.35f;
    }

    [Serializable]
    public class SpectraAccessibilityMetadata
    {
        public bool containsStrobes;
        public bool containsLasers;
        public bool containsRapidColorChanges;
        [Range(0f, 30f)] public float maximumStrobeHz = 15f;
        public string warning;
    }
}
