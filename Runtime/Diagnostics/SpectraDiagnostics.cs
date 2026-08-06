using UdonSharp;
using UnityEngine;
using TMPro;

namespace SpectraOverdrive
{
    [UdonBehaviourSyncMode(BehaviourSyncMode.None)]
    public class SpectraDiagnostics : UdonSharpBehaviour
    {
        public SpectraOverdriveBus bus;
        public SpectraPlatformManager platformManager;
        public SpectraCueController cueController;
        public TMP_Text output;
        public float refreshInterval = 0.5f;

        private float _nextRefresh;

        private void Update()
        {
            if (Time.time < _nextRefresh)
            {
                return;
            }

            _nextRefresh = Time.time + Mathf.Max(0.1f, refreshInterval);
            Refresh();
        }

        public void Refresh()
        {
            if (output == null)
            {
                return;
            }

            string platform = platformManager != null
                ? platformManager.detectedPlatform.ToString()
                : "Unknown";

            string source = bus != null
                ? bus.activeSource.ToString()
                : "None";

            int cue = cueController != null ? cueController.cueId : -1;
            float cueTime = cueController != null ? cueController.normalizedCueTime : 0f;
            int beams = platformManager != null ? platformManager.maxActiveBeams : 0;

            output.text =
                "<b>SpectraOverdrive</b>\n" +
                "Platform: " + platform + "\n" +
                "Source: " + source + "\n" +
                "Cue: " + cue + " @ " + cueTime.ToString("0.000") + "\n" +
                "Beam budget: " + beams + "\n" +
                "Bus publishes: " + (bus != null ? bus.publishCount.ToString() : "0");
        }
    }
}
