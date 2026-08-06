using System;
using UnityEditor;
using UnityEngine;

namespace SpectraOverdrive.Editor
{
    public static class SpectraShowSelfTest
    {
        [MenuItem("SpectraOverdrive/Show Programmer/Run Runtime Self-Test")]
        public static void Run()
        {
            SpectraShowAsset original = BuildTestShow();
            SpectraShowAsset imported = ScriptableObject.CreateInstance<SpectraShowAsset>();
            SpectraShowAsset editing = BuildTestShow();
            SpectraShowAsset legacySchema4 = BuildTestShow();
            GameObject groupObject = null;
            GameObject playerObject = null;
            AudioClip waveformClip = null;
            try
            {
                AssertNear(0d, original.beatGrid.MusicalToSeconds(new SpectraMusicalPosition(1, 1, 0f)), "first downbeat");
                AssertNear(2d, original.beatGrid.MusicalToSeconds(new SpectraMusicalPosition(2, 1, 0f)), "bar conversion at 120 BPM");
                AssertNear(6d, original.beatGrid.BeatToSeconds(8d), "variable-tempo conversion");

                string json = SpectraShowJson.Export(original, false);
                SpectraShowJson.ImportInto(json, imported);
                legacySchema4.schemaVersion = 4;
                legacySchema4.performanceMacros = null;
                legacySchema4.colorPalettes = null;
                legacySchema4.performanceMacroSnapshots = null;
                legacySchema4.EnsureStableIds();
                Assert(legacySchema4.schemaVersion == 7
                    && legacySchema4.performanceMacros.Length == 0
                    && legacySchema4.colorPalettes.Length == 0
                    && legacySchema4.performanceMacroSnapshots.Length == 0
                    && legacySchema4.tracks[0].cues[0].performanceMacroIndex == -1
                    && legacySchema4.tracks[0].cues[0].modulationWaveform
                        == SpectraModulationWaveform.Disabled
                    && legacySchema4.tracks[0].cues[0].gatePattern
                        == SpectraCueGatePattern.Disabled
                    && legacySchema4.tracks[0].cues[0].paletteMode
                        == SpectraPalettePlaybackMode.Disabled
                    && legacySchema4.tracks[0].cues[0].conditionMode
                        == SpectraCueConditionMode.Disabled
                    && legacySchema4.tracks[0].cues[0].variationMode
                        == SpectraVariationSelectionMode.Disabled,
                    "schema-v4 assets migrate without opting into later behavior");
                Assert(imported.tracks.Length == 2 && imported.tracks[0].cues.Length == 8, "JSON round trip retained cue graph");
                Assert(imported.schemaVersion == 7, "JSON round trip retained schema v7");
                Assert(json.Contains("\"contentHash\""), "portable JSON contains an integrity hash");
                bool rejectedCorruption = false;
                try
                {
                    string damaged = json.Replace("Automated Test Show", "Damaged Test Show");
                    SpectraShowJson.ImportInto(damaged, imported);
                }
                catch (System.IO.InvalidDataException) { rejectedCorruption = true; }
                Assert(rejectedCorruption, "portable JSON rejects content corruption");
                SpectraShowJson.ImportInto(json, imported);

                SpectraCompiledShow compiled = SpectraShowCompiler.Compile(imported);
                Assert(compiled.CueCount == 9 && compiled.HasConsistentArrays(), "compiler emitted complete flattened arrays");
                Assert(compiled.contentSignature != 0 && !string.IsNullOrEmpty(compiled.contentHash), "compiler emitted a content signature");
                Assert(compiled.groupSelections[0] == (int)SpectraFixtureSelection.Odd
                    && compiled.groupRandomSeeds[0] == 42, "compiler emitted deterministic fixture selection");
                Assert(compiled.cueStarts[0] <= compiled.cueStarts[1], "compiler sorted cues deterministically");
                Assert(compiled.markerTimes.Length == 1 && compiled.loopStarts.Length == 1, "compiler emitted markers and loops");
                Assert(compiled.cueAndroidFallbacks.Length == compiled.CueCount, "compiler emitted Android fallbacks");
                Assert(compiled.groupCapabilityMasks[0] == (int)SpectraFixtureCapability.All
                    && compiled.cueRequiredCapabilities.Length == compiled.CueCount,
                    "compiler emitted fixture capability contracts");
                bool foundFlattenedAutomation = false;
                for (int compiledCue = 0; compiledCue < compiled.CueCount; compiledCue++)
                    if (compiled.cueAutomationCounts[compiledCue] == 2)
                        foundFlattenedAutomation = true;
                Assert(compiled.automationTimes.Length == 2 && foundFlattenedAutomation,
                    "compiler flattened cue automation");
                Assert(compiled.markerHotCues[0]
                    && compiled.markerHotCueQuantizations[0] == (int)SpectraHotCueQuantization.Beat,
                    "compiler emitted live hot-cue metadata");
                Assert(compiled.tempoMarkerTimes.Length == 1,
                    "compiler emitted the variable-tempo runtime map");
                bool foundProceduralMacroCue = false;
                for (int compiledCue = 0; compiledCue < compiled.CueCount; compiledCue++)
                    if (compiled.cueModulationWaveforms[compiledCue]
                            == (int)SpectraModulationWaveform.Pulse
                        && compiled.cuePerformanceMacroIndices[compiledCue] == 0)
                    {
                        foundProceduralMacroCue = true;
                        break;
                    }
                Assert(foundProceduralMacroCue,
                    "compiler emitted procedural modulation and macro bindings");
                Assert(compiled.performanceMacroNames.Length == 1
                    && compiled.performanceMacroNames[0] == "Energy",
                    "compiler emitted performance macro metadata");
                bool foundRhythmPaletteCue = false;
                for (int compiledCue = 0; compiledCue < compiled.CueCount; compiledCue++)
                    if (compiled.cueGatePatterns[compiledCue]
                            == (int)SpectraCueGatePattern.Alternating
                        && compiled.cuePaletteModes[compiledCue]
                            == (int)SpectraPalettePlaybackMode.Step)
                    {
                        foundRhythmPaletteCue = true;
                        break;
                    }
                Assert(foundRhythmPaletteCue
                    && compiled.paletteNames.Length == 1
                    && compiled.paletteCounts[0] == 2
                    && compiled.paletteColors.Length == 2,
                    "compiler emitted rhythm gates and flattened dynamic palettes");
                bool foundCondition = false;
                int variationCueCount = 0;
                for (int compiledCue = 0; compiledCue < compiled.CueCount; compiledCue++)
                {
                    if (compiled.cueConditionModes[compiledCue]
                        == (int)SpectraCueConditionMode.EveryNthCycle)
                        foundCondition = true;
                    if (compiled.cueVariationModes[compiledCue]
                        == (int)SpectraVariationSelectionMode.Cycle)
                        variationCueCount++;
                }
                Assert(foundCondition && variationCueCount == 2
                    && compiled.performanceMacroSnapshotNames.Length == 1
                    && compiled.performanceMacroSnapshotNames[0] == "Breakdown",
                    "compiler emitted schema-v7 conditions, variation routing, and macro snapshots");
                Assert(compiled.markerScenes[0] && compiled.markerSceneOrders[0] == 0,
                    "compiler emitted ordered scene-stack metadata");
                Assert(compiled.questMaximumFixtures == 64
                    && compiled.iosMaximumTransparentBeams == 12
                    && compiled.androidShaderQualityTier == 1,
                    "compiler emitted runtime mobile fixture, beam, and shader policies");

                AssertNear(0.625d, SpectraTimelineEditing.SnapTime(original, 0.62f, SpectraTimelineSnap.QuarterBeat, 60f), "quarter-beat snapping");
                int originalEditingCueCount = editing.tracks[0].cues.Length;
                SpectraCueBlock duplicate = SpectraTimelineEditing.DuplicateCue(editing, 0, 0, 0.5f);
                Assert(editing.tracks[0].cues.Length == originalEditingCueCount + 1
                    && duplicate.id != editing.tracks[0].cues[0].id,
                    "cue duplication creates a stable independent cue");
                SpectraTimelineEditing.SplitCue(editing, 0, 0, 1f);
                Assert(editing.tracks[0].cues.Length == originalEditingCueCount + 2,
                    "cue split creates two valid regions");
                string clipboard = SpectraTimelineEditing.CopyCueToJson(editing.tracks[0].cues[0]);
                SpectraTimelineEditing.PasteCueFromJson(editing, 0, clipboard, 8f);
                Assert(editing.tracks[0].cues.Length == originalEditingCueCount + 3,
                    "cue clipboard round trip works");

                SpectraCueTemplateAsset cueTemplate = ScriptableObject.CreateInstance<SpectraCueTemplateAsset>();
                cueTemplate.templateName = "Template Test";
                cueTemplate.cue = new SpectraCueBlock { valueType = SpectraCueValueType.Color, color = Color.cyan };
                SpectraCueBlock instantiated = cueTemplate.InstantiateCue();
                Assert(instantiated.id != cueTemplate.cue.id && instantiated.color == Color.cyan, "cue template creates an independent cue");
                UnityEngine.Object.DestroyImmediate(cueTemplate);

                waveformClip = AudioClip.Create("SpectraWaveformTest", 512, 1, 128, false);
                float[] samples = new float[512];
                for (int i = 0; i < samples.Length; i++) samples[i] = Mathf.Sin(i * 0.1f);
                Assert(waveformClip.SetData(samples, 0), "waveform test clip accepts samples");
                SpectraWaveformData waveform = SpectraWaveformCache.Build(waveformClip, 64);
                Assert(waveform.BucketCount == 64 && string.IsNullOrEmpty(waveform.error), "waveform cache builds min/max data");

                SpectraPlatformCompatibilityResult iosReport = SpectraPlatformCompatibilityValidator.Analyze(imported, SpectraPlatformKind.IOS);
                Assert(iosReport.cueBudget == 32 && iosReport.FitsBudget
                    && iosReport.conditionCueCount == 1
                    && iosReport.variationCueCount == 2,
                    "iOS compatibility report applies mobile budgets and generative counts");

                groupObject = new GameObject("SpectraSelfTestGroup");
                SpectraFixtureGroup group = groupObject.AddComponent<SpectraFixtureGroup>();
                SpectraFixtureRuntime limitedFixture = groupObject.AddComponent<SpectraFixtureRuntime>();
                limitedFixture.capabilities = SpectraFixtureCapability.Intensity
                    | SpectraFixtureCapability.Color;
                group.fixtures = new[] { limitedFixture };
                playerObject = new GameObject("SpectraSelfTestPlayer");
                SpectraShowRuntimePlayer player = playerObject.AddComponent<SpectraShowRuntimePlayer>();
                SpectraShowCompiler.ApplyToRuntimePlayer(compiled, player);
                Assert(player.CueCount == compiled.CueCount && player.showId == compiled.showId,
                    "compiler bake copied flat Udon-safe runtime fields");
                player.groups = new[] { group };
                player.localPlatform = SpectraPlatformKind.PC;
                player.ApplyAtTime(1f);
                AssertNear(0.5d, group.intensityMultiplier, "runtime cue evaluation");
                AssertNear(2d, group.goboIndex, "runtime gobo evaluation");
                AssertNear(0.7d, group.zoom, "runtime zoom evaluation");
                AssertNear(-1d, limitedFixture.groupGoboIndex,
                    "per-fixture capability gating neutralized unsupported gobo");
                player.ApplyAtTime(0.125f);
                Assert(group.colorMultiplier == Color.white,
                    "runtime alternating rhythm gate suppressed an inactive palette step");
                player.ApplyAtTime(0.25f);
                AssertNear(0.25d, group.intensityMultiplier,
                    "runtime beat-based procedural pulse evaluation");
                AssertNear(0d, group.colorMultiplier.r,
                    "runtime dynamic palette advanced to the synchronized second color");
                AssertNear(1d, group.colorMultiplier.g,
                    "runtime dynamic palette preserved the synchronized palette color");
                player.SetPerformanceMacroValues(0f, 1f, 1f, 1f);
                player.ApplyAtTime(1f);
                AssertNear(0.2d, group.intensityMultiplier,
                    "runtime synchronized performance macro evaluation");
                player.SetPerformanceMacroValues(1f, 1f, 1f, 1f);
                int originalGroupCapabilities = player.groupCapabilityMasks[0];
                player.groupCapabilityMasks[0] = (int)SpectraFixtureCapability.Intensity;
                player.ApplyAtTime(1f);
                AssertNear(-1d, group.goboIndex,
                    "group capability contract selected the emissive fallback");
                player.groupCapabilityMasks[0] = originalGroupCapabilities;
                player.ApplyAtTime(5f);
                AssertNear(0.5d, group.intensityMultiplier, "runtime flattened automation evaluation");
                player.ApplyAtTime(6.25f);
                AssertNear(0.3d, group.intensityMultiplier,
                    "runtime synchronized variation selected option A");
                player.ApplyAtTime(6.75f);
                AssertNear(0.8d, group.intensityMultiplier,
                    "runtime synchronized variation selected option B");
                player.ApplyAtTime(7.25f);
                AssertNear(0.6d, group.intensityMultiplier,
                    "runtime every-N condition enabled its first cycle");
                player.ApplyAtTime(7.75f);
                AssertNear(1d, group.intensityMultiplier,
                    "runtime every-N condition rejected its alternate cycle");
                Assert(player.GetPerformanceMacroSnapshotCount() == 1
                    && player.GetPerformanceMacroSnapshotName(0) == "Breakdown",
                    "runtime exposes compiled macro snapshots");
                AssertNear(6d, player.RuntimeBeatToSeconds(8f), "runtime variable-tempo beat conversion");
                AssertNear(1.5d, player.ResolveHotCueExecutionShowTime(0, 1.2f),
                    "runtime beat-quantized hot-cue scheduling");
                player.ApplyAtTime(8.5f);
                AssertNear(0d, group.intensityMultiplier, "runtime global-track blackout");
                player.selectedLoopIndex = 0;
                AssertNear(1.5d, player.ResolveLoopedTime(3.5f), "runtime loop mapping");
                player.SetEmergencyBlackout(true);
                AssertNear(0d, group.intensityMultiplier, "emergency blackout");
                player.SetEmergencyBlackout(false);

                SpectraLiveOverrideLayer overrides = playerObject.AddComponent<SpectraLiveOverrideLayer>();
                overrides.configuredGroupCount = 1;
                overrides.player = player;
                overrides.modes = new[] { (int)SpectraOverrideMode.Replace };
                overrides.intensities = new[] { 0.25f };
                overrides.colorR = new[] { 1f };
                overrides.colorG = new[] { 0f };
                overrides.colorB = new[] { 1f };
                overrides.colorA = new[] { 1f };
                overrides.pans = new[] { 0f };
                overrides.tilts = new[] { 0f };
                overrides.movements = new[] { 1f };
                overrides.gobos = new[] { -1f };
                overrides.goboRotations = new[] { 0f };
                overrides.prisms = new[] { 0f };
                overrides.zooms = new[] { -1f };
                overrides.focuses = new[] { -1f };
                overrides.strobeRates = new[] { 0f };
                overrides.lasers = new[] { false };
                player.overrideLayer = overrides;
                player.ApplyAtTime(1f);
                AssertNear(0.25d, group.intensityMultiplier, "live override layer");

                SpectraShowSnapshotCache snapshots = playerObject.AddComponent<SpectraShowSnapshotCache>();
                snapshots.capacity = 4;
                snapshots.TryCapture(1f, new[] { group });
                group.intensityMultiplier = 0.9f;
                Assert(snapshots.RestoreNearestBefore(1f, new[] { group }), "snapshot lookup");
                AssertNear(0.25d, group.intensityMultiplier, "snapshot state restoration");

                SpectraShowNetworkController network = playerObject.AddComponent<SpectraShowNetworkController>();
                SpectraOverdriveBus bus = playerObject.AddComponent<SpectraOverdriveBus>();
                player.bus = bus;
                network.showPlayers = new[] { player };
                network.activeShowIndex = 0;
                network.activeContentSignature = player.contentSignature;
                network.playbackState = (int)SpectraShowPlaybackState.Paused;
                network.pausedOffset = 1.25f;
                network.synchronizedMasterIntensity = 0.4f;
                network.ApplyAuthoritativeState();
                AssertNear(1.25d, player.showTime, "offline authoritative network-state reconstruction");
                AssertNear(0.4d, bus.masterIntensity, "synchronized operator master intensity");
                network.playbackState = (int)SpectraShowPlaybackState.Playing;
                network.playStartedServerTime = 100d;
                network.synchronizedPlaybackSpeed = 1f;
                network.hotCueExecuteServerTime = 102d;
                network.hotCueTargetOffset = 5f;
                network.hotCueTransitionSeconds = 0.4f;
                AssertNear(1d, network.ResolveAuthoritativeTime(101d),
                    "network hot cue preserved the pre-jump clock");
                AssertNear(6d, network.ResolveAuthoritativeTime(103d),
                    "network hot cue reconstructed the post-jump clock");
                AssertNear(0d, network.ResolveHotCueTransitionIntensity(102d),
                    "network hot cue reached the synchronized transition floor");
                AssertNear(1d, network.ResolveHotCueTransitionIntensity(103d),
                    "network hot cue restored full intensity after transition");
                network.performanceMacroStart0 = 0f;
                network.performanceMacroTarget0 = 1f;
                network.performanceMacroChangeServerTime = 100d;
                network.performanceMacroTransitionSeconds = 2f;
                AssertNear(0.5d, network.ResolvePerformanceMacro(0, 101d),
                    "network performance macro reconstructed a deterministic smooth transition");
                network.RecallPerformanceMacroSnapshot(0);
                Assert(network.activePerformanceMacroSnapshotIndex == 0
                    && network.performanceMacroTarget0 == 0.2f
                    && network.performanceMacroTarget3 == 0.5f
                    && network.performanceMacroTransitionSeconds == 0.75f,
                    "network recalled all four macro targets from one synchronized snapshot");

                SpectraAssistedGenerationSettings generation = new SpectraAssistedGenerationSettings
                {
                    analysisBuckets = 64,
                    phraseBars = 2,
                    maximumImpactCues = 8
                };
                SpectraAssistedGenerationReport generationReport =
                    SpectraAssistedShowGenerator.Generate(editing, waveformClip, generation);
                Assert(generationReport.generatedTracks == 5 && editing.tracks.Length >= 5,
                    "assisted show generator emitted editable tracks");

                SpectraReleaseReadinessReport release = SpectraReleaseReadinessValidator.Validate(original);
                Assert(release.ready && release.contentSignature == compiled.contentSignature
                    && release.rhythmGateCueCount == 1
                    && release.paletteBindingCount == 1
                    && release.paletteColorCount == 2
                    && release.conditionCueCount == 1
                    && release.variationCueCount == 2
                    && release.variationGroupCount == 1
                    && release.performanceMacroSnapshotCount == 1,
                    "1.4 release-readiness pipeline");

                Debug.Log("SpectraOverdrive 1.4.0 self-test PASSED: schema-v7 migration, deterministic cue conditions, synchronized variation groups, macro snapshot recall, beat grid, variable-tempo runtime map, flattened automation, deterministic rhythm gates, dynamic color palettes, procedural modulation, synchronized performance macros, ordered scene stacks, quantized hot cues, capability contracts, timeline, waveform analysis, assisted generation, signed JSON, compiler, optics, overrides, snapshots, network reconstruction, platform validation, loops, and blackout.");
                EditorUtility.DisplayDialog("SpectraOverdrive", "All 1.4.0 Show Programmer self-tests passed.", "OK");
            }
            finally
            {
                if (groupObject != null) UnityEngine.Object.DestroyImmediate(groupObject);
                if (playerObject != null) UnityEngine.Object.DestroyImmediate(playerObject);
                UnityEngine.Object.DestroyImmediate(original);
                UnityEngine.Object.DestroyImmediate(imported);
                UnityEngine.Object.DestroyImmediate(editing);
                UnityEngine.Object.DestroyImmediate(legacySchema4);
                if (waveformClip != null) UnityEngine.Object.DestroyImmediate(waveformClip);
            }
        }

        private static SpectraShowAsset BuildTestShow()
        {
            SpectraShowAsset show = ScriptableObject.CreateInstance<SpectraShowAsset>();
            show.showName = "Automated Test Show";
            show.durationSeconds = 10f;
            show.beatGrid = new SpectraBeatGrid
            {
                bpm = 120f,
                beatsPerBar = 4,
                firstDownbeatSeconds = 0f,
                tempoChanges = new[] { new SpectraTempoMarker { timeSeconds = 2f, bpm = 60f, numerator = 4, denominator = 4 } }
            };
            SpectraShowFixtureGroup group = new SpectraShowFixtureGroup
            {
                name = "Test Group",
                runtimeGroupId = 7,
                selection = SpectraFixtureSelection.Odd,
                randomSeed = 42,
                capabilities = SpectraFixtureCapability.All
            };
            show.fixtureGroups = new[] { group };
            show.colorPalettes = new[]
            {
                new SpectraColorPalette
                {
                    name = "Test Palette",
                    description = "Self-test palette for deterministic runtime color selection.",
                    colors = new[] { Color.magenta, Color.cyan }
                }
            };
            show.performanceMacros = new[]
            {
                new SpectraPerformanceMacro
                {
                    name = "Energy",
                    description = "Synchronized test energy bus.",
                    defaultValue = 1f,
                    smoothingSeconds = 0.2f,
                    displayColor = Color.magenta
                }
            };
            show.performanceMacroSnapshots = new[]
            {
                new SpectraPerformanceMacroSnapshot
                {
                    name = "Breakdown",
                    description = "Self-test synchronized macro snapshot.",
                    displayColor = Color.blue,
                    values = new Vector4(0.2f, 0.3f, 0.4f, 0.5f),
                    transitionSeconds = 0.75f
                }
            };
            show.EnsureStableIds();
            show.tracks = new[]
            {
                new SpectraTimelineTrack
                {
                    name = "Intensity",
                    trackType = SpectraTrackType.Intensity,
                    fixtureGroupId = group.id,
                    cues = new[]
                    {
                        new SpectraCueBlock
                        {
                            name = "Half",
                            startSeconds = 0f,
                            durationSeconds = 2f,
                            valueType = SpectraCueValueType.Intensity,
                            intensity = 0.5f,
                            blendMode = SpectraCueBlendMode.Replace,
                            modulationWaveform = SpectraModulationWaveform.Pulse,
                            modulationTimeBase = SpectraModulationTimeBase.Beats,
                            modulationMode = SpectraAutomationMode.Multiply,
                            modulationCycleLength = 1f,
                            modulationDutyCycle = 0.25f,
                            modulationOffset = new Vector4(0.5f, 1f, 1f, 1f),
                            modulationDepth = new Vector4(0.5f, 0f, 0f, 0f),
                            performanceMacroIndex = 0,
                            performanceMacroMode = SpectraAutomationMode.Multiply,
                            performanceMacroMinimum = new Vector4(0.4f, 1f, 1f, 1f),
                            performanceMacroMaximum = Vector4.one
                        },
                        new SpectraCueBlock { name = "Gobo", startSeconds = 0f, durationSeconds = 2f, valueType = SpectraCueValueType.Gobo, goboIndex = 2, goboRotation = 0.5f, blendMode = SpectraCueBlendMode.Replace },
                        new SpectraCueBlock { name = "Zoom", startSeconds = 0f, durationSeconds = 2f, valueType = SpectraCueValueType.ZoomFocus, zoom = 0.7f, focus = 0.4f, blendMode = SpectraCueBlendMode.Replace },
                        new SpectraCueBlock
                        {
                            name = "Full",
                            startSeconds = 4f,
                            durationSeconds = 2f,
                            valueType = SpectraCueValueType.Intensity,
                            intensity = 1f,
                            blendMode = SpectraCueBlendMode.Replace,
                            automationMode = SpectraAutomationMode.Multiply,
                            automationKeys = new[]
                            {
                                new SpectraAutomationKey
                                {
                                    normalizedTime = 0f,
                                    value = Vector4.zero,
                                    interpolation = SpectraAutomationInterpolation.Linear
                                },
                                new SpectraAutomationKey
                                {
                                    normalizedTime = 1f,
                                    value = Vector4.one,
                                    interpolation = SpectraAutomationInterpolation.Linear
                                }
                            }
                        },
                        new SpectraCueBlock
                        {
                            name = "Gated Palette",
                            startSeconds = 0f,
                            durationSeconds = 2f,
                            valueType = SpectraCueValueType.Color,
                            color = Color.black,
                            blendMode = SpectraCueBlendMode.ColorMix,
                            gatePattern = SpectraCueGatePattern.Alternating,
                            gateTimeBase = SpectraModulationTimeBase.Beats,
                            gateStepLength = 0.25f,
                            gateStepCount = 8,
                            gateActiveSteps = 4,
                            gateDutyCycle = 0.9f,
                            gateAttack = 0f,
                            gateRelease = 0f,
                            paletteIndex = 0,
                            paletteMode = SpectraPalettePlaybackMode.Step,
                            paletteTimeBase = SpectraModulationTimeBase.Beats,
                            paletteStepLength = 0.5f,
                            palettePrimaryIndex = 0,
                            paletteSecondaryIndex = 1,
                            paletteMacroIndex = -1,
                            paletteBlend = 1f
                        },
                        new SpectraCueBlock
                        {
                            name = "Variation A",
                            startSeconds = 6f,
                            durationSeconds = 1f,
                            valueType = SpectraCueValueType.Intensity,
                            intensity = 0.3f,
                            blendMode = SpectraCueBlendMode.Replace,
                            variationMode = SpectraVariationSelectionMode.Cycle,
                            variationGroup = 2,
                            variationOption = 0,
                            variationOptionCount = 2,
                            variationTimeBase = SpectraModulationTimeBase.Seconds,
                            variationCycleLength = 0.5f
                        },
                        new SpectraCueBlock
                        {
                            name = "Variation B",
                            startSeconds = 6f,
                            durationSeconds = 1f,
                            valueType = SpectraCueValueType.Intensity,
                            intensity = 0.8f,
                            blendMode = SpectraCueBlendMode.Replace,
                            variationMode = SpectraVariationSelectionMode.Cycle,
                            variationGroup = 2,
                            variationOption = 1,
                            variationOptionCount = 2,
                            variationTimeBase = SpectraModulationTimeBase.Seconds,
                            variationCycleLength = 0.5f
                        },
                        new SpectraCueBlock
                        {
                            name = "Every Other Cycle",
                            startSeconds = 7f,
                            durationSeconds = 1f,
                            valueType = SpectraCueValueType.Intensity,
                            intensity = 0.6f,
                            blendMode = SpectraCueBlendMode.Replace,
                            conditionMode = SpectraCueConditionMode.EveryNthCycle,
                            conditionTimeBase = SpectraModulationTimeBase.Seconds,
                            conditionCycleLength = 0.5f,
                            conditionEveryN = 2
                        }
                    }
                },
                new SpectraTimelineTrack
                {
                    name = "Global Safety",
                    trackType = SpectraTrackType.Global,
                    cues = new[]
                    {
                        new SpectraCueBlock
                        {
                            name = "Global Blackout",
                            startSeconds = 8f,
                            durationSeconds = 1f,
                            valueType = SpectraCueValueType.Blackout,
                            boolValue = true,
                            priority = 1000,
                            blendMode = SpectraCueBlendMode.PriorityOverride
                        }
                    }
                }
            };
            show.markers = new[]
            {
                new SpectraTimelineMarker
                {
                    name = "Drop",
                    kind = SpectraMarkerKind.Drop,
                    timeSeconds = 2f,
                    hotCue = true,
                    hotCueQuantization = SpectraHotCueQuantization.Beat,
                    transitionSeconds = 0.4f,
                    scene = true,
                    sceneBank = 0,
                    sceneOrder = 0,
                    sceneAutoAdvance = true
                }
            };
            show.loopRegions = new[]
            {
                new SpectraLoopRegion { name = "Test Loop", startSeconds = 1f, endSeconds = 3f, enabled = true, repeatCount = 0 }
            };
            show.platformPolicies = new[]
            {
                SpectraPlatformPolicy.CreateDefault(SpectraPlatformKind.PC),
                SpectraPlatformPolicy.CreateDefault(SpectraPlatformKind.Quest),
                SpectraPlatformPolicy.CreateDefault(SpectraPlatformKind.IOS),
                SpectraPlatformPolicy.CreateDefault(SpectraPlatformKind.Android)
            };
            show.EnsureStableIds();
            return show;
        }

        private static void Assert(bool condition, string message) { if (!condition) throw new Exception("SpectraOverdrive self-test failed: " + message); }
        private static void AssertNear(double expected, double actual, string message) { if (Math.Abs(expected - actual) > 0.0001d) throw new Exception("SpectraOverdrive self-test failed: " + message + " (expected " + expected + ", got " + actual + ")"); }
    }
}
