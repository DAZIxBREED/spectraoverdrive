using System;
using UnityEngine;

namespace SpectraOverdrive
{
    [Serializable]
    public class SpectraCompiledShow
    {
        public int schemaVersion;
        public string showId;
        public string showName;
        public string contentHash;
        public int contentSignature;
        public float durationSeconds;
        public float bpm;
        public int beatsPerBar;
        public float firstDownbeatSeconds;
        public int pcMaximumActiveCues = 128;
        public int questMaximumActiveCues = 48;
        public int iosMaximumActiveCues = 32;
        public int androidMaximumActiveCues = 32;
        public int pcUpdateRate = 60;
        public int questUpdateRate = 36;
        public int iosUpdateRate = 30;
        public int androidUpdateRate = 30;
        public int pcMaximumFixtures = 128;
        public int questMaximumFixtures = 64;
        public int iosMaximumFixtures = 48;
        public int androidMaximumFixtures = 48;
        public int pcMaximumTransparentBeams = 64;
        public int questMaximumTransparentBeams = 20;
        public int iosMaximumTransparentBeams = 12;
        public int androidMaximumTransparentBeams = 10;
        public int pcAudioReactiveUpdateDivider = 1;
        public int questAudioReactiveUpdateDivider = 2;
        public int iosAudioReactiveUpdateDivider = 3;
        public int androidAudioReactiveUpdateDivider = 3;
        public int pcShaderQualityTier = 3;
        public int questShaderQualityTier = 2;
        public int iosShaderQualityTier = 1;
        public int androidShaderQualityTier = 1;
        public bool pcAllowStrobes = true;
        public bool questAllowStrobes = true;
        public bool iosAllowStrobes = true;
        public bool androidAllowStrobes = true;
        public bool pcAllowLasers = true;
        public bool questAllowLasers = true;
        public bool iosAllowLasers = true;
        public bool androidAllowLasers = true;
        public string[] paletteNames = new string[0];
        public int[] paletteOffsets = new int[0];
        public int[] paletteCounts = new int[0];
        public Color[] paletteColors = new Color[0];
        public string[] performanceMacroNames = new string[0];
        public float[] performanceMacroDefaults = new float[0];
        public float[] performanceMacroSmoothing = new float[0];
        public Color[] performanceMacroColors = new Color[0];
        public string[] performanceMacroSnapshotNames = new string[0];
        public Color[] performanceMacroSnapshotColors = new Color[0];
        public Vector4[] performanceMacroSnapshotValues = new Vector4[0];
        public float[] performanceMacroSnapshotTransitionSeconds = new float[0];
        public int[] runtimeGroupIds = new int[0];
        public string[] groupStableIds = new string[0];
        public int[] groupSelections = new int[0];
        public int[] groupRandomSeeds = new int[0];
        public int[] groupCapabilityMasks = new int[0];
        public int[] cueGroupIndices = new int[0];
        public int[] cueValueTypes = new int[0];
        public int[] cueBlendModes = new int[0];
        public int[] cueEasings = new int[0];
        public int[] cuePriorities = new int[0];
        public int[] cueQuestFallbacks = new int[0];
        public int[] cueIosFallbacks = new int[0];
        public int[] cueAndroidFallbacks = new int[0];
        public int[] cueMovementPatterns = new int[0];
        public int[] cueRandomSeeds = new int[0];
        public int[] cueAudioBands = new int[0];
        public int[] cueEventChannels = new int[0];
        public int[] cueRequiredCapabilities = new int[0];
        public int[] cueCapabilityFallbacks = new int[0];
        public int[] cueAutomationModes = new int[0];
        public int[] cueAutomationOffsets = new int[0];
        public int[] cueAutomationCounts = new int[0];
        public int[] cueModulationWaveforms = new int[0];
        public int[] cueModulationTimeBases = new int[0];
        public int[] cueModulationModes = new int[0];
        public int[] cueModulationQuantizeSteps = new int[0];
        public int[] cuePerformanceMacroIndices = new int[0];
        public int[] cuePerformanceMacroModes = new int[0];
        public int[] cueGatePatterns = new int[0];
        public int[] cueGateTimeBases = new int[0];
        public int[] cueGateStepCounts = new int[0];
        public int[] cueGateActiveSteps = new int[0];
        public int[] cueGateCustomMasks = new int[0];
        public int[] cuePaletteIndices = new int[0];
        public int[] cuePaletteModes = new int[0];
        public int[] cuePaletteTimeBases = new int[0];
        public int[] cuePalettePrimaryIndices = new int[0];
        public int[] cuePaletteSecondaryIndices = new int[0];
        public int[] cuePaletteMacroIndices = new int[0];
        public int[] cueConditionModes = new int[0];
        public int[] cueConditionTimeBases = new int[0];
        public int[] cueConditionEveryNs = new int[0];
        public int[] cueConditionCycleOffsets = new int[0];
        public int[] cueConditionMacroIndices = new int[0];
        public int[] cueConditionAudioBands = new int[0];
        public int[] cueVariationModes = new int[0];
        public int[] cueVariationGroups = new int[0];
        public int[] cueVariationOptions = new int[0];
        public int[] cueVariationOptionCounts = new int[0];
        public int[] cueVariationTimeBases = new int[0];
        public int[] cueVariationSeeds = new int[0];
        public int[] cueVariationMacroIndices = new int[0];
        public bool[] cueConditionInverts = new bool[0];
        public bool[] cueGateInverts = new bool[0];
        public bool[] cueEventOnce = new bool[0];
        public float[] cueStarts = new float[0];
        public float[] cueDurations = new float[0];
        public float[] cueFadeIns = new float[0];
        public float[] cueFadeOuts = new float[0];
        public float[] cueMovementSmoothing = new float[0];
        public float[] cueAudioAmounts = new float[0];
        public float[] cueAudioFloors = new float[0];
        public float[] cueModulationCycleLengths = new float[0];
        public float[] cueModulationPhases = new float[0];
        public float[] cueModulationDutyCycles = new float[0];
        public float[] cueGateStepLengths = new float[0];
        public float[] cueGateDutyCycles = new float[0];
        public float[] cueGateAttacks = new float[0];
        public float[] cueGateReleases = new float[0];
        public float[] cueGatePhases = new float[0];
        public float[] cuePaletteStepLengths = new float[0];
        public float[] cuePalettePhases = new float[0];
        public float[] cuePaletteBlends = new float[0];
        public float[] cueConditionCycleLengths = new float[0];
        public float[] cueConditionPhases = new float[0];
        public float[] cueConditionProbabilities = new float[0];
        public float[] cueConditionThresholds = new float[0];
        public float[] cueVariationCycleLengths = new float[0];
        public float[] cueVariationPhases = new float[0];
        public Color[] cueColors = new Color[0];
        public Vector4[] cueValues = new Vector4[0];
        public Vector4[] cueMovementParameters = new Vector4[0];
        public Vector4[] cueModulationOffsets = new Vector4[0];
        public Vector4[] cueModulationDepths = new Vector4[0];
        public Vector4[] cuePerformanceMacroMinimums = new Vector4[0];
        public Vector4[] cuePerformanceMacroMaximums = new Vector4[0];
        public float[] automationTimes = new float[0];
        public Vector4[] automationValues = new Vector4[0];
        public int[] automationInterpolations = new int[0];
        public string[] markerNames = new string[0];
        public float[] markerTimes = new float[0];
        public int[] markerKinds = new int[0];
        public bool[] markerHotCues = new bool[0];
        public int[] markerHotCueQuantizations = new int[0];
        public float[] markerTransitionSeconds = new float[0];
        public bool[] markerScenes = new bool[0];
        public int[] markerSceneBanks = new int[0];
        public int[] markerSceneOrders = new int[0];
        public bool[] markerSceneAutoAdvance = new bool[0];
        public string[] loopNames = new string[0];
        public float[] loopStarts = new float[0];
        public float[] loopEnds = new float[0];
        public bool[] loopEnabled = new bool[0];
        public int[] loopRepeatCounts = new int[0];
        public float[] tempoMarkerTimes = new float[0];
        public float[] tempoMarkerBpms = new float[0];
        public int[] tempoMarkerNumerators = new int[0];

        public int CueCount { get { return cueStarts == null ? 0 : cueStarts.Length; } }

        public bool HasConsistentArrays()
        {
            int count = CueCount;
            return cueGroupIndices != null && cueGroupIndices.Length == count
                && cueValueTypes != null && cueValueTypes.Length == count
                && cueBlendModes != null && cueBlendModes.Length == count
                && cueEasings != null && cueEasings.Length == count
                && cuePriorities != null && cuePriorities.Length == count
                && cueQuestFallbacks != null && cueQuestFallbacks.Length == count
                && cueIosFallbacks != null && cueIosFallbacks.Length == count
                && cueAndroidFallbacks != null && cueAndroidFallbacks.Length == count
                && cueMovementPatterns != null && cueMovementPatterns.Length == count
                && cueRandomSeeds != null && cueRandomSeeds.Length == count
                && cueAudioBands != null && cueAudioBands.Length == count
                && cueEventChannels != null && cueEventChannels.Length == count
                && cueRequiredCapabilities != null && cueRequiredCapabilities.Length == count
                && cueCapabilityFallbacks != null && cueCapabilityFallbacks.Length == count
                && cueAutomationModes != null && cueAutomationModes.Length == count
                && cueAutomationOffsets != null && cueAutomationOffsets.Length == count
                && cueAutomationCounts != null && cueAutomationCounts.Length == count
                && cueModulationWaveforms != null && cueModulationWaveforms.Length == count
                && cueModulationTimeBases != null && cueModulationTimeBases.Length == count
                && cueModulationModes != null && cueModulationModes.Length == count
                && cueModulationQuantizeSteps != null && cueModulationQuantizeSteps.Length == count
                && cuePerformanceMacroIndices != null && cuePerformanceMacroIndices.Length == count
                && cuePerformanceMacroModes != null && cuePerformanceMacroModes.Length == count
                && cueGatePatterns != null && cueGatePatterns.Length == count
                && cueGateTimeBases != null && cueGateTimeBases.Length == count
                && cueGateStepCounts != null && cueGateStepCounts.Length == count
                && cueGateActiveSteps != null && cueGateActiveSteps.Length == count
                && cueGateCustomMasks != null && cueGateCustomMasks.Length == count
                && cuePaletteIndices != null && cuePaletteIndices.Length == count
                && cuePaletteModes != null && cuePaletteModes.Length == count
                && cuePaletteTimeBases != null && cuePaletteTimeBases.Length == count
                && cuePalettePrimaryIndices != null && cuePalettePrimaryIndices.Length == count
                && cuePaletteSecondaryIndices != null && cuePaletteSecondaryIndices.Length == count
                && cuePaletteMacroIndices != null && cuePaletteMacroIndices.Length == count
                && cueConditionModes != null && cueConditionModes.Length == count
                && cueConditionTimeBases != null && cueConditionTimeBases.Length == count
                && cueConditionEveryNs != null && cueConditionEveryNs.Length == count
                && cueConditionCycleOffsets != null && cueConditionCycleOffsets.Length == count
                && cueConditionMacroIndices != null && cueConditionMacroIndices.Length == count
                && cueConditionAudioBands != null && cueConditionAudioBands.Length == count
                && cueVariationModes != null && cueVariationModes.Length == count
                && cueVariationGroups != null && cueVariationGroups.Length == count
                && cueVariationOptions != null && cueVariationOptions.Length == count
                && cueVariationOptionCounts != null && cueVariationOptionCounts.Length == count
                && cueVariationTimeBases != null && cueVariationTimeBases.Length == count
                && cueVariationSeeds != null && cueVariationSeeds.Length == count
                && cueVariationMacroIndices != null && cueVariationMacroIndices.Length == count
                && cueConditionInverts != null && cueConditionInverts.Length == count
                && cueGateInverts != null && cueGateInverts.Length == count
                && cueEventOnce != null && cueEventOnce.Length == count
                && cueDurations != null && cueDurations.Length == count
                && cueFadeIns != null && cueFadeIns.Length == count
                && cueFadeOuts != null && cueFadeOuts.Length == count
                && cueMovementSmoothing != null && cueMovementSmoothing.Length == count
                && cueAudioAmounts != null && cueAudioAmounts.Length == count
                && cueAudioFloors != null && cueAudioFloors.Length == count
                && cueModulationCycleLengths != null && cueModulationCycleLengths.Length == count
                && cueModulationPhases != null && cueModulationPhases.Length == count
                && cueModulationDutyCycles != null && cueModulationDutyCycles.Length == count
                && cueGateStepLengths != null && cueGateStepLengths.Length == count
                && cueGateDutyCycles != null && cueGateDutyCycles.Length == count
                && cueGateAttacks != null && cueGateAttacks.Length == count
                && cueGateReleases != null && cueGateReleases.Length == count
                && cueGatePhases != null && cueGatePhases.Length == count
                && cuePaletteStepLengths != null && cuePaletteStepLengths.Length == count
                && cuePalettePhases != null && cuePalettePhases.Length == count
                && cuePaletteBlends != null && cuePaletteBlends.Length == count
                && cueConditionCycleLengths != null && cueConditionCycleLengths.Length == count
                && cueConditionPhases != null && cueConditionPhases.Length == count
                && cueConditionProbabilities != null && cueConditionProbabilities.Length == count
                && cueConditionThresholds != null && cueConditionThresholds.Length == count
                && cueVariationCycleLengths != null && cueVariationCycleLengths.Length == count
                && cueVariationPhases != null && cueVariationPhases.Length == count
                && cueColors != null && cueColors.Length == count
                && cueValues != null && cueValues.Length == count
                && cueMovementParameters != null && cueMovementParameters.Length == count
                && cueModulationOffsets != null && cueModulationOffsets.Length == count
                && cueModulationDepths != null && cueModulationDepths.Length == count
                && cuePerformanceMacroMinimums != null && cuePerformanceMacroMinimums.Length == count
                && cuePerformanceMacroMaximums != null && cuePerformanceMacroMaximums.Length == count
                && HasConsistentPalettes()
                && HasConsistentPerformanceMacros()
                && HasConsistentPerformanceMacroSnapshots()
                && HasConsistentAutomation()
                && HasConsistentGroups()
                && HasConsistentMarkers()
                && HasConsistentLoops()
                && HasConsistentTempoMap();
        }

        public bool HasConsistentGroups()
        {
            int count = runtimeGroupIds == null ? 0 : runtimeGroupIds.Length;
            return groupStableIds != null && groupStableIds.Length == count
                && groupSelections != null && groupSelections.Length == count
                && groupRandomSeeds != null && groupRandomSeeds.Length == count
                && groupCapabilityMasks != null && groupCapabilityMasks.Length == count;
        }

        public bool HasConsistentMarkers()
        {
            int count = markerTimes == null ? 0 : markerTimes.Length;
            return markerNames != null && markerNames.Length == count
                && markerKinds != null && markerKinds.Length == count
                && markerHotCues != null && markerHotCues.Length == count
                && markerHotCueQuantizations != null && markerHotCueQuantizations.Length == count
                && markerTransitionSeconds != null && markerTransitionSeconds.Length == count
                && markerScenes != null && markerScenes.Length == count
                && markerSceneBanks != null && markerSceneBanks.Length == count
                && markerSceneOrders != null && markerSceneOrders.Length == count
                && markerSceneAutoAdvance != null && markerSceneAutoAdvance.Length == count;
        }

        public bool HasConsistentPalettes()
        {
            int count = paletteNames == null ? 0 : paletteNames.Length;
            if (count > 16 || paletteOffsets == null || paletteOffsets.Length != count
                || paletteCounts == null || paletteCounts.Length != count
                || paletteColors == null) return false;
            for (int i = 0; i < count; i++)
            {
                int offset = paletteOffsets[i];
                int length = paletteCounts[i];
                if (offset < 0 || length < 1 || length > 16
                    || offset + length > paletteColors.Length) return false;
            }
            return true;
        }

        public bool HasConsistentPerformanceMacros()
        {
            int count = performanceMacroNames == null ? 0 : performanceMacroNames.Length;
            return count <= 4
                && performanceMacroDefaults != null && performanceMacroDefaults.Length == count
                && performanceMacroSmoothing != null && performanceMacroSmoothing.Length == count
                && performanceMacroColors != null && performanceMacroColors.Length == count;
        }

        public bool HasConsistentPerformanceMacroSnapshots()
        {
            int count = performanceMacroSnapshotNames == null
                ? 0 : performanceMacroSnapshotNames.Length;
            return count <= 16
                && performanceMacroSnapshotColors != null
                    && performanceMacroSnapshotColors.Length == count
                && performanceMacroSnapshotValues != null
                    && performanceMacroSnapshotValues.Length == count
                && performanceMacroSnapshotTransitionSeconds != null
                    && performanceMacroSnapshotTransitionSeconds.Length == count;
        }

        public bool HasConsistentLoops()
        {
            int count = loopStarts == null ? 0 : loopStarts.Length;
            return loopNames != null && loopNames.Length == count
                && loopEnds != null && loopEnds.Length == count
                && loopEnabled != null && loopEnabled.Length == count
                && loopRepeatCounts != null && loopRepeatCounts.Length == count;
        }

        public bool HasConsistentAutomation()
        {
            int keyCount = automationTimes == null ? 0 : automationTimes.Length;
            if (automationValues == null || automationValues.Length != keyCount
                || automationInterpolations == null || automationInterpolations.Length != keyCount)
                return false;
            if (cueAutomationOffsets == null || cueAutomationCounts == null) return false;
            for (int i = 0; i < cueAutomationOffsets.Length; i++)
            {
                int offset = cueAutomationOffsets[i];
                int count = cueAutomationCounts[i];
                if (offset < 0 || count < 0 || offset + count > keyCount) return false;
            }
            return true;
        }

        public bool HasConsistentTempoMap()
        {
            int count = tempoMarkerTimes == null ? 0 : tempoMarkerTimes.Length;
            return tempoMarkerBpms != null && tempoMarkerBpms.Length == count
                && tempoMarkerNumerators != null && tempoMarkerNumerators.Length == count;
        }
    }
}
