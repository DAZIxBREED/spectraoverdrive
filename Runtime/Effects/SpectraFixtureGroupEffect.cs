using UdonSharp;
using UnityEngine;
using VRC.SDKBase;

namespace SpectraOverdrive
{
    [UdonBehaviourSyncMode(BehaviourSyncMode.None)]
    public class SpectraFixtureGroupEffect : UdonSharpBehaviour
    {
        public SpectraFixtureGroup group;
        public SpectraEffectPattern pattern = SpectraEffectPattern.ChaseForward;
        [Range(0.05f, 20f)] public float speed = 2f;
        [Range(0f, 1f)] public float width = 0.25f;
        [Range(0f, 2f)] public float peakIntensity = 1f;
        [Range(0f, 2f)] public float baseIntensity = 0.1f;
        [Range(0f, 1f)] public float movementAmount = 0.25f;
        public Color primaryColor = Color.white;
        public Color secondaryColor = Color.black;
        public bool useServerTime = true;

        public void Update()
        {
            ApplyEffect();
        }

        public void ApplyEffect()
        {
            if (group == null || group.fixtures == null || group.fixtures.Length == 0)
            {
                return;
            }

            float now = useServerTime
                ? (float)Networking.GetServerTimeInSeconds()
                : Time.time;

            int count = group.fixtures.Length;

            for (int i = 0; i < count; i++)
            {
                SpectraFixtureRuntime fixture = group.fixtures[i];
                if (fixture == null) continue;

                float normalizedIndex = count <= 1 ? 0f : (float)i / (float)(count - 1);
                float phase = Mathf.Repeat(now * speed, 1f);
                float value = EvaluatePattern(normalizedIndex, phase, i);

                fixture.groupIntensityMultiplier = Mathf.Lerp(baseIntensity, peakIntensity, value);
                fixture.groupColorMultiplier = Color.Lerp(secondaryColor, primaryColor, value);
                fixture.groupPanBias = (value - 0.5f) * movementAmount;
                fixture.groupTiltBias = Mathf.Sin((normalizedIndex + phase) * Mathf.PI * 2f) * movementAmount;
                fixture.PublishFixtureProperties();
            }
        }

        private float EvaluatePattern(float index, float phase, int integerIndex)
        {
            if (pattern == SpectraEffectPattern.Static) return 1f;

            if (pattern == SpectraEffectPattern.ChaseForward)
            {
                return PulseDistance(index, phase);
            }

            if (pattern == SpectraEffectPattern.ChaseReverse)
            {
                return PulseDistance(index, 1f - phase);
            }

            if (pattern == SpectraEffectPattern.PingPong)
            {
                float pingPong = Mathf.PingPong(phase * 2f, 1f);
                return PulseDistance(index, pingPong);
            }

            if (pattern == SpectraEffectPattern.Alternate)
            {
                int step = Mathf.FloorToInt(phase * 2f);
                return ((integerIndex + step) & 1) == 0 ? 1f : 0f;
            }

            if (pattern == SpectraEffectPattern.Random)
            {
                float seed = Mathf.Sin((integerIndex + 1) * 12.9898f + Mathf.Floor(phase * 8f) * 78.233f) * 43758.5453f;
                return Mathf.Repeat(seed, 1f);
            }

            if (pattern == SpectraEffectPattern.Wave)
            {
                return Mathf.Sin((index + phase) * Mathf.PI * 2f) * 0.5f + 0.5f;
            }

            if (pattern == SpectraEffectPattern.Pulse)
            {
                return Mathf.Sin(phase * Mathf.PI * 2f) * 0.5f + 0.5f;
            }

            return 1f;
        }

        private float PulseDistance(float index, float phase)
        {
            float distance = Mathf.Abs(index - phase);
            distance = Mathf.Min(distance, 1f - distance);
            float safeWidth = Mathf.Max(0.001f, width);
            return Mathf.Clamp01(1f - distance / safeWidth);
        }
    }
}
