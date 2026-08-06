using System;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

namespace SpectraOverdrive.Editor
{
    public static class SpectraOverrideRecordingImporter
    {
        private const string RecordedPrefix = "[Recorded] ";

        public static int ConvertToTimeline(
            SpectraLiveOverrideRecorder recording,
            SpectraShowAsset show,
            bool replacePrevious)
        {
            if (recording == null) throw new ArgumentNullException("recording");
            if (show == null) throw new ArgumentNullException("show");
            if (show.fixtureGroups == null || show.fixtureGroups.Length == 0)
                throw new InvalidOperationException("The target show has no fixture groups.");
            if (recording.actionCount <= 0) return 0;

            List<SpectraTimelineTrack> tracks = new List<SpectraTimelineTrack>();
            if (show.tracks != null)
                for (int i = 0; i < show.tracks.Length; i++)
                    if (show.tracks[i] != null
                        && (!replacePrevious
                            || !show.tracks[i].name.StartsWith(RecordedPrefix, StringComparison.Ordinal)))
                        tracks.Add(show.tracks[i]);

            int createdCues = 0;
            for (int groupIndex = 0; groupIndex < show.fixtureGroups.Length; groupIndex++)
            {
                SpectraShowFixtureGroup group = show.fixtureGroups[groupIndex];
                if (group == null) continue;
                List<SpectraCueBlock> intensity = new List<SpectraCueBlock>();
                List<SpectraCueBlock> color = new List<SpectraCueBlock>();
                List<SpectraCueBlock> movement = new List<SpectraCueBlock>();
                List<SpectraCueBlock> optics = new List<SpectraCueBlock>();
                List<SpectraCueBlock> effects = new List<SpectraCueBlock>();
                for (int action = 0; action < recording.actionCount; action++)
                {
                    if (recording.groupIndices[action] != groupIndex) continue;
                    SpectraRecordingAction kind = (SpectraRecordingAction)recording.actionTypes[action];
                    if (kind != SpectraRecordingAction.Intensity) continue;
                    float start = Mathf.Clamp(recording.showTimes[action], 0f, show.durationSeconds);
                    float end = FindActionEnd(recording, action, groupIndex, show.durationSeconds);
                    float duration = Mathf.Max(0.05f, end - start);
                    SpectraCueBlendMode blend = ConvertBlend((SpectraOverrideMode)recording.overrideModes[action]);
                    Vector4 primary = recording.primaryValues[action];
                    Vector4 secondary = recording.secondaryValues[action];
                    Vector4 tertiary = recording.tertiaryValues != null
                        && action < recording.tertiaryValues.Length
                        ? recording.tertiaryValues[action] : Vector4.zero;

                    intensity.Add(CreateCue("Recorded Intensity", start, duration,
                        SpectraCueValueType.Intensity, blend, 500));
                    intensity[intensity.Count - 1].intensity = Mathf.Max(0f, primary.x);
                    color.Add(CreateCue("Recorded Color", start, duration,
                        SpectraCueValueType.Color, blend, 501));
                    color[color.Count - 1].color = recording.colors[action];
                    SpectraCueBlock motion = CreateCue("Recorded Movement", start, duration,
                        SpectraCueValueType.Movement, blend, 502);
                    motion.pan = primary.y;
                    motion.tilt = primary.z;
                    motion.movementSpeed = Mathf.Max(0f, primary.w);
                    motion.movementPattern = SpectraMovementPatternKind.Static;
                    movement.Add(motion);

                    if (secondary.x >= 0f)
                    {
                        SpectraCueBlock gobo = CreateCue("Recorded Gobo", start, duration,
                            SpectraCueValueType.Gobo, blend, 503);
                        gobo.goboIndex = Mathf.RoundToInt(secondary.x);
                        gobo.goboRotation = tertiary.z;
                        optics.Add(gobo);
                    }
                    if (secondary.y > 0f)
                    {
                        SpectraCueBlock prism = CreateCue("Recorded Prism", start, duration,
                            SpectraCueValueType.Prism, blend, 504);
                        prism.prismAmount = secondary.y;
                        optics.Add(prism);
                    }
                    if (secondary.z >= 0f || secondary.w >= 0f)
                    {
                        SpectraCueBlock zoom = CreateCue("Recorded Zoom Focus", start, duration,
                            SpectraCueValueType.ZoomFocus, blend, 505);
                        zoom.zoom = secondary.z < 0f ? 0.5f : secondary.z;
                        zoom.focus = secondary.w < 0f ? 0.5f : secondary.w;
                        optics.Add(zoom);
                    }
                    if (tertiary.x > 0f)
                    {
                        SpectraCueBlock strobe = CreateCue("Recorded Strobe", start, duration,
                            SpectraCueValueType.Strobe, SpectraCueBlendMode.Maximum, 506);
                        strobe.strobeHz = tertiary.x;
                        strobe.iosFallback = SpectraPlatformFallback.Disabled;
                        strobe.androidFallback = SpectraPlatformFallback.Disabled;
                        effects.Add(strobe);
                    }
                    if (tertiary.y > 0.5f)
                    {
                        SpectraCueBlock laser = CreateCue("Recorded Laser", start, duration,
                            SpectraCueValueType.LaserEnable, SpectraCueBlendMode.PriorityOverride, 507);
                        laser.boolValue = true;
                        effects.Add(laser);
                    }
                }
                createdCues += AddTrackIfPopulated(tracks, RecordedPrefix + group.name + " Intensity",
                    SpectraTrackType.Intensity, group.id, intensity, new Color(1f, 0.55f, 0.2f, 1f));
                createdCues += AddTrackIfPopulated(tracks, RecordedPrefix + group.name + " Color",
                    SpectraTrackType.Color, group.id, color, new Color(0.8f, 0.15f, 1f, 1f));
                createdCues += AddTrackIfPopulated(tracks, RecordedPrefix + group.name + " Movement",
                    SpectraTrackType.Movement, group.id, movement, new Color(0.1f, 0.85f, 1f, 1f));
                createdCues += AddTrackIfPopulated(tracks, RecordedPrefix + group.name + " Optics",
                    SpectraTrackType.Gobo, group.id, optics, new Color(1f, 0.2f, 0.55f, 1f));
                createdCues += AddTrackIfPopulated(tracks, RecordedPrefix + group.name + " Effects",
                    SpectraTrackType.Strobe, group.id, effects, new Color(1f, 0.15f, 0.2f, 1f));
            }
            show.tracks = tracks.ToArray();
            show.EnsureStableIds();
            return createdCues;
        }

        private static float FindActionEnd(
            SpectraLiveOverrideRecorder recording,
            int actionIndex,
            int groupIndex,
            float showDuration)
        {
            float end = showDuration;
            for (int i = actionIndex + 1; i < recording.actionCount; i++)
            {
                SpectraRecordingAction kind = (SpectraRecordingAction)recording.actionTypes[i];
                if (kind == SpectraRecordingAction.ClearAll
                    || recording.groupIndices[i] == groupIndex)
                {
                    end = Mathf.Min(end, recording.showTimes[i]);
                    break;
                }
            }
            return end;
        }

        private static SpectraCueBlock CreateCue(
            string name,
            float start,
            float duration,
            SpectraCueValueType type,
            SpectraCueBlendMode blend,
            int priority)
        {
            return new SpectraCueBlock
            {
                name = name,
                startSeconds = start,
                durationSeconds = duration,
                valueType = type,
                blendMode = blend,
                priority = priority,
                questFallback = SpectraPlatformFallback.Simplified,
                iosFallback = type == SpectraCueValueType.Color || type == SpectraCueValueType.Intensity
                    ? SpectraPlatformFallback.Full : SpectraPlatformFallback.EmissiveOnly,
                androidFallback = type == SpectraCueValueType.Color || type == SpectraCueValueType.Intensity
                    ? SpectraPlatformFallback.Full : SpectraPlatformFallback.EmissiveOnly
            };
        }

        private static SpectraCueBlendMode ConvertBlend(SpectraOverrideMode mode)
        {
            if (mode == SpectraOverrideMode.Add) return SpectraCueBlendMode.Add;
            if (mode == SpectraOverrideMode.Multiply) return SpectraCueBlendMode.Multiply;
            return SpectraCueBlendMode.Replace;
        }

        private static int AddTrackIfPopulated(
            List<SpectraTimelineTrack> tracks,
            string name,
            SpectraTrackType type,
            string groupId,
            List<SpectraCueBlock> cues,
            Color color)
        {
            if (cues.Count == 0) return 0;
            tracks.Add(new SpectraTimelineTrack
            {
                name = name,
                trackType = type,
                fixtureGroupId = groupId,
                displayColor = color,
                cues = cues.ToArray()
            });
            return cues.Count;
        }
    }

    public class SpectraOverrideRecordingImporterWindow : EditorWindow
    {
        private SpectraLiveOverrideRecorder _recording;
        private SpectraShowAsset _show;
        private bool _replacePrevious = true;

        [MenuItem("SpectraOverdrive/Show Programmer/Convert Live Recording")]
        private static void Open()
        {
            GetWindow<SpectraOverrideRecordingImporterWindow>("Spectra Recording");
        }

        private void OnGUI()
        {
            EditorGUILayout.LabelField("Live Override Recording", EditorStyles.boldLabel);
            EditorGUILayout.HelpBox(
                "Converts captured operator moves into normal editable cue blocks. Existing hand-authored tracks are preserved.",
                MessageType.Info);
            _recording = (SpectraLiveOverrideRecorder)EditorGUILayout.ObjectField(
                "Recorder", _recording, typeof(SpectraLiveOverrideRecorder), true);
            _show = (SpectraShowAsset)EditorGUILayout.ObjectField(
                "Target Show", _show, typeof(SpectraShowAsset), false);
            _replacePrevious = EditorGUILayout.Toggle("Replace Recorded Tracks", _replacePrevious);
            if (_recording != null)
                EditorGUILayout.LabelField("Captured Actions", _recording.actionCount.ToString());
            EditorGUI.BeginDisabledGroup(_recording == null || _show == null || _recording.actionCount <= 0);
            if (GUILayout.Button("Convert to Editable Timeline Cues", GUILayout.Height(34f)))
            {
                Undo.RecordObject(_show, "Convert SpectraOverdrive recording");
                int cues = SpectraOverrideRecordingImporter.ConvertToTimeline(_recording, _show, _replacePrevious);
                EditorUtility.SetDirty(_show);
                AssetDatabase.SaveAssets();
                EditorUtility.DisplayDialog("SpectraOverdrive",
                    "Converted the performance into " + cues + " editable cue blocks.", "OK");
            }
            EditorGUI.EndDisabledGroup();
        }
    }
}
