using UdonSharp;
using UnityEngine;
using VRC.SDKBase;

namespace SpectraOverdrive
{
    [UdonBehaviourSyncMode(BehaviourSyncMode.None)]
    public class SpectraLaserSafetyController : UdonSharpBehaviour
    {
        public SpectraLaserRibbon[] lasers;

        [Header("Local safety")]
        public bool lasersEnabled = true;
        public bool audienceScanAllowed;
        [Range(0f, 1f)] public float maximumLocalIntensity = 0.5f;
        [Range(0f, 4f)] public float maximumScanSpeed = 2f;
        [Range(0f, 1f)] public float minimumVerticalBias = 0.15f;

        public void ApplySafety()
        {
            if (lasers == null) return;

            for (int i = 0; i < lasers.Length; i++)
            {
                SpectraLaserRibbon laser = lasers[i];
                if (laser == null) continue;

                laser.laserPower = lasersEnabled
                    ? Mathf.Min(laser.laserPower, maximumLocalIntensity)
                    : 0f;

                laser.scanSpeed = Mathf.Min(laser.scanSpeed, maximumScanSpeed);
                laser.Publish();
            }

            VRCShader.SetGlobalVector(
                Shader.PropertyToID("_SpectraLaserSafety"),
                new Vector4(
                    lasersEnabled ? 1f : 0f,
                    audienceScanAllowed ? 1f : 0f,
                    maximumLocalIntensity,
                    minimumVerticalBias
                )
            );
        }
    }
}
