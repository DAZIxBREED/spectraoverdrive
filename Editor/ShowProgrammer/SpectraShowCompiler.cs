using System;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

namespace SpectraOverdrive.Editor
{
    public static class SpectraShowCompiler
    {
        private sealed class ResolvedCue
        {
            public int groupIndex;
            public SpectraCueBlock cue;
            public float start;
            public float duration;
        }

        public static SpectraCompiledShow Compile(SpectraShowAsset asset)
        {
            if (asset == null) throw new ArgumentNullException("asset");
            asset.EnsureStableIds();
            SpectraValidationIssue[] issues = asset.ValidateShow();
            for (int i = 0; i < issues.Length; i++)
                if (issues[i].isError)
                    throw new InvalidOperationException(issues[i].path + ": " + issues[i].message);

            SpectraCompiledShow result = new SpectraCompiledShow();
            result.schemaVersion = asset.schemaVersion;
            result.showId = asset.showId;
            result.showName = asset.showName;
            result.durationSeconds = asset.durationSeconds;
            result.bpm = asset.beatGrid.bpm;
            result.beatsPerBar = asset.beatGrid.beatsPerBar;
            result.firstDownbeatSeconds = asset.beatGrid.firstDownbeatSeconds;
            ApplyPolicy(result, asset.GetPlatformPolicy(SpectraPlatformKind.PC));
            ApplyPolicy(result, asset.GetPlatformPolicy(SpectraPlatformKind.Quest));
            ApplyPolicy(result, asset.GetPlatformPolicy(SpectraPlatformKind.IOS));
            ApplyPolicy(result, asset.GetPlatformPolicy(SpectraPlatformKind.Android));
            CompileColorPalettes(asset, result);
            CompilePerformanceMacros(asset, result);
            CompilePerformanceMacroSnapshots(asset, result);
            CompileCueLayers(asset, result);

            int groupCount = asset.fixtureGroups == null ? 0 : asset.fixtureGroups.Length;
            result.runtimeGroupIds = new int[groupCount];
            result.groupStableIds = new string[groupCount];
            result.groupSelections = new int[groupCount];
            result.groupRandomSeeds = new int[groupCount];
            result.groupCapabilityMasks = new int[groupCount];
            Dictionary<string, int> groupLookup = new Dictionary<string, int>();
            for (int i = 0; i < groupCount; i++)
            {
                SpectraShowFixtureGroup group = asset.fixtureGroups[i];
                result.runtimeGroupIds[i] = group.runtimeGroupId;
                result.groupStableIds[i] = group.id;
                result.groupSelections[i] = (int)group.selection;
                result.groupRandomSeeds[i] = group.randomSeed;
                result.groupCapabilityMasks[i] = (int)group.capabilities;
                groupLookup[group.id] = i;
            }

            List<ResolvedCue> resolved = new List<ResolvedCue>();
            if (asset.tracks != null) for (int ti = 0; ti < asset.tracks.Length; ti++)
            {
                SpectraTimelineTrack track = asset.tracks[ti];
                if (track == null || track.muted || track.cues == null) continue;
                int groupIndex = -1;
                bool globalTrack = track.trackType == SpectraTrackType.Global
                    || track.trackType == SpectraTrackType.Event;
                if (!globalTrack && !groupLookup.TryGetValue(track.fixtureGroupId, out groupIndex)) continue;
                for (int ci = 0; ci < track.cues.Length; ci++)
                {
                    SpectraCueBlock cue = track.cues[ci];
                    if (cue == null || !cue.enabled) continue;
                    resolved.Add(new ResolvedCue
                    {
                        groupIndex = groupIndex,
                        cue = cue,
                        start = cue.ResolveStartSeconds(asset.beatGrid),
                        duration = cue.ResolveDurationSeconds(asset.beatGrid)
                    });
                }
            }
            resolved.Sort(delegate(ResolvedCue a, ResolvedCue b)
            {
                int timeOrder = a.start.CompareTo(b.start);
                if (timeOrder != 0) return timeOrder;
                int priorityOrder = a.cue.priority.CompareTo(b.cue.priority);
                if (priorityOrder != 0) return priorityOrder;
                return string.CompareOrdinal(a.cue.id, b.cue.id);
            });

            int count = resolved.Count;
            result.cueGroupIndices = new int[count];
            result.cueValueTypes = new int[count];
            result.cueBlendModes = new int[count];
            result.cueEasings = new int[count];
            result.cuePriorities = new int[count];
            result.cueQuestFallbacks = new int[count];
            result.cueIosFallbacks = new int[count];
            result.cueAndroidFallbacks = new int[count];
            result.cueMovementPatterns = new int[count];
            result.cueRandomSeeds = new int[count];
            result.cueAudioBands = new int[count];
            result.cueEventChannels = new int[count];
            result.cueRequiredCapabilities = new int[count];
            result.cueCapabilityFallbacks = new int[count];
            result.cueAutomationModes = new int[count];
            result.cueAutomationOffsets = new int[count];
            result.cueAutomationCounts = new int[count];
            result.cueModulationWaveforms = new int[count];
            result.cueModulationTimeBases = new int[count];
            result.cueModulationModes = new int[count];
            result.cueModulationQuantizeSteps = new int[count];
            result.cuePerformanceMacroIndices = new int[count];
            result.cuePerformanceMacroModes = new int[count];
            result.cueGatePatterns = new int[count];
            result.cueGateTimeBases = new int[count];
            result.cueGateStepCounts = new int[count];
            result.cueGateActiveSteps = new int[count];
            result.cueGateCustomMasks = new int[count];
            result.cuePaletteIndices = new int[count];
            result.cuePaletteModes = new int[count];
            result.cuePaletteTimeBases = new int[count];
            result.cuePalettePrimaryIndices = new int[count];
            result.cuePaletteSecondaryIndices = new int[count];
            result.cuePaletteMacroIndices = new int[count];
            result.cueConditionModes = new int[count];
            result.cueConditionTimeBases = new int[count];
            result.cueConditionEveryNs = new int[count];
            result.cueConditionCycleOffsets = new int[count];
            result.cueConditionMacroIndices = new int[count];
            result.cueConditionAudioBands = new int[count];
            result.cueVariationModes = new int[count];
            result.cueVariationGroups = new int[count];
            result.cueVariationOptions = new int[count];
            result.cueVariationOptionCounts = new int[count];
            result.cueVariationTimeBases = new int[count];
            result.cueVariationSeeds = new int[count];
            result.cueVariationMacroIndices = new int[count];
            result.cueLayerIndices = new int[count];
            result.cueArbitrationModes = new int[count];
            result.cueArbitrationGroups = new int[count];
            result.cueArbitrationTimeBases = new int[count];
            result.cueArbitrationSeeds = new int[count];
            result.cueConditionInverts = new bool[count];
            result.cueGateInverts = new bool[count];
            result.cueEventOnce = new bool[count];
            result.cueStarts = new float[count];
            result.cueDurations = new float[count];
            result.cueFadeIns = new float[count];
            result.cueFadeOuts = new float[count];
            result.cueMovementSmoothing = new float[count];
            result.cueAudioAmounts = new float[count];
            result.cueAudioFloors = new float[count];
            result.cueModulationCycleLengths = new float[count];
            result.cueModulationPhases = new float[count];
            result.cueModulationDutyCycles = new float[count];
            result.cueGateStepLengths = new float[count];
            result.cueGateDutyCycles = new float[count];
            result.cueGateAttacks = new float[count];
            result.cueGateReleases = new float[count];
            result.cueGatePhases = new float[count];
            result.cuePaletteStepLengths = new float[count];
            result.cuePalettePhases = new float[count];
            result.cuePaletteBlends = new float[count];
            result.cueConditionCycleLengths = new float[count];
            result.cueConditionPhases = new float[count];
            result.cueConditionProbabilities = new float[count];
            result.cueConditionThresholds = new float[count];
            result.cueVariationCycleLengths = new float[count];
            result.cueVariationPhases = new float[count];
            result.cueArbitrationCycleLengths = new float[count];
            result.cueArbitrationPhases = new float[count];
            result.cueColors = new Color[count];
            result.cueValues = new Vector4[count];
            result.cueMovementParameters = new Vector4[count];
            result.cueModulationOffsets = new Vector4[count];
            result.cueModulationDepths = new Vector4[count];
            result.cuePerformanceMacroMinimums = new Vector4[count];
            result.cuePerformanceMacroMaximums = new Vector4[count];
            List<float> automationTimes = new List<float>();
            List<Vector4> automationValues = new List<Vector4>();
            List<int> automationInterpolations = new List<int>();
            for (int i = 0; i < count; i++)
            {
                ResolvedCue source = resolved[i];
                SpectraCueBlock cue = source.cue;
                result.cueGroupIndices[i] = source.groupIndex;
                result.cueValueTypes[i] = (int)cue.valueType;
                result.cueBlendModes[i] = (int)cue.blendMode;
                result.cueEasings[i] = (int)cue.easing;
                result.cuePriorities[i] = cue.priority;
                result.cueQuestFallbacks[i] = (int)cue.questFallback;
                result.cueIosFallbacks[i] = (int)cue.iosFallback;
                result.cueAndroidFallbacks[i] = (int)cue.androidFallback;
                result.cueMovementPatterns[i] = (int)cue.movementPattern;
                result.cueRandomSeeds[i] = cue.randomSeed;
                result.cueAudioBands[i] = (int)cue.audioBand;
                result.cueEventChannels[i] = cue.eventChannel;
                result.cueEventOnce[i] = cue.eventOnce;
                result.cueRequiredCapabilities[i] = (int)ResolveRequiredCapabilities(cue.valueType);
                result.cueCapabilityFallbacks[i] = (int)cue.capabilityFallback;
                result.cueAutomationModes[i] = (int)cue.automationMode;
                result.cueAutomationOffsets[i] = automationTimes.Count;
                int automationCount = cue.automationMode == SpectraAutomationMode.Disabled
                    || cue.automationKeys == null ? 0 : cue.automationKeys.Length;
                result.cueAutomationCounts[i] = automationCount;
                for (int keyIndex = 0; keyIndex < automationCount; keyIndex++)
                {
                    SpectraAutomationKey key = cue.automationKeys[keyIndex];
                    automationTimes.Add(Mathf.Clamp01(key.normalizedTime));
                    automationValues.Add(key.value);
                    automationInterpolations.Add((int)key.interpolation);
                }
                result.cueModulationWaveforms[i] = (int)cue.modulationWaveform;
                result.cueModulationTimeBases[i] = (int)cue.modulationTimeBase;
                result.cueModulationModes[i] = (int)cue.modulationMode;
                result.cueModulationQuantizeSteps[i] = Mathf.Clamp(cue.modulationQuantizeSteps, 0, 32);
                result.cueModulationCycleLengths[i] = Mathf.Max(0.0001f, cue.modulationCycleLength);
                result.cueModulationPhases[i] = cue.modulationPhase;
                result.cueModulationDutyCycles[i] = Mathf.Clamp(cue.modulationDutyCycle, 0.01f, 0.99f);
                result.cueModulationOffsets[i] = cue.modulationOffset;
                result.cueModulationDepths[i] = cue.modulationDepth;
                result.cuePerformanceMacroIndices[i] = cue.performanceMacroIndex;
                result.cuePerformanceMacroModes[i] = (int)cue.performanceMacroMode;
                result.cuePerformanceMacroMinimums[i] = cue.performanceMacroMinimum;
                result.cuePerformanceMacroMaximums[i] = cue.performanceMacroMaximum;
                result.cueGatePatterns[i] = (int)cue.gatePattern;
                result.cueGateTimeBases[i] = (int)cue.gateTimeBase;
                result.cueGateStepCounts[i] = Mathf.Clamp(cue.gateStepCount, 1, 32);
                result.cueGateActiveSteps[i] = Mathf.Clamp(cue.gateActiveSteps, 1, result.cueGateStepCounts[i]);
                result.cueGateCustomMasks[i] = cue.gateCustomMask;
                result.cueGateInverts[i] = cue.gateInvert;
                result.cueGateStepLengths[i] = Mathf.Max(0.0001f, cue.gateStepLength);
                result.cueGateDutyCycles[i] = Mathf.Clamp(cue.gateDutyCycle, 0.01f, 0.99f);
                result.cueGateAttacks[i] = Mathf.Clamp(cue.gateAttack, 0f, 0.49f);
                result.cueGateReleases[i] = Mathf.Clamp(cue.gateRelease, 0f, 0.49f);
                result.cueGatePhases[i] = cue.gatePhase;
                result.cuePaletteIndices[i] = cue.paletteIndex;
                result.cuePaletteModes[i] = (int)cue.paletteMode;
                result.cuePaletteTimeBases[i] = (int)cue.paletteTimeBase;
                result.cuePalettePrimaryIndices[i] = cue.palettePrimaryIndex;
                result.cuePaletteSecondaryIndices[i] = cue.paletteSecondaryIndex;
                result.cuePaletteMacroIndices[i] = cue.paletteMacroIndex;
                result.cuePaletteStepLengths[i] = Mathf.Max(0.0001f, cue.paletteStepLength);
                result.cuePalettePhases[i] = cue.palettePhase;
                result.cuePaletteBlends[i] = Mathf.Clamp01(cue.paletteBlend);
                result.cueConditionModes[i] = (int)cue.conditionMode;
                result.cueConditionTimeBases[i] = (int)cue.conditionTimeBase;
                result.cueConditionEveryNs[i] = Mathf.Clamp(cue.conditionEveryN, 1, 32);
                result.cueConditionCycleOffsets[i] = Mathf.Clamp(cue.conditionCycleOffset, 0, 31);
                result.cueConditionMacroIndices[i] = cue.conditionMacroIndex;
                result.cueConditionAudioBands[i] = (int)cue.conditionAudioBand;
                result.cueConditionInverts[i] = cue.conditionInvert;
                result.cueConditionCycleLengths[i] = Mathf.Max(0.0001f, cue.conditionCycleLength);
                result.cueConditionPhases[i] = cue.conditionPhase;
                result.cueConditionProbabilities[i] = Mathf.Clamp01(cue.conditionProbability);
                result.cueConditionThresholds[i] = Mathf.Clamp01(cue.conditionThreshold);
                result.cueVariationModes[i] = (int)cue.variationMode;
                result.cueVariationGroups[i] = cue.variationGroup;
                result.cueVariationOptions[i] = Mathf.Clamp(cue.variationOption, 0, 7);
                result.cueVariationOptionCounts[i] = Mathf.Clamp(cue.variationOptionCount, 2, 8);
                result.cueVariationTimeBases[i] = (int)cue.variationTimeBase;
                result.cueVariationSeeds[i] = cue.variationSeed;
                result.cueVariationMacroIndices[i] = cue.variationMacroIndex;
                result.cueVariationCycleLengths[i] = Mathf.Max(0.0001f, cue.variationCycleLength);
                result.cueVariationPhases[i] = cue.variationPhase;
                result.cueLayerIndices[i] = cue.layerIndex;
                result.cueArbitrationModes[i] = (int)cue.arbitrationMode;
                result.cueArbitrationGroups[i] = cue.arbitrationGroup;
                result.cueArbitrationTimeBases[i] = (int)cue.arbitrationTimeBase;
                result.cueArbitrationSeeds[i] = cue.arbitrationSeed;
                result.cueArbitrationCycleLengths[i] = Mathf.Max(
                    0.0001f, cue.arbitrationCycleLength);
                result.cueArbitrationPhases[i] = cue.arbitrationPhase;
                result.cueStarts[i] = source.start;
                result.cueDurations[i] = source.duration;
                result.cueFadeIns[i] = Mathf.Min(cue.fadeIn, source.duration);
                result.cueFadeOuts[i] = Mathf.Min(cue.fadeOut, source.duration);
                result.cueMovementSmoothing[i] = Mathf.Clamp01(cue.movementSmoothing);
                result.cueAudioAmounts[i] = cue.audioAmount;
                result.cueAudioFloors[i] = cue.audioFloor;
                result.cueColors[i] = cue.color;
                result.cueValues[i] = new Vector4(cue.intensity, cue.tilt, cue.movementSpeed, cue.boolValue ? 1f : 0f);
                result.cueMovementParameters[i] = new Vector4(
                    cue.movementAmplitude,
                    cue.movementSpread,
                    cue.movementPhase,
                    cue.movementDirection);
                if (cue.valueType == SpectraCueValueType.Movement)
                    result.cueValues[i] = new Vector4(cue.pan, cue.tilt, cue.movementSpeed, 0f);
                else if (cue.valueType == SpectraCueValueType.Strobe)
                    result.cueValues[i] = new Vector4(cue.strobeHz, 0f, 0f, 0f);
                else if (cue.valueType == SpectraCueValueType.LaserEnable || cue.valueType == SpectraCueValueType.Blackout)
                    result.cueValues[i] = new Vector4(cue.boolValue ? 1f : 0f, 0f, 0f, 0f);
                else if (cue.valueType == SpectraCueValueType.Gobo)
                    result.cueValues[i] = new Vector4(cue.goboIndex, cue.goboRotation, 0f, 0f);
                else if (cue.valueType == SpectraCueValueType.Prism)
                    result.cueValues[i] = new Vector4(cue.prismAmount, 0f, 0f, 0f);
                else if (cue.valueType == SpectraCueValueType.ZoomFocus)
                    result.cueValues[i] = new Vector4(cue.zoom, cue.focus, 0f, 0f);
                else if (cue.valueType == SpectraCueValueType.Event)
                    result.cueValues[i] = new Vector4(cue.eventChannel, cue.eventOnce ? 1f : 0f, 0f, 0f);
            }
            result.automationTimes = automationTimes.ToArray();
            result.automationValues = automationValues.ToArray();
            result.automationInterpolations = automationInterpolations.ToArray();
            CompileMarkers(asset, result);
            CompileLoops(asset, result);
            CompileTempoMap(asset, result);
            if (!result.HasConsistentArrays()) throw new InvalidOperationException("Compiler produced inconsistent runtime arrays.");
            result.contentSignature = ComputeContentSignature(result);
            result.contentHash = result.contentSignature.ToString("X8");
            return result;
        }

        public static void ApplyToRuntimePlayer(SpectraCompiledShow source, SpectraShowRuntimePlayer player)
        {
            if (source == null) throw new ArgumentNullException("source");
            if (player == null) throw new ArgumentNullException("player");
            player.schemaVersion = source.schemaVersion;
            player.showId = source.showId;
            player.showName = source.showName;
            player.contentHash = source.contentHash;
            player.contentSignature = source.contentSignature;
            player.durationSeconds = source.durationSeconds;
            player.bpm = source.bpm;
            player.beatsPerBar = source.beatsPerBar;
            player.firstDownbeatSeconds = source.firstDownbeatSeconds;
            player.pcMaximumActiveCues = source.pcMaximumActiveCues;
            player.questMaximumActiveCues = source.questMaximumActiveCues;
            player.iosMaximumActiveCues = source.iosMaximumActiveCues;
            player.androidMaximumActiveCues = source.androidMaximumActiveCues;
            player.pcUpdateRate = source.pcUpdateRate;
            player.questUpdateRate = source.questUpdateRate;
            player.iosUpdateRate = source.iosUpdateRate;
            player.androidUpdateRate = source.androidUpdateRate;
            player.pcMaximumFixtures = source.pcMaximumFixtures;
            player.questMaximumFixtures = source.questMaximumFixtures;
            player.iosMaximumFixtures = source.iosMaximumFixtures;
            player.androidMaximumFixtures = source.androidMaximumFixtures;
            player.pcMaximumTransparentBeams = source.pcMaximumTransparentBeams;
            player.questMaximumTransparentBeams = source.questMaximumTransparentBeams;
            player.iosMaximumTransparentBeams = source.iosMaximumTransparentBeams;
            player.androidMaximumTransparentBeams = source.androidMaximumTransparentBeams;
            player.pcAudioReactiveUpdateDivider = source.pcAudioReactiveUpdateDivider;
            player.questAudioReactiveUpdateDivider = source.questAudioReactiveUpdateDivider;
            player.iosAudioReactiveUpdateDivider = source.iosAudioReactiveUpdateDivider;
            player.androidAudioReactiveUpdateDivider = source.androidAudioReactiveUpdateDivider;
            player.pcShaderQualityTier = source.pcShaderQualityTier;
            player.questShaderQualityTier = source.questShaderQualityTier;
            player.iosShaderQualityTier = source.iosShaderQualityTier;
            player.androidShaderQualityTier = source.androidShaderQualityTier;
            player.pcAllowStrobes = source.pcAllowStrobes;
            player.questAllowStrobes = source.questAllowStrobes;
            player.iosAllowStrobes = source.iosAllowStrobes;
            player.androidAllowStrobes = source.androidAllowStrobes;
            player.pcAllowLasers = source.pcAllowLasers;
            player.questAllowLasers = source.questAllowLasers;
            player.iosAllowLasers = source.iosAllowLasers;
            player.androidAllowLasers = source.androidAllowLasers;
            player.paletteNames = source.paletteNames;
            player.paletteOffsets = source.paletteOffsets;
            player.paletteCounts = source.paletteCounts;
            player.paletteColors = source.paletteColors;
            player.performanceMacroNames = source.performanceMacroNames;
            player.performanceMacroDefaults = source.performanceMacroDefaults;
            player.performanceMacroSmoothing = source.performanceMacroSmoothing;
            player.performanceMacroColors = source.performanceMacroColors;
            player.performanceMacroSnapshotNames = source.performanceMacroSnapshotNames;
            player.performanceMacroSnapshotColors = source.performanceMacroSnapshotColors;
            player.performanceMacroSnapshotValues = source.performanceMacroSnapshotValues;
            player.performanceMacroSnapshotTransitionSeconds = source.performanceMacroSnapshotTransitionSeconds;
            player.cueLayerNames = source.cueLayerNames;
            player.cueLayerColors = source.cueLayerColors;
            player.cueLayerDefaultEnabled = source.cueLayerDefaultEnabled;
            player.cueLayerPcEnabled = source.cueLayerPcEnabled;
            player.cueLayerQuestEnabled = source.cueLayerQuestEnabled;
            player.cueLayerIosEnabled = source.cueLayerIosEnabled;
            player.cueLayerAndroidEnabled = source.cueLayerAndroidEnabled;
            player.cueLayerPriorityBiases = source.cueLayerPriorityBiases;
            player.cueLayerMaximumActiveCues = source.cueLayerMaximumActiveCues;
            player.runtimeGroupIds = source.runtimeGroupIds;
            player.groupStableIds = source.groupStableIds;
            player.groupSelections = source.groupSelections;
            player.groupRandomSeeds = source.groupRandomSeeds;
            player.groupCapabilityMasks = source.groupCapabilityMasks;
            player.cueGroupIndices = source.cueGroupIndices;
            player.cueValueTypes = source.cueValueTypes;
            player.cueBlendModes = source.cueBlendModes;
            player.cueEasings = source.cueEasings;
            player.cuePriorities = source.cuePriorities;
            player.cueQuestFallbacks = source.cueQuestFallbacks;
            player.cueIosFallbacks = source.cueIosFallbacks;
            player.cueAndroidFallbacks = source.cueAndroidFallbacks;
            player.cueMovementPatterns = source.cueMovementPatterns;
            player.cueRandomSeeds = source.cueRandomSeeds;
            player.cueAudioBands = source.cueAudioBands;
            player.cueEventChannels = source.cueEventChannels;
            player.cueRequiredCapabilities = source.cueRequiredCapabilities;
            player.cueCapabilityFallbacks = source.cueCapabilityFallbacks;
            player.cueAutomationModes = source.cueAutomationModes;
            player.cueAutomationOffsets = source.cueAutomationOffsets;
            player.cueAutomationCounts = source.cueAutomationCounts;
            player.cueModulationWaveforms = source.cueModulationWaveforms;
            player.cueModulationTimeBases = source.cueModulationTimeBases;
            player.cueModulationModes = source.cueModulationModes;
            player.cueModulationQuantizeSteps = source.cueModulationQuantizeSteps;
            player.cuePerformanceMacroIndices = source.cuePerformanceMacroIndices;
            player.cuePerformanceMacroModes = source.cuePerformanceMacroModes;
            player.cueGatePatterns = source.cueGatePatterns;
            player.cueGateTimeBases = source.cueGateTimeBases;
            player.cueGateStepCounts = source.cueGateStepCounts;
            player.cueGateActiveSteps = source.cueGateActiveSteps;
            player.cueGateCustomMasks = source.cueGateCustomMasks;
            player.cuePaletteIndices = source.cuePaletteIndices;
            player.cuePaletteModes = source.cuePaletteModes;
            player.cuePaletteTimeBases = source.cuePaletteTimeBases;
            player.cuePalettePrimaryIndices = source.cuePalettePrimaryIndices;
            player.cuePaletteSecondaryIndices = source.cuePaletteSecondaryIndices;
            player.cuePaletteMacroIndices = source.cuePaletteMacroIndices;
            player.cueConditionModes = source.cueConditionModes;
            player.cueConditionTimeBases = source.cueConditionTimeBases;
            player.cueConditionEveryNs = source.cueConditionEveryNs;
            player.cueConditionCycleOffsets = source.cueConditionCycleOffsets;
            player.cueConditionMacroIndices = source.cueConditionMacroIndices;
            player.cueConditionAudioBands = source.cueConditionAudioBands;
            player.cueVariationModes = source.cueVariationModes;
            player.cueVariationGroups = source.cueVariationGroups;
            player.cueVariationOptions = source.cueVariationOptions;
            player.cueVariationOptionCounts = source.cueVariationOptionCounts;
            player.cueVariationTimeBases = source.cueVariationTimeBases;
            player.cueVariationSeeds = source.cueVariationSeeds;
            player.cueVariationMacroIndices = source.cueVariationMacroIndices;
            player.cueLayerIndices = source.cueLayerIndices;
            player.cueArbitrationModes = source.cueArbitrationModes;
            player.cueArbitrationGroups = source.cueArbitrationGroups;
            player.cueArbitrationTimeBases = source.cueArbitrationTimeBases;
            player.cueArbitrationSeeds = source.cueArbitrationSeeds;
            player.cueConditionInverts = source.cueConditionInverts;
            player.cueGateInverts = source.cueGateInverts;
            player.cueEventOnce = source.cueEventOnce;
            player.cueStarts = source.cueStarts;
            player.cueDurations = source.cueDurations;
            player.cueFadeIns = source.cueFadeIns;
            player.cueFadeOuts = source.cueFadeOuts;
            player.cueMovementSmoothing = source.cueMovementSmoothing;
            player.cueAudioAmounts = source.cueAudioAmounts;
            player.cueAudioFloors = source.cueAudioFloors;
            player.cueModulationCycleLengths = source.cueModulationCycleLengths;
            player.cueModulationPhases = source.cueModulationPhases;
            player.cueModulationDutyCycles = source.cueModulationDutyCycles;
            player.cueGateStepLengths = source.cueGateStepLengths;
            player.cueGateDutyCycles = source.cueGateDutyCycles;
            player.cueGateAttacks = source.cueGateAttacks;
            player.cueGateReleases = source.cueGateReleases;
            player.cueGatePhases = source.cueGatePhases;
            player.cuePaletteStepLengths = source.cuePaletteStepLengths;
            player.cuePalettePhases = source.cuePalettePhases;
            player.cuePaletteBlends = source.cuePaletteBlends;
            player.cueConditionCycleLengths = source.cueConditionCycleLengths;
            player.cueConditionPhases = source.cueConditionPhases;
            player.cueConditionProbabilities = source.cueConditionProbabilities;
            player.cueConditionThresholds = source.cueConditionThresholds;
            player.cueVariationCycleLengths = source.cueVariationCycleLengths;
            player.cueVariationPhases = source.cueVariationPhases;
            player.cueArbitrationCycleLengths = source.cueArbitrationCycleLengths;
            player.cueArbitrationPhases = source.cueArbitrationPhases;
            player.cueColors = source.cueColors;
            player.cueValues = source.cueValues;
            player.cueMovementParameters = source.cueMovementParameters;
            player.cueModulationOffsets = source.cueModulationOffsets;
            player.cueModulationDepths = source.cueModulationDepths;
            player.cuePerformanceMacroMinimums = source.cuePerformanceMacroMinimums;
            player.cuePerformanceMacroMaximums = source.cuePerformanceMacroMaximums;
            player.automationTimes = source.automationTimes;
            player.automationValues = source.automationValues;
            player.automationInterpolations = source.automationInterpolations;
            player.markerNames = source.markerNames;
            player.markerTimes = source.markerTimes;
            player.markerKinds = source.markerKinds;
            player.markerHotCues = source.markerHotCues;
            player.markerHotCueQuantizations = source.markerHotCueQuantizations;
            player.markerTransitionSeconds = source.markerTransitionSeconds;
            player.markerScenes = source.markerScenes;
            player.markerSceneBanks = source.markerSceneBanks;
            player.markerSceneOrders = source.markerSceneOrders;
            player.markerSceneAutoAdvance = source.markerSceneAutoAdvance;
            player.loopNames = source.loopNames;
            player.loopStarts = source.loopStarts;
            player.loopEnds = source.loopEnds;
            player.loopEnabled = source.loopEnabled;
            player.loopRepeatCounts = source.loopRepeatCounts;
            player.tempoMarkerTimes = source.tempoMarkerTimes;
            player.tempoMarkerBpms = source.tempoMarkerBpms;
            player.tempoMarkerNumerators = source.tempoMarkerNumerators;
            player.ResetPerformanceMacrosToDefaults();
            player.ResetCueLayerMasksToDefaults();
        }

        private static void CompileColorPalettes(SpectraShowAsset asset, SpectraCompiledShow result)
        {
            int count = asset.colorPalettes == null
                ? 0 : Mathf.Min(16, asset.colorPalettes.Length);
            result.paletteNames = new string[count];
            result.paletteOffsets = new int[count];
            result.paletteCounts = new int[count];
            List<Color> colors = new List<Color>();
            for (int i = 0; i < count; i++)
            {
                SpectraColorPalette palette = asset.colorPalettes[i];
                result.paletteNames[i] = palette.name;
                result.paletteOffsets[i] = colors.Count;
                int colorCount = palette.colors == null ? 0 : Mathf.Min(16, palette.colors.Length);
                result.paletteCounts[i] = colorCount;
                for (int colorIndex = 0; colorIndex < colorCount; colorIndex++)
                    colors.Add(palette.colors[colorIndex]);
            }
            result.paletteColors = colors.ToArray();
        }

        private static void CompilePerformanceMacros(SpectraShowAsset asset, SpectraCompiledShow result)
        {
            int count = asset.performanceMacros == null
                ? 0 : Mathf.Min(4, asset.performanceMacros.Length);
            result.performanceMacroNames = new string[count];
            result.performanceMacroDefaults = new float[count];
            result.performanceMacroSmoothing = new float[count];
            result.performanceMacroColors = new Color[count];
            for (int i = 0; i < count; i++)
            {
                SpectraPerformanceMacro macro = asset.performanceMacros[i];
                result.performanceMacroNames[i] = macro.name;
                result.performanceMacroDefaults[i] = Mathf.Clamp01(macro.defaultValue);
                result.performanceMacroSmoothing[i] = Mathf.Clamp(macro.smoothingSeconds, 0f, 4f);
                result.performanceMacroColors[i] = macro.displayColor;
            }
        }

        private static void CompilePerformanceMacroSnapshots(
            SpectraShowAsset asset,
            SpectraCompiledShow result)
        {
            int count = asset.performanceMacroSnapshots == null
                ? 0 : Mathf.Min(16, asset.performanceMacroSnapshots.Length);
            result.performanceMacroSnapshotNames = new string[count];
            result.performanceMacroSnapshotColors = new Color[count];
            result.performanceMacroSnapshotValues = new Vector4[count];
            result.performanceMacroSnapshotTransitionSeconds = new float[count];
            for (int i = 0; i < count; i++)
            {
                SpectraPerformanceMacroSnapshot snapshot = asset.performanceMacroSnapshots[i];
                Vector4 values = snapshot.values;
                values.x = Mathf.Clamp01(values.x);
                values.y = Mathf.Clamp01(values.y);
                values.z = Mathf.Clamp01(values.z);
                values.w = Mathf.Clamp01(values.w);
                result.performanceMacroSnapshotNames[i] = snapshot.name;
                result.performanceMacroSnapshotColors[i] = snapshot.displayColor;
                result.performanceMacroSnapshotValues[i] = values;
                result.performanceMacroSnapshotTransitionSeconds[i] =
                    Mathf.Clamp(snapshot.transitionSeconds, 0f, 8f);
            }
        }

        private static void CompileCueLayers(
            SpectraShowAsset asset,
            SpectraCompiledShow result)
        {
            int count = asset.cueLayers == null
                ? 0 : Mathf.Min(16, asset.cueLayers.Length);
            result.cueLayerNames = new string[count];
            result.cueLayerColors = new Color[count];
            result.cueLayerDefaultEnabled = new bool[count];
            result.cueLayerPcEnabled = new bool[count];
            result.cueLayerQuestEnabled = new bool[count];
            result.cueLayerIosEnabled = new bool[count];
            result.cueLayerAndroidEnabled = new bool[count];
            result.cueLayerPriorityBiases = new int[count];
            result.cueLayerMaximumActiveCues = new int[count];
            for (int i = 0; i < count; i++)
            {
                SpectraCueLayer layer = asset.cueLayers[i];
                result.cueLayerNames[i] = layer.name;
                result.cueLayerColors[i] = layer.displayColor;
                result.cueLayerDefaultEnabled[i] = layer.defaultEnabled;
                result.cueLayerPcEnabled[i] = layer.pcEnabled;
                result.cueLayerQuestEnabled[i] = layer.questEnabled;
                result.cueLayerIosEnabled[i] = layer.iosEnabled;
                result.cueLayerAndroidEnabled[i] = layer.androidEnabled;
                result.cueLayerPriorityBiases[i] = Mathf.Clamp(
                    layer.priorityBias, -100, 100);
                result.cueLayerMaximumActiveCues[i] = Mathf.Clamp(
                    layer.maximumActiveCues, 0, 32);
            }
        }

        private static void ApplyPolicy(SpectraCompiledShow result, SpectraPlatformPolicy policy)
        {
            if (policy == null) return;
            if (policy.platform == SpectraPlatformKind.PC)
            {
                result.pcMaximumActiveCues = Mathf.Max(1, policy.maximumActiveCues);
                result.pcUpdateRate = Mathf.Max(1, policy.updateRate);
                result.pcMaximumFixtures = Mathf.Max(1, policy.maximumFixtures);
                result.pcMaximumTransparentBeams = Mathf.Max(0, policy.maximumTransparentBeams);
                result.pcAudioReactiveUpdateDivider = Mathf.Max(1, policy.audioReactiveUpdateDivider);
                result.pcShaderQualityTier = Mathf.Clamp(policy.shaderQualityTier, 0, 3);
                result.pcAllowStrobes = policy.allowStrobes;
                result.pcAllowLasers = policy.allowLasers;
            }
            else if (policy.platform == SpectraPlatformKind.Quest)
            {
                result.questMaximumActiveCues = Mathf.Max(1, policy.maximumActiveCues);
                result.questUpdateRate = Mathf.Max(1, policy.updateRate);
                result.questMaximumFixtures = Mathf.Max(1, policy.maximumFixtures);
                result.questMaximumTransparentBeams = Mathf.Max(0, policy.maximumTransparentBeams);
                result.questAudioReactiveUpdateDivider = Mathf.Max(1, policy.audioReactiveUpdateDivider);
                result.questShaderQualityTier = Mathf.Clamp(policy.shaderQualityTier, 0, 3);
                result.questAllowStrobes = policy.allowStrobes;
                result.questAllowLasers = policy.allowLasers;
            }
            else if (policy.platform == SpectraPlatformKind.IOS)
            {
                result.iosMaximumActiveCues = Mathf.Max(1, policy.maximumActiveCues);
                result.iosUpdateRate = Mathf.Max(1, policy.updateRate);
                result.iosMaximumFixtures = Mathf.Max(1, policy.maximumFixtures);
                result.iosMaximumTransparentBeams = Mathf.Max(0, policy.maximumTransparentBeams);
                result.iosAudioReactiveUpdateDivider = Mathf.Max(1, policy.audioReactiveUpdateDivider);
                result.iosShaderQualityTier = Mathf.Clamp(policy.shaderQualityTier, 0, 3);
                result.iosAllowStrobes = policy.allowStrobes;
                result.iosAllowLasers = policy.allowLasers;
            }
            else if (policy.platform == SpectraPlatformKind.Android)
            {
                result.androidMaximumActiveCues = Mathf.Max(1, policy.maximumActiveCues);
                result.androidUpdateRate = Mathf.Max(1, policy.updateRate);
                result.androidMaximumFixtures = Mathf.Max(1, policy.maximumFixtures);
                result.androidMaximumTransparentBeams = Mathf.Max(0, policy.maximumTransparentBeams);
                result.androidAudioReactiveUpdateDivider = Mathf.Max(1, policy.audioReactiveUpdateDivider);
                result.androidShaderQualityTier = Mathf.Clamp(policy.shaderQualityTier, 0, 3);
                result.androidAllowStrobes = policy.allowStrobes;
                result.androidAllowLasers = policy.allowLasers;
            }
        }

        private static void CompileMarkers(SpectraShowAsset asset, SpectraCompiledShow result)
        {
            List<SpectraTimelineMarker> markers = new List<SpectraTimelineMarker>();
            if (asset.markers != null)
                for (int i = 0; i < asset.markers.Length; i++)
                    if (asset.markers[i] != null) markers.Add(asset.markers[i]);
            markers.Sort(delegate(SpectraTimelineMarker a, SpectraTimelineMarker b)
            {
                return a.ResolveSeconds(asset.beatGrid).CompareTo(b.ResolveSeconds(asset.beatGrid));
            });
            result.markerNames = new string[markers.Count];
            result.markerTimes = new float[markers.Count];
            result.markerKinds = new int[markers.Count];
            result.markerHotCues = new bool[markers.Count];
            result.markerHotCueQuantizations = new int[markers.Count];
            result.markerTransitionSeconds = new float[markers.Count];
            result.markerScenes = new bool[markers.Count];
            result.markerSceneBanks = new int[markers.Count];
            result.markerSceneOrders = new int[markers.Count];
            result.markerSceneAutoAdvance = new bool[markers.Count];
            for (int i = 0; i < markers.Count; i++)
            {
                result.markerNames[i] = markers[i].name;
                result.markerTimes[i] = markers[i].ResolveSeconds(asset.beatGrid);
                result.markerKinds[i] = (int)markers[i].kind;
                result.markerHotCues[i] = markers[i].hotCue;
                result.markerHotCueQuantizations[i] = (int)markers[i].hotCueQuantization;
                result.markerTransitionSeconds[i] = Mathf.Clamp(markers[i].transitionSeconds, 0f, 4f);
                result.markerScenes[i] = markers[i].scene;
                result.markerSceneBanks[i] = Mathf.Clamp(markers[i].sceneBank, 0, 7);
                result.markerSceneOrders[i] = Mathf.Max(0, markers[i].sceneOrder);
                result.markerSceneAutoAdvance[i] = markers[i].sceneAutoAdvance;
            }
        }

        private static void CompileTempoMap(SpectraShowAsset asset, SpectraCompiledShow result)
        {
            List<SpectraTempoMarker> tempo = new List<SpectraTempoMarker>();
            if (asset.beatGrid != null && asset.beatGrid.tempoChanges != null)
                for (int i = 0; i < asset.beatGrid.tempoChanges.Length; i++)
                    if (asset.beatGrid.tempoChanges[i] != null)
                        tempo.Add(asset.beatGrid.tempoChanges[i]);
            tempo.Sort(delegate(SpectraTempoMarker a, SpectraTempoMarker b)
            {
                return a.timeSeconds.CompareTo(b.timeSeconds);
            });
            result.tempoMarkerTimes = new float[tempo.Count];
            result.tempoMarkerBpms = new float[tempo.Count];
            result.tempoMarkerNumerators = new int[tempo.Count];
            for (int i = 0; i < tempo.Count; i++)
            {
                result.tempoMarkerTimes[i] = Mathf.Max(0f, tempo[i].timeSeconds);
                result.tempoMarkerBpms[i] = Mathf.Max(1f, tempo[i].bpm);
                result.tempoMarkerNumerators[i] = Mathf.Max(1, tempo[i].numerator);
            }
        }

        private static SpectraFixtureCapability ResolveRequiredCapabilities(SpectraCueValueType type)
        {
            if (type == SpectraCueValueType.Intensity || type == SpectraCueValueType.Blackout)
                return SpectraFixtureCapability.Intensity;
            if (type == SpectraCueValueType.Color) return SpectraFixtureCapability.Color;
            if (type == SpectraCueValueType.Movement) return SpectraFixtureCapability.Movement;
            if (type == SpectraCueValueType.Gobo) return SpectraFixtureCapability.Gobo;
            if (type == SpectraCueValueType.Prism) return SpectraFixtureCapability.Prism;
            if (type == SpectraCueValueType.ZoomFocus) return SpectraFixtureCapability.ZoomFocus;
            if (type == SpectraCueValueType.Strobe) return SpectraFixtureCapability.Strobe;
            if (type == SpectraCueValueType.LaserEnable) return SpectraFixtureCapability.Laser;
            if (type == SpectraCueValueType.AudioReactiveIntensity)
                return SpectraFixtureCapability.Intensity | SpectraFixtureCapability.AudioReactive;
            if (type == SpectraCueValueType.Event) return SpectraFixtureCapability.WorldEvent;
            return SpectraFixtureCapability.None;
        }

        private static void CompileLoops(SpectraShowAsset asset, SpectraCompiledShow result)
        {
            List<SpectraLoopRegion> loops = new List<SpectraLoopRegion>();
            if (asset.loopRegions != null)
                for (int i = 0; i < asset.loopRegions.Length; i++)
                    if (asset.loopRegions[i] != null) loops.Add(asset.loopRegions[i]);
            loops.Sort(delegate(SpectraLoopRegion a, SpectraLoopRegion b)
            {
                return a.startSeconds.CompareTo(b.startSeconds);
            });
            result.loopNames = new string[loops.Count];
            result.loopStarts = new float[loops.Count];
            result.loopEnds = new float[loops.Count];
            result.loopEnabled = new bool[loops.Count];
            result.loopRepeatCounts = new int[loops.Count];
            for (int i = 0; i < loops.Count; i++)
            {
                result.loopNames[i] = loops[i].name;
                result.loopStarts[i] = loops[i].startSeconds;
                result.loopEnds[i] = loops[i].endSeconds;
                result.loopEnabled[i] = loops[i].enabled;
                result.loopRepeatCounts[i] = Mathf.Max(0, loops[i].repeatCount);
            }
        }

        public static int ComputeContentSignature(SpectraCompiledShow show)
        {
            unchecked
            {
                uint hash = 2166136261u;
                HashString(ref hash, show.showId);
                HashInt(ref hash, show.schemaVersion);
                HashFloat(ref hash, show.durationSeconds);
                HashFloat(ref hash, show.bpm);
                HashInt(ref hash, show.beatsPerBar);
                HashFloat(ref hash, show.firstDownbeatSeconds);
                HashInt(ref hash, show.pcMaximumActiveCues);
                HashInt(ref hash, show.questMaximumActiveCues);
                HashInt(ref hash, show.iosMaximumActiveCues);
                HashInt(ref hash, show.androidMaximumActiveCues);
                HashInt(ref hash, show.pcUpdateRate);
                HashInt(ref hash, show.questUpdateRate);
                HashInt(ref hash, show.iosUpdateRate);
                HashInt(ref hash, show.androidUpdateRate);
                HashInt(ref hash, show.pcMaximumFixtures);
                HashInt(ref hash, show.questMaximumFixtures);
                HashInt(ref hash, show.iosMaximumFixtures);
                HashInt(ref hash, show.androidMaximumFixtures);
                HashInt(ref hash, show.pcMaximumTransparentBeams);
                HashInt(ref hash, show.questMaximumTransparentBeams);
                HashInt(ref hash, show.iosMaximumTransparentBeams);
                HashInt(ref hash, show.androidMaximumTransparentBeams);
                HashInt(ref hash, show.pcAudioReactiveUpdateDivider);
                HashInt(ref hash, show.questAudioReactiveUpdateDivider);
                HashInt(ref hash, show.iosAudioReactiveUpdateDivider);
                HashInt(ref hash, show.androidAudioReactiveUpdateDivider);
                HashInt(ref hash, show.pcShaderQualityTier);
                HashInt(ref hash, show.questShaderQualityTier);
                HashInt(ref hash, show.iosShaderQualityTier);
                HashInt(ref hash, show.androidShaderQualityTier);
                HashInt(ref hash, show.pcAllowStrobes ? 1 : 0);
                HashInt(ref hash, show.questAllowStrobes ? 1 : 0);
                HashInt(ref hash, show.iosAllowStrobes ? 1 : 0);
                HashInt(ref hash, show.androidAllowStrobes ? 1 : 0);
                HashInt(ref hash, show.pcAllowLasers ? 1 : 0);
                HashInt(ref hash, show.questAllowLasers ? 1 : 0);
                HashInt(ref hash, show.iosAllowLasers ? 1 : 0);
                HashInt(ref hash, show.androidAllowLasers ? 1 : 0);
                for (int i = 0; i < show.paletteNames.Length; i++)
                {
                    HashString(ref hash, show.paletteNames[i]);
                    HashInt(ref hash, show.paletteOffsets[i]);
                    HashInt(ref hash, show.paletteCounts[i]);
                }
                for (int i = 0; i < show.paletteColors.Length; i++)
                {
                    HashFloat(ref hash, show.paletteColors[i].r);
                    HashFloat(ref hash, show.paletteColors[i].g);
                    HashFloat(ref hash, show.paletteColors[i].b);
                    HashFloat(ref hash, show.paletteColors[i].a);
                }
                for (int i = 0; i < show.performanceMacroNames.Length; i++)
                {
                    HashString(ref hash, show.performanceMacroNames[i]);
                    HashFloat(ref hash, show.performanceMacroDefaults[i]);
                    HashFloat(ref hash, show.performanceMacroSmoothing[i]);
                    HashFloat(ref hash, show.performanceMacroColors[i].r);
                    HashFloat(ref hash, show.performanceMacroColors[i].g);
                    HashFloat(ref hash, show.performanceMacroColors[i].b);
                    HashFloat(ref hash, show.performanceMacroColors[i].a);
                }
                for (int i = 0; i < show.performanceMacroSnapshotNames.Length; i++)
                {
                    HashString(ref hash, show.performanceMacroSnapshotNames[i]);
                    HashFloat(ref hash, show.performanceMacroSnapshotColors[i].r);
                    HashFloat(ref hash, show.performanceMacroSnapshotColors[i].g);
                    HashFloat(ref hash, show.performanceMacroSnapshotColors[i].b);
                    HashFloat(ref hash, show.performanceMacroSnapshotColors[i].a);
                    HashFloat(ref hash, show.performanceMacroSnapshotValues[i].x);
                    HashFloat(ref hash, show.performanceMacroSnapshotValues[i].y);
                    HashFloat(ref hash, show.performanceMacroSnapshotValues[i].z);
                    HashFloat(ref hash, show.performanceMacroSnapshotValues[i].w);
                    HashFloat(ref hash, show.performanceMacroSnapshotTransitionSeconds[i]);
                }
                for (int i = 0; i < show.cueLayerNames.Length; i++)
                {
                    HashString(ref hash, show.cueLayerNames[i]);
                    HashFloat(ref hash, show.cueLayerColors[i].r);
                    HashFloat(ref hash, show.cueLayerColors[i].g);
                    HashFloat(ref hash, show.cueLayerColors[i].b);
                    HashFloat(ref hash, show.cueLayerColors[i].a);
                    HashInt(ref hash, show.cueLayerDefaultEnabled[i] ? 1 : 0);
                    HashInt(ref hash, show.cueLayerPcEnabled[i] ? 1 : 0);
                    HashInt(ref hash, show.cueLayerQuestEnabled[i] ? 1 : 0);
                    HashInt(ref hash, show.cueLayerIosEnabled[i] ? 1 : 0);
                    HashInt(ref hash, show.cueLayerAndroidEnabled[i] ? 1 : 0);
                    HashInt(ref hash, show.cueLayerPriorityBiases[i]);
                    HashInt(ref hash, show.cueLayerMaximumActiveCues[i]);
                }
                for (int i = 0; i < show.runtimeGroupIds.Length; i++)
                {
                    HashInt(ref hash, show.runtimeGroupIds[i]);
                    HashString(ref hash, show.groupStableIds[i]);
                    HashInt(ref hash, show.groupSelections[i]);
                    HashInt(ref hash, show.groupRandomSeeds[i]);
                    HashInt(ref hash, show.groupCapabilityMasks[i]);
                }
                for (int i = 0; i < show.CueCount; i++)
                {
                    HashInt(ref hash, show.cueGroupIndices[i]);
                    HashInt(ref hash, show.cueValueTypes[i]);
                    HashInt(ref hash, show.cueBlendModes[i]);
                    HashInt(ref hash, show.cueEasings[i]);
                    HashInt(ref hash, show.cuePriorities[i]);
                    HashInt(ref hash, show.cueQuestFallbacks[i]);
                    HashInt(ref hash, show.cueIosFallbacks[i]);
                    HashInt(ref hash, show.cueAndroidFallbacks[i]);
                    HashInt(ref hash, show.cueMovementPatterns[i]);
                    HashInt(ref hash, show.cueRandomSeeds[i]);
                    HashInt(ref hash, show.cueAudioBands[i]);
                    HashInt(ref hash, show.cueEventChannels[i]);
                    HashInt(ref hash, show.cueRequiredCapabilities[i]);
                    HashInt(ref hash, show.cueCapabilityFallbacks[i]);
                    HashInt(ref hash, show.cueAutomationModes[i]);
                    HashInt(ref hash, show.cueAutomationOffsets[i]);
                    HashInt(ref hash, show.cueAutomationCounts[i]);
                    HashInt(ref hash, show.cueModulationWaveforms[i]);
                    HashInt(ref hash, show.cueModulationTimeBases[i]);
                    HashInt(ref hash, show.cueModulationModes[i]);
                    HashInt(ref hash, show.cueModulationQuantizeSteps[i]);
                    HashInt(ref hash, show.cuePerformanceMacroIndices[i]);
                    HashInt(ref hash, show.cuePerformanceMacroModes[i]);
                    HashInt(ref hash, show.cueGatePatterns[i]);
                    HashInt(ref hash, show.cueGateTimeBases[i]);
                    HashInt(ref hash, show.cueGateStepCounts[i]);
                    HashInt(ref hash, show.cueGateActiveSteps[i]);
                    HashInt(ref hash, show.cueGateCustomMasks[i]);
                    HashInt(ref hash, show.cuePaletteIndices[i]);
                    HashInt(ref hash, show.cuePaletteModes[i]);
                    HashInt(ref hash, show.cuePaletteTimeBases[i]);
                    HashInt(ref hash, show.cuePalettePrimaryIndices[i]);
                    HashInt(ref hash, show.cuePaletteSecondaryIndices[i]);
                    HashInt(ref hash, show.cuePaletteMacroIndices[i]);
                    HashInt(ref hash, show.cueConditionModes[i]);
                    HashInt(ref hash, show.cueConditionTimeBases[i]);
                    HashInt(ref hash, show.cueConditionEveryNs[i]);
                    HashInt(ref hash, show.cueConditionCycleOffsets[i]);
                    HashInt(ref hash, show.cueConditionMacroIndices[i]);
                    HashInt(ref hash, show.cueConditionAudioBands[i]);
                    HashInt(ref hash, show.cueVariationModes[i]);
                    HashInt(ref hash, show.cueVariationGroups[i]);
                    HashInt(ref hash, show.cueVariationOptions[i]);
                    HashInt(ref hash, show.cueVariationOptionCounts[i]);
                    HashInt(ref hash, show.cueVariationTimeBases[i]);
                    HashInt(ref hash, show.cueVariationSeeds[i]);
                    HashInt(ref hash, show.cueVariationMacroIndices[i]);
                    HashInt(ref hash, show.cueLayerIndices[i]);
                    HashInt(ref hash, show.cueArbitrationModes[i]);
                    HashInt(ref hash, show.cueArbitrationGroups[i]);
                    HashInt(ref hash, show.cueArbitrationTimeBases[i]);
                    HashInt(ref hash, show.cueArbitrationSeeds[i]);
                    HashInt(ref hash, show.cueConditionInverts[i] ? 1 : 0);
                    HashInt(ref hash, show.cueGateInverts[i] ? 1 : 0);
                    HashInt(ref hash, show.cueEventOnce[i] ? 1 : 0);
                    HashFloat(ref hash, show.cueStarts[i]);
                    HashFloat(ref hash, show.cueDurations[i]);
                    HashFloat(ref hash, show.cueFadeIns[i]);
                    HashFloat(ref hash, show.cueFadeOuts[i]);
                    HashFloat(ref hash, show.cueMovementSmoothing[i]);
                    HashFloat(ref hash, show.cueAudioAmounts[i]);
                    HashFloat(ref hash, show.cueAudioFloors[i]);
                    HashFloat(ref hash, show.cueModulationCycleLengths[i]);
                    HashFloat(ref hash, show.cueModulationPhases[i]);
                    HashFloat(ref hash, show.cueModulationDutyCycles[i]);
                    HashFloat(ref hash, show.cueGateStepLengths[i]);
                    HashFloat(ref hash, show.cueGateDutyCycles[i]);
                    HashFloat(ref hash, show.cueGateAttacks[i]);
                    HashFloat(ref hash, show.cueGateReleases[i]);
                    HashFloat(ref hash, show.cueGatePhases[i]);
                    HashFloat(ref hash, show.cuePaletteStepLengths[i]);
                    HashFloat(ref hash, show.cuePalettePhases[i]);
                    HashFloat(ref hash, show.cuePaletteBlends[i]);
                    HashFloat(ref hash, show.cueConditionCycleLengths[i]);
                    HashFloat(ref hash, show.cueConditionPhases[i]);
                    HashFloat(ref hash, show.cueConditionProbabilities[i]);
                    HashFloat(ref hash, show.cueConditionThresholds[i]);
                    HashFloat(ref hash, show.cueVariationCycleLengths[i]);
                    HashFloat(ref hash, show.cueVariationPhases[i]);
                    HashFloat(ref hash, show.cueArbitrationCycleLengths[i]);
                    HashFloat(ref hash, show.cueArbitrationPhases[i]);
                    HashFloat(ref hash, show.cueValues[i].x);
                    HashFloat(ref hash, show.cueValues[i].y);
                    HashFloat(ref hash, show.cueValues[i].z);
                    HashFloat(ref hash, show.cueValues[i].w);
                    HashFloat(ref hash, show.cueColors[i].r);
                    HashFloat(ref hash, show.cueColors[i].g);
                    HashFloat(ref hash, show.cueColors[i].b);
                    HashFloat(ref hash, show.cueColors[i].a);
                    HashFloat(ref hash, show.cueMovementParameters[i].x);
                    HashFloat(ref hash, show.cueMovementParameters[i].y);
                    HashFloat(ref hash, show.cueMovementParameters[i].z);
                    HashFloat(ref hash, show.cueMovementParameters[i].w);
                    HashFloat(ref hash, show.cueModulationOffsets[i].x);
                    HashFloat(ref hash, show.cueModulationOffsets[i].y);
                    HashFloat(ref hash, show.cueModulationOffsets[i].z);
                    HashFloat(ref hash, show.cueModulationOffsets[i].w);
                    HashFloat(ref hash, show.cueModulationDepths[i].x);
                    HashFloat(ref hash, show.cueModulationDepths[i].y);
                    HashFloat(ref hash, show.cueModulationDepths[i].z);
                    HashFloat(ref hash, show.cueModulationDepths[i].w);
                    HashFloat(ref hash, show.cuePerformanceMacroMinimums[i].x);
                    HashFloat(ref hash, show.cuePerformanceMacroMinimums[i].y);
                    HashFloat(ref hash, show.cuePerformanceMacroMinimums[i].z);
                    HashFloat(ref hash, show.cuePerformanceMacroMinimums[i].w);
                    HashFloat(ref hash, show.cuePerformanceMacroMaximums[i].x);
                    HashFloat(ref hash, show.cuePerformanceMacroMaximums[i].y);
                    HashFloat(ref hash, show.cuePerformanceMacroMaximums[i].z);
                    HashFloat(ref hash, show.cuePerformanceMacroMaximums[i].w);
                }
                for (int i = 0; i < show.automationTimes.Length; i++)
                {
                    HashFloat(ref hash, show.automationTimes[i]);
                    HashFloat(ref hash, show.automationValues[i].x);
                    HashFloat(ref hash, show.automationValues[i].y);
                    HashFloat(ref hash, show.automationValues[i].z);
                    HashFloat(ref hash, show.automationValues[i].w);
                    HashInt(ref hash, show.automationInterpolations[i]);
                }
                for (int i = 0; i < show.markerTimes.Length; i++)
                {
                    HashString(ref hash, show.markerNames[i]);
                    HashFloat(ref hash, show.markerTimes[i]);
                    HashInt(ref hash, show.markerKinds[i]);
                    HashInt(ref hash, show.markerHotCues[i] ? 1 : 0);
                    HashInt(ref hash, show.markerHotCueQuantizations[i]);
                    HashFloat(ref hash, show.markerTransitionSeconds[i]);
                    HashInt(ref hash, show.markerScenes[i] ? 1 : 0);
                    HashInt(ref hash, show.markerSceneBanks[i]);
                    HashInt(ref hash, show.markerSceneOrders[i]);
                    HashInt(ref hash, show.markerSceneAutoAdvance[i] ? 1 : 0);
                }
                for (int i = 0; i < show.loopStarts.Length; i++)
                {
                    HashString(ref hash, show.loopNames[i]);
                    HashFloat(ref hash, show.loopStarts[i]);
                    HashFloat(ref hash, show.loopEnds[i]);
                    HashInt(ref hash, show.loopEnabled[i] ? 1 : 0);
                    HashInt(ref hash, show.loopRepeatCounts[i]);
                }
                for (int i = 0; i < show.tempoMarkerTimes.Length; i++)
                {
                    HashFloat(ref hash, show.tempoMarkerTimes[i]);
                    HashFloat(ref hash, show.tempoMarkerBpms[i]);
                    HashInt(ref hash, show.tempoMarkerNumerators[i]);
                }
                return (int)hash;
            }
        }

        private static void HashString(ref uint hash, string value)
        {
            if (value == null) value = string.Empty;
            for (int i = 0; i < value.Length; i++)
            {
                hash ^= value[i];
                hash *= 16777619u;
            }
        }

        private static void HashInt(ref uint hash, int value)
        {
            unchecked
            {
                hash ^= (uint)value;
                hash *= 16777619u;
            }
        }

        private static void HashFloat(ref uint hash, float value)
        {
            byte[] bytes = BitConverter.GetBytes(value);
            HashInt(ref hash, BitConverter.ToInt32(bytes, 0));
        }

        [MenuItem("SpectraOverdrive/Show Programmer/Compile Selected Show")]
        private static void CompileSelected()
        {
            SpectraShowAsset asset = Selection.activeObject as SpectraShowAsset;
            if (asset == null) { EditorUtility.DisplayDialog("SpectraOverdrive", "Select a SpectraShowAsset first.", "OK"); return; }
            SpectraCompiledShow compiled = Compile(asset);
            string output = EditorUtility.SaveFilePanel("Save compiled show JSON", "", asset.showName + ".compiled.spectrashow.json", "json");
            if (string.IsNullOrEmpty(output)) return;
            System.IO.File.WriteAllText(output, JsonUtility.ToJson(compiled, true));
            AssetDatabase.Refresh();
            EditorUtility.DisplayDialog("SpectraOverdrive", "Compiled " + compiled.CueCount + " cues successfully.", "OK");
        }
    }
}
