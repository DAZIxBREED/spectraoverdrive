using System;
using System.Collections.Generic;
using System.Text;
using UnityEditor;
using UnityEngine;

namespace SpectraOverdrive.Editor
{
    public sealed class SpectraPlatformCompatibilityResult
    {
        public SpectraPlatformKind platform;
        public int cueCount;
        public int maximumConcurrentCues;
        public int cueBudget;
        public int updateRate;
        public int disabledCues;
        public int simplifiedCues;
        public int emissiveOnlyCues;
        public int capabilityFallbackCues;
        public int automationKeyCount;
        public int proceduralCueCount;
        public int performanceMacroBindingCount;
        public int rhythmGateCueCount;
        public int paletteBindingCount;
        public int conditionCueCount;
        public int variationCueCount;
        public readonly List<string> warnings = new List<string>();

        public bool FitsBudget { get { return maximumConcurrentCues <= cueBudget; } }
    }

    public static class SpectraPlatformCompatibilityValidator
    {
        private struct Edge
        {
            public float time;
            public int delta;
        }

        public static SpectraPlatformCompatibilityResult Analyze(SpectraShowAsset show, SpectraPlatformKind platform)
        {
            if (show == null) throw new ArgumentNullException("show");
            SpectraPlatformPolicy policy = show.GetPlatformPolicy(platform);
            SpectraPlatformCompatibilityResult result = new SpectraPlatformCompatibilityResult
            {
                platform = platform,
                cueBudget = Mathf.Max(1, policy.maximumActiveCues),
                updateRate = Mathf.Max(1, policy.updateRate)
            };
            Dictionary<string, SpectraFixtureCapability> groupCapabilities =
                new Dictionary<string, SpectraFixtureCapability>();
            if (show.fixtureGroups != null)
                for (int groupIndex = 0; groupIndex < show.fixtureGroups.Length; groupIndex++)
                {
                    SpectraShowFixtureGroup group = show.fixtureGroups[groupIndex];
                    if (group != null && !string.IsNullOrEmpty(group.id))
                        groupCapabilities[group.id] = group.capabilities;
                }
            List<Edge> edges = new List<Edge>();
            if (show.tracks != null)
                for (int trackIndex = 0; trackIndex < show.tracks.Length; trackIndex++)
                {
                    SpectraTimelineTrack track = show.tracks[trackIndex];
                    if (track == null || track.muted || track.cues == null) continue;
                    for (int cueIndex = 0; cueIndex < track.cues.Length; cueIndex++)
                    {
                        SpectraCueBlock cue = track.cues[cueIndex];
                        if (cue == null || !cue.enabled) continue;
                        result.cueCount++;
                        int keyCount = cue.automationMode == SpectraAutomationMode.Disabled
                            || cue.automationKeys == null ? 0 : cue.automationKeys.Length;
                        result.automationKeyCount += keyCount;
                        if (cue.modulationWaveform != SpectraModulationWaveform.Disabled)
                        {
                            result.proceduralCueCount++;
                            if (IsMobile(platform)
                                && cue.modulationTimeBase == SpectraModulationTimeBase.Seconds
                                && cue.modulationCycleLength < 0.08f)
                                result.warnings.Add(CueName(track, cue)
                                    + " uses sub-80ms procedural modulation; mobile update cadence will intentionally limit it.");
                        }
                        if (cue.performanceMacroIndex >= 0)
                            result.performanceMacroBindingCount++;
                        if (cue.gatePattern != SpectraCueGatePattern.Disabled)
                        {
                            result.rhythmGateCueCount++;
                            if (IsMobile(platform)
                                && cue.gateTimeBase == SpectraModulationTimeBase.Seconds
                                && cue.gateStepLength < 0.08f)
                                result.warnings.Add(CueName(track, cue)
                                    + " uses sub-80ms rhythm-gate steps; mobile cadence will intentionally limit them.");
                        }
                        if (cue.paletteMode != SpectraPalettePlaybackMode.Disabled)
                            result.paletteBindingCount++;
                        if (cue.conditionMode != SpectraCueConditionMode.Disabled)
                        {
                            result.conditionCueCount++;
                            if (IsMobile(platform)
                                && (cue.conditionMode == SpectraCueConditionMode.Probability
                                    || cue.conditionMode == SpectraCueConditionMode.EveryNthCycle)
                                && cue.conditionTimeBase == SpectraModulationTimeBase.Seconds
                                && cue.conditionCycleLength < 0.08f)
                                result.warnings.Add(CueName(track, cue)
                                    + " uses a sub-80ms condition cycle; mobile cadence will intentionally limit it.");
                        }
                        if (cue.variationMode != SpectraVariationSelectionMode.Disabled)
                        {
                            result.variationCueCount++;
                            if (IsMobile(platform)
                                && cue.variationMode != SpectraVariationSelectionMode.MacroSelect
                                && cue.variationTimeBase == SpectraModulationTimeBase.Seconds
                                && cue.variationCycleLength < 0.08f)
                                result.warnings.Add(CueName(track, cue)
                                    + " uses sub-80ms variation switching; mobile cadence will intentionally limit it.");
                        }
                        if (IsMobile(platform) && keyCount > 8)
                            result.warnings.Add(CueName(track, cue) + " uses " + keyCount
                                + " automation keys; mobile evaluation is supported but should be device-tested.");
                        bool groupBound = track.trackType != SpectraTrackType.Global
                            && track.trackType != SpectraTrackType.Event;
                        if (groupBound && !string.IsNullOrEmpty(track.fixtureGroupId)
                            && groupCapabilities.TryGetValue(track.fixtureGroupId, out SpectraFixtureCapability available))
                        {
                            SpectraFixtureCapability required = RequiredCapability(cue.valueType);
                            if ((available & required) != required)
                            {
                                result.capabilityFallbackCues++;
                                if (cue.capabilityFallback == SpectraCapabilityFallback.DisableCue)
                                    result.warnings.Add(CueName(track, cue)
                                        + " is disabled because its group lacks " + required + ".");
                            }
                        }
                        SpectraPlatformFallback fallback = Fallback(cue, platform);
                        if (fallback == SpectraPlatformFallback.Disabled)
                        {
                            result.disabledCues++;
                            continue;
                        }
                        if (fallback == SpectraPlatformFallback.Simplified) result.simplifiedCues++;
                        if (fallback == SpectraPlatformFallback.EmissiveOnly) result.emissiveOnlyCues++;
                        if (fallback == SpectraPlatformFallback.EmissiveOnly && IsNonEmissive(cue.valueType))
                            continue;
                        float start = cue.ResolveStartSeconds(show.beatGrid);
                        float end = start + cue.ResolveDurationSeconds(show.beatGrid);
                        edges.Add(new Edge { time = start, delta = 1 });
                        edges.Add(new Edge { time = end, delta = -1 });
                        if (IsMobile(platform) && fallback == SpectraPlatformFallback.Full
                            && cue.valueType == SpectraCueValueType.Movement && cue.movementSpeed > 4f)
                            result.warnings.Add(CueName(track, cue) + " uses full high-speed movement on mobile.");
                        if ((platform == SpectraPlatformKind.IOS || platform == SpectraPlatformKind.Android)
                            && fallback == SpectraPlatformFallback.Full
                            && (cue.valueType == SpectraCueValueType.Gobo
                                || cue.valueType == SpectraCueValueType.Prism
                                || cue.valueType == SpectraCueValueType.ZoomFocus))
                            result.warnings.Add(CueName(track, cue) + " requests full optical simulation on phone/tablet.");
                        if (!policy.allowStrobes && cue.valueType == SpectraCueValueType.Strobe)
                            result.warnings.Add(CueName(track, cue) + " contains a strobe while the platform policy disables strobes.");
                        if (!policy.allowLasers && cue.valueType == SpectraCueValueType.LaserEnable)
                            result.warnings.Add(CueName(track, cue) + " enables lasers while the platform policy disables lasers.");
                    }
                }
            edges.Sort(delegate(Edge a, Edge b)
            {
                int time = a.time.CompareTo(b.time);
                return time != 0 ? time : a.delta.CompareTo(b.delta);
            });
            int active = 0;
            for (int i = 0; i < edges.Count; i++)
            {
                active += edges[i].delta;
                result.maximumConcurrentCues = Mathf.Max(result.maximumConcurrentCues, active);
            }
            if (!result.FitsBudget)
                result.warnings.Insert(0, "Peak cue concurrency " + result.maximumConcurrentCues
                    + " exceeds the platform budget of " + result.cueBudget + ".");
            if (IsMobile(platform) && policy.updateRate > 45)
                result.warnings.Add("The " + policy.updateRate + " Hz update rate is aggressive for a mobile target.");
            return result;
        }

        public static SpectraPlatformCompatibilityResult[] AnalyzeAll(SpectraShowAsset show)
        {
            return new[]
            {
                Analyze(show, SpectraPlatformKind.PC),
                Analyze(show, SpectraPlatformKind.Quest),
                Analyze(show, SpectraPlatformKind.IOS),
                Analyze(show, SpectraPlatformKind.Android)
            };
        }

        public static string Format(SpectraPlatformCompatibilityResult[] results)
        {
            StringBuilder builder = new StringBuilder();
            for (int i = 0; i < results.Length; i++)
            {
                SpectraPlatformCompatibilityResult result = results[i];
                builder.AppendLine(result.platform + ": peak " + result.maximumConcurrentCues + "/" + result.cueBudget
                    + " cues, " + result.updateRate + " Hz, "
                    + result.simplifiedCues + " simplified, "
                    + result.emissiveOnlyCues + " emissive, "
                    + result.capabilityFallbackCues + " capability fallbacks, "
                    + result.automationKeyCount + " automation keys, "
                    + result.proceduralCueCount + " procedural cues, "
                    + result.performanceMacroBindingCount + " macro bindings, "
                    + result.rhythmGateCueCount + " rhythm gates, "
                    + result.paletteBindingCount + " palette bindings, "
                    + result.conditionCueCount + " conditioned cues, "
                    + result.variationCueCount + " variation cues, "
                    + result.disabledCues + " disabled.");
                for (int warning = 0; warning < result.warnings.Count; warning++)
                    builder.AppendLine("  - " + result.warnings[warning]);
            }
            return builder.ToString();
        }

        [MenuItem("SpectraOverdrive/Show Programmer/Validate Cross-Platform Budgets")]
        private static void ValidateSelected()
        {
            SpectraShowAsset show = Selection.activeObject as SpectraShowAsset;
            if (show == null)
            {
                EditorUtility.DisplayDialog("SpectraOverdrive", "Select a SpectraShowAsset first.", "OK");
                return;
            }
            string report = Format(AnalyzeAll(show));
            Debug.Log("[SpectraOverdrive] Cross-platform report for " + show.showName + "\n" + report, show);
            EditorUtility.DisplayDialog("SpectraOverdrive Cross-Platform Report", report, "OK");
        }

        private static SpectraPlatformFallback Fallback(SpectraCueBlock cue, SpectraPlatformKind platform)
        {
            if (platform == SpectraPlatformKind.Quest) return cue.questFallback;
            if (platform == SpectraPlatformKind.IOS) return cue.iosFallback;
            if (platform == SpectraPlatformKind.Android) return cue.androidFallback;
            return SpectraPlatformFallback.Full;
        }

        private static bool IsMobile(SpectraPlatformKind platform)
        {
            return platform == SpectraPlatformKind.Quest || platform == SpectraPlatformKind.IOS || platform == SpectraPlatformKind.Android;
        }

        private static bool IsNonEmissive(SpectraCueValueType type)
        {
            return type != SpectraCueValueType.Intensity
                && type != SpectraCueValueType.AudioReactiveIntensity
                && type != SpectraCueValueType.Color
                && type != SpectraCueValueType.Event
                && type != SpectraCueValueType.Blackout;
        }

        private static SpectraFixtureCapability RequiredCapability(SpectraCueValueType type)
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

        private static string CueName(SpectraTimelineTrack track, SpectraCueBlock cue)
        {
            return "'" + cue.name + "' on '" + track.name + "'";
        }
    }
}
