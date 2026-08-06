using System;
using System.Collections.Generic;
using UnityEngine;

namespace SpectraOverdrive
{
    [Serializable]
    public class SpectraShowFixtureGroup
    {
        public string id;
        public string name;
        public int runtimeGroupId;
        public SpectraFixtureSelection selection;
        public int randomSeed;
        public SpectraFixtureCapability capabilities = SpectraFixtureCapability.All;
    }

    [CreateAssetMenu(fileName = "SpectraShow", menuName = "SpectraOverdrive/Show Asset")]
    public class SpectraShowAsset : ScriptableObject
    {
        public const int CurrentSchemaVersion = 7;
        public int schemaVersion = CurrentSchemaVersion;
        public string showId;
        public string showName = "New SpectraOverdrive Show";
        public string artist;
        public string songName;
        public string author;
        [TextArea] public string authorNotes;
        [Min(0f)] public float durationSeconds = 60f;
        [Min(0f)] public float audioStartOffset;
        public string audioReference;
#if UNITY_EDITOR
        [Tooltip("Editor-only waveform source. It is never compiled into the runtime show or portable JSON.")]
        public AudioClip authoringAudio;
#endif
        public SpectraBeatGrid beatGrid = new SpectraBeatGrid();
        public SpectraShowFixtureGroup[] fixtureGroups = new SpectraShowFixtureGroup[0];
        public SpectraTimelineTrack[] tracks = new SpectraTimelineTrack[0];
        public SpectraTimelineMarker[] markers = new SpectraTimelineMarker[0];
        public SpectraLoopRegion[] loopRegions = new SpectraLoopRegion[0];
        [Tooltip("Up to sixteen portable color palettes. Colors are flattened into primitive runtime arrays.")]
        public SpectraColorPalette[] colorPalettes = new SpectraColorPalette[0];
        [Tooltip("Up to four synchronized live macro buses. Cue bindings are compiled to fixed indices.")]
        public SpectraPerformanceMacro[] performanceMacros = new SpectraPerformanceMacro[0];
        [Tooltip("Up to sixteen synchronized four-macro snapshots for live look recall.")]
        public SpectraPerformanceMacroSnapshot[] performanceMacroSnapshots = new SpectraPerformanceMacroSnapshot[0];
        public SpectraPlatformPolicy[] platformPolicies = new SpectraPlatformPolicy[0];
        public SpectraAccessibilityMetadata accessibility = new SpectraAccessibilityMetadata();

        public SpectraPlatformPolicy GetPlatformPolicy(SpectraPlatformKind platform)
        {
            if (platformPolicies != null)
                for (int i = 0; i < platformPolicies.Length; i++)
                    if (platformPolicies[i] != null && platformPolicies[i].platform == platform)
                        return platformPolicies[i];
            return SpectraPlatformPolicy.CreateDefault(platform);
        }

        public void EnsureStableIds()
        {
            UpgradeSchemaIfNeeded();
            if (!IsGuid(showId)) showId = Guid.NewGuid().ToString("N");
            EnsureGroupIds();
            EnsureTrackAndCueIds();
            EnsureMarkerIds();
            EnsureLoopIds();
        }

        public SpectraValidationIssue[] ValidateShow()
        {
            List<SpectraValidationIssue> issues = new List<SpectraValidationIssue>();
            if (schemaVersion <= 0 || schemaVersion > CurrentSchemaVersion)
                issues.Add(new SpectraValidationIssue(true, "schemaVersion", "Unsupported schema version."));
            if (string.IsNullOrWhiteSpace(showName))
                issues.Add(new SpectraValidationIssue(true, "showName", "Show name is required."));
            if (durationSeconds <= 0f)
                issues.Add(new SpectraValidationIssue(true, "durationSeconds", "Duration must be greater than zero."));
            if (beatGrid == null || beatGrid.bpm <= 0f || beatGrid.beatsPerBar <= 0)
                issues.Add(new SpectraValidationIssue(true, "beatGrid", "Beat grid requires positive BPM and beats per bar."));

            HashSet<string> groupIds = new HashSet<string>();
            HashSet<int> runtimeGroupIds = new HashSet<int>();
            if (fixtureGroups != null) for (int i = 0; i < fixtureGroups.Length; i++)
            {
                SpectraShowFixtureGroup group = fixtureGroups[i];
                if (group == null || string.IsNullOrEmpty(group.id))
                    issues.Add(new SpectraValidationIssue(true, "fixtureGroups[" + i + "]", "Group and stable ID are required."));
                else if (!groupIds.Add(group.id))
                    issues.Add(new SpectraValidationIssue(true, "fixtureGroups[" + i + "].id", "Duplicate group ID."));
                if (group != null && group.runtimeGroupId < 0)
                    issues.Add(new SpectraValidationIssue(true, "fixtureGroups[" + i + "].runtimeGroupId", "Runtime group IDs cannot be negative."));
                if (group != null && !runtimeGroupIds.Add(group.runtimeGroupId))
                    issues.Add(new SpectraValidationIssue(true, "fixtureGroups[" + i + "].runtimeGroupId", "Runtime group IDs must be unique."));
            }

            HashSet<string> trackIds = new HashSet<string>();
            HashSet<string> cueIds = new HashSet<string>();
            if (tracks != null) for (int ti = 0; ti < tracks.Length; ti++)
            {
                SpectraTimelineTrack track = tracks[ti];
                if (track == null) { issues.Add(new SpectraValidationIssue(true, "tracks[" + ti + "]", "Track is null.")); continue; }
                if (string.IsNullOrEmpty(track.id) || !trackIds.Add(track.id))
                    issues.Add(new SpectraValidationIssue(true, "tracks[" + ti + "].id", "Track stable ID is missing or duplicated."));
                bool hasEnabledCues = false;
                if (track.cues != null)
                    for (int enabledIndex = 0; enabledIndex < track.cues.Length; enabledIndex++)
                        if (track.cues[enabledIndex] != null && track.cues[enabledIndex].enabled) { hasEnabledCues = true; break; }
                bool groupOptional = track.trackType == SpectraTrackType.Global
                    || track.trackType == SpectraTrackType.Event;
                if (hasEnabledCues && string.IsNullOrEmpty(track.fixtureGroupId) && !groupOptional)
                    issues.Add(new SpectraValidationIssue(true, "tracks[" + ti + "].fixtureGroupId", "A cue-bearing track requires a fixture group."));
                else if (!string.IsNullOrEmpty(track.fixtureGroupId) && !groupIds.Contains(track.fixtureGroupId))
                    issues.Add(new SpectraValidationIssue(true, "tracks[" + ti + "].fixtureGroupId", "Track references a missing fixture group."));
                if (track.cues == null) continue;
                for (int ci = 0; ci < track.cues.Length; ci++)
                {
                    SpectraCueBlock cue = track.cues[ci];
                    if (cue == null) { issues.Add(new SpectraValidationIssue(true, "tracks[" + ti + "].cues[" + ci + "]", "Cue is null.")); continue; }
                    if (string.IsNullOrEmpty(cue.id) || !cueIds.Add(cue.id))
                        issues.Add(new SpectraValidationIssue(true, CuePath(ti, ci) + ".id", "Cue stable ID is missing or duplicated."));
                    if (!cue.enabled) continue;
                    float start = cue.ResolveStartSeconds(beatGrid);
                    float duration = cue.ResolveDurationSeconds(beatGrid);
                    if (duration <= 0f) issues.Add(new SpectraValidationIssue(true, CuePath(ti, ci), "Cue duration must be positive."));
                    if (cue.fadeIn > duration || cue.fadeOut > duration)
                        issues.Add(new SpectraValidationIssue(false, CuePath(ti, ci), "Fade exceeds cue duration and will be clamped by the compiler."));
                    if (start + duration > durationSeconds + 0.001f)
                        issues.Add(new SpectraValidationIssue(false, CuePath(ti, ci), "Cue extends beyond show duration."));
                    if (cue.strobeHz > 20f)
                        issues.Add(new SpectraValidationIssue(false, CuePath(ti, ci), "Strobe exceeds the recommended 20 Hz virtual safety ceiling."));
                    if (cue.valueType == SpectraCueValueType.Event && cue.eventChannel < 0)
                        issues.Add(new SpectraValidationIssue(true, CuePath(ti, ci), "Event channel cannot be negative."));
                    if (cue.valueType == SpectraCueValueType.AudioReactiveIntensity
                        && Mathf.Abs(cue.audioAmount) < 0.0001f)
                        issues.Add(new SpectraValidationIssue(false, CuePath(ti, ci), "Audio-reactive cue has zero modulation amount."));
                    if (cue.valueType == SpectraCueValueType.Movement && cue.movementSpeed > 6f
                        && (cue.questFallback == SpectraPlatformFallback.Full
                            || cue.iosFallback == SpectraPlatformFallback.Full
                            || cue.androidFallback == SpectraPlatformFallback.Full))
                        issues.Add(new SpectraValidationIssue(false, CuePath(ti, ci), "High-frequency movement has a Full mobile fallback and may be expensive."));
                    ValidateAutomation(cue, ti, ci, issues);
                    ValidateProceduralModulation(cue, ti, ci, issues);
                    ValidateRhythmGate(cue, ti, ci, issues);
                    ValidatePaletteBinding(cue, ti, ci, issues);
                    ValidateCueCondition(cue, ti, ci, issues);
                    ValidateVariation(cue, ti, ci, issues);
                    if (cue.performanceMacroIndex < -1 || cue.performanceMacroIndex > 3)
                        issues.Add(new SpectraValidationIssue(true, CuePath(ti, ci) + ".performanceMacroIndex", "Performance macro index must be -1 or 0 through 3."));
                    if (cue.performanceMacroIndex >= 0
                        && (performanceMacros == null || cue.performanceMacroIndex >= performanceMacros.Length))
                        issues.Add(new SpectraValidationIssue(true, CuePath(ti, ci) + ".performanceMacroIndex", "Cue references a missing performance macro."));
                }
            }
            HashSet<string> markerIds = new HashSet<string>();
            if (markers != null) for (int i = 0; i < markers.Length; i++)
            {
                SpectraTimelineMarker marker = markers[i];
                if (marker == null) { issues.Add(new SpectraValidationIssue(true, "markers[" + i + "]", "Marker is null.")); continue; }
                if (string.IsNullOrEmpty(marker.id) || !markerIds.Add(marker.id))
                    issues.Add(new SpectraValidationIssue(true, "markers[" + i + "].id", "Marker stable ID is missing or duplicated."));
                float markerTime = marker.ResolveSeconds(beatGrid);
                if (markerTime > durationSeconds)
                    issues.Add(new SpectraValidationIssue(false, "markers[" + i + "]", "Marker is beyond the show duration."));
                if (marker.hotCue && marker.transitionSeconds > 2f)
                    issues.Add(new SpectraValidationIssue(false, "markers[" + i + "].transitionSeconds", "Long hot-cue transitions may feel unresponsive during live operation."));
                if (marker.scene && !marker.hotCue)
                    issues.Add(new SpectraValidationIssue(true, "markers[" + i + "].scene", "Scene-stack markers must also be enabled as hot cues."));
            }
            HashSet<string> loopIds = new HashSet<string>();
            if (loopRegions != null) for (int i = 0; i < loopRegions.Length; i++)
            {
                SpectraLoopRegion loop = loopRegions[i];
                if (loop == null) { issues.Add(new SpectraValidationIssue(true, "loopRegions[" + i + "]", "Loop is null.")); continue; }
                if (string.IsNullOrEmpty(loop.id) || !loopIds.Add(loop.id))
                    issues.Add(new SpectraValidationIssue(true, "loopRegions[" + i + "].id", "Loop stable ID is missing or duplicated."));
                if (loop.endSeconds <= loop.startSeconds)
                    issues.Add(new SpectraValidationIssue(true, "loopRegions[" + i + "]", "Loop end must be after its start."));
                if (loop.endSeconds > durationSeconds)
                    issues.Add(new SpectraValidationIssue(false, "loopRegions[" + i + "]", "Loop extends beyond the show duration."));
            }
            ValidateColorPalettes(issues);
            ValidatePerformanceMacros(issues);
            ValidatePerformanceMacroSnapshots(issues);
            ValidateVariationGroups(issues);
            ValidatePlatformPolicies(issues);
            return issues.ToArray();
        }

        private void OnValidate() { EnsureStableIds(); }
        private static bool IsGuid(string value) { Guid id; return Guid.TryParse(value, out id); }
        private static string CuePath(int track, int cue) { return "tracks[" + track + "].cues[" + cue + "]"; }
        private void EnsureGroupIds() { if (fixtureGroups == null) return; for (int i = 0; i < fixtureGroups.Length; i++) if (fixtureGroups[i] != null && !IsGuid(fixtureGroups[i].id)) fixtureGroups[i].id = Guid.NewGuid().ToString("N"); }
        private void EnsureTrackAndCueIds() { if (tracks == null) return; for (int i = 0; i < tracks.Length; i++) { if (tracks[i] == null) continue; if (!IsGuid(tracks[i].id)) tracks[i].id = Guid.NewGuid().ToString("N"); if (tracks[i].cues == null) continue; for (int c = 0; c < tracks[i].cues.Length; c++) if (tracks[i].cues[c] != null && !IsGuid(tracks[i].cues[c].id)) tracks[i].cues[c].id = Guid.NewGuid().ToString("N"); } }
        private void EnsureMarkerIds() { if (markers == null) return; for (int i = 0; i < markers.Length; i++) if (markers[i] != null && !IsGuid(markers[i].id)) markers[i].id = Guid.NewGuid().ToString("N"); }
        private void EnsureLoopIds() { if (loopRegions == null) return; for (int i = 0; i < loopRegions.Length; i++) if (loopRegions[i] != null && !IsGuid(loopRegions[i].id)) loopRegions[i].id = Guid.NewGuid().ToString("N"); }

        private void ValidatePlatformPolicies(List<SpectraValidationIssue> issues)
        {
            HashSet<SpectraPlatformKind> platforms = new HashSet<SpectraPlatformKind>();
            if (platformPolicies == null) return;
            for (int i = 0; i < platformPolicies.Length; i++)
            {
                SpectraPlatformPolicy policy = platformPolicies[i];
                if (policy == null)
                {
                    issues.Add(new SpectraValidationIssue(true, "platformPolicies[" + i + "]", "Platform policy is null."));
                    continue;
                }
                if (!platforms.Add(policy.platform))
                    issues.Add(new SpectraValidationIssue(true, "platformPolicies[" + i + "]", "Duplicate platform policy."));
                if (policy.maximumActiveCues < 1 || policy.updateRate < 1
                    || policy.maximumFixtures < 1 || policy.audioReactiveUpdateDivider < 1
                    || policy.snapshotCapacity < 2)
                    issues.Add(new SpectraValidationIssue(true, "platformPolicies[" + i + "]", "Runtime, fixture, audio, and snapshot budgets must be positive."));
            }
        }

        private static void ValidateAutomation(
            SpectraCueBlock cue,
            int trackIndex,
            int cueIndex,
            List<SpectraValidationIssue> issues)
        {
            if (cue.automationMode == SpectraAutomationMode.Disabled) return;
            string path = CuePath(trackIndex, cueIndex) + ".automationKeys";
            if (cue.automationKeys == null || cue.automationKeys.Length < 2)
            {
                issues.Add(new SpectraValidationIssue(true, path, "Enabled automation requires at least two keys."));
                return;
            }
            if (cue.automationKeys.Length > 16)
                issues.Add(new SpectraValidationIssue(true, path, "Cross-platform automation is limited to 16 keys per cue."));
            float previous = -1f;
            for (int i = 0; i < cue.automationKeys.Length; i++)
            {
                SpectraAutomationKey key = cue.automationKeys[i];
                if (key == null)
                {
                    issues.Add(new SpectraValidationIssue(true, path + "[" + i + "]", "Automation key is null."));
                    continue;
                }
                if (key.normalizedTime + 0.000001f < previous)
                    issues.Add(new SpectraValidationIssue(true, path, "Automation keys must be sorted by normalized time."));
                if (Mathf.Abs(key.normalizedTime - previous) < 0.000001f)
                    issues.Add(new SpectraValidationIssue(true, path, "Automation keys cannot share the same normalized time."));
                previous = key.normalizedTime;
            }
        }

        private static void ValidateProceduralModulation(
            SpectraCueBlock cue,
            int trackIndex,
            int cueIndex,
            List<SpectraValidationIssue> issues)
        {
            if (cue.modulationWaveform == SpectraModulationWaveform.Disabled) return;
            string path = CuePath(trackIndex, cueIndex) + ".modulation";
            if (cue.modulationMode == SpectraAutomationMode.Disabled)
                issues.Add(new SpectraValidationIssue(true, path, "An enabled procedural waveform requires Replace, Add, or Multiply mode."));
            if (cue.modulationCycleLength <= 0.0001f)
                issues.Add(new SpectraValidationIssue(true, path, "Procedural cycle length must be positive."));
            if (cue.modulationWaveform == SpectraModulationWaveform.SampleAndHold
                && cue.randomSeed == 0)
                issues.Add(new SpectraValidationIssue(false, path, "Sample-and-hold uses a zero seed. It is deterministic, but sharing a seed can repeat patterns across cues."));
        }

        private static void ValidateRhythmGate(
            SpectraCueBlock cue,
            int trackIndex,
            int cueIndex,
            List<SpectraValidationIssue> issues)
        {
            if (cue.gatePattern == SpectraCueGatePattern.Disabled) return;
            string path = CuePath(trackIndex, cueIndex) + ".rhythmGate";
            if (cue.gateStepLength <= 0.0001f)
                issues.Add(new SpectraValidationIssue(true, path, "Rhythm-gate step length must be positive."));
            if (cue.gateStepCount < 1 || cue.gateStepCount > 32)
                issues.Add(new SpectraValidationIssue(true, path, "Rhythm gates support one through 32 steps."));
            if (cue.gateActiveSteps < 1 || cue.gateActiveSteps > cue.gateStepCount)
                issues.Add(new SpectraValidationIssue(true, path, "Active steps must be within the configured step count."));
            if (cue.gatePattern == SpectraCueGatePattern.SeededRandom && cue.randomSeed == 0)
                issues.Add(new SpectraValidationIssue(false, path, "Seeded-random gate uses a zero seed. It is deterministic, but may repeat another cue's pattern."));
            if (cue.gateTimeBase == SpectraModulationTimeBase.Seconds && cue.gateStepLength < 0.05f)
                issues.Add(new SpectraValidationIssue(false, path, "Sub-50 ms rhythm steps may be visually unsafe and expensive on mobile devices."));
            if (cue.gateTimeBase != SpectraModulationTimeBase.Seconds && cue.gateStepLength < 0.0625f)
                issues.Add(new SpectraValidationIssue(false, path, "Rhythm steps shorter than 1/16 beat should be device-tested for comfort and mobile cadence."));
        }

        private void ValidatePaletteBinding(
            SpectraCueBlock cue,
            int trackIndex,
            int cueIndex,
            List<SpectraValidationIssue> issues)
        {
            if (cue.paletteMode == SpectraPalettePlaybackMode.Disabled && cue.paletteIndex < 0) return;
            string path = CuePath(trackIndex, cueIndex) + ".palette";
            if (cue.valueType != SpectraCueValueType.Color)
                issues.Add(new SpectraValidationIssue(true, path, "Dynamic palettes can only be bound to Color cues."));
            if (cue.paletteIndex < 0 || colorPalettes == null || cue.paletteIndex >= colorPalettes.Length)
            {
                issues.Add(new SpectraValidationIssue(true, path, "Cue references a missing color palette."));
                return;
            }
            SpectraColorPalette palette = colorPalettes[cue.paletteIndex];
            int colorCount = palette == null || palette.colors == null ? 0 : palette.colors.Length;
            if ((cue.paletteMode == SpectraPalettePlaybackMode.Fixed
                    || cue.paletteMode == SpectraPalettePlaybackMode.MacroMorph)
                && (cue.palettePrimaryIndex < 0 || cue.palettePrimaryIndex >= colorCount))
                issues.Add(new SpectraValidationIssue(true, path + ".primaryIndex", "Primary color index is outside the selected palette."));
            if (cue.paletteMode == SpectraPalettePlaybackMode.MacroMorph
                && (cue.paletteSecondaryIndex < 0 || cue.paletteSecondaryIndex >= colorCount))
                issues.Add(new SpectraValidationIssue(true, path + ".secondaryIndex", "Secondary color index is outside the selected palette."));
            if ((cue.paletteMode == SpectraPalettePlaybackMode.Step
                    || cue.paletteMode == SpectraPalettePlaybackMode.PingPong
                    || cue.paletteMode == SpectraPalettePlaybackMode.SeededRandom)
                && cue.paletteStepLength <= 0.0001f)
                issues.Add(new SpectraValidationIssue(true, path, "Animated palette step length must be positive."));
            if (cue.paletteMode == SpectraPalettePlaybackMode.MacroMorph
                && (cue.paletteMacroIndex < 0 || cue.paletteMacroIndex > 3
                    || performanceMacros == null || cue.paletteMacroIndex >= performanceMacros.Length))
                issues.Add(new SpectraValidationIssue(true, path + ".macroIndex", "Macro Morph requires an existing performance macro."));
        }

        private void ValidateCueCondition(
            SpectraCueBlock cue,
            int trackIndex,
            int cueIndex,
            List<SpectraValidationIssue> issues)
        {
            if (cue.conditionMode == SpectraCueConditionMode.Disabled) return;
            string path = CuePath(trackIndex, cueIndex) + ".condition";
            if (cue.conditionCycleLength <= 0.0001f
                && (cue.conditionMode == SpectraCueConditionMode.Probability
                    || cue.conditionMode == SpectraCueConditionMode.EveryNthCycle))
                issues.Add(new SpectraValidationIssue(true, path, "Condition cycle length must be positive."));
            if (cue.conditionMode == SpectraCueConditionMode.EveryNthCycle
                && (cue.conditionEveryN < 1 || cue.conditionEveryN > 32))
                issues.Add(new SpectraValidationIssue(true, path, "Every-N conditions support one through 32 cycles."));
            bool macroCondition = cue.conditionMode == SpectraCueConditionMode.MacroAbove
                || cue.conditionMode == SpectraCueConditionMode.MacroBelow;
            if (macroCondition && (cue.conditionMacroIndex < 0 || cue.conditionMacroIndex > 3
                    || performanceMacros == null || cue.conditionMacroIndex >= performanceMacros.Length))
                issues.Add(new SpectraValidationIssue(true, path + ".macroIndex", "Macro conditions require an existing performance macro."));
            if (cue.conditionMode == SpectraCueConditionMode.Probability && cue.randomSeed == 0)
                issues.Add(new SpectraValidationIssue(false, path, "Probability condition uses a zero seed. It remains deterministic but may match another cue."));
            if (cue.conditionTimeBase == SpectraModulationTimeBase.Seconds
                && cue.conditionCycleLength < 0.05f
                && (cue.conditionMode == SpectraCueConditionMode.Probability
                    || cue.conditionMode == SpectraCueConditionMode.EveryNthCycle))
                issues.Add(new SpectraValidationIssue(false, path, "Sub-50 ms condition cycles can thrash mobile cue budgets."));
        }

        private void ValidateVariation(
            SpectraCueBlock cue,
            int trackIndex,
            int cueIndex,
            List<SpectraValidationIssue> issues)
        {
            if (cue.variationMode == SpectraVariationSelectionMode.Disabled) return;
            string path = CuePath(trackIndex, cueIndex) + ".variation";
            if (cue.variationGroup < 0 || cue.variationGroup > 15)
                issues.Add(new SpectraValidationIssue(true, path + ".group", "Variation group must be 0 through 15."));
            if (cue.variationOptionCount < 2 || cue.variationOptionCount > 8)
                issues.Add(new SpectraValidationIssue(true, path + ".optionCount", "Variation groups support two through eight options."));
            if (cue.variationOption < 0 || cue.variationOption >= cue.variationOptionCount)
                issues.Add(new SpectraValidationIssue(true, path + ".option", "Variation option must be within the configured option count."));
            if (cue.variationMode != SpectraVariationSelectionMode.MacroSelect
                && cue.variationCycleLength <= 0.0001f)
                issues.Add(new SpectraValidationIssue(true, path, "Variation cycle length must be positive."));
            if (cue.variationMode == SpectraVariationSelectionMode.MacroSelect
                && (cue.variationMacroIndex < 0 || cue.variationMacroIndex > 3
                    || performanceMacros == null || cue.variationMacroIndex >= performanceMacros.Length))
                issues.Add(new SpectraValidationIssue(true, path + ".macroIndex", "Macro Select requires an existing performance macro."));
            if (cue.variationMode == SpectraVariationSelectionMode.SeededRandom
                && cue.variationSeed == 0)
                issues.Add(new SpectraValidationIssue(false, path, "Seeded-random variation uses a zero seed. It remains deterministic but may repeat another group."));
        }

        private void ValidateVariationGroups(List<SpectraValidationIssue> issues)
        {
            SpectraCueBlock[] first = new SpectraCueBlock[16];
            if (tracks == null) return;
            for (int ti = 0; ti < tracks.Length; ti++)
            {
                SpectraTimelineTrack track = tracks[ti];
                if (track == null || track.cues == null) continue;
                for (int ci = 0; ci < track.cues.Length; ci++)
                {
                    SpectraCueBlock cue = track.cues[ci];
                    if (cue == null || !cue.enabled
                        || cue.variationMode == SpectraVariationSelectionMode.Disabled
                        || cue.variationGroup < 0 || cue.variationGroup > 15) continue;
                    SpectraCueBlock reference = first[cue.variationGroup];
                    if (reference == null)
                    {
                        first[cue.variationGroup] = cue;
                        continue;
                    }
                    bool consistent = reference.variationMode == cue.variationMode
                        && reference.variationOptionCount == cue.variationOptionCount
                        && reference.variationTimeBase == cue.variationTimeBase
                        && Mathf.Abs(reference.variationCycleLength - cue.variationCycleLength) < 0.0001f
                        && Mathf.Abs(reference.variationPhase - cue.variationPhase) < 0.0001f
                        && reference.variationSeed == cue.variationSeed
                        && reference.variationMacroIndex == cue.variationMacroIndex;
                    if (!consistent)
                        issues.Add(new SpectraValidationIssue(true, CuePath(ti, ci) + ".variation",
                            "All cues in variation group " + cue.variationGroup + " must share mode, option count, clock, phase, seed, and macro binding."));
                }
            }
        }

        private void ValidateColorPalettes(List<SpectraValidationIssue> issues)
        {
            if (colorPalettes == null) return;
            if (colorPalettes.Length > 16)
                issues.Add(new SpectraValidationIssue(true, "colorPalettes", "Cross-platform runtime supports a maximum of sixteen palettes."));
            for (int i = 0; i < colorPalettes.Length; i++)
            {
                SpectraColorPalette palette = colorPalettes[i];
                if (palette == null)
                {
                    issues.Add(new SpectraValidationIssue(true, "colorPalettes[" + i + "]", "Color palette is null."));
                    continue;
                }
                if (string.IsNullOrWhiteSpace(palette.name))
                    issues.Add(new SpectraValidationIssue(true, "colorPalettes[" + i + "].name", "Color palette name is required."));
                int count = palette.colors == null ? 0 : palette.colors.Length;
                if (count < 1 || count > 16)
                    issues.Add(new SpectraValidationIssue(true, "colorPalettes[" + i + "].colors", "Each palette requires one through sixteen colors."));
            }
        }

        private void ValidatePerformanceMacros(List<SpectraValidationIssue> issues)
        {
            if (performanceMacros == null) return;
            if (performanceMacros.Length > 4)
                issues.Add(new SpectraValidationIssue(true, "performanceMacros", "Cross-platform runtime supports a maximum of four synchronized performance macros."));
            for (int i = 0; i < performanceMacros.Length; i++)
            {
                SpectraPerformanceMacro macro = performanceMacros[i];
                if (macro == null)
                {
                    issues.Add(new SpectraValidationIssue(true, "performanceMacros[" + i + "]", "Performance macro is null."));
                    continue;
                }
                if (string.IsNullOrWhiteSpace(macro.name))
                    issues.Add(new SpectraValidationIssue(true, "performanceMacros[" + i + "].name", "Performance macro name is required."));
            }
        }

        private void ValidatePerformanceMacroSnapshots(List<SpectraValidationIssue> issues)
        {
            if (performanceMacroSnapshots == null) return;
            if (performanceMacroSnapshots.Length > 16)
                issues.Add(new SpectraValidationIssue(true, "performanceMacroSnapshots", "Cross-platform runtime supports a maximum of sixteen macro snapshots."));
            for (int i = 0; i < performanceMacroSnapshots.Length; i++)
            {
                SpectraPerformanceMacroSnapshot snapshot = performanceMacroSnapshots[i];
                if (snapshot == null)
                {
                    issues.Add(new SpectraValidationIssue(true, "performanceMacroSnapshots[" + i + "]", "Macro snapshot is null."));
                    continue;
                }
                if (string.IsNullOrWhiteSpace(snapshot.name))
                    issues.Add(new SpectraValidationIssue(true, "performanceMacroSnapshots[" + i + "].name", "Macro snapshot name is required."));
                if (snapshot.transitionSeconds < 0f)
                    issues.Add(new SpectraValidationIssue(false, "performanceMacroSnapshots[" + i + "].transitionSeconds", "Negative snapshot transition time will be clamped to zero by the compiler."));
                else if (snapshot.transitionSeconds > 8f)
                    issues.Add(new SpectraValidationIssue(false, "performanceMacroSnapshots[" + i + "].transitionSeconds", "Snapshot transitions above eight seconds may feel unresponsive during live operation."));
                Vector4 values = snapshot.values;
                if (values.x < 0f || values.x > 1f || values.y < 0f || values.y > 1f
                    || values.z < 0f || values.z > 1f || values.w < 0f || values.w > 1f)
                    issues.Add(new SpectraValidationIssue(false, "performanceMacroSnapshots[" + i + "].values", "Snapshot values will be clamped to zero through one by the compiler."));
            }
        }

        private void UpgradeSchemaIfNeeded()
        {
            if (schemaVersion < 1 || schemaVersion >= CurrentSchemaVersion) return;
            int previousVersion = schemaVersion;
            schemaVersion = CurrentSchemaVersion;
            if (tracks != null)
                for (int trackIndex = 0; trackIndex < tracks.Length; trackIndex++)
                {
                    SpectraTimelineTrack track = tracks[trackIndex];
                    if (track == null) continue;
                    if (track.displayColor.a <= 0f)
                        track.displayColor = new Color(0.55f, 0.2f, 0.95f, 1f);
                    if (track.cues == null) continue;
                    for (int cueIndex = 0; cueIndex < track.cues.Length; cueIndex++)
                    {
                        SpectraCueBlock cue = track.cues[cueIndex];
                        if (cue == null) continue;
                        if (previousVersion == 1) cue.enabled = true;
                        if (previousVersion == 1) cue.androidFallback = cue.iosFallback;
                        if (Mathf.Abs(cue.movementDirection) < 0.001f) cue.movementDirection = 1f;
                        if (cue.movementAmplitude <= 0f) cue.movementAmplitude = 1f;
                        if (cue.movementSpread <= 0f) cue.movementSpread = 1f;
                        if (previousVersion < 3)
                        {
                            cue.zoom = 0.5f;
                            cue.focus = 0.5f;
                            cue.audioAmount = 0.5f;
                            cue.eventOnce = true;
                        }
                        if (previousVersion < 4)
                        {
                            cue.automationMode = SpectraAutomationMode.Disabled;
                            cue.automationKeys = new SpectraAutomationKey[0];
                            cue.capabilityFallback = SpectraCapabilityFallback.EmissiveApproximation;
                        }
                        if (previousVersion < 5)
                        {
                            cue.modulationWaveform = SpectraModulationWaveform.Disabled;
                            cue.modulationTimeBase = SpectraModulationTimeBase.Beats;
                            cue.modulationMode = SpectraAutomationMode.Multiply;
                            cue.modulationCycleLength = 1f;
                            cue.modulationDutyCycle = 0.5f;
                            cue.modulationOffset = Vector4.one;
                            cue.modulationDepth = Vector4.zero;
                            cue.performanceMacroIndex = -1;
                            cue.performanceMacroMode = SpectraAutomationMode.Multiply;
                            cue.performanceMacroMinimum = Vector4.one;
                            cue.performanceMacroMaximum = Vector4.one;
                        }
                        if (previousVersion < 6)
                        {
                            cue.gatePattern = SpectraCueGatePattern.Disabled;
                            cue.gateTimeBase = SpectraModulationTimeBase.Beats;
                            cue.gateStepLength = 0.25f;
                            cue.gateStepCount = 8;
                            cue.gateActiveSteps = 4;
                            cue.gateDutyCycle = 0.72f;
                            cue.gateAttack = 0.02f;
                            cue.gateRelease = 0.06f;
                            cue.gateCustomMask = -1;
                            cue.paletteIndex = -1;
                            cue.paletteMode = SpectraPalettePlaybackMode.Disabled;
                            cue.paletteTimeBase = SpectraModulationTimeBase.Beats;
                            cue.paletteStepLength = 1f;
                            cue.paletteSecondaryIndex = 1;
                            cue.paletteMacroIndex = -1;
                            cue.paletteBlend = 1f;
                        }
                        if (previousVersion < 7)
                        {
                            cue.conditionMode = SpectraCueConditionMode.Disabled;
                            cue.conditionTimeBase = SpectraModulationTimeBase.Beats;
                            cue.conditionCycleLength = 1f;
                            cue.conditionProbability = 0.5f;
                            cue.conditionEveryN = 2;
                            cue.conditionMacroIndex = -1;
                            cue.conditionThreshold = 0.5f;
                            cue.variationMode = SpectraVariationSelectionMode.Disabled;
                            cue.variationGroup = -1;
                            cue.variationOptionCount = 2;
                            cue.variationTimeBase = SpectraModulationTimeBase.Bars;
                            cue.variationCycleLength = 1f;
                            cue.variationMacroIndex = -1;
                        }
                    }
                }
            if (platformPolicies == null || platformPolicies.Length == 0)
                platformPolicies = new[]
                {
                    SpectraPlatformPolicy.CreateDefault(SpectraPlatformKind.PC),
                    SpectraPlatformPolicy.CreateDefault(SpectraPlatformKind.Quest),
                    SpectraPlatformPolicy.CreateDefault(SpectraPlatformKind.IOS),
                    SpectraPlatformPolicy.CreateDefault(SpectraPlatformKind.Android)
                };
            else if (previousVersion < 3)
                for (int i = 0; i < platformPolicies.Length; i++)
                {
                    SpectraPlatformPolicy policy = platformPolicies[i];
                    if (policy == null) continue;
                    SpectraPlatformPolicy defaults = SpectraPlatformPolicy.CreateDefault(policy.platform);
                    if (policy.maximumFixtures <= 0) policy.maximumFixtures = defaults.maximumFixtures;
                    if (policy.maximumTransparentBeams <= 0) policy.maximumTransparentBeams = defaults.maximumTransparentBeams;
                    if (policy.audioReactiveUpdateDivider <= 0) policy.audioReactiveUpdateDivider = defaults.audioReactiveUpdateDivider;
                    if (policy.snapshotCapacity < 2) policy.snapshotCapacity = defaults.snapshotCapacity;
                    policy.shaderQualityTier = defaults.shaderQualityTier;
                }
            if (previousVersion < 4 && fixtureGroups != null)
                for (int i = 0; i < fixtureGroups.Length; i++)
                    if (fixtureGroups[i] != null)
                        fixtureGroups[i].capabilities = SpectraFixtureCapability.All;
            if (previousVersion < 5)
            {
                if (performanceMacros == null) performanceMacros = new SpectraPerformanceMacro[0];
                if (markers != null)
                    for (int i = 0; i < markers.Length; i++)
                        if (markers[i] != null)
                        {
                            markers[i].scene = false;
                            markers[i].sceneBank = 0;
                            markers[i].sceneOrder = i;
                            markers[i].sceneAutoAdvance = false;
                        }
            }
            if (previousVersion < 6 && colorPalettes == null)
                colorPalettes = new SpectraColorPalette[0];
            if (previousVersion < 7 && performanceMacroSnapshots == null)
                performanceMacroSnapshots = new SpectraPerformanceMacroSnapshot[0];
        }
    }
}
