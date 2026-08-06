using UdonSharp;
using UnityEngine;
using VRC.SDKBase;

namespace SpectraOverdrive
{
    [UdonBehaviourSyncMode(BehaviourSyncMode.None)]
    public class SpectraAccessibilityController : UdonSharpBehaviour
    {
        public SpectraShowRuntimePlayer showPlayer;
        public SpectraShowNetworkController networkController;

        [Header("Local-only comfort settings")]
        [Range(0f, 1f)] public float localMaster = 1f;
        [Range(0f, 1f)] public float beamIntensity = 1f;
        [Range(0f, 1f)] public float projectionIntensity = 1f;
        [Range(0f, 1f)] public float laserIntensity = 1f;
        [Range(0f, 1f)] public float discoIntensity = 1f;
        public bool photosensitiveMode;
        public bool reducedMotionMode;
        public bool disableStrobes;
        public bool disableLasers;
        public bool reduceFlashes;
        public bool disableRapidColorChanges;
        [Range(0f, 2f)] public float colorTransitionSeconds = 0.35f;
        [Range(0f, 1f)] public float movementLimit = 1f;
        [Range(0f, 20f)] public float maximumStrobeHz = 12f;

        private int _accessibilityId;

        private void Start()
        {
            _accessibilityId = Shader.PropertyToID("_SpectraAccessibility");
            PublishLocalSettings();
        }

        public void PublishLocalSettings()
        {
            float safeBeam = beamIntensity;
            float safeProjection = projectionIntensity;
            float safeLaser = laserIntensity;

            if (photosensitiveMode)
            {
                safeBeam = Mathf.Min(safeBeam, 0.65f);
                safeProjection = Mathf.Min(safeProjection, 0.65f);
                safeLaser = Mathf.Min(safeLaser, 0.45f);
                disableStrobes = true;
                reduceFlashes = true;
                disableRapidColorChanges = true;
            }

            float resolvedMovement = reducedMotionMode ? Mathf.Min(movementLimit, 0.25f) : movementLimit;
            float resolvedStrobe = disableStrobes ? 0f : (reduceFlashes ? Mathf.Min(maximumStrobeHz, 6f) : maximumStrobeHz);
            if (disableLasers) safeLaser = 0f;

            VRCShader.SetGlobalVector(
                _accessibilityId,
                new Vector4(
                    Mathf.Clamp01(localMaster),
                    Mathf.Clamp01(safeBeam),
                    Mathf.Clamp01(safeProjection),
                    Mathf.Clamp01(safeLaser)
                )
            );

            VRCShader.SetGlobalFloat(
                Shader.PropertyToID("_SpectraDisableStrobes"),
                disableStrobes ? 1f : 0f
            );

            VRCShader.SetGlobalFloat(
                Shader.PropertyToID("_SpectraReducedMotion"),
                reducedMotionMode ? 1f : 0f
            );

            VRCShader.SetGlobalFloat(Shader.PropertyToID("_SpectraDisableLasers"), disableLasers ? 1f : 0f);
            VRCShader.SetGlobalFloat(Shader.PropertyToID("_SpectraMovementLimit"), Mathf.Clamp01(resolvedMovement));
            VRCShader.SetGlobalFloat(Shader.PropertyToID("_SpectraStrobeFrequencyLimit"), Mathf.Max(0f, resolvedStrobe));

            SpectraShowRuntimePlayer targetPlayer = networkController == null
                ? showPlayer : networkController.GetActivePlayer();
            if (targetPlayer != null)
            {
                targetPlayer.localBrightnessLimit = Mathf.Clamp01(localMaster);
                targetPlayer.localStrobesEnabled = !disableStrobes;
                targetPlayer.localLasersEnabled = !disableLasers;
                targetPlayer.localMovementLimit = Mathf.Clamp01(resolvedMovement);
                targetPlayer.localStrobeFrequencyLimit = Mathf.Max(0f, resolvedStrobe);
                targetPlayer.localRapidColorChangesEnabled = !disableRapidColorChanges;
                targetPlayer.localColorTransitionSeconds = disableRapidColorChanges
                    ? Mathf.Max(0.35f, colorTransitionSeconds) : colorTransitionSeconds;
                targetPlayer.ApplyAtTime(targetPlayer.showTime);
            }
        }
    }
}
