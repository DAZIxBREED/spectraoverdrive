using System;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

namespace SpectraOverdrive.Editor
{
    [Serializable]
    public class SpectraAssistedGenerationSettings
    {
        [Range(64, 2048)] public int analysisBuckets = 512;
        [Range(1.05f, 3f)] public float transientThreshold = 1.45f;
        [Range(1, 32)] public int phraseBars = 8;
        [Range(8, 256)] public int maximumImpactCues = 96;
        public bool replacePreviouslyGeneratedTracks = true;
        public int deterministicSeed = 174;
        public Color primaryColor = new Color(0.55f, 0.08f, 1f, 1f);
        public Color secondaryColor = new Color(0f, 0.85f, 1f, 1f);
        public Color impactColor = Color.white;
    }

    public sealed class SpectraAssistedGenerationReport
    {
        public int energyBuckets;
        public int phraseCues;
        public int impactCues;
        public int generatedTracks;
        public float peakEnergy;
    }

    public static class SpectraAssistedShowGenerator
    {
        private const string GeneratedPrefix = "[Generated] ";

        public static SpectraAssistedGenerationReport Generate(
            SpectraShowAsset show,
            AudioClip clip,
            SpectraAssistedGenerationSettings settings)
        {
            if (show == null) throw new ArgumentNullException("show");
            if (clip == null) throw new ArgumentNullException("clip");
            if (settings == null) throw new ArgumentNullException("settings");
            if (show.beatGrid == null || show.beatGrid.bpm <= 0f)
                throw new InvalidOperationException("The show needs a valid beat grid before assisted generation.");
            if (show.fixtureGroups == null || show.fixtureGroups.Length == 0 || show.fixtureGroups[0] == null)
                throw new InvalidOperationException("The show needs at least one fixture group.");

            show.EnsureStableIds();
            float[] energy;
            float[] brightness;
            AnalyzeClip(clip, settings.analysisBuckets, out energy, out brightness);
            string groupId = show.fixtureGroups[0].id;
            List<SpectraTimelineTrack> tracks = new List<SpectraTimelineTrack>();
            if (show.tracks != null)
                for (int i = 0; i < show.tracks.Length; i++)
                    if (show.tracks[i] != null
                        && (!settings.replacePreviouslyGeneratedTracks
                            || !show.tracks[i].name.StartsWith(GeneratedPrefix, StringComparison.Ordinal)))
                        tracks.Add(show.tracks[i]);

            List<SpectraCueBlock> intensityCues = new List<SpectraCueBlock>();
            List<SpectraCueBlock> colorCues = new List<SpectraCueBlock>();
            List<SpectraCueBlock> movementCues = new List<SpectraCueBlock>();
            List<SpectraCueBlock> impactCues = new List<SpectraCueBlock>();
            List<SpectraTimelineMarker> generatedMarkers = new List<SpectraTimelineMarker>();

            int phraseBars = Mathf.Max(1, settings.phraseBars);
            double phraseStart = 0d;
            int phraseIndex = 0;
            while (phraseStart < show.durationSeconds - 0.001d)
            {
                SpectraMusicalPosition position = new SpectraMusicalPosition(1 + phraseIndex * phraseBars, 1, 0f);
                float start = (float)show.beatGrid.MusicalToSeconds(position);
                if (start >= show.durationSeconds) break;
                float end = (float)show.beatGrid.MusicalToSeconds(
                    new SpectraMusicalPosition(1 + (phraseIndex + 1) * phraseBars, 1, 0f));
                end = Mathf.Clamp(end, start + 0.01f, show.durationSeconds);
                float average = AverageEnergy(energy, start, end, show.durationSeconds);
                float high = AverageEnergy(brightness, start, end, show.durationSeconds);
                Color phraseColor = Color.Lerp(settings.primaryColor, settings.secondaryColor,
                    Mathf.Repeat(phraseIndex * 0.6180339f + high * 0.35f, 1f));

                intensityCues.Add(new SpectraCueBlock
                {
                    name = "Phrase Energy " + (phraseIndex + 1),
                    startSeconds = start,
                    durationSeconds = end - start,
                    fadeIn = Mathf.Min(0.35f, (end - start) * 0.2f),
                    fadeOut = Mathf.Min(0.35f, (end - start) * 0.2f),
                    valueType = SpectraCueValueType.Intensity,
                    intensity = Mathf.Lerp(0.35f, 1f, average),
                    blendMode = SpectraCueBlendMode.Replace,
                    priority = 10,
                    questFallback = SpectraPlatformFallback.Full,
                    iosFallback = SpectraPlatformFallback.Full,
                    androidFallback = SpectraPlatformFallback.Full,
                    accessibilitySafe = true
                });
                colorCues.Add(new SpectraCueBlock
                {
                    name = "Phrase Color " + (phraseIndex + 1),
                    startSeconds = start,
                    durationSeconds = end - start,
                    fadeIn = Mathf.Min(0.5f, (end - start) * 0.25f),
                    fadeOut = Mathf.Min(0.5f, (end - start) * 0.25f),
                    valueType = SpectraCueValueType.Color,
                    color = phraseColor,
                    blendMode = SpectraCueBlendMode.Replace,
                    priority = 12,
                    questFallback = SpectraPlatformFallback.Full,
                    iosFallback = SpectraPlatformFallback.Full,
                    androidFallback = SpectraPlatformFallback.Full,
                    accessibilitySafe = true
                });
                movementCues.Add(new SpectraCueBlock
                {
                    name = "Phrase Movement " + (phraseIndex + 1),
                    startSeconds = start,
                    durationSeconds = end - start,
                    fadeIn = Mathf.Min(0.5f, (end - start) * 0.25f),
                    fadeOut = Mathf.Min(0.5f, (end - start) * 0.25f),
                    valueType = SpectraCueValueType.Movement,
                    movementPattern = SelectMovementPattern(phraseIndex, settings.deterministicSeed),
                    movementSpeed = Mathf.Lerp(0.35f, 2.4f, average),
                    movementAmplitude = Mathf.Lerp(0.25f, 1f, average),
                    movementSpread = Mathf.Lerp(0.4f, 1.4f, high),
                    movementDirection = phraseIndex % 2 == 0 ? 1f : -1f,
                    movementSmoothing = 0.7f,
                    randomSeed = settings.deterministicSeed + phraseIndex * 7919,
                    blendMode = SpectraCueBlendMode.Replace,
                    priority = 8,
                    questFallback = SpectraPlatformFallback.Simplified,
                    iosFallback = SpectraPlatformFallback.EmissiveOnly,
                    androidFallback = SpectraPlatformFallback.EmissiveOnly
                });
                generatedMarkers.Add(new SpectraTimelineMarker
                {
                    name = ClassifySection(phraseIndex, average),
                    kind = ClassifyMarkerKind(phraseIndex, average),
                    timeSeconds = start,
                    timingMode = SpectraTimingMode.Seconds,
                    color = phraseColor
                });
                phraseIndex++;
                phraseStart = end;
            }

            float previousEnergy = energy.Length > 0 ? energy[0] : 0f;
            int impactCount = 0;
            float minimumSpacing = (float)Math.Max(0.05d,
                show.beatGrid.BeatToSeconds(0.5d) - show.beatGrid.BeatToSeconds(0d));
            float lastImpact = -minimumSpacing;
            for (int i = 1; i < energy.Length && impactCount < settings.maximumImpactCues; i++)
            {
                float ratio = energy[i] / Mathf.Max(0.025f, previousEnergy);
                float time = i / (float)Mathf.Max(1, energy.Length - 1) * show.durationSeconds;
                previousEnergy = Mathf.Lerp(previousEnergy, energy[i], 0.2f);
                if (ratio < settings.transientThreshold || time - lastImpact < minimumSpacing) continue;
                float snapped = SpectraTimelineEditing.SnapTime(show, time, SpectraTimelineSnap.QuarterBeat, 60f);
                impactCues.Add(new SpectraCueBlock
                {
                    name = "Detected Impact " + (impactCount + 1),
                    startSeconds = snapped,
                    durationSeconds = Mathf.Max(0.05f, minimumSpacing * 0.5f),
                    fadeOut = Mathf.Max(0.04f, minimumSpacing * 0.45f),
                    valueType = SpectraCueValueType.Color,
                    color = settings.impactColor,
                    blendMode = SpectraCueBlendMode.Add,
                    priority = 100,
                    questFallback = SpectraPlatformFallback.Simplified,
                    iosFallback = SpectraPlatformFallback.EmissiveOnly,
                    androidFallback = SpectraPlatformFallback.EmissiveOnly,
                    accessibilitySafe = true
                });
                impactCount++;
                lastImpact = snapped;
            }

            tracks.Add(CreateTrack(GeneratedPrefix + "Phrase Intensity", SpectraTrackType.Intensity, groupId, intensityCues, new Color(1f, 0.55f, 0.2f, 1f)));
            tracks.Add(CreateTrack(GeneratedPrefix + "Phrase Colors", SpectraTrackType.Color, groupId, colorCues, settings.primaryColor));
            tracks.Add(CreateTrack(GeneratedPrefix + "Musical Movement", SpectraTrackType.Movement, groupId, movementCues, settings.secondaryColor));
            tracks.Add(CreateTrack(GeneratedPrefix + "Detected Impacts", SpectraTrackType.Color, groupId, impactCues, Color.white));
            tracks.Add(CreateTrack(GeneratedPrefix + "Audio Energy", SpectraTrackType.AudioReactive, groupId,
                new List<SpectraCueBlock>
                {
                    new SpectraCueBlock
                    {
                        name = "Bass Energy Modulation",
                        startSeconds = 0f,
                        durationSeconds = show.durationSeconds,
                        valueType = SpectraCueValueType.AudioReactiveIntensity,
                        intensity = 1f,
                        audioBand = SpectraAudioBand.Bass,
                        audioFloor = 0.3f,
                        audioAmount = 0.7f,
                        blendMode = SpectraCueBlendMode.Multiply,
                        priority = 20,
                        questFallback = SpectraPlatformFallback.Full,
                        iosFallback = SpectraPlatformFallback.Full,
                        androidFallback = SpectraPlatformFallback.Full,
                        accessibilitySafe = true
                    }
                }, new Color(1f, 0.1f, 0.5f, 1f)));

            show.tracks = tracks.ToArray();
            MergeGeneratedMarkers(show, generatedMarkers, settings.replacePreviouslyGeneratedTracks);
            show.durationSeconds = Mathf.Max(show.durationSeconds, clip.length);
            show.audioReference = clip.name;
            show.EnsureStableIds();

            float peak = 0f;
            for (int i = 0; i < energy.Length; i++) peak = Mathf.Max(peak, energy[i]);
            return new SpectraAssistedGenerationReport
            {
                energyBuckets = energy.Length,
                phraseCues = phraseIndex,
                impactCues = impactCount,
                generatedTracks = 5,
                peakEnergy = peak
            };
        }

        private static void AnalyzeClip(AudioClip clip, int requestedBuckets, out float[] energy, out float[] brightness)
        {
            int bucketCount = Mathf.Clamp(requestedBuckets, 64, 2048);
            float[] samples = new float[clip.samples * clip.channels];
            if (!clip.GetData(samples, 0))
                throw new InvalidOperationException("AudioClip.GetData failed. Set the authoring clip to Decompress On Load.");
            energy = new float[bucketCount];
            brightness = new float[bucketCount];
            int framesPerBucket = Mathf.Max(1, clip.samples / bucketCount);
            float peak = 0f;
            for (int bucket = 0; bucket < bucketCount; bucket++)
            {
                int firstFrame = bucket * framesPerBucket;
                int lastFrame = Mathf.Min(clip.samples, firstFrame + framesPerBucket);
                double square = 0d;
                double difference = 0d;
                int count = 0;
                float previous = 0f;
                for (int frame = firstFrame; frame < lastFrame; frame++)
                {
                    float mono = 0f;
                    for (int channel = 0; channel < clip.channels; channel++)
                        mono += samples[frame * clip.channels + channel];
                    mono /= Mathf.Max(1, clip.channels);
                    square += mono * mono;
                    if (count > 0) difference += Mathf.Abs(mono - previous);
                    previous = mono;
                    count++;
                }
                energy[bucket] = count == 0 ? 0f : Mathf.Sqrt((float)(square / count));
                brightness[bucket] = count <= 1 ? 0f : (float)(difference / (count - 1));
                peak = Mathf.Max(peak, energy[bucket]);
            }
            float brightnessPeak = 0f;
            for (int i = 0; i < brightness.Length; i++) brightnessPeak = Mathf.Max(brightnessPeak, brightness[i]);
            for (int i = 0; i < energy.Length; i++)
            {
                energy[i] = peak <= 0.000001f ? 0f : Mathf.Clamp01(energy[i] / peak);
                brightness[i] = brightnessPeak <= 0.000001f ? 0f : Mathf.Clamp01(brightness[i] / brightnessPeak);
            }
        }

        private static SpectraTimelineTrack CreateTrack(string name, SpectraTrackType type, string groupId, List<SpectraCueBlock> cues, Color color)
        {
            return new SpectraTimelineTrack
            {
                name = name,
                trackType = type,
                fixtureGroupId = groupId,
                displayColor = color,
                cues = cues.ToArray()
            };
        }

        private static void MergeGeneratedMarkers(SpectraShowAsset show, List<SpectraTimelineMarker> generated, bool replace)
        {
            List<SpectraTimelineMarker> merged = new List<SpectraTimelineMarker>();
            if (show.markers != null)
                for (int i = 0; i < show.markers.Length; i++)
                    if (show.markers[i] != null
                        && (!replace || !show.markers[i].name.StartsWith(GeneratedPrefix, StringComparison.Ordinal)))
                        merged.Add(show.markers[i]);
            for (int i = 0; i < generated.Count; i++)
            {
                generated[i].name = GeneratedPrefix + generated[i].name;
                merged.Add(generated[i]);
            }
            show.markers = merged.ToArray();
        }

        private static float AverageEnergy(float[] values, float start, float end, float duration)
        {
            if (values == null || values.Length == 0 || duration <= 0f) return 0f;
            int first = Mathf.Clamp(Mathf.FloorToInt(start / duration * values.Length), 0, values.Length - 1);
            int last = Mathf.Clamp(Mathf.CeilToInt(end / duration * values.Length), first + 1, values.Length);
            float sum = 0f;
            for (int i = first; i < last; i++) sum += values[i];
            return sum / Mathf.Max(1, last - first);
        }

        private static SpectraMovementPatternKind SelectMovementPattern(int phrase, int seed)
        {
            SpectraMovementPatternKind[] patterns =
            {
                SpectraMovementPatternKind.Fan,
                SpectraMovementPatternKind.HorizontalSweep,
                SpectraMovementPatternKind.Circle,
                SpectraMovementPatternKind.AlternatingWave,
                SpectraMovementPatternKind.Mirrored,
                SpectraMovementPatternKind.FollowTheLeader
            };
            int index = (seed + phrase * 1103515245) & 0x7fffffff;
            index %= patterns.Length;
            return patterns[index];
        }

        private static string ClassifySection(int phrase, float energy)
        {
            if (phrase == 0) return "Intro";
            if (energy > 0.72f) return "Drop";
            if (energy < 0.3f) return "Breakdown";
            return phrase % 2 == 0 ? "Build" : "Section";
        }

        private static SpectraMarkerKind ClassifyMarkerKind(int phrase, float energy)
        {
            if (phrase == 0) return SpectraMarkerKind.Intro;
            if (energy > 0.72f) return SpectraMarkerKind.Drop;
            if (energy < 0.3f) return SpectraMarkerKind.Breakdown;
            return SpectraMarkerKind.Build;
        }
    }

    public class SpectraAssistedShowGeneratorWindow : EditorWindow
    {
        private SpectraShowAsset _show;
        private AudioClip _clip;
        private SpectraAssistedGenerationSettings _settings = new SpectraAssistedGenerationSettings();

        [MenuItem("SpectraOverdrive/Show Programmer/Assisted Starter Show")]
        private static void Open()
        {
            GetWindow<SpectraAssistedShowGeneratorWindow>("Spectra Starter Show");
        }

        private void OnGUI()
        {
            EditorGUILayout.LabelField("Assisted Starter Show", EditorStyles.boldLabel);
            EditorGUILayout.HelpBox(
                "Analyzes real waveform energy and transients, then creates editable phrase, color, movement, impact, marker, and audio-reactive tracks.",
                MessageType.Info);
            _show = (SpectraShowAsset)EditorGUILayout.ObjectField("Show", _show, typeof(SpectraShowAsset), false);
            _clip = (AudioClip)EditorGUILayout.ObjectField("Audio Clip", _clip, typeof(AudioClip), false);
            _settings.analysisBuckets = EditorGUILayout.IntSlider("Analysis Buckets", _settings.analysisBuckets, 64, 2048);
            _settings.transientThreshold = EditorGUILayout.Slider("Transient Threshold", _settings.transientThreshold, 1.05f, 3f);
            _settings.phraseBars = EditorGUILayout.IntSlider("Phrase Bars", _settings.phraseBars, 1, 32);
            _settings.maximumImpactCues = EditorGUILayout.IntSlider("Maximum Impacts", _settings.maximumImpactCues, 8, 256);
            _settings.deterministicSeed = EditorGUILayout.IntField("Deterministic Seed", _settings.deterministicSeed);
            _settings.replacePreviouslyGeneratedTracks = EditorGUILayout.Toggle("Replace Generated Tracks", _settings.replacePreviouslyGeneratedTracks);
            _settings.primaryColor = EditorGUILayout.ColorField("Primary Color", _settings.primaryColor);
            _settings.secondaryColor = EditorGUILayout.ColorField("Secondary Color", _settings.secondaryColor);
            _settings.impactColor = EditorGUILayout.ColorField("Impact Color", _settings.impactColor);
            EditorGUI.BeginDisabledGroup(_show == null || _clip == null);
            if (GUILayout.Button("Analyze and Generate Editable Show", GUILayout.Height(34f)))
            {
                Undo.RecordObject(_show, "Generate SpectraOverdrive starter show");
                SpectraAssistedGenerationReport report = SpectraAssistedShowGenerator.Generate(_show, _clip, _settings);
                EditorUtility.SetDirty(_show);
                AssetDatabase.SaveAssets();
                EditorUtility.DisplayDialog("SpectraOverdrive",
                    "Generated " + report.generatedTracks + " tracks, " + report.phraseCues
                    + " phrase cues, and " + report.impactCues + " detected impacts.", "OK");
            }
            EditorGUI.EndDisabledGroup();
        }
    }
}
