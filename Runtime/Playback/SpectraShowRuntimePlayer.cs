using UdonSharp;
using UnityEngine;
using VRC.SDKBase;

namespace SpectraOverdrive
{
    [UdonBehaviourSyncMode(BehaviourSyncMode.None)]
    public class SpectraShowRuntimePlayer : UdonSharpBehaviour
    {
        [Header("Baked Udon-safe show data")]
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
        public string[] cueLayerNames = new string[0];
        public Color[] cueLayerColors = new Color[0];
        public bool[] cueLayerDefaultEnabled = new bool[0];
        public bool[] cueLayerPcEnabled = new bool[0];
        public bool[] cueLayerQuestEnabled = new bool[0];
        public bool[] cueLayerIosEnabled = new bool[0];
        public bool[] cueLayerAndroidEnabled = new bool[0];
        public int[] cueLayerPriorityBiases = new int[0];
        public int[] cueLayerMaximumActiveCues = new int[0];
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
        public int[] cueLayerIndices = new int[0];
        public int[] cueArbitrationModes = new int[0];
        public int[] cueArbitrationGroups = new int[0];
        public int[] cueArbitrationTimeBases = new int[0];
        public int[] cueArbitrationSeeds = new int[0];
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
        public float[] cueArbitrationCycleLengths = new float[0];
        public float[] cueArbitrationPhases = new float[0];
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

        public SpectraFixtureGroup[] groups;
        public SpectraPlatformManager platformManager;
        public SpectraLocalQualityController qualityController;
        public SpectraOverdriveBus bus;
        public SpectraAudioLinkAdapter audioLinkAdapter;
        public SpectraLiveOverrideLayer overrideLayer;
        public SpectraShowEventRouter eventRouter;
        public SpectraShowSnapshotCache snapshotCache;
        public bool playOnStart;
        public bool externalClock;
        [Range(0.25f, 4f)] public float playbackSpeed = 1f;
        public bool showStrobesEnabled = true;
        public bool showLasersEnabled = true;
        public bool localStrobesEnabled = true;
        public bool localLasersEnabled = true;
        [Range(0f, 1f)] public float localBrightnessLimit = 1f;
        [Range(0f, 1f)] public float localMovementLimit = 1f;
        [Range(0f, 30f)] public float localStrobeFrequencyLimit = 20f;
        public bool localRapidColorChangesEnabled = true;
        [Range(0f, 2f)] public float localColorTransitionSeconds;
        public SpectraShowPlaybackState state;
        public float showTime;
        public bool emergencyBlackout;
        public SpectraPlatformKind localPlatform = SpectraPlatformKind.Unknown;
        public int selectedLoopIndex = -1;
        public int activeCueCount;
        public int droppedCueCount;
        public int activeFixtureBudget;
        public int activeTransparentBeamBudget;
        public int activeShaderQualityTier;
        public int activeAudioReactiveUpdateDivider;
        [Range(0f, 1f)] public float performanceMacro0 = 1f;
        [Range(0f, 1f)] public float performanceMacro1 = 1f;
        [Range(0f, 1f)] public float performanceMacro2 = 1f;
        [Range(0f, 1f)] public float performanceMacro3 = 1f;
        [Tooltip("Bit mask for enabled authored cue layers. Layer-free cues are always eligible.")]
        public int cueLayerEnabledMask = -1;
        [Tooltip("When non-zero, only layers present in this mask are admitted.")]
        public int cueLayerSoloMask;
        public int arbitrationSuppressedCueCount;
        public int arbitrationConfigurationMismatchCount;
        public int layerSuppressedCueCount;

        private float _playStartedAt;
        private float _playStartedOffset;
        private float _nextEvaluationTime;
        private float _resolvedStrobeHz;
        private float _resolvedLaserEnabled;
        private int[] _selectedCueIndices;
        private int[] _candidateCueIndices;
        private int[] _selectedLayerCounts;
        private int[] _arbitrationWinners;
        private int[] _arbitrationCandidateCounts;
        private int[] _arbitrationModes;
        private int[] _arbitrationTimeBases;
        private int[] _arbitrationSeeds;
        private float[] _arbitrationCycleLengths;
        private float[] _arbitrationPhases;
        private bool[] _eventFired;
        private float _lastAppliedTime = -1f;
        private int _evaluationFrame;
        private Color[] _smoothedGroupColors;
        private int _platformBaseBeamBudget;
        private int _showSafetyId;
        private int _showTimeId;
        private int _shaderQualityTierId;
        private int _audioReactiveUpdateDividerId;

        public int CueCount { get { return cueStarts == null ? 0 : cueStarts.Length; } }

        private void Start()
        {
            EnsureShaderPropertyIds();
            ResolveLocalPlatform();
            ApplyRuntimePlatformBudgets();
            ResetCueLayerMasksToDefaults();
            if (!externalClock) ResetPerformanceMacrosToDefaults();
            if (externalClock) return;
            if (playOnStart) Play(); else ApplyAtTime(0f);
        }

        private void Update()
        {
            if (state != SpectraShowPlaybackState.Playing || durationSeconds <= 0f) return;
            float updateInterval = 1f / Mathf.Max(1, ResolveUpdateRate());
            if (Time.time < _nextEvaluationTime) return;
            _nextEvaluationTime = Time.time + updateInterval;
            if (externalClock) return;
            float rawShowTime = _playStartedOffset + (Time.time - _playStartedAt) * playbackSpeed;
            showTime = ResolveLoopedTime(rawShowTime);
            if (showTime >= durationSeconds) { Stop(); return; }
            ApplyAtTime(showTime);
        }

        public void Play()
        {
            if (!IsShowUsable()) return;
            _playStartedOffset = Mathf.Clamp(showTime, 0f, durationSeconds);
            _playStartedAt = Time.time;
            _nextEvaluationTime = 0f;
            state = SpectraShowPlaybackState.Playing;
        }

        public void Pause()
        {
            if (state != SpectraShowPlaybackState.Playing) return;
            showTime = ResolveLoopedTime(
                _playStartedOffset + (Time.time - _playStartedAt) * playbackSpeed);
            state = SpectraShowPlaybackState.Paused;
            ApplyAtTime(showTime);
        }

        public void Stop()
        {
            state = SpectraShowPlaybackState.Stopped;
            showTime = 0f;
            ApplyAtTime(0f);
        }

        public void Seek(float seconds)
        {
            if (!IsShowUsable()) return;
            showTime = Mathf.Clamp(seconds, 0f, durationSeconds);
            if (state == SpectraShowPlaybackState.Playing) { _playStartedOffset = showTime; _playStartedAt = Time.time; }
            ApplyAtTime(showTime);
        }

        public void ApplyExternalClock(float seconds, SpectraShowPlaybackState playbackState, float speed)
        {
            externalClock = true;
            playbackSpeed = Mathf.Clamp(speed, 0.25f, 4f);
            state = playbackState;
            showTime = Mathf.Clamp(ResolveLoopedTime(seconds), 0f, durationSeconds);
            ApplyAtTime(showTime);
        }

        public void ReleaseExternalClock()
        {
            externalClock = false;
            if (state == SpectraShowPlaybackState.Playing)
            {
                _playStartedOffset = showTime;
                _playStartedAt = Time.time;
            }
        }

        public void SetEmergencyBlackout(bool enabled) { emergencyBlackout = enabled; ApplyAtTime(showTime); }

        public bool IsLoopSelectionUsable(int loopIndex)
        {
            return IsLoopUsable(loopIndex);
        }

        public void SetLoop(int loopIndex)
        {
            selectedLoopIndex = IsLoopUsable(loopIndex) ? loopIndex : -1;
        }

        public void ClearLoop() { selectedLoopIndex = -1; }

        public void SeekNextMarker()
        {
            if (markerTimes == null) return;
            for (int i = 0; i < markerTimes.Length; i++)
                if (markerTimes[i] > showTime + 0.001f) { Seek(markerTimes[i]); return; }
        }

        public void SeekPreviousMarker()
        {
            if (markerTimes == null) return;
            for (int i = markerTimes.Length - 1; i >= 0; i--)
                if (markerTimes[i] < showTime - 0.001f) { Seek(markerTimes[i]); return; }
            Seek(0f);
        }

        public bool IsHotCueUsable(int markerIndex)
        {
            return HasConsistentMarkerArrays()
                && markerIndex >= 0
                && markerIndex < markerTimes.Length
                && markerHotCues[markerIndex];
        }

        public float GetHotCueTargetTime(int markerIndex)
        {
            return IsHotCueUsable(markerIndex)
                ? Mathf.Clamp(markerTimes[markerIndex], 0f, durationSeconds)
                : showTime;
        }

        public float GetHotCueTransitionSeconds(int markerIndex)
        {
            return IsHotCueUsable(markerIndex)
                ? Mathf.Clamp(markerTransitionSeconds[markerIndex], 0f, 4f)
                : 0f;
        }

        public bool IsSceneUsable(int markerIndex, int sceneBank)
        {
            return IsHotCueUsable(markerIndex)
                && markerScenes != null
                && markerSceneBanks != null
                && markerIndex < markerScenes.Length
                && markerIndex < markerSceneBanks.Length
                && markerScenes[markerIndex]
                && markerSceneBanks[markerIndex] == sceneBank;
        }

        public int GetSceneOrder(int markerIndex)
        {
            return markerSceneOrders != null
                && markerIndex >= 0
                && markerIndex < markerSceneOrders.Length
                ? markerSceneOrders[markerIndex] : int.MaxValue;
        }

        public void SetPerformanceMacroValues(float macro0, float macro1, float macro2, float macro3)
        {
            performanceMacro0 = Mathf.Clamp01(macro0);
            performanceMacro1 = Mathf.Clamp01(macro1);
            performanceMacro2 = Mathf.Clamp01(macro2);
            performanceMacro3 = Mathf.Clamp01(macro3);
        }

        public void ResetPerformanceMacrosToDefaults()
        {
            performanceMacro0 = GetPerformanceMacroDefault(0);
            performanceMacro1 = GetPerformanceMacroDefault(1);
            performanceMacro2 = GetPerformanceMacroDefault(2);
            performanceMacro3 = GetPerformanceMacroDefault(3);
        }

        public float GetPerformanceMacroDefault(int index)
        {
            if (performanceMacroDefaults == null || index < 0
                || index >= performanceMacroDefaults.Length)
                return 1f;
            return Mathf.Clamp01(performanceMacroDefaults[index]);
        }

        public float GetPerformanceMacroSmoothing(int index)
        {
            if (performanceMacroSmoothing == null || index < 0
                || index >= performanceMacroSmoothing.Length)
                return 0f;
            return Mathf.Clamp(performanceMacroSmoothing[index], 0f, 4f);
        }

        public int GetCueLayerCount()
        {
            return HasConsistentCueLayerArrays() ? cueLayerNames.Length : 0;
        }

        public bool IsCueLayerUsable(int index)
        {
            return HasConsistentCueLayerArrays()
                && index >= 0 && index < cueLayerNames.Length;
        }

        public string GetCueLayerName(int index)
        {
            return IsCueLayerUsable(index) ? cueLayerNames[index] : string.Empty;
        }

        public Color GetCueLayerColor(int index)
        {
            return IsCueLayerUsable(index) ? cueLayerColors[index] : Color.white;
        }

        public bool IsCueLayerEnabled(int index)
        {
            if (!IsCueLayerUsable(index)) return false;
            int bit = 1 << index;
            return (cueLayerEnabledMask & bit) != 0
                && (cueLayerSoloMask == 0 || (cueLayerSoloMask & bit) != 0)
                && IsCueLayerAllowedOnPlatform(index);
        }

        public void SetCueLayerMasks(int enabledMask, int soloMask)
        {
            int layerCount = GetCueLayerCount();
            int validMask = layerCount <= 0 ? 0 : (1 << layerCount) - 1;
            cueLayerEnabledMask = enabledMask & validMask;
            cueLayerSoloMask = soloMask & cueLayerEnabledMask & validMask;
        }

        public int GetDefaultCueLayerEnabledMask()
        {
            if (!HasConsistentCueLayerArrays()) return -1;
            int mask = 0;
            for (int i = 0; i < cueLayerDefaultEnabled.Length; i++)
                if (cueLayerDefaultEnabled[i]) mask |= 1 << i;
            return mask;
        }

        public void ResetCueLayerMasksToDefaults()
        {
            cueLayerEnabledMask = GetDefaultCueLayerEnabledMask();
            cueLayerSoloMask = 0;
        }

        public int GetPerformanceMacroSnapshotCount()
        {
            return HasConsistentPerformanceMacroSnapshotArrays()
                ? performanceMacroSnapshotNames.Length : 0;
        }

        public bool IsPerformanceMacroSnapshotUsable(int index)
        {
            return HasConsistentPerformanceMacroSnapshotArrays()
                && index >= 0 && index < performanceMacroSnapshotNames.Length;
        }

        public string GetPerformanceMacroSnapshotName(int index)
        {
            return IsPerformanceMacroSnapshotUsable(index)
                ? performanceMacroSnapshotNames[index] : string.Empty;
        }

        public Color GetPerformanceMacroSnapshotColor(int index)
        {
            return IsPerformanceMacroSnapshotUsable(index)
                ? performanceMacroSnapshotColors[index] : Color.white;
        }

        public Vector4 GetPerformanceMacroSnapshotValues(int index)
        {
            if (!IsPerformanceMacroSnapshotUsable(index)) return Vector4.one;
            Vector4 values = performanceMacroSnapshotValues[index];
            values.x = Mathf.Clamp01(values.x);
            values.y = Mathf.Clamp01(values.y);
            values.z = Mathf.Clamp01(values.z);
            values.w = Mathf.Clamp01(values.w);
            return values;
        }

        public float GetPerformanceMacroSnapshotTransitionSeconds(int index)
        {
            return IsPerformanceMacroSnapshotUsable(index)
                ? Mathf.Max(0f, performanceMacroSnapshotTransitionSeconds[index]) : 0f;
        }

        public float ResolveHotCueExecutionShowTime(int markerIndex, float currentShowTime)
        {
            if (!IsHotCueUsable(markerIndex)) return currentShowTime;
            SpectraHotCueQuantization quantization =
                (SpectraHotCueQuantization)markerHotCueQuantizations[markerIndex];
            if (quantization == SpectraHotCueQuantization.Immediate)
                return currentShowTime;
            float barBeats = Mathf.Max(1f, RuntimeBeatsPerBarAtTime(currentShowTime));
            float quantum = 1f;
            if (quantization == SpectraHotCueQuantization.HalfBar) quantum = barBeats * 0.5f;
            else if (quantization == SpectraHotCueQuantization.Bar) quantum = barBeats;
            else if (quantization == SpectraHotCueQuantization.TwoBars) quantum = barBeats * 2f;
            else if (quantization == SpectraHotCueQuantization.FourBars) quantum = barBeats * 4f;
            float currentBeat = RuntimeSecondsToBeat(currentShowTime);
            float nextBeat = Mathf.Ceil((currentBeat + 0.0001f) / quantum) * quantum;
            return Mathf.Max(currentShowTime, RuntimeBeatToSeconds(nextBeat));
        }

        public float RuntimeSecondsToBeat(float seconds)
        {
            if (seconds <= firstDownbeatSeconds) return 0f;
            float segmentStart = firstDownbeatSeconds;
            float consumedBeats = 0f;
            float currentBpm = Mathf.Max(1f, bpm);
            for (int i = 0; i < tempoMarkerTimes.Length; i++)
            {
                float markerTime = tempoMarkerTimes[i];
                if (markerTime <= segmentStart) continue;
                if (markerTime >= seconds) break;
                consumedBeats += (markerTime - segmentStart) * currentBpm / 60f;
                segmentStart = markerTime;
                currentBpm = Mathf.Max(1f, tempoMarkerBpms[i]);
            }
            return Mathf.Max(0f, consumedBeats
                + (seconds - segmentStart) * currentBpm / 60f);
        }

        public float RuntimeBeatToSeconds(float targetBeat)
        {
            if (targetBeat <= 0f) return firstDownbeatSeconds;
            float seconds = firstDownbeatSeconds;
            float consumedBeats = 0f;
            float currentBpm = Mathf.Max(1f, bpm);
            for (int i = 0; i < tempoMarkerTimes.Length; i++)
            {
                float markerTime = tempoMarkerTimes[i];
                if (markerTime <= seconds) continue;
                float segmentBeats = (markerTime - seconds) * currentBpm / 60f;
                if (consumedBeats + segmentBeats >= targetBeat)
                    return seconds + (targetBeat - consumedBeats) * 60f / currentBpm;
                consumedBeats += segmentBeats;
                seconds = markerTime;
                currentBpm = Mathf.Max(1f, tempoMarkerBpms[i]);
            }
            return seconds + (targetBeat - consumedBeats) * 60f / currentBpm;
        }

        public int RuntimeBeatsPerBarAtTime(float seconds)
        {
            int numerator = Mathf.Max(1, beatsPerBar);
            for (int i = 0; i < tempoMarkerTimes.Length; i++)
            {
                if (tempoMarkerTimes[i] > seconds) break;
                numerator = Mathf.Max(1, tempoMarkerNumerators[i]);
            }
            return numerator;
        }

        public float ResolveLoopedTime(float rawTime)
        {
            if (!IsLoopUsable(selectedLoopIndex)) return rawTime;
            float start = loopStarts[selectedLoopIndex];
            float end = loopEnds[selectedLoopIndex];
            if (rawTime < start) return rawTime;
            float length = end - start;
            int repeats = loopRepeatCounts[selectedLoopIndex];
            float elapsed = rawTime - start;
            if (repeats <= 0) return start + Mathf.Repeat(elapsed, length);
            float loopedDuration = length * (repeats + 1);
            if (elapsed < loopedDuration) return start + Mathf.Repeat(elapsed, length);
            return end + elapsed - loopedDuration;
        }

        public void ApplyAtTime(float time)
        {
            if (groups == null) return;
            EnsureShaderPropertyIds();
            ApplyRuntimePlatformBudgets();
            if (_lastAppliedTime >= 0f && time + 0.001f < _lastAppliedTime && _eventFired != null)
                for (int eventIndex = 0; eventIndex < _eventFired.Length; eventIndex++)
                    _eventFired[eventIndex] = false;
            ResetGroups();
            if (!HasConsistentCueArrays() || !HasConsistentGroupArrays()) { PublishGroups(); return; }
            _resolvedStrobeHz = 0f;
            _resolvedLaserEnabled = 0f;
            _evaluationFrame++;
            PrepareArbitrationWinners(time);
            int cueBudget = ResolveMaximumActiveCues();
            int selectedCount = SelectActiveCues(time, cueBudget);
            activeCueCount = selectedCount;
            droppedCueCount = CountActiveCues(time) - selectedCount;

            for (int selected = selectedCount - 1; selected >= 0; selected--)
            {
                int cue = _selectedCueIndices[selected];
                int groupIndex = cueGroupIndices[cue];
                float start = cueStarts[cue];
                float duration = cueDurations[cue];
                float weight = EvaluateWeight(time, start, duration, cueFadeIns[cue], cueFadeOuts[cue], cueEasings[cue]);
                weight *= EvaluateRhythmGate(cue, time, start);
                if (weight <= 0.0001f) continue;
                SpectraCueValueType type = (SpectraCueValueType)cueValueTypes[cue];
                SpectraCueBlendMode blend = (SpectraCueBlendMode)cueBlendModes[cue];
                if (blend == SpectraCueBlendMode.IntensityOnly
                    && type != SpectraCueValueType.Intensity
                    && type != SpectraCueValueType.AudioReactiveIntensity
                    && type != SpectraCueValueType.Strobe)
                    continue;
                if (blend == SpectraCueBlendMode.MovementOnly
                    && type != SpectraCueValueType.Movement)
                    continue;
                SpectraPlatformFallback fallback = ResolveFallback(cue);
                if (fallback == SpectraPlatformFallback.Disabled) continue;
                if (fallback == SpectraPlatformFallback.EmissiveOnly
                    && type != SpectraCueValueType.Intensity
                    && type != SpectraCueValueType.AudioReactiveIntensity
                    && type != SpectraCueValueType.Color
                    && type != SpectraCueValueType.Event
                    && type != SpectraCueValueType.Blackout)
                    continue;
                if (type == SpectraCueValueType.Event)
                {
                    DispatchEvent(cue, start, time);
                    continue;
                }
                Vector4 value = EvaluateAutomatedValue(cue, time, start, duration);
                if (type == SpectraCueValueType.Blackout && value.x > 0.5f)
                {
                    for (int blackoutGroup = 0; blackoutGroup < groups.Length; blackoutGroup++)
                        if (groups[blackoutGroup] != null) groups[blackoutGroup].intensityMultiplier = 0f;
                    continue;
                }
                if (groupIndex < 0)
                {
                    for (int globalGroup = 0; globalGroup < groups.Length; globalGroup++)
                        if (groups[globalGroup] != null)
                            ApplyCueToGroup(cue, globalGroup, type, blend, fallback, weight, start, duration, time, groups[globalGroup]);
                    continue;
                }
                if (groupIndex >= groups.Length || groups[groupIndex] == null) continue;
                ApplyCueToGroup(cue, groupIndex, type, blend, fallback, weight, start, duration, time, groups[groupIndex]);
            }
            if (overrideLayer != null) overrideLayer.ApplyToGroups(groups);
            ApplyLocalColorComfort();
            if (!showStrobesEnabled || !localStrobesEnabled || !PlatformAllowsStrobes()) _resolvedStrobeHz = 0f;
            if (!showLasersEnabled || !localLasersEnabled || !PlatformAllowsLasers()) _resolvedLaserEnabled = 0f;
            for (int safetyGroup = 0; safetyGroup < groups.Length; safetyGroup++)
            {
                SpectraFixtureGroup safeGroup = groups[safetyGroup];
                if (safeGroup == null) continue;
                if (_resolvedStrobeHz <= 0f) safeGroup.strobeHz = 0f;
                else safeGroup.strobeHz = Mathf.Min(safeGroup.strobeHz, _resolvedStrobeHz);
                if (_resolvedLaserEnabled <= 0f) safeGroup.laserEnabled = false;
            }
            if (emergencyBlackout)
            {
                _resolvedStrobeHz = 0f;
                _resolvedLaserEnabled = 0f;
                for (int i = 0; i < groups.Length; i++) if (groups[i] != null)
                {
                    groups[i].intensityMultiplier = 0f;
                    groups[i].strobeHz = 0f;
                    groups[i].laserEnabled = false;
                }
            }
            for (int i = 0; i < groups.Length; i++) if (groups[i] != null)
                groups[i].intensityMultiplier = Mathf.Min(groups[i].intensityMultiplier, localBrightnessLimit);
            VRCShader.SetGlobalVector(_showSafetyId,
                new Vector4(_resolvedStrobeHz, _resolvedLaserEnabled, emergencyBlackout ? 1f : 0f, localBrightnessLimit));
            VRCShader.SetGlobalFloat(_showTimeId, time);
            if (bus != null)
            {
                bus.externalShowClock = true;
                bus.externalShowTime = time;
            }
            PublishGroups();
            if (snapshotCache != null) snapshotCache.TryCapture(time, groups);
            _lastAppliedTime = time;
        }

        private bool IsShowUsable() { return durationSeconds > 0f && HasConsistentCueArrays() && HasConsistentGroupArrays(); }
        private bool HasConsistentGroupArrays()
        {
            int count = runtimeGroupIds == null ? 0 : runtimeGroupIds.Length;
            return groups != null && groups.Length == count
                && groupStableIds != null && groupStableIds.Length == count
                && groupSelections != null && groupSelections.Length == count
                && groupRandomSeeds != null && groupRandomSeeds.Length == count
                && groupCapabilityMasks != null && groupCapabilityMasks.Length == count;
        }
        private bool HasConsistentCueArrays()
        {
            if (cueStarts == null) return false;
            int count = cueStarts.Length;
            return cueGroupIndices != null && cueGroupIndices.Length == count
                && cueValueTypes != null && cueValueTypes.Length == count
                && cueBlendModes != null && cueBlendModes.Length == count
                && cueEasings != null && cueEasings.Length == count
                && cuePriorities != null && cuePriorities.Length == count
                && cueDurations != null && cueDurations.Length == count
                && cueFadeIns != null && cueFadeIns.Length == count
                && cueFadeOuts != null && cueFadeOuts.Length == count
                && cueMovementSmoothing != null && cueMovementSmoothing.Length == count
                && cueColors != null && cueColors.Length == count
                && cueValues != null && cueValues.Length == count
                && cueQuestFallbacks != null && cueQuestFallbacks.Length == count
                && cueIosFallbacks != null && cueIosFallbacks.Length == count
                && cueAndroidFallbacks != null && cueAndroidFallbacks.Length == count
                && cueMovementPatterns != null && cueMovementPatterns.Length == count
                && cueMovementParameters != null && cueMovementParameters.Length == count
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
                && cueLayerIndices != null && cueLayerIndices.Length == count
                && cueArbitrationModes != null && cueArbitrationModes.Length == count
                && cueArbitrationGroups != null && cueArbitrationGroups.Length == count
                && cueArbitrationTimeBases != null && cueArbitrationTimeBases.Length == count
                && cueArbitrationSeeds != null && cueArbitrationSeeds.Length == count
                && cueConditionInverts != null && cueConditionInverts.Length == count
                && cueGateInverts != null && cueGateInverts.Length == count
                && cueEventOnce != null && cueEventOnce.Length == count
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
                && cueArbitrationCycleLengths != null && cueArbitrationCycleLengths.Length == count
                && cueArbitrationPhases != null && cueArbitrationPhases.Length == count
                && cueModulationOffsets != null && cueModulationOffsets.Length == count
                && cueModulationDepths != null && cueModulationDepths.Length == count
                && cuePerformanceMacroMinimums != null && cuePerformanceMacroMinimums.Length == count
                && cuePerformanceMacroMaximums != null && cuePerformanceMacroMaximums.Length == count
                && HasConsistentPaletteArrays()
                && HasConsistentPerformanceMacroArrays()
                && HasConsistentPerformanceMacroSnapshotArrays()
                && HasConsistentCueLayerArrays()
                && HasConsistentAutomationArrays()
                && HasConsistentMarkerArrays()
                && HasConsistentTempoArrays();
        }

        private bool HasConsistentPaletteArrays()
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

        private bool HasConsistentCueLayerArrays()
        {
            int count = cueLayerNames == null ? 0 : cueLayerNames.Length;
            return count <= 16
                && cueLayerColors != null && cueLayerColors.Length == count
                && cueLayerDefaultEnabled != null && cueLayerDefaultEnabled.Length == count
                && cueLayerPcEnabled != null && cueLayerPcEnabled.Length == count
                && cueLayerQuestEnabled != null && cueLayerQuestEnabled.Length == count
                && cueLayerIosEnabled != null && cueLayerIosEnabled.Length == count
                && cueLayerAndroidEnabled != null && cueLayerAndroidEnabled.Length == count
                && cueLayerPriorityBiases != null && cueLayerPriorityBiases.Length == count
                && cueLayerMaximumActiveCues != null
                && cueLayerMaximumActiveCues.Length == count;
        }

        private bool HasConsistentPerformanceMacroArrays()
        {
            int count = performanceMacroNames == null ? 0 : performanceMacroNames.Length;
            return count <= 4
                && performanceMacroDefaults != null && performanceMacroDefaults.Length == count
                && performanceMacroSmoothing != null && performanceMacroSmoothing.Length == count
                && performanceMacroColors != null && performanceMacroColors.Length == count;
        }

        private bool HasConsistentPerformanceMacroSnapshotArrays()
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

        private bool HasConsistentAutomationArrays()
        {
            int keyCount = automationTimes == null ? 0 : automationTimes.Length;
            if (automationValues == null || automationValues.Length != keyCount
                || automationInterpolations == null || automationInterpolations.Length != keyCount)
                return false;
            for (int i = 0; i < cueAutomationOffsets.Length; i++)
            {
                int offset = cueAutomationOffsets[i];
                int count = cueAutomationCounts[i];
                if (offset < 0 || count < 0 || offset + count > keyCount) return false;
            }
            return true;
        }

        private bool HasConsistentMarkerArrays()
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

        private bool HasConsistentTempoArrays()
        {
            int count = tempoMarkerTimes == null ? 0 : tempoMarkerTimes.Length;
            return tempoMarkerBpms != null && tempoMarkerBpms.Length == count
                && tempoMarkerNumerators != null && tempoMarkerNumerators.Length == count;
        }
        private void ResetGroups() { for (int i = 0; i < groups.Length; i++) if (groups[i] != null) { groups[i].colorMultiplier = Color.white; groups[i].intensityMultiplier = 1f; groups[i].panBias = 0f; groups[i].tiltBias = 0f; groups[i].movementScale = 1f; groups[i].movementPattern = SpectraMovementPatternKind.Static; groups[i].movementPatternWeight = 0f; groups[i].goboIndex = -1f; groups[i].goboRotation = 0f; groups[i].prismAmount = 0f; groups[i].zoom = -1f; groups[i].focus = -1f; groups[i].strobeHz = 0f; groups[i].laserEnabled = false; groups[i].audioReactiveBand = -1; groups[i].audioReactiveAmount = 0f; groups[i].audioReactiveFloor = 1f; groups[i].audioReactiveWeight = 0f; if (groupSelections != null && i < groupSelections.Length) groups[i].selection = (SpectraFixtureSelection)groupSelections[i]; if (groupRandomSeeds != null && i < groupRandomSeeds.Length) groups[i].selectionSeed = groupRandomSeeds[i]; } }
        private void ApplyCueToGroup(
            int cue,
            int groupIndex,
            SpectraCueValueType type,
            SpectraCueBlendMode blend,
            SpectraPlatformFallback fallback,
            float weight,
            float start,
            float duration,
            float time,
            SpectraFixtureGroup group)
        {
            Vector4 value = EvaluateAutomatedValue(cue, time, start, duration);
            if (!GroupSupportsCue(groupIndex, cue))
            {
                SpectraCapabilityFallback capabilityFallback =
                    (SpectraCapabilityFallback)cueCapabilityFallbacks[cue];
                if (capabilityFallback == SpectraCapabilityFallback.DisableCue) return;
                if (capabilityFallback == SpectraCapabilityFallback.EmissiveApproximation)
                {
                    ApplyEmissiveCapabilityFallback(group, type, value, weight, blend);
                    return;
                }
            }
            if (type == SpectraCueValueType.Intensity)
                group.intensityMultiplier = BlendFloat(group.intensityMultiplier, value.x, weight, blend);
            else if (type == SpectraCueValueType.AudioReactiveIntensity)
            {
                group.intensityMultiplier = BlendFloat(group.intensityMultiplier, value.x, weight, blend);
                if (weight >= group.audioReactiveWeight)
                {
                    group.audioReactiveBand = cueAudioBands[cue];
                    group.audioReactiveAmount = cueAudioAmounts[cue];
                    group.audioReactiveFloor = cueAudioFloors[cue];
                    group.audioReactiveWeight = weight;
                }
            }
            else if (type == SpectraCueValueType.Color)
                group.colorMultiplier = BlendColor(group.colorMultiplier,
                    new Color(value.x, value.y, value.z, value.w), weight, blend);
            else if (type == SpectraCueValueType.Movement)
            {
                group.panBias = BlendFloat(group.panBias, value.x, weight, blend);
                group.tiltBias = BlendFloat(group.tiltBias, value.y, weight, blend);
                float speed = (fallback == SpectraPlatformFallback.Simplified
                    ? Mathf.Min(2f, value.z) : value.z) * localMovementLimit;
                group.movementScale = BlendFloat(group.movementScale, speed, weight, blend);
                if (weight >= group.movementPatternWeight)
                {
                    int pattern = cueMovementPatterns[cue];
                    if (fallback == SpectraPlatformFallback.Simplified
                        && pattern != (int)SpectraMovementPatternKind.Static
                        && pattern != (int)SpectraMovementPatternKind.HorizontalSweep
                        && pattern != (int)SpectraMovementPatternKind.VerticalSweep
                        && pattern != (int)SpectraMovementPatternKind.Fan)
                        pattern = (int)SpectraMovementPatternKind.HorizontalSweep;
                    Vector4 movement = cueMovementParameters[cue];
                    group.movementPattern = (SpectraMovementPatternKind)pattern;
                    group.movementPatternTime = time - start;
                    group.movementPatternSpeed = speed;
                    group.movementPatternAmplitude = movement.x * localMovementLimit;
                    group.movementPatternSpread = movement.y;
                    group.movementPatternPhase = movement.z;
                    group.movementPatternDirection = movement.w;
                    group.movementPatternSmoothing = cueMovementSmoothing[cue];
                    group.movementPatternSeed = cueRandomSeeds[cue];
                    group.movementPatternWeight = weight;
                }
            }
            else if (type == SpectraCueValueType.Strobe)
            {
                float resolvedRate = Mathf.Min(
                    localStrobeFrequencyLimit,
                    fallback == SpectraPlatformFallback.Simplified
                        ? Mathf.Min(8f, value.x) : value.x) * weight;
                _resolvedStrobeHz = Mathf.Max(_resolvedStrobeHz, resolvedRate);
                group.strobeHz = Mathf.Max(group.strobeHz, resolvedRate);
            }
            else if (type == SpectraCueValueType.LaserEnable && value.x > 0.5f && weight > 0f)
            {
                _resolvedLaserEnabled = 1f;
                group.laserEnabled = true;
            }
            else if (type == SpectraCueValueType.Gobo && weight > 0f)
            {
                group.goboIndex = value.x;
                group.goboRotation = value.y;
            }
            else if (type == SpectraCueValueType.Prism && weight > 0f)
                group.prismAmount = BlendFloat(group.prismAmount, value.x, weight, blend);
            else if (type == SpectraCueValueType.ZoomFocus && weight > 0f)
            {
                group.zoom = BlendFloat(group.zoom < 0f ? 0.5f : group.zoom, value.x, weight, blend);
                group.focus = BlendFloat(group.focus < 0f ? 0.5f : group.focus, value.y, weight, blend);
            }
        }

        private Vector4 EvaluateAutomatedValue(int cue, float time, float start, float duration)
        {
            Vector4 baseValue = cueValues[cue];
            if ((SpectraCueValueType)cueValueTypes[cue] == SpectraCueValueType.Color)
            {
                Color baseColor = EvaluatePaletteColor(cue, time, start, cueColors[cue]);
                baseValue = new Vector4(baseColor.r, baseColor.g, baseColor.b, baseColor.a);
            }
            SpectraAutomationMode mode = (SpectraAutomationMode)cueAutomationModes[cue];
            int offset = cueAutomationOffsets[cue];
            int count = cueAutomationCounts[cue];
            Vector4 resolved = baseValue;
            if (mode != SpectraAutomationMode.Disabled && count > 0)
            {
                float normalized = duration <= 0.000001f
                    ? 1f : Mathf.Clamp01((time - start) / duration);
                Vector4 automated = automationValues[offset];
                if (normalized >= automationTimes[offset + count - 1])
                    automated = automationValues[offset + count - 1];
                else if (normalized > automationTimes[offset])
                {
                    for (int key = 0; key < count - 1; key++)
                    {
                        int leftIndex = offset + key;
                        int rightIndex = leftIndex + 1;
                        float leftTime = automationTimes[leftIndex];
                        float rightTime = automationTimes[rightIndex];
                        if (normalized > rightTime) continue;
                        SpectraAutomationInterpolation interpolation =
                            (SpectraAutomationInterpolation)automationInterpolations[leftIndex];
                        if (interpolation == SpectraAutomationInterpolation.Step)
                            automated = automationValues[leftIndex];
                        else
                        {
                            float range = Mathf.Max(0.000001f, rightTime - leftTime);
                            float t = Mathf.Clamp01((normalized - leftTime) / range);
                            if (interpolation == SpectraAutomationInterpolation.Smooth)
                                t = t * t * (3f - 2f * t);
                            automated = Vector4.Lerp(
                                automationValues[leftIndex],
                                automationValues[rightIndex],
                                t);
                        }
                        break;
                    }
                }
                resolved = ApplyValueMode(resolved, automated, mode);
            }
            resolved = ApplyProceduralModulation(cue, resolved, time, start);
            return ApplyPerformanceMacro(cue, resolved);
        }

        private bool EvaluateCueCondition(int cue, float time, float start)
        {
            SpectraCueConditionMode mode =
                (SpectraCueConditionMode)cueConditionModes[cue];
            if (mode == SpectraCueConditionMode.Disabled) return true;
            bool active = true;
            if (mode == SpectraCueConditionMode.Probability
                || mode == SpectraCueConditionMode.EveryNthCycle)
            {
                float clock = ResolveRelativeClock(time, start,
                    (SpectraModulationTimeBase)cueConditionTimeBases[cue]);
                int cycle = Mathf.FloorToInt(clock / Mathf.Max(0.0001f,
                    cueConditionCycleLengths[cue]) + cueConditionPhases[cue]);
                if (mode == SpectraCueConditionMode.Probability)
                {
                    int seed = cueRandomSeeds[cue]
                        ^ (cueConditionCycleOffsets[cue] * 668265263);
                    active = DeterministicUnitValue(seed, cycle)
                        < Mathf.Clamp01(cueConditionProbabilities[cue]);
                }
                else
                {
                    int every = Mathf.Clamp(cueConditionEveryNs[cue], 1, 32);
                    active = PositiveModulo(cycle + cueConditionCycleOffsets[cue], every) == 0;
                }
            }
            else if (mode == SpectraCueConditionMode.MacroAbove)
                active = PerformanceMacroValue(cueConditionMacroIndices[cue])
                    >= cueConditionThresholds[cue];
            else if (mode == SpectraCueConditionMode.MacroBelow)
                active = PerformanceMacroValue(cueConditionMacroIndices[cue])
                    < cueConditionThresholds[cue];
            else if (mode == SpectraCueConditionMode.AudioAbove)
                active = ResolveConditionAudioValue(cueConditionAudioBands[cue])
                    >= cueConditionThresholds[cue];
            else if (mode == SpectraCueConditionMode.AudioBelow)
                active = ResolveConditionAudioValue(cueConditionAudioBands[cue])
                    < cueConditionThresholds[cue];
            return cueConditionInverts[cue] ? !active : active;
        }

        private bool EvaluateVariationSelection(int cue, float time)
        {
            SpectraVariationSelectionMode mode =
                (SpectraVariationSelectionMode)cueVariationModes[cue];
            if (mode == SpectraVariationSelectionMode.Disabled) return true;
            int count = Mathf.Clamp(cueVariationOptionCounts[cue], 2, 8);
            int selected = 0;
            if (mode == SpectraVariationSelectionMode.MacroSelect)
            {
                float macro = PerformanceMacroValue(cueVariationMacroIndices[cue]);
                selected = Mathf.Min(count - 1, Mathf.FloorToInt(macro * count));
            }
            else
            {
                float clock = ResolveRelativeClock(time, 0f,
                    (SpectraModulationTimeBase)cueVariationTimeBases[cue]);
                int cycle = Mathf.FloorToInt(clock / Mathf.Max(0.0001f,
                    cueVariationCycleLengths[cue]) + cueVariationPhases[cue]);
                if (mode == SpectraVariationSelectionMode.SeededRandom)
                {
                    int seed = cueVariationSeeds[cue]
                        ^ (cueVariationGroups[cue] * 374761393);
                    selected = Mathf.Min(count - 1, Mathf.FloorToInt(
                        DeterministicUnitValue(seed, cycle) * count));
                }
                else if (mode == SpectraVariationSelectionMode.PingPong && count > 1)
                {
                    int period = count * 2 - 2;
                    int ping = PositiveModulo(cycle, period);
                    selected = ping < count ? ping : period - ping;
                }
                else selected = PositiveModulo(cycle, count);
            }
            return Mathf.Clamp(cueVariationOptions[cue], 0, count - 1) == selected;
        }

        private float ResolveConditionAudioValue(int bandValue)
        {
            if (audioLinkAdapter == null) return 0f;
            SpectraAudioBand band = (SpectraAudioBand)bandValue;
            if (band == SpectraAudioBand.Bass) return Mathf.Clamp01(audioLinkAdapter.manualBass);
            if (band == SpectraAudioBand.LowMid) return Mathf.Clamp01(audioLinkAdapter.manualLowMid);
            if (band == SpectraAudioBand.HighMid) return Mathf.Clamp01(audioLinkAdapter.manualHighMid);
            if (band == SpectraAudioBand.Treble) return Mathf.Clamp01(audioLinkAdapter.manualTreble);
            return Mathf.Clamp01(audioLinkAdapter.manualOverall);
        }

        private float EvaluateRhythmGate(int cue, float time, float start)
        {
            SpectraCueGatePattern pattern = (SpectraCueGatePattern)cueGatePatterns[cue];
            if (pattern == SpectraCueGatePattern.Disabled) return 1f;
            float clock = ResolveRelativeClock(time, start,
                (SpectraModulationTimeBase)cueGateTimeBases[cue]);
            float step = clock / Mathf.Max(0.0001f, cueGateStepLengths[cue])
                + cueGatePhases[cue];
            int absoluteStep = Mathf.FloorToInt(step);
            float position = Mathf.Repeat(step, 1f);
            int stepCount = Mathf.Clamp(cueGateStepCounts[cue], 1, 32);
            int stepIndex = PositiveModulo(absoluteStep, stepCount);
            int activeSteps = Mathf.Clamp(cueGateActiveSteps[cue], 1, stepCount);
            bool active = true;
            if (pattern == SpectraCueGatePattern.Alternating)
                active = (stepIndex & 1) == 0;
            else if (pattern == SpectraCueGatePattern.Euclidean)
                active = PositiveModulo(stepIndex * activeSteps, stepCount) < activeSteps;
            else if (pattern == SpectraCueGatePattern.SeededRandom)
                active = DeterministicUnitValue(cueRandomSeeds[cue], absoluteStep)
                    < activeSteps / (float)stepCount;
            else if (pattern == SpectraCueGatePattern.CustomMask)
                active = (cueGateCustomMasks[cue] & (1 << stepIndex)) != 0;
            if (cueGateInverts[cue]) active = !active;
            if (!active) return 0f;

            float duty = Mathf.Clamp(cueGateDutyCycles[cue], 0.01f, 0.99f);
            if (position >= duty) return 0f;
            float attack = Mathf.Min(Mathf.Clamp(cueGateAttacks[cue], 0f, 0.49f), duty * 0.5f);
            float release = Mathf.Min(Mathf.Clamp(cueGateReleases[cue], 0f, 0.49f), duty * 0.5f);
            float weight = 1f;
            if (attack > 0.0001f && position < attack)
                weight = Smooth01(position / attack);
            if (release > 0.0001f && position > duty - release)
                weight = Mathf.Min(weight, Smooth01((duty - position) / release));
            return Mathf.Clamp01(weight);
        }

        private Color EvaluatePaletteColor(int cue, float time, float start, Color fallback)
        {
            SpectraPalettePlaybackMode mode =
                (SpectraPalettePlaybackMode)cuePaletteModes[cue];
            int palette = cuePaletteIndices[cue];
            if (mode == SpectraPalettePlaybackMode.Disabled
                || palette < 0 || palette >= paletteCounts.Length) return fallback;
            int count = paletteCounts[palette];
            int offset = paletteOffsets[palette];
            if (count < 1 || offset < 0 || offset + count > paletteColors.Length) return fallback;

            int primary = Mathf.Clamp(cuePalettePrimaryIndices[cue], 0, count - 1);
            int secondary = Mathf.Clamp(cuePaletteSecondaryIndices[cue], 0, count - 1);
            Color resolved = paletteColors[offset + primary];
            if (mode == SpectraPalettePlaybackMode.Step
                || mode == SpectraPalettePlaybackMode.PingPong
                || mode == SpectraPalettePlaybackMode.SeededRandom)
            {
                float clock = ResolveRelativeClock(time, start,
                    (SpectraModulationTimeBase)cuePaletteTimeBases[cue]);
                int step = Mathf.FloorToInt(clock / Mathf.Max(0.0001f,
                    cuePaletteStepLengths[cue]) + cuePalettePhases[cue]);
                int colorIndex;
                if (mode == SpectraPalettePlaybackMode.SeededRandom)
                    colorIndex = Mathf.Min(count - 1, Mathf.FloorToInt(
                        DeterministicUnitValue(cueRandomSeeds[cue], step) * count));
                else if (mode == SpectraPalettePlaybackMode.PingPong && count > 1)
                {
                    int period = count * 2 - 2;
                    int ping = PositiveModulo(step, period);
                    colorIndex = ping < count ? ping : period - ping;
                }
                else colorIndex = PositiveModulo(step, count);
                resolved = paletteColors[offset + colorIndex];
            }
            else if (mode == SpectraPalettePlaybackMode.MacroMorph)
            {
                float macro = PerformanceMacroValue(cuePaletteMacroIndices[cue]);
                resolved = Color.Lerp(paletteColors[offset + primary],
                    paletteColors[offset + secondary], macro);
            }
            return Color.Lerp(fallback, resolved, Mathf.Clamp01(cuePaletteBlends[cue]));
        }

        private float ResolveRelativeClock(
            float time,
            float start,
            SpectraModulationTimeBase timeBase)
        {
            if (timeBase == SpectraModulationTimeBase.Seconds)
                return Mathf.Max(0f, time - start);
            float beats = Mathf.Max(0f,
                RuntimeSecondsToBeat(time) - RuntimeSecondsToBeat(start));
            return timeBase == SpectraModulationTimeBase.Bars
                ? beats / Mathf.Max(1, RuntimeBeatsPerBarAtTime(time))
                : beats;
        }

        private int PositiveModulo(int value, int modulus)
        {
            if (modulus <= 0) return 0;
            int result = value % modulus;
            return result < 0 ? result + modulus : result;
        }

        private float Smooth01(float value)
        {
            float t = Mathf.Clamp01(value);
            return t * t * (3f - 2f * t);
        }

        private Vector4 ApplyProceduralModulation(
            int cue,
            Vector4 current,
            float time,
            float start)
        {
            SpectraModulationWaveform waveform =
                (SpectraModulationWaveform)cueModulationWaveforms[cue];
            SpectraAutomationMode mode =
                (SpectraAutomationMode)cueModulationModes[cue];
            if (waveform == SpectraModulationWaveform.Disabled
                || mode == SpectraAutomationMode.Disabled)
                return current;

            float localSeconds = Mathf.Max(0f, time - start);
            float clock = localSeconds;
            SpectraModulationTimeBase timeBase =
                (SpectraModulationTimeBase)cueModulationTimeBases[cue];
            if (timeBase != SpectraModulationTimeBase.Seconds)
            {
                float localBeats = Mathf.Max(0f,
                    RuntimeSecondsToBeat(time) - RuntimeSecondsToBeat(start));
                clock = timeBase == SpectraModulationTimeBase.Bars
                    ? localBeats / Mathf.Max(1, RuntimeBeatsPerBarAtTime(time))
                    : localBeats;
            }
            float cycle = clock / Mathf.Max(0.0001f, cueModulationCycleLengths[cue])
                + cueModulationPhases[cue];
            float position = Mathf.Repeat(cycle, 1f);
            float signal = EvaluateModulationWaveform(
                waveform,
                position,
                Mathf.FloorToInt(cycle),
                cueRandomSeeds[cue],
                cueModulationDutyCycles[cue]);
            int steps = cueModulationQuantizeSteps[cue];
            if (steps > 1)
                signal = Mathf.Round(signal * (steps - 1)) / (steps - 1);
            Vector4 modifier = cueModulationOffsets[cue]
                + cueModulationDepths[cue] * signal;
            return ApplyValueMode(current, modifier, mode);
        }

        private float EvaluateModulationWaveform(
            SpectraModulationWaveform waveform,
            float position,
            int cycle,
            int seed,
            float duty)
        {
            if (waveform == SpectraModulationWaveform.Sine)
                return 0.5f + 0.5f * Mathf.Sin(position * Mathf.PI * 2f);
            if (waveform == SpectraModulationWaveform.Triangle)
                return 1f - Mathf.Abs(position * 2f - 1f);
            if (waveform == SpectraModulationWaveform.SawUp) return position;
            if (waveform == SpectraModulationWaveform.SawDown) return 1f - position;
            if (waveform == SpectraModulationWaveform.Square)
                return position < 0.5f ? 1f : 0f;
            if (waveform == SpectraModulationWaveform.Pulse)
                return position < Mathf.Clamp(duty, 0.01f, 0.99f) ? 1f : 0f;
            if (waveform == SpectraModulationWaveform.SampleAndHold)
                return DeterministicUnitValue(seed, cycle);
            return 1f;
        }

        private float DeterministicUnitValue(int seed, int cycle)
        {
            int value = seed ^ (cycle * 374761393);
            value = (value ^ (value >> 13)) * 1274126177;
            value ^= value >> 16;
            return (value & 0x7fffffff) / 2147483647f;
        }

        private Vector4 ApplyPerformanceMacro(int cue, Vector4 current)
        {
            int index = cuePerformanceMacroIndices[cue];
            SpectraAutomationMode mode =
                (SpectraAutomationMode)cuePerformanceMacroModes[cue];
            if (index < 0 || index > 3 || mode == SpectraAutomationMode.Disabled)
                return current;
            float value = PerformanceMacroValue(index);
            Vector4 modifier = Vector4.Lerp(
                cuePerformanceMacroMinimums[cue],
                cuePerformanceMacroMaximums[cue],
                value);
            return ApplyValueMode(current, modifier, mode);
        }

        private float PerformanceMacroValue(int index)
        {
            if (index == 0) return performanceMacro0;
            if (index == 1) return performanceMacro1;
            if (index == 2) return performanceMacro2;
            if (index == 3) return performanceMacro3;
            return 1f;
        }

        private Vector4 ApplyValueMode(
            Vector4 current,
            Vector4 modifier,
            SpectraAutomationMode mode)
        {
            if (mode == SpectraAutomationMode.Replace) return modifier;
            if (mode == SpectraAutomationMode.Add) return current + modifier;
            if (mode == SpectraAutomationMode.Multiply)
                return new Vector4(
                    current.x * modifier.x,
                    current.y * modifier.y,
                    current.z * modifier.z,
                    current.w * modifier.w);
            return current;
        }

        private bool GroupSupportsCue(int groupIndex, int cue)
        {
            if (groupIndex < 0 || groupCapabilityMasks == null
                || groupIndex >= groupCapabilityMasks.Length)
                return true;
            int required = cueRequiredCapabilities[cue];
            return (groupCapabilityMasks[groupIndex] & required) == required;
        }

        private void ApplyEmissiveCapabilityFallback(
            SpectraFixtureGroup group,
            SpectraCueValueType type,
            Vector4 value,
            float weight,
            SpectraCueBlendMode blend)
        {
            float approximation = 1f;
            if (type == SpectraCueValueType.Strobe)
                approximation = value.x > 0f ? 1.15f : 1f;
            else if (type == SpectraCueValueType.LaserEnable)
                approximation = value.x > 0.5f ? 1.1f : 1f;
            else if (type == SpectraCueValueType.Prism)
                approximation = 1f + Mathf.Clamp01(value.x) * 0.12f;
            else if (type == SpectraCueValueType.Gobo)
                approximation = 0.85f + Mathf.Repeat(value.x, 4f) * 0.05f;
            else if (type == SpectraCueValueType.ZoomFocus)
                approximation = 0.8f + Mathf.Clamp01(value.x) * 0.3f;
            else if (type == SpectraCueValueType.Movement)
                approximation = 0.9f + Mathf.Clamp01(Mathf.Abs(value.x) + Mathf.Abs(value.y)) * 0.1f;
            group.intensityMultiplier = BlendFloat(
                group.intensityMultiplier,
                approximation,
                weight,
                blend);
        }

        private void PublishGroups()
        {
            int remaining = ResolveMaximumFixtures();
            activeFixtureBudget = remaining;
            for (int i = 0; i < groups.Length; i++)
                if (groups[i] != null)
                {
                    int fixtureCount = groups[i].fixtures == null ? 0 : groups[i].fixtures.Length;
                    int allowed = Mathf.Min(Mathf.Max(0, remaining), fixtureCount);
                    groups[i].fixtureBudget = allowed;
                    groups[i].ApplyToFixtures();
                    remaining -= allowed;
                }
        }
        private float EvaluateWeight(float time, float start, float duration, float fadeIn, float fadeOut, int easing)
        {
            float w = 1f;
            if (fadeIn > 0f) w = Mathf.Min(w, (time - start) / fadeIn);
            if (fadeOut > 0f) w = Mathf.Min(w, (start + duration - time) / fadeOut);
            w = Mathf.Clamp01(w);
            SpectraCueEasing curve = (SpectraCueEasing)easing;
            if (curve == SpectraCueEasing.SmoothStep || curve == SpectraCueEasing.EaseInOut) return w * w * (3f - 2f * w);
            if (curve == SpectraCueEasing.EaseIn) return w * w;
            if (curve == SpectraCueEasing.EaseOut) return 1f - (1f - w) * (1f - w);
            return w;
        }
        private float BlendFloat(float current, float value, float weight, SpectraCueBlendMode mode)
        {
            weight = Mathf.Clamp01(weight);
            if (mode == SpectraCueBlendMode.Add) return current + value * weight;
            if (mode == SpectraCueBlendMode.Multiply) return current * Mathf.Lerp(1f, value, weight);
            if (mode == SpectraCueBlendMode.Mask) return current * Mathf.Lerp(1f, value, weight);
            if (mode == SpectraCueBlendMode.Maximum) return Mathf.Max(current, value * weight);
            if (mode == SpectraCueBlendMode.Minimum) return Mathf.Min(current, Mathf.Lerp(current, value, weight));
            return Mathf.Lerp(current, value, weight);
        }
        private Color BlendColor(Color current, Color value, float weight, SpectraCueBlendMode mode)
        {
            weight = Mathf.Clamp01(weight);
            if (mode == SpectraCueBlendMode.Add) return current + value * weight;
            if (mode == SpectraCueBlendMode.Multiply) return current * Color.Lerp(Color.white, value, weight);
            if (mode == SpectraCueBlendMode.Mask) return current * Color.Lerp(Color.white, value, weight);
            if (mode == SpectraCueBlendMode.Maximum) return new Color(Mathf.Max(current.r, value.r * weight), Mathf.Max(current.g, value.g * weight), Mathf.Max(current.b, value.b * weight), 1f);
            if (mode == SpectraCueBlendMode.Minimum)
            {
                Color weighted = Color.Lerp(current, value, weight);
                return new Color(Mathf.Min(current.r, weighted.r), Mathf.Min(current.g, weighted.g), Mathf.Min(current.b, weighted.b), 1f);
            }
            return Color.Lerp(current, value, weight);
        }

        private void ResolveLocalPlatform()
        {
            if (platformManager != null)
            {
                platformManager.DetectAndApply();
                localPlatform = platformManager.detectedPlatform;
                _platformBaseBeamBudget = platformManager.maxActiveBeams;
                return;
            }
#if UNITY_ANDROID
            localPlatform = Networking.LocalPlayer != null && Networking.LocalPlayer.IsUserInVR()
                ? SpectraPlatformKind.Quest : SpectraPlatformKind.Android;
#elif UNITY_IOS
            localPlatform = SpectraPlatformKind.IOS;
#else
            localPlatform = SpectraPlatformKind.PC;
#endif
        }

        private int ResolveMaximumActiveCues()
        {
            int maximum = pcMaximumActiveCues;
            if (localPlatform == SpectraPlatformKind.Quest) maximum = questMaximumActiveCues;
            else if (localPlatform == SpectraPlatformKind.IOS) maximum = iosMaximumActiveCues;
            else if (localPlatform == SpectraPlatformKind.Android) maximum = androidMaximumActiveCues;
            return Mathf.Max(1, Mathf.RoundToInt(maximum * ResolveQualityFactor()));
        }

        private int ResolveUpdateRate()
        {
            int rate = pcUpdateRate;
            if (localPlatform == SpectraPlatformKind.Quest) rate = questUpdateRate;
            else if (localPlatform == SpectraPlatformKind.IOS) rate = iosUpdateRate;
            else if (localPlatform == SpectraPlatformKind.Android) rate = androidUpdateRate;
            return Mathf.Max(1, Mathf.RoundToInt(rate * ResolveQualityFactor()));
        }

        private int ResolveMaximumFixtures()
        {
            int maximum = pcMaximumFixtures;
            if (localPlatform == SpectraPlatformKind.Quest) maximum = questMaximumFixtures;
            else if (localPlatform == SpectraPlatformKind.IOS) maximum = iosMaximumFixtures;
            else if (localPlatform == SpectraPlatformKind.Android) maximum = androidMaximumFixtures;
            return Mathf.Max(1, Mathf.RoundToInt(maximum * ResolveQualityFactor()));
        }

        private int ResolveMaximumTransparentBeams()
        {
            int maximum = pcMaximumTransparentBeams;
            if (localPlatform == SpectraPlatformKind.Quest) maximum = questMaximumTransparentBeams;
            else if (localPlatform == SpectraPlatformKind.IOS) maximum = iosMaximumTransparentBeams;
            else if (localPlatform == SpectraPlatformKind.Android) maximum = androidMaximumTransparentBeams;
            return Mathf.Max(0, Mathf.RoundToInt(maximum * ResolveQualityFactor()));
        }

        private int ResolveAudioReactiveUpdateDivider()
        {
            int divider = pcAudioReactiveUpdateDivider;
            if (localPlatform == SpectraPlatformKind.Quest) divider = questAudioReactiveUpdateDivider;
            else if (localPlatform == SpectraPlatformKind.IOS) divider = iosAudioReactiveUpdateDivider;
            else if (localPlatform == SpectraPlatformKind.Android) divider = androidAudioReactiveUpdateDivider;
            return Mathf.Max(1, divider);
        }

        private int ResolveShaderQualityTier()
        {
            int tier = pcShaderQualityTier;
            if (localPlatform == SpectraPlatformKind.Quest) tier = questShaderQualityTier;
            else if (localPlatform == SpectraPlatformKind.IOS) tier = iosShaderQualityTier;
            else if (localPlatform == SpectraPlatformKind.Android) tier = androidShaderQualityTier;
            return Mathf.Clamp(Mathf.Min(tier, qualityController == null ? 3 : qualityController.qualityLevel), 0, 3);
        }

        private void ApplyRuntimePlatformBudgets()
        {
            activeTransparentBeamBudget = ResolveMaximumTransparentBeams();
            activeShaderQualityTier = ResolveShaderQualityTier();
            activeAudioReactiveUpdateDivider = ResolveAudioReactiveUpdateDivider();
            if (platformManager != null)
            {
                if (localPlatform == SpectraPlatformKind.Quest)
                    _platformBaseBeamBudget = platformManager.questMaxActiveBeams;
                else if (localPlatform == SpectraPlatformKind.IOS)
                    _platformBaseBeamBudget = platformManager.iosMaxActiveBeams;
                else if (localPlatform == SpectraPlatformKind.Android)
                    _platformBaseBeamBudget = platformManager.androidMaxActiveBeams;
                else
                    _platformBaseBeamBudget = platformManager.pcMaxActiveBeams;
                platformManager.maxActiveBeams = Mathf.Min(
                    Mathf.Max(0, _platformBaseBeamBudget),
                    activeTransparentBeamBudget);
            }
            VRCShader.SetGlobalFloat(
                _shaderQualityTierId,
                activeShaderQualityTier);
            VRCShader.SetGlobalFloat(
                _audioReactiveUpdateDividerId,
                activeAudioReactiveUpdateDivider);
        }

        private void EnsureShaderPropertyIds()
        {
            if (_showSafetyId != 0) return;
            _showSafetyId = Shader.PropertyToID("_SpectraShowSafety");
            _showTimeId = Shader.PropertyToID("_SpectraShowTime");
            _shaderQualityTierId = Shader.PropertyToID("_SpectraShaderQualityTier");
            _audioReactiveUpdateDividerId = Shader.PropertyToID("_SpectraAudioReactiveUpdateDivider");
        }

        private float ResolveQualityFactor()
        {
            if (qualityController == null) return 1f;
            if (qualityController.qualityLevel <= 0) return 0.4f;
            if (qualityController.qualityLevel == 1) return 0.65f;
            if (qualityController.qualityLevel == 2) return 0.82f;
            return 1f;
        }

        private SpectraPlatformFallback ResolveFallback(int cue)
        {
            if (localPlatform == SpectraPlatformKind.Quest) return (SpectraPlatformFallback)cueQuestFallbacks[cue];
            if (localPlatform == SpectraPlatformKind.IOS) return (SpectraPlatformFallback)cueIosFallbacks[cue];
            if (localPlatform == SpectraPlatformKind.Android) return (SpectraPlatformFallback)cueAndroidFallbacks[cue];
            return SpectraPlatformFallback.Full;
        }

        private bool PlatformAllowsStrobes()
        {
            if (localPlatform == SpectraPlatformKind.Quest) return questAllowStrobes;
            if (localPlatform == SpectraPlatformKind.IOS) return iosAllowStrobes;
            if (localPlatform == SpectraPlatformKind.Android) return androidAllowStrobes;
            return pcAllowStrobes;
        }

        private bool PlatformAllowsLasers()
        {
            if (localPlatform == SpectraPlatformKind.Quest) return questAllowLasers;
            if (localPlatform == SpectraPlatformKind.IOS) return iosAllowLasers;
            if (localPlatform == SpectraPlatformKind.Android) return androidAllowLasers;
            return pcAllowLasers;
        }

        private int SelectActiveCues(float time, int budget)
        {
            budget = Mathf.Max(1, budget);
            if (_selectedCueIndices == null || _selectedCueIndices.Length != budget)
                _selectedCueIndices = new int[budget];
            if (_candidateCueIndices == null || _candidateCueIndices.Length != CueCount)
                _candidateCueIndices = new int[CueCount];
            if (_selectedLayerCounts == null || _selectedLayerCounts.Length != 16)
                _selectedLayerCounts = new int[16];
            for (int layer = 0; layer < _selectedLayerCounts.Length; layer++)
                _selectedLayerCounts[layer] = 0;

            int candidateCount = 0;
            for (int cue = 0; cue < CueCount; cue++)
            {
                if (!IsCueActive(cue, time) || !IsCueSupportedOnPlatform(cue)) continue;
                int insert = candidateCount;
                int priority = ResolveCuePriority(cue);
                for (int i = 0; i < candidateCount; i++)
                {
                    int other = _candidateCueIndices[i];
                    int otherPriority = ResolveCuePriority(other);
                    if (priority > otherPriority
                        || (priority == otherPriority && cue > other))
                    {
                        insert = i;
                        break;
                    }
                }
                for (int move = candidateCount; move > insert; move--)
                    _candidateCueIndices[move] = _candidateCueIndices[move - 1];
                _candidateCueIndices[insert] = cue;
                candidateCount++;
            }

            int selected = 0;
            for (int candidate = 0; candidate < candidateCount; candidate++)
            {
                int cue = _candidateCueIndices[candidate];
                int layerIndex = cueLayerIndices[cue];
                if (layerIndex >= 0 && layerIndex < 16)
                {
                    int layerLimit = GetCueLayerAdmissionLimit(layerIndex);
                    if (layerLimit > 0 && _selectedLayerCounts[layerIndex] >= layerLimit)
                    {
                        layerSuppressedCueCount++;
                        continue;
                    }
                }
                if (selected >= budget) continue;
                _selectedCueIndices[selected++] = cue;
                if (layerIndex >= 0 && layerIndex < 16)
                    _selectedLayerCounts[layerIndex]++;
            }
            return selected;
        }

        private int CountActiveCues(float time)
        {
            int count = 0;
            for (int cue = 0; cue < CueCount; cue++)
                if (IsCueActive(cue, time) && IsCueSupportedOnPlatform(cue)) count++;
            return count;
        }

        private bool IsCueActive(int cue, float time)
        {
            if (!IsCuePreArbitrationActive(cue, time)) return false;
            SpectraCueArbitrationMode mode =
                (SpectraCueArbitrationMode)cueArbitrationModes[cue];
            if (mode == SpectraCueArbitrationMode.Disabled) return true;
            int group = cueArbitrationGroups[cue];
            return group >= 0 && group < 16
                && _arbitrationWinners != null
                && _arbitrationWinners[group] == cue;
        }

        private bool IsCuePreArbitrationActive(int cue, float time)
        {
            if (!IsCueTimeConditionActive(cue, time)) return false;
            return IsCueLayerEnabledForCue(cue);
        }

        private bool IsCueTimeConditionActive(int cue, float time)
        {
            if (time < cueStarts[cue] || time > cueStarts[cue] + cueDurations[cue])
                return false;
            float start = cueStarts[cue];
            if (!EvaluateCueCondition(cue, time, start)) return false;
            if (!EvaluateVariationSelection(cue, time)) return false;
            return EvaluateRhythmGate(cue, time, start) > 0.0001f;
        }

        private void PrepareArbitrationWinners(float time)
        {
            EnsureArbitrationScratchArrays();
            arbitrationSuppressedCueCount = 0;
            arbitrationConfigurationMismatchCount = 0;
            layerSuppressedCueCount = 0;
            for (int group = 0; group < 16; group++)
            {
                _arbitrationWinners[group] = -1;
                _arbitrationCandidateCounts[group] = 0;
                _arbitrationModes[group] = (int)SpectraCueArbitrationMode.Disabled;
                _arbitrationTimeBases[group] = (int)SpectraModulationTimeBase.Bars;
                _arbitrationSeeds[group] = 0;
                _arbitrationCycleLengths[group] = 1f;
                _arbitrationPhases[group] = 0f;
            }

            for (int cue = 0; cue < CueCount; cue++)
            {
                if (!IsCueTimeConditionActive(cue, time)) continue;
                if (!IsCueLayerEnabledForCue(cue))
                {
                    layerSuppressedCueCount++;
                    continue;
                }
                SpectraCueArbitrationMode mode =
                    (SpectraCueArbitrationMode)cueArbitrationModes[cue];
                int group = cueArbitrationGroups[cue];
                if (mode == SpectraCueArbitrationMode.Disabled
                    || group < 0 || group >= 16) continue;

                if (_arbitrationCandidateCounts[group] == 0)
                {
                    _arbitrationModes[group] = (int)mode;
                    _arbitrationTimeBases[group] = cueArbitrationTimeBases[cue];
                    _arbitrationSeeds[group] = cueArbitrationSeeds[cue];
                    _arbitrationCycleLengths[group] = cueArbitrationCycleLengths[cue];
                    _arbitrationPhases[group] = cueArbitrationPhases[cue];
                    _arbitrationWinners[group] = cue;
                }
                else if (!MatchesPreparedArbitrationConfiguration(cue, group))
                {
                    arbitrationConfigurationMismatchCount++;
                    continue;
                }
                _arbitrationCandidateCounts[group]++;
                SpectraCueArbitrationMode preparedMode =
                    (SpectraCueArbitrationMode)_arbitrationModes[group];
                if (preparedMode != SpectraCueArbitrationMode.DeterministicCycle
                    && IsBetterArbitrationCandidate(
                        cue, _arbitrationWinners[group], preparedMode))
                    _arbitrationWinners[group] = cue;
            }

            for (int group = 0; group < 16; group++)
            {
                int count = _arbitrationCandidateCounts[group];
                if (count <= 0) continue;
                SpectraCueArbitrationMode mode =
                    (SpectraCueArbitrationMode)_arbitrationModes[group];
                if (mode == SpectraCueArbitrationMode.DeterministicCycle)
                {
                    float clock = ResolveRelativeClock(time, 0f,
                        (SpectraModulationTimeBase)_arbitrationTimeBases[group]);
                    int cycle = Mathf.FloorToInt(clock / Mathf.Max(0.0001f,
                        _arbitrationCycleLengths[group]) + _arbitrationPhases[group]);
                    int seedOffset = Mathf.Min(count - 1, Mathf.FloorToInt(
                        DeterministicUnitValue(_arbitrationSeeds[group], group) * count));
                    int targetOrdinal = PositiveModulo(cycle + seedOffset, count);
                    int ordinal = 0;
                    for (int cue = 0; cue < CueCount; cue++)
                    {
                        if (cueArbitrationGroups[cue] != group
                            || (SpectraCueArbitrationMode)cueArbitrationModes[cue]
                                == SpectraCueArbitrationMode.Disabled
                            || !MatchesPreparedArbitrationConfiguration(cue, group)
                            || !IsCuePreArbitrationActive(cue, time)) continue;
                        if (ordinal == targetOrdinal)
                        {
                            _arbitrationWinners[group] = cue;
                            break;
                        }
                        ordinal++;
                    }
                }
                arbitrationSuppressedCueCount += Mathf.Max(0, count - 1);
            }
        }

        private void EnsureArbitrationScratchArrays()
        {
            if (_arbitrationWinners != null && _arbitrationWinners.Length == 16)
                return;
            _arbitrationWinners = new int[16];
            _arbitrationCandidateCounts = new int[16];
            _arbitrationModes = new int[16];
            _arbitrationTimeBases = new int[16];
            _arbitrationSeeds = new int[16];
            _arbitrationCycleLengths = new float[16];
            _arbitrationPhases = new float[16];
        }

        private bool MatchesPreparedArbitrationConfiguration(int cue, int group)
        {
            if (cue < 0 || cue >= CueCount || group < 0 || group >= 16) return false;
            return cueArbitrationModes[cue] == _arbitrationModes[group]
                && cueArbitrationTimeBases[cue] == _arbitrationTimeBases[group]
                && cueArbitrationSeeds[cue] == _arbitrationSeeds[group]
                && Mathf.Abs(cueArbitrationCycleLengths[cue]
                    - _arbitrationCycleLengths[group]) < 0.0001f
                && Mathf.Abs(cueArbitrationPhases[cue]
                    - _arbitrationPhases[group]) < 0.0001f;
        }

        private bool IsBetterArbitrationCandidate(
            int cue,
            int current,
            SpectraCueArbitrationMode mode)
        {
            if (current < 0) return true;
            if (mode == SpectraCueArbitrationMode.LatestStart)
            {
                if (cueStarts[cue] > cueStarts[current] + 0.0001f) return true;
                if (cueStarts[cue] + 0.0001f < cueStarts[current]) return false;
            }
            else if (mode == SpectraCueArbitrationMode.EarliestStart)
            {
                if (cueStarts[cue] + 0.0001f < cueStarts[current]) return true;
                if (cueStarts[cue] > cueStarts[current] + 0.0001f) return false;
            }
            int priority = ResolveCuePriority(cue);
            int currentPriority = ResolveCuePriority(current);
            return priority > currentPriority
                || (priority == currentPriority && cue > current);
        }

        private int ResolveCuePriority(int cue)
        {
            int priority = cuePriorities[cue];
            int layerIndex = cueLayerIndices[cue];
            if (layerIndex >= 0 && cueLayerPriorityBiases != null
                && layerIndex < cueLayerPriorityBiases.Length)
                priority += cueLayerPriorityBiases[layerIndex];
            return priority;
        }

        private bool IsCueLayerEnabledForCue(int cue)
        {
            int layerIndex = cueLayerIndices[cue];
            if (layerIndex < 0) return true;
            return IsCueLayerEnabled(layerIndex);
        }

        private bool IsCueLayerAllowedOnPlatform(int layerIndex)
        {
            if (!IsCueLayerUsable(layerIndex)) return false;
            if (localPlatform == SpectraPlatformKind.Quest)
                return cueLayerQuestEnabled[layerIndex];
            if (localPlatform == SpectraPlatformKind.IOS)
                return cueLayerIosEnabled[layerIndex];
            if (localPlatform == SpectraPlatformKind.Android)
                return cueLayerAndroidEnabled[layerIndex];
            return cueLayerPcEnabled[layerIndex];
        }

        private int GetCueLayerAdmissionLimit(int layerIndex)
        {
            if (!IsCueLayerUsable(layerIndex)) return 0;
            return Mathf.Clamp(cueLayerMaximumActiveCues[layerIndex], 0, 32);
        }

        private bool IsCueSupportedOnPlatform(int cue)
        {
            SpectraPlatformFallback fallback = ResolveFallback(cue);
            if (fallback == SpectraPlatformFallback.Disabled) return false;
            int groupIndex = cueGroupIndices[cue];
            if (groupIndex >= 0 && !GroupSupportsCue(groupIndex, cue)
                && (SpectraCapabilityFallback)cueCapabilityFallbacks[cue]
                    == SpectraCapabilityFallback.DisableCue)
                return false;
            if (fallback != SpectraPlatformFallback.EmissiveOnly) return true;
            SpectraCueValueType type = (SpectraCueValueType)cueValueTypes[cue];
            return type == SpectraCueValueType.Intensity
                || type == SpectraCueValueType.AudioReactiveIntensity
                || type == SpectraCueValueType.Color
                || type == SpectraCueValueType.Event
                || type == SpectraCueValueType.Blackout;
        }

        private bool IsLoopUsable(int loopIndex)
        {
            if (loopStarts == null || loopEnds == null || loopEnabled == null || loopRepeatCounts == null) return false;
            int count = loopStarts.Length;
            if (loopEnds.Length != count || loopEnabled.Length != count || loopRepeatCounts.Length != count) return false;
            return loopIndex >= 0 && loopIndex < count
                && loopEnabled[loopIndex]
                && loopEnds[loopIndex] > loopStarts[loopIndex];
        }

        private void ApplyLocalColorComfort()
        {
            float transition = localRapidColorChangesEnabled ? localColorTransitionSeconds
                : Mathf.Max(0.35f, localColorTransitionSeconds);
            if (transition <= 0f) return;
            if (_smoothedGroupColors == null || _smoothedGroupColors.Length != groups.Length)
            {
                _smoothedGroupColors = new Color[groups.Length];
                for (int i = 0; i < groups.Length; i++)
                    _smoothedGroupColors[i] = groups[i] == null ? Color.white : groups[i].colorMultiplier;
            }
            float amount = Mathf.Clamp01(Time.unscaledDeltaTime / Mathf.Max(0.001f, transition));
            for (int i = 0; i < groups.Length; i++)
            {
                SpectraFixtureGroup group = groups[i];
                if (group == null) continue;
                _smoothedGroupColors[i] = Color.Lerp(_smoothedGroupColors[i], group.colorMultiplier, amount);
                group.colorMultiplier = _smoothedGroupColors[i];
            }
        }

        private void DispatchEvent(int cue, float start, float time)
        {
            if (eventRouter == null || cueEventChannels == null) return;
            if (_eventFired == null || _eventFired.Length != CueCount)
                _eventFired = new bool[CueCount];
            if (time < start) _eventFired[cue] = false;
            if (cueEventOnce[cue] && _eventFired[cue]) return;
            eventRouter.TriggerChannel(cueEventChannels[cue]);
            _eventFired[cue] = true;
        }
    }
}
