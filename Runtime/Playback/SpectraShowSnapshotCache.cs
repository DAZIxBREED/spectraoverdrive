using UdonSharp;
using UnityEngine;

namespace SpectraOverdrive
{
    [UdonBehaviourSyncMode(BehaviourSyncMode.None)]
    public class SpectraShowSnapshotCache : UdonSharpBehaviour
    {
        [Range(2, 32)] public int capacity = 8;
        [Range(0.25f, 30f)] public float captureInterval = 4f;
        public int snapshotCount;
        public int writeIndex;
        public float[] snapshotTimes = new float[0];
        public Color[] groupColors = new Color[0];
        public float[] groupIntensities = new float[0];
        public float[] groupPans = new float[0];
        public float[] groupTilts = new float[0];
        public float[] groupMovements = new float[0];
        public float[] groupGobos = new float[0];
        public float[] groupPrisms = new float[0];
        public float[] groupZooms = new float[0];
        public float[] groupFocuses = new float[0];
        public float[] groupStrobes = new float[0];
        public bool[] groupLasers = new bool[0];
        public int[] groupAudioBands = new int[0];
        public float[] groupAudioAmounts = new float[0];
        public float[] groupAudioFloors = new float[0];

        private float _nextCaptureTime = -1f;
        private int _groupCount;

        public void TryCapture(float showTime, SpectraFixtureGroup[] groups)
        {
            if (groups == null || showTime + 0.001f < _nextCaptureTime) return;
            EnsureCapacity(groups.Length);
            int slot = writeIndex;
            snapshotTimes[slot] = showTime;
            int baseIndex = slot * _groupCount;
            for (int i = 0; i < _groupCount; i++)
            {
                SpectraFixtureGroup group = groups[i];
                if (group == null) continue;
                int index = baseIndex + i;
                groupColors[index] = group.colorMultiplier;
                groupIntensities[index] = group.intensityMultiplier;
                groupPans[index] = group.panBias;
                groupTilts[index] = group.tiltBias;
                groupMovements[index] = group.movementScale;
                groupGobos[index] = group.goboIndex;
                groupPrisms[index] = group.prismAmount;
                groupZooms[index] = group.zoom;
                groupFocuses[index] = group.focus;
                groupStrobes[index] = group.strobeHz;
                groupLasers[index] = group.laserEnabled;
                groupAudioBands[index] = group.audioReactiveBand;
                groupAudioAmounts[index] = group.audioReactiveAmount;
                groupAudioFloors[index] = group.audioReactiveFloor;
            }
            writeIndex = (writeIndex + 1) % capacity;
            snapshotCount = Mathf.Min(snapshotCount + 1, capacity);
            _nextCaptureTime = showTime + captureInterval;
        }

        public bool RestoreNearestBefore(float showTime, SpectraFixtureGroup[] groups)
        {
            if (groups == null || snapshotCount <= 0 || groups.Length != _groupCount) return false;
            int bestSlot = -1;
            float bestTime = -1f;
            for (int i = 0; i < snapshotCount; i++)
            {
                if (snapshotTimes[i] <= showTime && snapshotTimes[i] >= bestTime)
                {
                    bestTime = snapshotTimes[i];
                    bestSlot = i;
                }
            }
            if (bestSlot < 0) return false;
            int baseIndex = bestSlot * _groupCount;
            for (int i = 0; i < _groupCount; i++)
            {
                SpectraFixtureGroup group = groups[i];
                if (group == null) continue;
                int index = baseIndex + i;
                group.colorMultiplier = groupColors[index];
                group.intensityMultiplier = groupIntensities[index];
                group.panBias = groupPans[index];
                group.tiltBias = groupTilts[index];
                group.movementScale = groupMovements[index];
                group.goboIndex = groupGobos[index];
                group.prismAmount = groupPrisms[index];
                group.zoom = groupZooms[index];
                group.focus = groupFocuses[index];
                group.strobeHz = groupStrobes[index];
                group.laserEnabled = groupLasers[index];
                group.audioReactiveBand = groupAudioBands[index];
                group.audioReactiveAmount = groupAudioAmounts[index];
                group.audioReactiveFloor = groupAudioFloors[index];
                group.ApplyToFixtures();
            }
            return true;
        }

        public void Clear()
        {
            snapshotCount = 0;
            writeIndex = 0;
            _nextCaptureTime = -1f;
        }

        private void EnsureCapacity(int groupCount)
        {
            int resolvedCapacity = Mathf.Clamp(capacity, 2, 32);
            if (_groupCount == groupCount && snapshotTimes != null && snapshotTimes.Length == resolvedCapacity) return;
            capacity = resolvedCapacity;
            _groupCount = Mathf.Max(0, groupCount);
            int flattened = capacity * _groupCount;
            snapshotTimes = new float[capacity];
            groupColors = new Color[flattened];
            groupIntensities = new float[flattened];
            groupPans = new float[flattened];
            groupTilts = new float[flattened];
            groupMovements = new float[flattened];
            groupGobos = new float[flattened];
            groupPrisms = new float[flattened];
            groupZooms = new float[flattened];
            groupFocuses = new float[flattened];
            groupStrobes = new float[flattened];
            groupLasers = new bool[flattened];
            groupAudioBands = new int[flattened];
            groupAudioAmounts = new float[flattened];
            groupAudioFloors = new float[flattened];
            Clear();
        }
    }
}
