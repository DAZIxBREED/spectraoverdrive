using System;
using UnityEditor;
using UnityEngine;

namespace SpectraOverdrive.Editor
{
    public static class SpectraNeonDropDemoBuilder
    {
        [MenuItem("SpectraOverdrive/Show Programmer/Create Neon Drop Demo")]
        public static void CreateDemo()
        {
            string path = EditorUtility.SaveFilePanelInProject(
                "Create SpectraOverdrive Neon Drop Demo",
                "SpectraOverdriveNeonDropDemo", "asset",
                "Choose where to save the complete 174 BPM demo show.");
            if (string.IsNullOrEmpty(path)) return;

            SpectraShowAsset show = ScriptableObject.CreateInstance<SpectraShowAsset>();
            show.showName = "SpectraOverdrive Neon Drop Demo";
            show.artist = "SpectraOverdrive";
            show.songName = "Neon Drop";
            show.author = "DAZIxBREED";
            show.authorNotes = "Advanced 1.4 synchronized cross-platform demonstration with deterministic cue conditions, synchronized variation groups, macro snapshots, rhythm gates, dynamic color palettes, flattened automation, procedural modulation, synchronized performance macros, ordered scene stacks, quantized hot cues, fixture capability contracts, optics, AudioLink modulation, events, safety fallbacks, and runtime signatures. Attach runtime group IDs to matching scene groups.";
            show.beatGrid = new SpectraBeatGrid { bpm = 174f, beatsPerBar = 4, firstDownbeatSeconds = 0f };
            show.durationSeconds = (float)show.beatGrid.BeatToSeconds(80d * 4d);
            show.platformPolicies = new[]
            {
                SpectraPlatformPolicy.CreateDefault(SpectraPlatformKind.PC),
                SpectraPlatformPolicy.CreateDefault(SpectraPlatformKind.Quest),
                SpectraPlatformPolicy.CreateDefault(SpectraPlatformKind.IOS),
                SpectraPlatformPolicy.CreateDefault(SpectraPlatformKind.Android)
            };
            show.accessibility = new SpectraAccessibilityMetadata
            {
                containsStrobes = true,
                containsLasers = true,
                containsRapidColorChanges = true,
                maximumStrobeHz = 10f,
                warning = "Includes virtual lasers, flashes, and rapid color changes. Local safe-mode fallbacks are available."
            };
            show.colorPalettes = new[]
            {
                new SpectraColorPalette
                {
                    name = "Neon Voltage",
                    description = "Magenta, violet, cyan, and white for high-energy room chases.",
                    colors = new[]
                    {
                        new Color(1f, 0.03f, 0.62f),
                        new Color(0.38f, 0.02f, 1f),
                        new Color(0.02f, 0.9f, 1f),
                        Color.white
                    }
                },
                new SpectraColorPalette
                {
                    name = "Drop Heat",
                    description = "Red, amber, white-hot, and ultraviolet impact colors.",
                    colors = new[]
                    {
                        new Color(1f, 0.06f, 0.02f),
                        new Color(1f, 0.55f, 0.03f),
                        new Color(1f, 0.94f, 0.75f),
                        new Color(0.28f, 0.01f, 0.5f)
                    }
                }
            };
            show.performanceMacros = new[]
            {
                Macro("Energy", "Scales programmed intensity without changing show timing.", 1f, new Color(1f, 0.2f, 0.8f)),
                Macro("Motion", "Scales movement speed while preserving synchronized patterns.", 1f, new Color(0.1f, 0.8f, 1f)),
                Macro("Impact", "Controls drop and blinder impact strength.", 1f, new Color(1f, 0.65f, 0.1f)),
                Macro("Audio Drive", "Controls the base of AudioLink-reactive intensity cues.", 1f, new Color(0.35f, 1f, 0.4f))
            };
            SpectraGenerativeAuthoring.EnsureStarterSnapshots(show);

            show.fixtureGroups = new[]
            {
                Group("Room Washes", 1, SpectraFixtureCapability.Intensity | SpectraFixtureCapability.Color | SpectraFixtureCapability.AudioReactive),
                Group("Moving Heads", 2, SpectraFixtureCapability.Intensity | SpectraFixtureCapability.Color
                    | SpectraFixtureCapability.Movement | SpectraFixtureCapability.Gobo
                    | SpectraFixtureCapability.Prism | SpectraFixtureCapability.ZoomFocus
                    | SpectraFixtureCapability.Strobe),
                Group("Stage PARs", 3, SpectraFixtureCapability.Intensity | SpectraFixtureCapability.Color
                    | SpectraFixtureCapability.Strobe | SpectraFixtureCapability.AudioReactive),
                Group("Ceiling Lasers", 4, SpectraFixtureCapability.Intensity | SpectraFixtureCapability.Color
                    | SpectraFixtureCapability.Movement | SpectraFixtureCapability.Laser
                    | SpectraFixtureCapability.AudioReactive),
                Group("Crowd Blinders", 5, SpectraFixtureCapability.Intensity
                    | SpectraFixtureCapability.Color | SpectraFixtureCapability.Strobe)
            };
            show.EnsureStableIds();

            show.tracks = new[]
            {
                Track("Room Color", SpectraTrackType.Color, show.fixtureGroups[0].id,
                    ColorCue("Purple Intro", 1, 8, new Color(0.42f, 0.03f, 1f)),
                    ColorCue("Blue Purple Chase", 9, 8, new Color(0.08f, 0.3f, 1f)),
                    ColorCue("Focused Violet", 17, 8, new Color(0.32f, 0.02f, 0.7f)),
                    ColorCue("Build Magenta", 25, 8, new Color(0.9f, 0.03f, 0.65f)),
                    ColorCue("Drop Cyan", 33, 16, new Color(0.02f, 0.9f, 1f)),
                    ColorCue("Breakdown Blue", 49, 8, new Color(0.04f, 0.22f, 0.9f)),
                    ColorCue("Second Drop Purple", 57, 16, new Color(0.55f, 0.02f, 1f)),
                    ColorCue("Outro Purple", 73, 8, new Color(0.3f, 0.01f, 0.55f))),

                Track("Moving Heads", SpectraTrackType.Movement, show.fixtureGroups[1].id,
                    MovementCue("Slow Fan", 1, 8, SpectraMovementPatternKind.Fan, 0.25f, 0.6f),
                    MovementCue("Rising Wave", 9, 8, SpectraMovementPatternKind.Wave, 0.75f, 0.75f),
                    MovementCue("DJ Focus", 17, 8, SpectraMovementPatternKind.DjFocus, 0.3f, 0.65f),
                    MovementCue("Build Spiral", 25, 8, SpectraMovementPatternKind.Spiral, 1.25f, 0.9f),
                    MovementCue("Drop Alternating Fan", 33, 16, SpectraMovementPatternKind.AlternatingWave, 2f, 1f),
                    MovementCue("Breakdown Circle", 49, 8, SpectraMovementPatternKind.Circle, 0.3f, 0.55f),
                    MovementCue("Mirrored Second Drop", 57, 16, SpectraMovementPatternKind.Mirrored, 1.75f, 0.95f),
                    MovementCue("Outro Sweep", 73, 8, SpectraMovementPatternKind.HorizontalSweep, 0.2f, 0.45f)),

                Track("Moving Head Optics", SpectraTrackType.Gobo, show.fixtureGroups[1].id,
                    GoboCue("Hex Tunnel", 9, 16, 2, 0.35f),
                    ZoomCue("Narrow DJ Focus", 17, 8, 0.82f, 0.75f),
                    GoboCue("Drop Rotation", 33, 16, 5, 1.25f),
                    PrismCue("First Drop Prism", 33, 16, 0.65f),
                    ZoomCue("Breakdown Wide", 49, 8, 0.2f, 0.45f),
                    GoboCue("Second Drop Alternating", 57, 16, 6, -1f),
                    PrismCue("Second Drop Prism", 57, 16, 1f)),

                Track("PAR Intensity", SpectraTrackType.Intensity, show.fixtureGroups[2].id,
                    IntensityCue("Intro PAR", 1, 8, 0.55f),
                    IntensityCue("Alternating Chase", 9, 8, 0.82f),
                    IntensityCue("Build Rise", 25, 8, 1f),
                    IntensityCue("First Drop", 33, 16, 1f),
                    IntensityCue("Breakdown Low", 49, 8, 0.35f),
                    IntensityCue("Second Drop", 57, 16, 1f),
                    IntensityCue("Outro Fade", 73, 8, 0.4f)),

                Track("PAR Bass Modulation", SpectraTrackType.AudioReactive, show.fixtureGroups[2].id,
                    AudioCue("Bass Reactive Dimmers", 17, 56, SpectraAudioBand.Bass, 0.35f, 0.65f)),

                Track("Lasers", SpectraTrackType.Laser, show.fixtureGroups[3].id,
                    ToggleCue("Laser Fan", 33, 16, SpectraCueValueType.LaserEnable, true),
                    ToggleCue("Laser Ribbon", 57, 16, SpectraCueValueType.LaserEnable, true)),

                Track("Blinder Impacts", SpectraTrackType.Intensity, show.fixtureGroups[4].id,
                    ImpactCue("First Drop Impact", 33, 1),
                    ImpactCue("Second Drop Impact", 57, 1),
                    ImpactCue("Second Drop Accent A", 61, 1),
                    ImpactCue("Second Drop Accent B", 65, 1),
                    ImpactCue("Second Drop Accent C", 69, 1)),

                Track("Safe Strobe Accents", SpectraTrackType.Strobe, show.fixtureGroups[4].id,
                    StrobeCue("First Drop Strobe", 33, 1, 8f),
                    StrobeCue("Second Drop Strobe", 57, 1, 10f)),

                Track("World Events", SpectraTrackType.Event, show.fixtureGroups[0].id,
                    EventCue("Drop One Stage Event", 33, 0),
                    EventCue("Drop Two Stage Event", 57, 1)),

                Track("Safety and Blackout", SpectraTrackType.Global, show.fixtureGroups[0].id,
                    ToggleCue("Opening Blackout", 1, 0.5f, SpectraCueValueType.Blackout, true),
                    ToggleCue("Drop Pre-Blackout", 32, 0.25f, SpectraCueValueType.Blackout, true),
                    ToggleCue("Final Blackout", 80, 0.25f, SpectraCueValueType.Blackout, true))
            };
            SpectraAutomationAuthoring.ApplyRiserEnvelope(show.tracks[3].cues[2]);
            SpectraAutomationAuthoring.ApplyPulseEnvelope(show.tracks[6].cues[0]);
            SpectraAutomationAuthoring.ApplyPulseEnvelope(show.tracks[6].cues[1]);
            SpectraProceduralAuthoring.ApplyEightBarBreathing(show.tracks[0].cues[0]);
            SpectraProceduralAuthoring.ApplyBeatPulse(show.tracks[3].cues[3]);
            SpectraProceduralAuthoring.ApplyDeterministicFlicker(show.tracks[6].cues[2]);
            SpectraRhythmPaletteAuthoring.ApplyPaletteStep(show.tracks[0].cues[1], 0);
            show.tracks[0].cues[1].paletteStepLength = 0.5f;
            SpectraRhythmPaletteAuthoring.ApplyPaletteMacroMorph(show.tracks[0].cues[4], 1, 2);
            SpectraRhythmPaletteAuthoring.ApplyEuclideanGate(show.tracks[3].cues[1], 5, 8);
            SpectraRhythmPaletteAuthoring.ApplyEuclideanGate(show.tracks[3].cues[3], 7, 16);
            SpectraRhythmPaletteAuthoring.ApplySyncopatedMask(show.tracks[6].cues[2]);
            BindMacro(show.tracks[3], 0, new Vector4(0.35f, 1f, 1f, 1f), Vector4.one);
            BindMacro(show.tracks[1], 1, new Vector4(1f, 1f, 0.35f, 1f), Vector4.one);
            BindMacro(show.tracks[6], 2, new Vector4(0.25f, 1f, 1f, 1f), Vector4.one);
            BindMacro(show.tracks[4], 3, new Vector4(0.45f, 1f, 1f, 1f), Vector4.one);

            // Schema-v7 generative routing: both movement options share absolute show-time
            // selection, so every client chooses the same look without extra network traffic.
            SpectraCueBlock dropVariantA = show.tracks[1].cues[4];
            SpectraCueBlock dropVariantB = MovementCue(
                "Drop Orbit Variant", 33, 16,
                SpectraMovementPatternKind.Circle, 1.55f, 0.92f);
            SpectraGenerativeAuthoring.ApplyCycleVariation(dropVariantA, 1, 0, 2);
            SpectraGenerativeAuthoring.ApplyCycleVariation(dropVariantB, 1, 1, 2);
            AppendCue(show.tracks[1], dropVariantB);
            SpectraGenerativeAuthoring.ApplyProbabilityCondition(show.tracks[6].cues[2], 0.6f);
            show.tracks[6].cues[2].conditionCycleLength = 0.5f;
            SpectraGenerativeAuthoring.ApplyMacroCondition(show.tracks[7].cues[1], 2, 0.45f, true);

            show.markers = new[]
            {
                Marker("Intro", 1, SpectraMarkerKind.Intro),
                Marker("Chase", 9, SpectraMarkerKind.Verse),
                Marker("Pre-Build", 17, SpectraMarkerKind.Breakdown),
                Marker("Build", 25, SpectraMarkerKind.Build),
                Marker("Drop 1", 33, SpectraMarkerKind.Drop),
                Marker("Breakdown", 49, SpectraMarkerKind.Breakdown),
                Marker("Drop 2", 57, SpectraMarkerKind.Drop),
                Marker("Outro", 73, SpectraMarkerKind.Outro),
                Marker("Recovery", 79, SpectraMarkerKind.Recovery)
            };
            show.loopRegions = new[]
            {
                Loop("Extend Breakdown", show.beatGrid, 49, 57),
                Loop("Repeat Second Drop", show.beatGrid, 57, 73)
            };
            show.EnsureStableIds();

            AssetDatabase.CreateAsset(show, path);
            AssetDatabase.SaveAssets();
            Selection.activeObject = show;
            EditorGUIUtility.PingObject(show);
            SpectraShowProgrammerWindow.Open();
        }

        private static SpectraShowFixtureGroup Group(
            string name,
            int runtimeId,
            SpectraFixtureCapability capabilities)
        {
            return new SpectraShowFixtureGroup
            {
                name = name,
                runtimeGroupId = runtimeId,
                selection = SpectraFixtureSelection.All,
                capabilities = capabilities
            };
        }

        private static SpectraPerformanceMacro Macro(
            string name,
            string description,
            float defaultValue,
            Color color)
        {
            return new SpectraPerformanceMacro
            {
                name = name,
                description = description,
                defaultValue = defaultValue,
                smoothingSeconds = 0.12f,
                displayColor = color
            };
        }

        private static void BindMacro(
            SpectraTimelineTrack track,
            int macroIndex,
            Vector4 minimum,
            Vector4 maximum)
        {
            if (track == null || track.cues == null) return;
            for (int i = 0; i < track.cues.Length; i++)
            {
                SpectraCueBlock cue = track.cues[i];
                if (cue == null) continue;
                cue.performanceMacroIndex = macroIndex;
                cue.performanceMacroMode = SpectraAutomationMode.Multiply;
                cue.performanceMacroMinimum = minimum;
                cue.performanceMacroMaximum = maximum;
            }
        }


        private static void AppendCue(SpectraTimelineTrack track, SpectraCueBlock cue)
        {
            if (track == null || cue == null) return;
            int count = track.cues == null ? 0 : track.cues.Length;
            SpectraCueBlock[] expanded = new SpectraCueBlock[count + 1];
            if (count > 0) System.Array.Copy(track.cues, expanded, count);
            expanded[count] = cue;
            track.cues = expanded;
        }

        private static SpectraTimelineTrack Track(string name, SpectraTrackType type, string groupId, params SpectraCueBlock[] cues)
        {
            return new SpectraTimelineTrack
            {
                name = name,
                trackType = type,
                fixtureGroupId = groupId,
                displayColor = TrackColor(type),
                cues = cues
            };
        }

        private static SpectraCueBlock BaseCue(string name, int bar, float durationBars, SpectraCueValueType type)
        {
            return new SpectraCueBlock
            {
                name = name,
                enabled = true,
                timingMode = SpectraTimingMode.Musical,
                startMusical = new SpectraMusicalPosition(bar, 1, 0f),
                durationBeats = Mathf.Max(0.01f, durationBars * 4f),
                valueType = type,
                easing = SpectraCueEasing.SmoothStep,
                blendMode = SpectraCueBlendMode.Replace,
                movementAmplitude = 1f,
                movementSpread = 1f,
                movementDirection = 1f,
                randomSeed = 174090,
                questFallback = SpectraPlatformFallback.Simplified,
                iosFallback = SpectraPlatformFallback.EmissiveOnly,
                androidFallback = SpectraPlatformFallback.EmissiveOnly
            };
        }

        private static SpectraCueBlock ColorCue(string name, int bar, float bars, Color color)
        {
            SpectraCueBlock cue = BaseCue(name, bar, bars, SpectraCueValueType.Color);
            cue.color = color;
            cue.fadeIn = 0.35f;
            cue.fadeOut = 0.35f;
            cue.blendMode = SpectraCueBlendMode.ColorMix;
            return cue;
        }

        private static SpectraCueBlock MovementCue(string name, int bar, float bars, SpectraMovementPatternKind pattern, float speed, float amplitude)
        {
            SpectraCueBlock cue = BaseCue(name, bar, bars, SpectraCueValueType.Movement);
            cue.movementPattern = pattern;
            cue.movementSpeed = speed;
            cue.movementAmplitude = amplitude;
            cue.movementSpread = 1.25f;
            cue.pan = 0f;
            cue.tilt = -0.1f;
            return cue;
        }

        private static SpectraCueBlock IntensityCue(string name, int bar, float bars, float intensity)
        {
            SpectraCueBlock cue = BaseCue(name, bar, bars, SpectraCueValueType.Intensity);
            cue.intensity = intensity;
            cue.fadeIn = 0.25f;
            cue.fadeOut = 0.25f;
            return cue;
        }

        private static SpectraCueBlock ImpactCue(string name, int bar, float beats)
        {
            SpectraCueBlock cue = BaseCue(name, bar, beats / 4f, SpectraCueValueType.Intensity);
            cue.intensity = 1.2f;
            cue.fadeOut = 0.12f;
            cue.priority = 50;
            cue.blendMode = SpectraCueBlendMode.Maximum;
            return cue;
        }

        private static SpectraCueBlock ToggleCue(string name, int bar, float bars, SpectraCueValueType type, bool value)
        {
            SpectraCueBlock cue = BaseCue(name, bar, bars, type);
            cue.boolValue = value;
            cue.priority = 25;
            return cue;
        }

        private static SpectraCueBlock GoboCue(string name, int bar, float bars, int index, float rotation)
        {
            SpectraCueBlock cue = BaseCue(name, bar, bars, SpectraCueValueType.Gobo);
            cue.goboIndex = index;
            cue.goboRotation = rotation;
            cue.questFallback = SpectraPlatformFallback.Simplified;
            cue.iosFallback = SpectraPlatformFallback.EmissiveOnly;
            cue.androidFallback = SpectraPlatformFallback.EmissiveOnly;
            return cue;
        }

        private static SpectraCueBlock PrismCue(string name, int bar, float bars, float amount)
        {
            SpectraCueBlock cue = BaseCue(name, bar, bars, SpectraCueValueType.Prism);
            cue.prismAmount = amount;
            return cue;
        }

        private static SpectraCueBlock ZoomCue(string name, int bar, float bars, float zoom, float focus)
        {
            SpectraCueBlock cue = BaseCue(name, bar, bars, SpectraCueValueType.ZoomFocus);
            cue.zoom = zoom;
            cue.focus = focus;
            return cue;
        }

        private static SpectraCueBlock AudioCue(
            string name,
            int bar,
            float bars,
            SpectraAudioBand band,
            float floor,
            float amount)
        {
            SpectraCueBlock cue = BaseCue(name, bar, bars, SpectraCueValueType.AudioReactiveIntensity);
            cue.intensity = 1f;
            cue.audioBand = band;
            cue.audioFloor = floor;
            cue.audioAmount = amount;
            cue.blendMode = SpectraCueBlendMode.Multiply;
            cue.questFallback = SpectraPlatformFallback.Full;
            cue.iosFallback = SpectraPlatformFallback.Full;
            cue.androidFallback = SpectraPlatformFallback.Full;
            cue.accessibilitySafe = true;
            return cue;
        }

        private static SpectraCueBlock StrobeCue(string name, int bar, float beats, float hz)
        {
            SpectraCueBlock cue = BaseCue(name, bar, beats / 4f, SpectraCueValueType.Strobe);
            cue.strobeHz = hz;
            cue.priority = 75;
            cue.questFallback = SpectraPlatformFallback.Simplified;
            cue.iosFallback = SpectraPlatformFallback.Disabled;
            cue.androidFallback = SpectraPlatformFallback.Disabled;
            return cue;
        }

        private static SpectraCueBlock EventCue(string name, int bar, int channel)
        {
            SpectraCueBlock cue = BaseCue(name, bar, 0.25f, SpectraCueValueType.Event);
            cue.eventChannel = channel;
            cue.eventOnce = true;
            cue.priority = 1000;
            cue.questFallback = SpectraPlatformFallback.Full;
            cue.iosFallback = SpectraPlatformFallback.Full;
            cue.androidFallback = SpectraPlatformFallback.Full;
            return cue;
        }

        private static SpectraTimelineMarker Marker(string name, int bar, SpectraMarkerKind kind)
        {
            return new SpectraTimelineMarker
            {
                name = name,
                kind = kind,
                timingMode = SpectraTimingMode.Musical,
                musicalPosition = new SpectraMusicalPosition(bar, 1, 0f),
                color = kind == SpectraMarkerKind.Drop ? new Color(1f, 0.2f, 0.15f)
                    : kind == SpectraMarkerKind.Build ? new Color(1f, 0.65f, 0.1f)
                    : new Color(0.65f, 0.25f, 1f),
                hotCue = true,
                hotCueQuantization = kind == SpectraMarkerKind.Recovery
                    ? SpectraHotCueQuantization.Immediate
                    : SpectraHotCueQuantization.Bar,
                transitionSeconds = kind == SpectraMarkerKind.Recovery ? 0.1f : 0.4f,
                scene = true,
                sceneBank = 0,
                sceneOrder = bar,
                sceneAutoAdvance = kind != SpectraMarkerKind.Recovery
            };
        }

        private static SpectraLoopRegion Loop(string name, SpectraBeatGrid grid, int startBar, int endBar)
        {
            return new SpectraLoopRegion
            {
                name = name,
                startSeconds = (float)grid.MusicalToSeconds(new SpectraMusicalPosition(startBar, 1, 0f)),
                endSeconds = (float)grid.MusicalToSeconds(new SpectraMusicalPosition(endBar, 1, 0f)),
                enabled = false,
                repeatCount = 0,
                quantizeExitToBar = true
            };
        }

        private static Color TrackColor(SpectraTrackType type)
        {
            if (type == SpectraTrackType.Color) return new Color(0.95f, 0.2f, 0.75f);
            if (type == SpectraTrackType.Movement) return new Color(0.15f, 0.75f, 1f);
            if (type == SpectraTrackType.Laser) return new Color(0.15f, 1f, 0.5f);
            if (type == SpectraTrackType.Global) return new Color(1f, 0.25f, 0.25f);
            return new Color(0.55f, 0.2f, 0.95f);
        }
    }
}
