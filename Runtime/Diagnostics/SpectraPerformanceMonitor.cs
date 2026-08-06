using UdonSharp;
using UnityEngine;
using TMPro;

namespace SpectraOverdrive
{
    [UdonBehaviourSyncMode(BehaviourSyncMode.None)]
    public class SpectraPerformanceMonitor : UdonSharpBehaviour
    {
        public TMP_Text output;
        public SpectraPlatformManager platform;
        public SpectraLocalQualityController quality;
        public SpectraAdaptiveBudgetAllocator allocator;

        [Header("Counters")]
        public int fixtureCount;
        public int activeBeamCount;
        public int activeLaserCount;
        public int activeProjectionCount;

        [Header("Refresh")]
        [Range(0.1f, 5f)] public float refreshInterval = 0.5f;
        private float _nextRefresh;

        private void Update()
        {
            if (Time.time < _nextRefresh) return;
            _nextRefresh = Time.time + refreshInterval;
            Refresh();
        }

        public void Refresh()
        {
            fixtureCount = FindObjectsOfType<SpectraFixtureIdentity>().Length;
            activeBeamCount = CountActive(SpectraFixtureType.MovingBeam) + CountActive(SpectraFixtureType.MovingSpot);
            activeLaserCount = CountActive(SpectraFixtureType.Laser);
            activeProjectionCount = CountActive(SpectraFixtureType.MovingWash);

            if (output == null) return;

            string fps = allocator != null ? allocator.measuredFps.ToString("0.0") : "n/a";
            string platformName = platform != null ? platform.detectedPlatform.ToString() : "Unknown";
            string qualityLevel = quality != null ? quality.qualityLevel.ToString() : "n/a";
            string pressure = allocator != null ? (allocator.devicePressure * 100f).ToString("0") + "%" : "n/a";

            output.text =
                "<b>SpectraOverdrive Performance</b>\n" +
                "Platform: " + platformName + "\n" +
                "FPS: " + fps + "\n" +
                "Device pressure: " + pressure + "\n" +
                "Quality: " + qualityLevel + "\n" +
                "Fixtures: " + fixtureCount + "\n" +
                "Beams: " + activeBeamCount + "\n" +
                "Lasers: " + activeLaserCount + "\n" +
                "Projections: " + activeProjectionCount;
        }

        private int CountActive(SpectraFixtureType type)
        {
            SpectraFixtureIdentity[] fixtures = FindObjectsOfType<SpectraFixtureIdentity>();
            int count = 0;

            for (int i = 0; i < fixtures.Length; i++)
            {
                if (fixtures[i] != null &&
                    fixtures[i].fixtureType == type &&
                    fixtures[i].gameObject.activeInHierarchy)
                {
                    count++;
                }
            }

            return count;
        }
    }
}
