using System;
using UnityEngine;

namespace SpectraOverdrive.Editor
{
    public static class SpectraTimelineEditing
    {
        public static float SnapTime(SpectraShowAsset show, float seconds, SpectraTimelineSnap snap, float frameRate)
        {
            seconds = Mathf.Max(0f, seconds);
            if (snap == SpectraTimelineSnap.Off) return seconds;
            if (snap == SpectraTimelineSnap.Frame)
                return Mathf.Round(seconds * Mathf.Max(1f, frameRate)) / Mathf.Max(1f, frameRate);
            if (snap == SpectraTimelineSnap.TenthSecond) return Mathf.Round(seconds * 10f) / 10f;
            if (snap == SpectraTimelineSnap.QuarterSecond) return Mathf.Round(seconds * 4f) / 4f;
            if (show == null || show.beatGrid == null) return seconds;

            double beat = show.beatGrid.SecondsToBeat(seconds);
            double unit = 1d;
            if (snap == SpectraTimelineSnap.HalfBeat) unit = 0.5d;
            else if (snap == SpectraTimelineSnap.QuarterBeat) unit = 0.25d;
            else if (snap == SpectraTimelineSnap.EighthBeat) unit = 0.125d;
            else if (snap == SpectraTimelineSnap.Bar) unit = Math.Max(1, show.beatGrid.beatsPerBar);
            else if (snap == SpectraTimelineSnap.TwoBars) unit = Math.Max(1, show.beatGrid.beatsPerBar) * 2d;
            else if (snap == SpectraTimelineSnap.FourBars) unit = Math.Max(1, show.beatGrid.beatsPerBar) * 4d;
            else if (snap == SpectraTimelineSnap.EightBars) unit = Math.Max(1, show.beatGrid.beatsPerBar) * 8d;
            else if (snap == SpectraTimelineSnap.SixteenBars) unit = Math.Max(1, show.beatGrid.beatsPerBar) * 16d;
            return (float)show.beatGrid.BeatToSeconds(Math.Round(beat / unit) * unit);
        }

        public static SpectraCueBlock CreateCue(SpectraShowAsset show, int trackIndex, float startSeconds, float durationSeconds)
        {
            SpectraTimelineTrack track = GetTrack(show, trackIndex);
            SpectraCueBlock cue = new SpectraCueBlock
            {
                id = Guid.NewGuid().ToString("N"),
                name = track.trackType + " Cue",
                enabled = true,
                timingMode = SpectraTimingMode.Seconds,
                startSeconds = Mathf.Max(0f, startSeconds),
                durationSeconds = Mathf.Max(0.01f, durationSeconds),
                valueType = DefaultValueType(track.trackType),
                blendMode = SpectraCueBlendMode.Replace,
                easing = SpectraCueEasing.SmoothStep,
                movementDirection = 1f,
                movementAmplitude = 1f,
                movementSpread = 1f,
                questFallback = SpectraPlatformFallback.Simplified,
                iosFallback = SpectraPlatformFallback.EmissiveOnly,
                androidFallback = SpectraPlatformFallback.EmissiveOnly
            };
            track.cues = Append(track.cues, cue);
            return cue;
        }

        public static SpectraTimelineTrack AddTrack(SpectraShowAsset show, SpectraTrackType type, string fixtureGroupId)
        {
            if (show == null) throw new ArgumentNullException("show");
            SpectraTimelineTrack track = new SpectraTimelineTrack
            {
                id = Guid.NewGuid().ToString("N"),
                name = type + " Track",
                trackType = type,
                fixtureGroupId = fixtureGroupId,
                displayColor = TrackColor(type),
                cues = new SpectraCueBlock[0]
            };
            show.tracks = Append(show.tracks, track);
            return track;
        }

        public static void DeleteTrack(SpectraShowAsset show, int trackIndex)
        {
            if (show == null || show.tracks == null || trackIndex < 0 || trackIndex >= show.tracks.Length) return;
            show.tracks = RemoveAt(show.tracks, trackIndex);
        }

        public static void MoveTrack(SpectraShowAsset show, int fromIndex, int toIndex)
        {
            if (show == null || show.tracks == null || fromIndex < 0 || fromIndex >= show.tracks.Length) return;
            toIndex = Mathf.Clamp(toIndex, 0, show.tracks.Length - 1);
            if (fromIndex == toIndex) return;
            SpectraTimelineTrack item = show.tracks[fromIndex];
            if (fromIndex < toIndex)
                for (int i = fromIndex; i < toIndex; i++) show.tracks[i] = show.tracks[i + 1];
            else
                for (int i = fromIndex; i > toIndex; i--) show.tracks[i] = show.tracks[i - 1];
            show.tracks[toIndex] = item;
        }

        public static void SetCueStart(SpectraShowAsset show, SpectraCueBlock cue, float seconds)
        {
            if (cue == null) return;
            seconds = Mathf.Max(0f, seconds);
            if (cue.timingMode == SpectraTimingMode.Musical && show != null && show.beatGrid != null)
                cue.startMusical = show.beatGrid.SecondsToMusical(seconds);
            else
                cue.startSeconds = seconds;
        }

        public static void SetCueDuration(SpectraShowAsset show, SpectraCueBlock cue, float durationSeconds)
        {
            if (cue == null) return;
            durationSeconds = Mathf.Max(0.01f, durationSeconds);
            if (cue.timingMode == SpectraTimingMode.Musical && show != null && show.beatGrid != null)
            {
                double startBeat = show.beatGrid.SecondsToBeat(cue.ResolveStartSeconds(show.beatGrid));
                double endBeat = show.beatGrid.SecondsToBeat(cue.ResolveStartSeconds(show.beatGrid) + durationSeconds);
                cue.durationBeats = Mathf.Max(0.001f, (float)(endBeat - startBeat));
            }
            else
                cue.durationSeconds = durationSeconds;
            cue.fadeIn = Mathf.Min(cue.fadeIn, durationSeconds);
            cue.fadeOut = Mathf.Min(cue.fadeOut, durationSeconds);
        }

        public static SpectraCueBlock DuplicateCue(SpectraShowAsset show, int trackIndex, int cueIndex, float offsetSeconds)
        {
            SpectraTimelineTrack track = GetTrack(show, trackIndex);
            SpectraCueBlock source = GetCue(track, cueIndex);
            SpectraCueBlock duplicate = CloneCue(source);
            duplicate.id = Guid.NewGuid().ToString("N");
            duplicate.name = source.name + " Copy";
            SetCueStart(show, duplicate, source.ResolveStartSeconds(show.beatGrid) + offsetSeconds);
            track.cues = Append(track.cues, duplicate);
            return duplicate;
        }

        public static void DeleteCue(SpectraShowAsset show, int trackIndex, int cueIndex)
        {
            SpectraTimelineTrack track = GetTrack(show, trackIndex);
            if (track.cues == null || cueIndex < 0 || cueIndex >= track.cues.Length) return;
            track.cues = RemoveAt(track.cues, cueIndex);
        }

        public static SpectraCueBlock SplitCue(SpectraShowAsset show, int trackIndex, int cueIndex, float splitSeconds)
        {
            SpectraTimelineTrack track = GetTrack(show, trackIndex);
            SpectraCueBlock source = GetCue(track, cueIndex);
            float start = source.ResolveStartSeconds(show.beatGrid);
            float end = start + source.ResolveDurationSeconds(show.beatGrid);
            if (splitSeconds <= start + 0.001f || splitSeconds >= end - 0.001f)
                throw new ArgumentOutOfRangeException("splitSeconds", "Split must be inside the cue.");

            SpectraCueBlock right = CloneCue(source);
            right.id = Guid.NewGuid().ToString("N");
            right.name = source.name + " B";
            source.name = source.name + " A";
            SetCueDuration(show, source, splitSeconds - start);
            SetCueStart(show, right, splitSeconds);
            SetCueDuration(show, right, end - splitSeconds);
            track.cues = InsertAfter(track.cues, cueIndex, right);
            return right;
        }

        public static string CopyCueToJson(SpectraCueBlock cue)
        {
            if (cue == null) return string.Empty;
            return JsonUtility.ToJson(cue);
        }

        public static SpectraCueBlock PasteCueFromJson(SpectraShowAsset show, int trackIndex, string json, float startSeconds)
        {
            if (string.IsNullOrWhiteSpace(json)) throw new ArgumentException("Cue clipboard is empty.", "json");
            SpectraCueBlock cue = JsonUtility.FromJson<SpectraCueBlock>(json);
            if (cue == null) throw new ArgumentException("Cue clipboard is invalid.", "json");
            cue.id = Guid.NewGuid().ToString("N");
            SetCueStart(show, cue, startSeconds);
            SpectraTimelineTrack track = GetTrack(show, trackIndex);
            track.cues = Append(track.cues, cue);
            return cue;
        }

        public static SpectraTimelineMarker AddMarker(SpectraShowAsset show, float seconds, string name, SpectraMarkerKind kind)
        {
            if (show == null) throw new ArgumentNullException("show");
            SpectraTimelineMarker marker = new SpectraTimelineMarker
            {
                id = Guid.NewGuid().ToString("N"),
                name = string.IsNullOrWhiteSpace(name) ? kind.ToString() : name,
                kind = kind,
                timingMode = SpectraTimingMode.Seconds,
                timeSeconds = Mathf.Max(0f, seconds),
                color = MarkerColor(kind)
            };
            show.markers = Append(show.markers, marker);
            return marker;
        }

        public static SpectraLoopRegion AddLoop(SpectraShowAsset show, float startSeconds, float endSeconds, string name)
        {
            if (show == null) throw new ArgumentNullException("show");
            SpectraLoopRegion loop = new SpectraLoopRegion
            {
                id = Guid.NewGuid().ToString("N"),
                name = string.IsNullOrWhiteSpace(name) ? "Loop" : name,
                startSeconds = Mathf.Max(0f, Mathf.Min(startSeconds, endSeconds)),
                endSeconds = Mathf.Max(startSeconds + 0.01f, endSeconds),
                enabled = true,
                repeatCount = 0,
                quantizeExitToBar = true
            };
            show.loopRegions = Append(show.loopRegions, loop);
            return loop;
        }

        public static SpectraCueBlock CloneCue(SpectraCueBlock cue)
        {
            if (cue == null) throw new ArgumentNullException("cue");
            return JsonUtility.FromJson<SpectraCueBlock>(JsonUtility.ToJson(cue));
        }

        private static SpectraTimelineTrack GetTrack(SpectraShowAsset show, int trackIndex)
        {
            if (show == null || show.tracks == null || trackIndex < 0 || trackIndex >= show.tracks.Length)
                throw new ArgumentOutOfRangeException("trackIndex");
            SpectraTimelineTrack track = show.tracks[trackIndex];
            if (track == null) throw new InvalidOperationException("Track is null.");
            return track;
        }

        private static SpectraCueBlock GetCue(SpectraTimelineTrack track, int cueIndex)
        {
            if (track.cues == null || cueIndex < 0 || cueIndex >= track.cues.Length || track.cues[cueIndex] == null)
                throw new ArgumentOutOfRangeException("cueIndex");
            return track.cues[cueIndex];
        }

        private static SpectraCueValueType DefaultValueType(SpectraTrackType type)
        {
            if (type == SpectraTrackType.Color) return SpectraCueValueType.Color;
            if (type == SpectraTrackType.Movement) return SpectraCueValueType.Movement;
            if (type == SpectraTrackType.Strobe) return SpectraCueValueType.Strobe;
            if (type == SpectraTrackType.Laser) return SpectraCueValueType.LaserEnable;
            if (type == SpectraTrackType.Global) return SpectraCueValueType.Blackout;
            return SpectraCueValueType.Intensity;
        }

        private static Color TrackColor(SpectraTrackType type)
        {
            if (type == SpectraTrackType.Color) return new Color(0.95f, 0.22f, 0.75f, 1f);
            if (type == SpectraTrackType.Movement) return new Color(0.18f, 0.75f, 1f, 1f);
            if (type == SpectraTrackType.Strobe) return new Color(1f, 0.9f, 0.35f, 1f);
            if (type == SpectraTrackType.Laser) return new Color(0.2f, 1f, 0.55f, 1f);
            if (type == SpectraTrackType.Global) return new Color(1f, 0.3f, 0.3f, 1f);
            return new Color(0.55f, 0.2f, 0.95f, 1f);
        }

        private static Color MarkerColor(SpectraMarkerKind kind)
        {
            if (kind == SpectraMarkerKind.Drop) return new Color(1f, 0.25f, 0.2f, 1f);
            if (kind == SpectraMarkerKind.Build || kind == SpectraMarkerKind.PreDrop) return new Color(1f, 0.65f, 0.1f, 1f);
            if (kind == SpectraMarkerKind.Breakdown) return new Color(0.2f, 0.7f, 1f, 1f);
            if (kind == SpectraMarkerKind.Recovery) return new Color(1f, 0.15f, 0.65f, 1f);
            return new Color(0.75f, 0.35f, 1f, 1f);
        }

        private static T[] Append<T>(T[] source, T item)
        {
            int length = source == null ? 0 : source.Length;
            T[] result = new T[length + 1];
            if (length > 0) Array.Copy(source, result, length);
            result[length] = item;
            return result;
        }

        private static T[] RemoveAt<T>(T[] source, int index)
        {
            T[] result = new T[source.Length - 1];
            if (index > 0) Array.Copy(source, 0, result, 0, index);
            if (index < source.Length - 1) Array.Copy(source, index + 1, result, index, source.Length - index - 1);
            return result;
        }

        private static T[] InsertAfter<T>(T[] source, int index, T item)
        {
            T[] result = new T[source.Length + 1];
            Array.Copy(source, 0, result, 0, index + 1);
            result[index + 1] = item;
            Array.Copy(source, index + 1, result, index + 2, source.Length - index - 1);
            return result;
        }
    }
}
