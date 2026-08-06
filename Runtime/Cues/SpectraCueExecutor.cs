using UdonSharp;
using UnityEngine;
using VRC.SDKBase;

namespace SpectraOverdrive
{
    [UdonBehaviourSyncMode(BehaviourSyncMode.None)]
    public class SpectraCueExecutor : UdonSharpBehaviour
    {
        public SpectraCueController controller;
        public SpectraOverdriveBus bus;
        public SpectraCuePreset[] presets;

        [Header("Resolved output")]
        public SpectraCuePreset activePreset;
        public Color currentPrimary = Color.black;
        public Color currentSecondary = Color.black;
        public float currentIntensity;

        private int _cueColorsId;
        private int _cueMotionId;

        private void Start()
        {
            _cueColorsId = Shader.PropertyToID("_SpectraCueColors");
            _cueMotionId = Shader.PropertyToID("_SpectraCueMotion");
        }

        private void Update()
        {
            RefreshNow();
        }

        public void RefreshNow()
        {
            ResolveActivePreset();
            ApplyPreset();
        }

        private void ResolveActivePreset()
        {
            activePreset = null;
            if (controller == null || presets == null) return;

            for (int i = 0; i < presets.Length; i++)
            {
                SpectraCuePreset preset = presets[i];
                if (preset != null && preset.cueId == controller.cueId)
                {
                    activePreset = preset;
                    return;
                }
            }
        }

        private void ApplyPreset()
        {
            if (activePreset == null)
            {
                currentIntensity = 0f;
                return;
            }

            float t = controller != null ? controller.normalizedCueTime : 0f;
            float fadeDuration = Mathf.Max(0.0001f, activePreset.fadeTime);
            float elapsed = t * Mathf.Max(0.0001f, activePreset.duration);
            float fade = Mathf.Clamp01(elapsed / fadeDuration);

            currentPrimary = Color.Lerp(Color.black, activePreset.primaryColor, fade);
            currentSecondary = Color.Lerp(Color.black, activePreset.secondaryColor, fade);
            currentIntensity = Mathf.Lerp(0f, activePreset.masterIntensity, fade);

            if (bus != null && bus.activeSource == SpectraControlSource.InternalCue)
            {
                bus.masterIntensity = currentIntensity;
            }

            VRCShader.SetGlobalVector(
                _cueColorsId,
                new Vector4(currentPrimary.r, currentPrimary.g, currentPrimary.b, currentIntensity)
            );

            VRCShader.SetGlobalVector(
                _cueMotionId,
                new Vector4(activePreset.movementSpeed, activePreset.strobeRate, t, activePreset.loop ? 1f : 0f)
            );
        }
    }
}
