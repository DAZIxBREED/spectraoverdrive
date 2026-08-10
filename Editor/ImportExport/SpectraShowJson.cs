using System;
using System.IO;
using UnityEditor;
using UnityEngine;

namespace SpectraOverdrive.Editor
{
    [Serializable]
    internal class SpectraPortableShowData
    {
        public string format = "SpectraOverdrive.Show";
        public string createdWith = "1.5.2";
        public string contentHash;
        public int schemaVersion;
        public string showId;
        public string showName;
        public string artist;
        public string songName;
        public string author;
        public string authorNotes;
        public float durationSeconds;
        public float audioStartOffset;
        public string audioReference;
        public SpectraBeatGrid beatGrid;
        public SpectraShowFixtureGroup[] fixtureGroups;
        public SpectraTimelineTrack[] tracks;
        public SpectraTimelineMarker[] markers;
        public SpectraLoopRegion[] loopRegions;
        public SpectraColorPalette[] colorPalettes;
        public SpectraPerformanceMacro[] performanceMacros;
        public SpectraPerformanceMacroSnapshot[] performanceMacroSnapshots;
        public SpectraCueLayer[] cueLayers;
        public SpectraPlatformPolicy[] platformPolicies;
        public SpectraAccessibilityMetadata accessibility;

        public static SpectraPortableShowData FromAsset(SpectraShowAsset show)
        {
            return new SpectraPortableShowData
            {
                format = "SpectraOverdrive.Show",
                createdWith = "1.5.2",
                contentHash = string.Empty,
                schemaVersion = show.schemaVersion,
                showId = show.showId,
                showName = show.showName,
                artist = show.artist,
                songName = show.songName,
                author = show.author,
                authorNotes = show.authorNotes,
                durationSeconds = show.durationSeconds,
                audioStartOffset = show.audioStartOffset,
                audioReference = show.audioReference,
                beatGrid = show.beatGrid,
                fixtureGroups = show.fixtureGroups,
                tracks = show.tracks,
                markers = show.markers,
                loopRegions = show.loopRegions,
                colorPalettes = show.colorPalettes,
                performanceMacros = show.performanceMacros,
                performanceMacroSnapshots = show.performanceMacroSnapshots,
                cueLayers = show.cueLayers,
                platformPolicies = show.platformPolicies,
                accessibility = show.accessibility
            };
        }

        public void ApplyTo(SpectraShowAsset show)
        {
            show.schemaVersion = schemaVersion;
            show.showId = showId;
            show.showName = showName;
            show.artist = artist;
            show.songName = songName;
            show.author = author;
            show.authorNotes = authorNotes;
            show.durationSeconds = durationSeconds;
            show.audioStartOffset = audioStartOffset;
            show.audioReference = audioReference;
            show.beatGrid = beatGrid ?? new SpectraBeatGrid();
            show.fixtureGroups = fixtureGroups ?? new SpectraShowFixtureGroup[0];
            show.tracks = tracks ?? new SpectraTimelineTrack[0];
            show.markers = markers ?? new SpectraTimelineMarker[0];
            show.loopRegions = loopRegions ?? new SpectraLoopRegion[0];
            show.colorPalettes = colorPalettes ?? new SpectraColorPalette[0];
            show.performanceMacros = performanceMacros ?? new SpectraPerformanceMacro[0];
            show.performanceMacroSnapshots = performanceMacroSnapshots
                ?? new SpectraPerformanceMacroSnapshot[0];
            show.cueLayers = cueLayers ?? new SpectraCueLayer[0];
            show.platformPolicies = platformPolicies ?? new SpectraPlatformPolicy[0];
            show.accessibility = accessibility ?? new SpectraAccessibilityMetadata();
        }
    }

    public static class SpectraShowJson
    {
        public static string Export(SpectraShowAsset show, bool pretty)
        {
            if (show == null) throw new ArgumentNullException("show");
            show.EnsureStableIds();
            SpectraValidationIssue[] issues = show.ValidateShow();
            for (int i = 0; i < issues.Length; i++)
                if (issues[i].isError) throw new InvalidOperationException(issues[i].path + ": " + issues[i].message);
            SpectraPortableShowData data = SpectraPortableShowData.FromAsset(show);
            string canonical = JsonUtility.ToJson(data, false);
            data.contentHash = SpectraShowIntegrity.ComputeSha256(canonical);
            return JsonUtility.ToJson(data, pretty);
        }

        public static void ImportInto(string json, SpectraShowAsset target)
        {
            if (target == null) throw new ArgumentNullException("target");
            if (string.IsNullOrWhiteSpace(json)) throw new ArgumentException("JSON is empty.", "json");
            SpectraPortableShowData data = JsonUtility.FromJson<SpectraPortableShowData>(json);
            if (data == null) throw new InvalidDataException("JSON did not contain a SpectraOverdrive show.");
            if (!string.IsNullOrEmpty(data.format) && data.format != "SpectraOverdrive.Show")
                throw new InvalidDataException("JSON is not a SpectraOverdrive show.");
            string expectedHash = data.contentHash;
            if (!string.IsNullOrEmpty(expectedHash))
            {
                data.contentHash = string.Empty;
                string calculatedHash = SpectraShowIntegrity.ComputeSha256(JsonUtility.ToJson(data, false));
                if (!string.Equals(expectedHash, calculatedHash, StringComparison.OrdinalIgnoreCase))
                {
                    if (data.schemaVersion >= SpectraShowAsset.CurrentSchemaVersion)
                        throw new InvalidDataException("Show integrity check failed. The file is damaged or was modified outside SpectraOverdrive.");
                    Debug.LogWarning("[SpectraOverdrive 1.5] A legacy show could not be verified with the schema-8 serializer. It will be migrated, validated, and re-signed when exported.");
                }
                data.contentHash = expectedHash;
            }
            if (data.schemaVersion == 1) MigrateV1ToV2(data);
            if (data.schemaVersion == 2) MigrateV2ToV3(data);
            if (data.schemaVersion == 3) MigrateV3ToV4(data);
            if (data.schemaVersion == 4) MigrateV4ToV5(data);
            if (data.schemaVersion == 5) MigrateV5ToV6(data);
            if (data.schemaVersion == 6) MigrateV6ToV7(data);
            if (data.schemaVersion == 7) MigrateV7ToV8(data);
            if (data.schemaVersion != SpectraShowAsset.CurrentSchemaVersion)
                throw new InvalidDataException("Unsupported show schema version " + data.schemaVersion + ".");
            data.ApplyTo(target);
            target.EnsureStableIds();
            SpectraValidationIssue[] issues = target.ValidateShow();
            for (int i = 0; i < issues.Length; i++)
                if (issues[i].isError) throw new InvalidDataException(issues[i].path + ": " + issues[i].message);
        }

        private static void MigrateV1ToV2(SpectraPortableShowData data)
        {
            data.schemaVersion = 2;
            if (data.tracks != null)
                for (int trackIndex = 0; trackIndex < data.tracks.Length; trackIndex++)
                {
                    SpectraTimelineTrack track = data.tracks[trackIndex];
                    if (track == null) continue;
                    if (track.displayColor.a <= 0f)
                        track.displayColor = new Color(0.55f, 0.2f, 0.95f, 1f);
                    if (track.cues == null) continue;
                    for (int cueIndex = 0; cueIndex < track.cues.Length; cueIndex++)
                    {
                        SpectraCueBlock cue = track.cues[cueIndex];
                        if (cue == null) continue;
                        cue.enabled = true;
                        cue.androidFallback = cue.iosFallback;
                        if (Mathf.Abs(cue.movementDirection) < 0.001f) cue.movementDirection = 1f;
                        if (cue.movementAmplitude <= 0f) cue.movementAmplitude = 1f;
                        if (cue.movementSpread <= 0f) cue.movementSpread = 1f;
                    }
                }
            if (data.platformPolicies == null || data.platformPolicies.Length == 0)
                data.platformPolicies = new[]
                {
                    SpectraPlatformPolicy.CreateDefault(SpectraPlatformKind.PC),
                    SpectraPlatformPolicy.CreateDefault(SpectraPlatformKind.Quest),
                    SpectraPlatformPolicy.CreateDefault(SpectraPlatformKind.IOS),
                    SpectraPlatformPolicy.CreateDefault(SpectraPlatformKind.Android)
                };
        }

        private static void MigrateV2ToV3(SpectraPortableShowData data)
        {
            data.schemaVersion = 3;
            data.format = "SpectraOverdrive.Show";
            data.createdWith = "1.0.0";
            if (data.tracks != null)
                for (int trackIndex = 0; trackIndex < data.tracks.Length; trackIndex++)
                {
                    SpectraTimelineTrack track = data.tracks[trackIndex];
                    if (track == null || track.cues == null) continue;
                    for (int cueIndex = 0; cueIndex < track.cues.Length; cueIndex++)
                    {
                        SpectraCueBlock cue = track.cues[cueIndex];
                        if (cue == null) continue;
                        cue.zoom = 0.5f;
                        cue.focus = 0.5f;
                        cue.audioAmount = 0.5f;
                        cue.eventOnce = true;
                    }
                }
            if (data.platformPolicies != null)
                for (int i = 0; i < data.platformPolicies.Length; i++)
                {
                    SpectraPlatformPolicy existing = data.platformPolicies[i];
                    if (existing == null) continue;
                    SpectraPlatformPolicy defaults = SpectraPlatformPolicy.CreateDefault(existing.platform);
                    if (existing.maximumFixtures <= 0) existing.maximumFixtures = defaults.maximumFixtures;
                    if (existing.maximumTransparentBeams <= 0) existing.maximumTransparentBeams = defaults.maximumTransparentBeams;
                    if (existing.audioReactiveUpdateDivider <= 0) existing.audioReactiveUpdateDivider = defaults.audioReactiveUpdateDivider;
                    if (existing.snapshotCapacity < 2) existing.snapshotCapacity = defaults.snapshotCapacity;
                    existing.shaderQualityTier = defaults.shaderQualityTier;
                }
        }

        private static void MigrateV3ToV4(SpectraPortableShowData data)
        {
            data.schemaVersion = 4;
            data.format = "SpectraOverdrive.Show";
            data.createdWith = "1.1.0";
            if (data.fixtureGroups != null)
                for (int groupIndex = 0; groupIndex < data.fixtureGroups.Length; groupIndex++)
                    if (data.fixtureGroups[groupIndex] != null)
                        data.fixtureGroups[groupIndex].capabilities = SpectraFixtureCapability.All;
            if (data.tracks != null)
                for (int trackIndex = 0; trackIndex < data.tracks.Length; trackIndex++)
                {
                    SpectraTimelineTrack track = data.tracks[trackIndex];
                    if (track == null || track.cues == null) continue;
                    for (int cueIndex = 0; cueIndex < track.cues.Length; cueIndex++)
                    {
                        SpectraCueBlock cue = track.cues[cueIndex];
                        if (cue == null) continue;
                        cue.automationMode = SpectraAutomationMode.Disabled;
                        cue.automationKeys = new SpectraAutomationKey[0];
                        cue.capabilityFallback = SpectraCapabilityFallback.EmissiveApproximation;
                    }
                }
        }

        private static void MigrateV4ToV5(SpectraPortableShowData data)
        {
            data.schemaVersion = 5;
            data.format = "SpectraOverdrive.Show";
            data.createdWith = "1.2.0";
            data.performanceMacros = new SpectraPerformanceMacro[0];
            if (data.tracks != null)
                for (int trackIndex = 0; trackIndex < data.tracks.Length; trackIndex++)
                {
                    SpectraTimelineTrack track = data.tracks[trackIndex];
                    if (track == null || track.cues == null) continue;
                    for (int cueIndex = 0; cueIndex < track.cues.Length; cueIndex++)
                    {
                        SpectraCueBlock cue = track.cues[cueIndex];
                        if (cue == null) continue;
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
                }
            if (data.markers != null)
                for (int markerIndex = 0; markerIndex < data.markers.Length; markerIndex++)
                {
                    SpectraTimelineMarker marker = data.markers[markerIndex];
                    if (marker == null) continue;
                    marker.scene = false;
                    marker.sceneBank = 0;
                    marker.sceneOrder = markerIndex;
                    marker.sceneAutoAdvance = false;
                }
        }

        private static void MigrateV5ToV6(SpectraPortableShowData data)
        {
            data.schemaVersion = 6;
            data.format = "SpectraOverdrive.Show";
            data.createdWith = "1.3.0";
            data.colorPalettes = new SpectraColorPalette[0];
            if (data.tracks == null) return;
            for (int trackIndex = 0; trackIndex < data.tracks.Length; trackIndex++)
            {
                SpectraTimelineTrack track = data.tracks[trackIndex];
                if (track == null || track.cues == null) continue;
                for (int cueIndex = 0; cueIndex < track.cues.Length; cueIndex++)
                {
                    SpectraCueBlock cue = track.cues[cueIndex];
                    if (cue == null) continue;
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
            }
        }

        private static void MigrateV6ToV7(SpectraPortableShowData data)
        {
            data.schemaVersion = 7;
            data.format = "SpectraOverdrive.Show";
            data.createdWith = "1.4.0";
            data.performanceMacroSnapshots = new SpectraPerformanceMacroSnapshot[0];
            if (data.tracks == null) return;
            for (int trackIndex = 0; trackIndex < data.tracks.Length; trackIndex++)
            {
                SpectraTimelineTrack track = data.tracks[trackIndex];
                if (track == null || track.cues == null) continue;
                for (int cueIndex = 0; cueIndex < track.cues.Length; cueIndex++)
                {
                    SpectraCueBlock cue = track.cues[cueIndex];
                    if (cue == null) continue;
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

        private static void MigrateV7ToV8(SpectraPortableShowData data)
        {
            data.schemaVersion = 8;
            data.format = "SpectraOverdrive.Show";
            data.createdWith = "1.5.2";
            data.cueLayers = new SpectraCueLayer[0];
            if (data.tracks == null) return;
            for (int trackIndex = 0; trackIndex < data.tracks.Length; trackIndex++)
            {
                SpectraTimelineTrack track = data.tracks[trackIndex];
                if (track == null || track.cues == null) continue;
                for (int cueIndex = 0; cueIndex < track.cues.Length; cueIndex++)
                {
                    SpectraCueBlock cue = track.cues[cueIndex];
                    if (cue == null) continue;
                    cue.layerIndex = -1;
                    cue.arbitrationMode = SpectraCueArbitrationMode.Disabled;
                    cue.arbitrationGroup = -1;
                    cue.arbitrationTimeBase = SpectraModulationTimeBase.Bars;
                    cue.arbitrationCycleLength = 1f;
                    cue.arbitrationPhase = 0f;
                    cue.arbitrationSeed = 0;
                }
            }
        }

        [MenuItem("SpectraOverdrive/Show Programmer/Export Selected Show")]
        private static void ExportSelected()
        {
            SpectraShowAsset show = Selection.activeObject as SpectraShowAsset;
            if (show == null) { EditorUtility.DisplayDialog("SpectraOverdrive", "Select a SpectraShowAsset first.", "OK"); return; }
            string path = EditorUtility.SaveFilePanel("Export SpectraOverdrive show", "", show.showName + ".spectrashow.json", "json");
            if (string.IsNullOrEmpty(path)) return;
            File.WriteAllText(path, Export(show, true));
            EditorUtility.DisplayDialog("SpectraOverdrive", "Show exported and validated.", "OK");
        }

        [MenuItem("SpectraOverdrive/Show Programmer/Import Show")]
        private static void ImportShow()
        {
            string source = EditorUtility.OpenFilePanel("Import SpectraOverdrive show", "", "json");
            if (string.IsNullOrEmpty(source)) return;
            string destination = EditorUtility.SaveFilePanelInProject("Create Show Asset", "Imported Spectra Show", "asset", "Choose where to save the imported show.");
            if (string.IsNullOrEmpty(destination)) return;
            SpectraShowAsset show = ScriptableObject.CreateInstance<SpectraShowAsset>();
            try
            {
                ImportInto(File.ReadAllText(source), show);
                AssetDatabase.CreateAsset(show, destination);
                AssetDatabase.SaveAssets();
                Selection.activeObject = show;
            }
            catch { UnityEngine.Object.DestroyImmediate(show); throw; }
        }
    }
}
