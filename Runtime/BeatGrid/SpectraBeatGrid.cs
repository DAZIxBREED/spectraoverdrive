using System;
using UnityEngine;

namespace SpectraOverdrive
{
    [Serializable]
    public class SpectraTempoMarker
    {
        [Min(0f)] public float timeSeconds;
        [Min(1f)] public float bpm = 120f;
        [Min(1)] public int numerator = 4;
        [Min(1)] public int denominator = 4;
    }

    [Serializable]
    public class SpectraBeatGrid
    {
        [Min(1f)] public float bpm = 120f;
        [Min(1)] public int beatsPerBar = 4;
        [Min(0f)] public float firstDownbeatSeconds;
        public SpectraTempoMarker[] tempoChanges = new SpectraTempoMarker[0];

        public double MusicalToSeconds(SpectraMusicalPosition position)
        {
            double targetBeat = (Math.Max(1, position.bar) - 1) * Math.Max(1, beatsPerBar)
                + (Math.Max(1, position.beat) - 1) + Mathf.Clamp01(position.beatFraction);
            return BeatToSeconds(targetBeat);
        }

        public double BeatToSeconds(double targetBeat)
        {
            if (targetBeat <= 0d) return firstDownbeatSeconds;
            double seconds = firstDownbeatSeconds;
            double consumedBeats = 0d;
            float currentBpm = Mathf.Max(1f, bpm);
            SpectraTempoMarker[] sorted = GetSortedTempoMarkers();

            for (int i = 0; i < sorted.Length; i++)
            {
                SpectraTempoMarker marker = sorted[i];
                if (marker == null || marker.timeSeconds <= seconds) continue;
                double segmentBeats = (marker.timeSeconds - seconds) * currentBpm / 60d;
                if (consumedBeats + segmentBeats >= targetBeat)
                    return seconds + (targetBeat - consumedBeats) * 60d / currentBpm;
                consumedBeats += segmentBeats;
                seconds = marker.timeSeconds;
                currentBpm = Mathf.Max(1f, marker.bpm);
            }
            return seconds + (targetBeat - consumedBeats) * 60d / currentBpm;
        }

        public double SecondsToBeat(double timeSeconds)
        {
            if (timeSeconds <= firstDownbeatSeconds) return 0d;
            double seconds = firstDownbeatSeconds;
            double beats = 0d;
            float currentBpm = Mathf.Max(1f, bpm);
            SpectraTempoMarker[] sorted = GetSortedTempoMarkers();

            for (int i = 0; i < sorted.Length; i++)
            {
                SpectraTempoMarker marker = sorted[i];
                if (marker == null || marker.timeSeconds <= seconds) continue;
                if (marker.timeSeconds >= timeSeconds) break;
                beats += (marker.timeSeconds - seconds) * currentBpm / 60d;
                seconds = marker.timeSeconds;
                currentBpm = Mathf.Max(1f, marker.bpm);
            }
            beats += (timeSeconds - seconds) * currentBpm / 60d;
            return Math.Max(0d, beats);
        }

        public SpectraMusicalPosition SecondsToMusical(double seconds)
        {
            double absoluteBeat = SecondsToBeat(seconds);
            int wholeBeat = (int)Math.Floor(absoluteBeat);
            return new SpectraMusicalPosition(
                wholeBeat / Math.Max(1, beatsPerBar) + 1,
                wholeBeat % Math.Max(1, beatsPerBar) + 1,
                (float)(absoluteBeat - wholeBeat));
        }

        private SpectraTempoMarker[] GetSortedTempoMarkers()
        {
            if (tempoChanges == null || tempoChanges.Length == 0) return new SpectraTempoMarker[0];
            SpectraTempoMarker[] copy = (SpectraTempoMarker[])tempoChanges.Clone();
            Array.Sort(copy, delegate(SpectraTempoMarker a, SpectraTempoMarker b)
            {
                if (a == null) return 1;
                if (b == null) return -1;
                return a.timeSeconds.CompareTo(b.timeSeconds);
            });
            return copy;
        }
    }
}
