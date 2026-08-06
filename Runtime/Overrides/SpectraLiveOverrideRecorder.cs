using UdonSharp;
using UnityEngine;

namespace SpectraOverdrive
{
    [UdonBehaviourSyncMode(BehaviourSyncMode.None)]
    public class SpectraLiveOverrideRecorder : UdonSharpBehaviour
    {
        public SpectraShowRuntimePlayer player;
        [Range(32, 2048)] public int capacity = 512;
        public bool recording;
        public int actionCount;
        public bool capacityReached;
        public float recordingStartedAtShowTime;
        public int[] actionTypes = new int[0];
        public int[] groupIndices = new int[0];
        public int[] overrideModes = new int[0];
        public float[] showTimes = new float[0];
        public Vector4[] primaryValues = new Vector4[0];
        public Vector4[] secondaryValues = new Vector4[0];
        public Vector4[] tertiaryValues = new Vector4[0];
        public Color[] colors = new Color[0];

        public void BeginRecording()
        {
            EnsureCapacity();
            actionCount = 0;
            capacityReached = false;
            recordingStartedAtShowTime = player == null ? 0f : player.showTime;
            recording = true;
        }

        public void StopRecording()
        {
            recording = false;
        }

        public void ClearRecording()
        {
            recording = false;
            actionCount = 0;
            capacityReached = false;
        }

        public void RecordCurrentOverride(int group, SpectraLiveOverrideLayer layer)
        {
            if (!recording || layer == null || !CanAppend()) return;
            int index = actionCount++;
            actionTypes[index] = (int)SpectraRecordingAction.Intensity;
            groupIndices[index] = group;
            overrideModes[index] = layer.modes[group];
            showTimes[index] = ResolveTime();
            primaryValues[index] = new Vector4(
                layer.intensities[group],
                layer.pans[group],
                layer.tilts[group],
                layer.movements[group]);
            secondaryValues[index] = new Vector4(
                layer.gobos[group],
                layer.prisms[group],
                layer.zooms[group],
                layer.focuses[group]);
            tertiaryValues[index] = new Vector4(
                layer.strobeRates[group],
                layer.lasers[group] ? 1f : 0f,
                layer.goboRotations[group],
                0f);
            colors[index] = new Color(
                layer.colorR[group],
                layer.colorG[group],
                layer.colorB[group],
                layer.colorA[group]);
        }

        public void RecordClearGroup(int group)
        {
            AppendSimple(SpectraRecordingAction.ClearGroup, group);
        }

        public void RecordClearAll()
        {
            AppendSimple(SpectraRecordingAction.ClearAll, -1);
        }

        private void AppendSimple(SpectraRecordingAction action, int group)
        {
            if (!recording || !CanAppend()) return;
            int index = actionCount++;
            actionTypes[index] = (int)action;
            groupIndices[index] = group;
            showTimes[index] = ResolveTime();
            primaryValues[index] = Vector4.zero;
            secondaryValues[index] = Vector4.zero;
            tertiaryValues[index] = Vector4.zero;
            colors[index] = Color.white;
        }

        private bool CanAppend()
        {
            EnsureCapacity();
            if (actionCount < actionTypes.Length) return true;
            capacityReached = true;
            return false;
        }

        private float ResolveTime()
        {
            return player == null ? 0f : player.showTime;
        }

        private void EnsureCapacity()
        {
            int resolved = Mathf.Clamp(capacity, 32, 2048);
            if (actionTypes != null && actionTypes.Length == resolved
                && groupIndices != null && groupIndices.Length == resolved
                && overrideModes != null && overrideModes.Length == resolved
                && showTimes != null && showTimes.Length == resolved
                && primaryValues != null && primaryValues.Length == resolved
                && secondaryValues != null && secondaryValues.Length == resolved
                && tertiaryValues != null && tertiaryValues.Length == resolved
                && colors != null && colors.Length == resolved) return;
            capacity = resolved;
            actionTypes = new int[capacity];
            groupIndices = new int[capacity];
            overrideModes = new int[capacity];
            showTimes = new float[capacity];
            primaryValues = new Vector4[capacity];
            secondaryValues = new Vector4[capacity];
            tertiaryValues = new Vector4[capacity];
            colors = new Color[capacity];
            actionCount = 0;
        }
    }
}
