using UdonSharp;
using UnityEngine;

namespace SpectraOverdrive
{
    [UdonBehaviourSyncMode(BehaviourSyncMode.None)]
    public class SpectraAdaptiveBudgetAllocator : UdonSharpBehaviour
    {
        public SpectraPlatformManager platformManager;
        public SpectraLocalQualityController qualityController;

        [Header("Measured frame targets")]
        [Range(15f, 120f)] public float targetFps = 72f;
        [Range(15f, 120f)] public float pcTargetFps = 72f;
        [Range(15f, 120f)] public float questTargetFps = 72f;
        [Range(15f, 120f)] public float iosTargetFps = 60f;
        [Range(15f, 120f)] public float androidTargetFps = 60f;
        [Range(0.1f, 5f)] public float evaluationInterval = 1f;
        [Range(1, 20)] public int badSamplesBeforeDrop = 3;
        [Range(1, 20)] public int goodSamplesBeforeRaise = 8;
        [Range(0.5f, 30f)] public float qualityChangeCooldown = 4f;
        [Range(0.5f, 10f)] public float smoothingSeconds = 2f;
        [Range(0.5f, 0.99f)] public float dropThreshold = 0.82f;
        [Range(0.5f, 1.25f)] public float raiseThreshold = 0.96f;
        public bool adaptiveEnabled = true;

        [Header("Diagnostics")]
        public float measuredFps;
        public int badSamples;
        public int goodSamples;
        [Range(0f, 1f)] public float devicePressure;
        public int qualityChangeCount;
        public float minimumObservedFps = 1000f;
        public float maximumObservedFps;

        private float _nextEvaluation;
        private float _smoothedFrameSeconds;
        private float _lastQualityChangeTime = -100f;

        private void Start()
        {
            if (platformManager == null) return;
            platformManager.DetectAndApply();
            if (platformManager.detectedPlatform == SpectraPlatformKind.Quest) targetFps = questTargetFps;
            else if (platformManager.detectedPlatform == SpectraPlatformKind.IOS) targetFps = iosTargetFps;
            else if (platformManager.detectedPlatform == SpectraPlatformKind.Android) targetFps = androidTargetFps;
            else targetFps = pcTargetFps;
        }

        private void Update()
        {
            float delta = Mathf.Max(0.0001f, Time.unscaledDeltaTime);
            if (_smoothedFrameSeconds <= 0f) _smoothedFrameSeconds = delta;
            float smoothing = 1f - Mathf.Exp(-delta / Mathf.Max(0.1f, smoothingSeconds));
            _smoothedFrameSeconds = Mathf.Lerp(_smoothedFrameSeconds, delta, smoothing);
            measuredFps = 1f / Mathf.Max(0.0001f, _smoothedFrameSeconds);
            minimumObservedFps = Mathf.Min(minimumObservedFps, measuredFps);
            maximumObservedFps = Mathf.Max(maximumObservedFps, measuredFps);
            devicePressure = Mathf.Clamp01(
                (targetFps - measuredFps) / Mathf.Max(1f, targetFps * 0.5f));

            if (!adaptiveEnabled || qualityController == null)
            {
                return;
            }

            if (Time.unscaledTime < _nextEvaluation)
            {
                return;
            }

            _nextEvaluation = Time.unscaledTime + evaluationInterval;

            if (measuredFps < targetFps * dropThreshold)
            {
                badSamples++;
                goodSamples = 0;
            }
            else if (measuredFps > targetFps * raiseThreshold)
            {
                goodSamples++;
                badSamples = 0;
            }
            else
            {
                goodSamples = 0;
                badSamples = 0;
            }

            bool cooldownReady = Time.unscaledTime - _lastQualityChangeTime
                >= qualityChangeCooldown;
            if (cooldownReady && badSamples >= badSamplesBeforeDrop)
            {
                qualityController.qualityLevel = Mathf.Max(
                    qualityController.qualityFloor,
                    qualityController.qualityLevel - 1);
                qualityController.ApplyQuality();
                badSamples = 0;
                goodSamples = 0;
                qualityChangeCount++;
                _lastQualityChangeTime = Time.unscaledTime;
            }

            if (cooldownReady && goodSamples >= goodSamplesBeforeRaise)
            {
                qualityController.qualityLevel = Mathf.Min(
                    qualityController.qualityCeiling,
                    qualityController.qualityLevel + 1);
                qualityController.ApplyQuality();
                goodSamples = 0;
                badSamples = 0;
                qualityChangeCount++;
                _lastQualityChangeTime = Time.unscaledTime;
            }
        }

        public void ResetMeasurements()
        {
            _smoothedFrameSeconds = 0f;
            measuredFps = 0f;
            minimumObservedFps = 1000f;
            maximumObservedFps = 0f;
            devicePressure = 0f;
            badSamples = 0;
            goodSamples = 0;
        }
    }
}
