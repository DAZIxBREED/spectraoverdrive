using System;
using System.Collections.Generic;
using System.IO;
using UnityEditor;
using UnityEngine;

namespace SpectraOverdrive.Editor
{
    [Serializable]
    public sealed class SpectraReleaseReadinessReport
    {
        public string generatorVersion = "1.4.0";
        public string showName;
        public string showId;
        public string contentHash;
        public int contentSignature;
        public int cueCount;
        public int markerCount;
        public int loopCount;
        public int hotCueCount;
        public int automationKeyCount;
        public int proceduralCueCount;
        public int performanceMacroBindingCount;
        public int rhythmGateCueCount;
        public int paletteBindingCount;
        public int paletteCount;
        public int paletteColorCount;
        public int conditionCueCount;
        public int variationCueCount;
        public int variationGroupCount;
        public int performanceMacroSnapshotCount;
        public int sceneCount;
        public bool ready;
        public string[] errors = new string[0];
        public string[] warnings = new string[0];
        public string[] platformSummaries = new string[0];
    }

    public static class SpectraReleaseReadinessValidator
    {
        public static SpectraReleaseReadinessReport Validate(SpectraShowAsset show)
        {
            if (show == null) throw new ArgumentNullException("show");
            show.EnsureStableIds();
            List<string> errors = new List<string>();
            List<string> warnings = new List<string>();
            SpectraValidationIssue[] issues = show.ValidateShow();
            for (int i = 0; i < issues.Length; i++)
            {
                string text = issues[i].path + ": " + issues[i].message;
                if (issues[i].isError) errors.Add(text); else warnings.Add(text);
            }
            RequirePlatform(show, SpectraPlatformKind.PC, errors);
            RequirePlatform(show, SpectraPlatformKind.Quest, errors);
            RequirePlatform(show, SpectraPlatformKind.IOS, errors);
            RequirePlatform(show, SpectraPlatformKind.Android, errors);
            ValidateSafetyMetadata(show, warnings);

            SpectraCompiledShow compiled = null;
            if (errors.Count == 0)
            {
                try
                {
                    compiled = SpectraShowCompiler.Compile(show);
                    string json = SpectraShowJson.Export(show, false);
                    SpectraShowAsset roundTrip = ScriptableObject.CreateInstance<SpectraShowAsset>();
                    try
                    {
                        SpectraShowJson.ImportInto(json, roundTrip);
                        SpectraCompiledShow second = SpectraShowCompiler.Compile(roundTrip);
                        if (second.contentSignature != compiled.contentSignature)
                            errors.Add("Portable JSON round trip changed the runtime content signature.");
                    }
                    finally
                    {
                        UnityEngine.Object.DestroyImmediate(roundTrip);
                    }
                }
                catch (Exception exception)
                {
                    errors.Add("Compile/export verification failed: " + exception.Message);
                }
            }

            SpectraPlatformCompatibilityResult[] platforms = SpectraPlatformCompatibilityValidator.AnalyzeAll(show);
            string[] platformSummaries = new string[platforms.Length];
            for (int i = 0; i < platforms.Length; i++)
            {
                SpectraPlatformCompatibilityResult platform = platforms[i];
                platformSummaries[i] = platform.platform + ": peak "
                    + platform.maximumConcurrentCues + "/" + platform.cueBudget
                    + " cues at " + platform.updateRate + " Hz";
                if (!platform.FitsBudget)
                    errors.Add(platform.platform + " cue concurrency exceeds its release budget.");
                for (int warning = 0; warning < platform.warnings.Count; warning++)
                    warnings.Add(platform.platform + ": " + platform.warnings[warning]);
            }

            return new SpectraReleaseReadinessReport
            {
                showName = show.showName,
                showId = show.showId,
                contentHash = compiled == null ? string.Empty : compiled.contentHash,
                contentSignature = compiled == null ? 0 : compiled.contentSignature,
                cueCount = compiled == null ? CountCues(show) : compiled.CueCount,
                markerCount = show.markers == null ? 0 : show.markers.Length,
                loopCount = show.loopRegions == null ? 0 : show.loopRegions.Length,
                hotCueCount = CountHotCues(show),
                automationKeyCount = CountAutomationKeys(show),
                proceduralCueCount = CountProceduralCues(show),
                performanceMacroBindingCount = CountPerformanceMacroBindings(show),
                rhythmGateCueCount = CountRhythmGateCues(show),
                paletteBindingCount = CountPaletteBindings(show),
                paletteCount = show.colorPalettes == null ? 0 : show.colorPalettes.Length,
                paletteColorCount = CountPaletteColors(show),
                conditionCueCount = CountConditionCues(show),
                variationCueCount = CountVariationCues(show),
                variationGroupCount = CountVariationGroups(show),
                performanceMacroSnapshotCount = show.performanceMacroSnapshots == null
                    ? 0 : show.performanceMacroSnapshots.Length,
                sceneCount = CountScenes(show),
                ready = errors.Count == 0,
                errors = errors.ToArray(),
                warnings = warnings.ToArray(),
                platformSummaries = platformSummaries
            };
        }

        private static void RequirePlatform(
            SpectraShowAsset show,
            SpectraPlatformKind platform,
            List<string> errors)
        {
            if (show.platformPolicies == null)
            {
                errors.Add("Missing " + platform + " platform policy.");
                return;
            }
            for (int i = 0; i < show.platformPolicies.Length; i++)
                if (show.platformPolicies[i] != null && show.platformPolicies[i].platform == platform)
                    return;
            errors.Add("Missing explicit " + platform + " platform policy.");
        }

        private static void ValidateSafetyMetadata(SpectraShowAsset show, List<string> warnings)
        {
            bool strobes = false;
            bool lasers = false;
            if (show.tracks != null)
                for (int track = 0; track < show.tracks.Length; track++)
                {
                    SpectraTimelineTrack timelineTrack = show.tracks[track];
                    if (timelineTrack == null || timelineTrack.cues == null) continue;
                    for (int cue = 0; cue < timelineTrack.cues.Length; cue++)
                    {
                        SpectraCueBlock block = timelineTrack.cues[cue];
                        if (block == null || !block.enabled) continue;
                        if (block.valueType == SpectraCueValueType.Strobe && block.strobeHz > 0f) strobes = true;
                        if (block.valueType == SpectraCueValueType.LaserEnable && block.boolValue) lasers = true;
                    }
                }
            if (show.accessibility == null)
            {
                warnings.Add("Accessibility metadata is missing.");
                return;
            }
            if (strobes && !show.accessibility.containsStrobes)
                warnings.Add("Show contains strobes but accessibility metadata does not declare them.");
            if (lasers && !show.accessibility.containsLasers)
                warnings.Add("Show contains lasers but accessibility metadata does not declare them.");
        }

        private static int CountCues(SpectraShowAsset show)
        {
            int count = 0;
            if (show.tracks == null) return count;
            for (int i = 0; i < show.tracks.Length; i++)
                if (show.tracks[i] != null && show.tracks[i].cues != null)
                    count += show.tracks[i].cues.Length;
            return count;
        }

        private static int CountHotCues(SpectraShowAsset show)
        {
            int count = 0;
            if (show.markers == null) return count;
            for (int i = 0; i < show.markers.Length; i++)
                if (show.markers[i] != null && show.markers[i].hotCue) count++;
            return count;
        }

        private static int CountAutomationKeys(SpectraShowAsset show)
        {
            int count = 0;
            if (show.tracks == null) return count;
            for (int trackIndex = 0; trackIndex < show.tracks.Length; trackIndex++)
            {
                SpectraTimelineTrack track = show.tracks[trackIndex];
                if (track == null || track.cues == null) continue;
                for (int cueIndex = 0; cueIndex < track.cues.Length; cueIndex++)
                {
                    SpectraCueBlock cue = track.cues[cueIndex];
                    if (cue != null && cue.automationMode != SpectraAutomationMode.Disabled
                        && cue.automationKeys != null)
                        count += cue.automationKeys.Length;
                }
            }
            return count;
        }

        private static int CountProceduralCues(SpectraShowAsset show)
        {
            int count = 0;
            if (show.tracks == null) return count;
            for (int trackIndex = 0; trackIndex < show.tracks.Length; trackIndex++)
            {
                SpectraTimelineTrack track = show.tracks[trackIndex];
                if (track == null || track.cues == null) continue;
                for (int cueIndex = 0; cueIndex < track.cues.Length; cueIndex++)
                    if (track.cues[cueIndex] != null
                        && track.cues[cueIndex].modulationWaveform
                            != SpectraModulationWaveform.Disabled)
                        count++;
            }
            return count;
        }

        private static int CountPerformanceMacroBindings(SpectraShowAsset show)
        {
            int count = 0;
            if (show.tracks == null) return count;
            for (int trackIndex = 0; trackIndex < show.tracks.Length; trackIndex++)
            {
                SpectraTimelineTrack track = show.tracks[trackIndex];
                if (track == null || track.cues == null) continue;
                for (int cueIndex = 0; cueIndex < track.cues.Length; cueIndex++)
                    if (track.cues[cueIndex] != null
                        && track.cues[cueIndex].performanceMacroIndex >= 0)
                        count++;
            }
            return count;
        }

        private static int CountRhythmGateCues(SpectraShowAsset show)
        {
            int count = 0;
            if (show.tracks == null) return count;
            for (int trackIndex = 0; trackIndex < show.tracks.Length; trackIndex++)
            {
                SpectraTimelineTrack track = show.tracks[trackIndex];
                if (track == null || track.cues == null) continue;
                for (int cueIndex = 0; cueIndex < track.cues.Length; cueIndex++)
                    if (track.cues[cueIndex] != null
                        && track.cues[cueIndex].gatePattern != SpectraCueGatePattern.Disabled)
                        count++;
            }
            return count;
        }

        private static int CountPaletteBindings(SpectraShowAsset show)
        {
            int count = 0;
            if (show.tracks == null) return count;
            for (int trackIndex = 0; trackIndex < show.tracks.Length; trackIndex++)
            {
                SpectraTimelineTrack track = show.tracks[trackIndex];
                if (track == null || track.cues == null) continue;
                for (int cueIndex = 0; cueIndex < track.cues.Length; cueIndex++)
                    if (track.cues[cueIndex] != null
                        && track.cues[cueIndex].paletteMode != SpectraPalettePlaybackMode.Disabled)
                        count++;
            }
            return count;
        }

        private static int CountPaletteColors(SpectraShowAsset show)
        {
            int count = 0;
            if (show.colorPalettes == null) return count;
            for (int i = 0; i < show.colorPalettes.Length; i++)
                if (show.colorPalettes[i] != null && show.colorPalettes[i].colors != null)
                    count += show.colorPalettes[i].colors.Length;
            return count;
        }


        private static int CountConditionCues(SpectraShowAsset show)
        {
            int count = 0;
            if (show.tracks == null) return count;
            for (int trackIndex = 0; trackIndex < show.tracks.Length; trackIndex++)
            {
                SpectraTimelineTrack track = show.tracks[trackIndex];
                if (track == null || track.cues == null) continue;
                for (int cueIndex = 0; cueIndex < track.cues.Length; cueIndex++)
                    if (track.cues[cueIndex] != null
                        && track.cues[cueIndex].conditionMode != SpectraCueConditionMode.Disabled)
                        count++;
            }
            return count;
        }

        private static int CountVariationCues(SpectraShowAsset show)
        {
            int count = 0;
            if (show.tracks == null) return count;
            for (int trackIndex = 0; trackIndex < show.tracks.Length; trackIndex++)
            {
                SpectraTimelineTrack track = show.tracks[trackIndex];
                if (track == null || track.cues == null) continue;
                for (int cueIndex = 0; cueIndex < track.cues.Length; cueIndex++)
                    if (track.cues[cueIndex] != null
                        && track.cues[cueIndex].variationMode != SpectraVariationSelectionMode.Disabled)
                        count++;
            }
            return count;
        }

        private static int CountVariationGroups(SpectraShowAsset show)
        {
            bool[] groups = new bool[16];
            int count = 0;
            if (show.tracks == null) return count;
            for (int trackIndex = 0; trackIndex < show.tracks.Length; trackIndex++)
            {
                SpectraTimelineTrack track = show.tracks[trackIndex];
                if (track == null || track.cues == null) continue;
                for (int cueIndex = 0; cueIndex < track.cues.Length; cueIndex++)
                {
                    SpectraCueBlock cue = track.cues[cueIndex];
                    if (cue == null || cue.variationMode == SpectraVariationSelectionMode.Disabled) continue;
                    int group = Mathf.Clamp(cue.variationGroup, 0, groups.Length - 1);
                    if (!groups[group]) { groups[group] = true; count++; }
                }
            }
            return count;
        }

        private static int CountScenes(SpectraShowAsset show)
        {
            int count = 0;
            if (show.markers == null) return count;
            for (int i = 0; i < show.markers.Length; i++)
                if (show.markers[i] != null && show.markers[i].scene) count++;
            return count;
        }

        [MenuItem("SpectraOverdrive/Show Programmer/Run 1.4 Release Readiness Check")]
        private static void ValidateSelected()
        {
            SpectraShowAsset show = Selection.activeObject as SpectraShowAsset;
            if (show == null)
            {
                EditorUtility.DisplayDialog("SpectraOverdrive", "Select a SpectraShowAsset first.", "OK");
                return;
            }
            SpectraReleaseReadinessReport report = Validate(show);
            string summary = report.ready ? "READY FOR DEVICE TESTING" : "NOT READY";
            summary += "\n\nCues: " + report.cueCount
                + "\nRhythm gates: " + report.rhythmGateCueCount
                + "\nPalette bindings: " + report.paletteBindingCount
                + "\nPalette colors: " + report.paletteColorCount
                + "\nSignature: " + report.contentHash
                + "\nErrors: " + report.errors.Length
                + "\nWarnings: " + report.warnings.Length;
            Debug.Log("[SpectraOverdrive 1.4] " + summary + "\n"
                + JsonUtility.ToJson(report, true), show);
            if (EditorUtility.DisplayDialog("SpectraOverdrive 1.4 - " + summary,
                summary + "\n\nWrite the full machine-readable report to disk?", "Write Report", "Close"))
            {
                string path = EditorUtility.SaveFilePanel(
                    "Save SpectraOverdrive release report", "",
                    show.showName + ".spectra-release-report.json", "json");
                if (!string.IsNullOrEmpty(path))
                    File.WriteAllText(path, JsonUtility.ToJson(report, true));
            }
        }
    }
}
