using UdonSharp;
using UnityEngine;
using VRC.SDKBase;

namespace SpectraOverdrive
{
    [UdonBehaviourSyncMode(BehaviourSyncMode.None)]
    public class SpectraPlatformManager : UdonSharpBehaviour
    {
        [Header("Detected target")]
        public SpectraPlatformKind detectedPlatform = SpectraPlatformKind.Unknown;

        [Header("Quest defaults")]
        public int questMaxActiveBeams = 20;
        public int questMaxLaserSegments = 40;
        public int questMaxDiscoPoints = 256;
        public int questMaxProjectionReceivers = 20;

        [Header("iOS defaults")]
        public int iosMaxActiveBeams = 12;
        public int iosMaxLaserSegments = 24;
        public int iosMaxDiscoPoints = 128;
        public int iosMaxProjectionReceivers = 12;

        [Header("Android phone/tablet defaults")]
        public int androidMaxActiveBeams = 10;
        public int androidMaxLaserSegments = 20;
        public int androidMaxDiscoPoints = 96;
        public int androidMaxProjectionReceivers = 10;

        [Header("PC defaults")]
        public int pcMaxActiveBeams = 64;
        public int pcMaxLaserSegments = 256;
        public int pcMaxDiscoPoints = 2048;
        public int pcMaxProjectionReceivers = 64;

        [Header("Resolved local budget")]
        public int maxActiveBeams;
        public int maxLaserSegments;
        public int maxDiscoPoints;
        public int maxProjectionReceivers;

        private void Start()
        {
            DetectAndApply();
        }

        public void DetectAndApply()
        {
#if UNITY_ANDROID
            detectedPlatform = Networking.LocalPlayer != null && Networking.LocalPlayer.IsUserInVR()
                ? SpectraPlatformKind.Quest : SpectraPlatformKind.Android;
#elif UNITY_IOS
            detectedPlatform = SpectraPlatformKind.IOS;
#else
            detectedPlatform = SpectraPlatformKind.PC;
#endif

            if (detectedPlatform == SpectraPlatformKind.Quest)
            {
                ApplyBudget(questMaxActiveBeams, questMaxLaserSegments, questMaxDiscoPoints, questMaxProjectionReceivers);
                return;
            }

            if (detectedPlatform == SpectraPlatformKind.IOS)
            {
                ApplyBudget(iosMaxActiveBeams, iosMaxLaserSegments, iosMaxDiscoPoints, iosMaxProjectionReceivers);
                return;
            }

            if (detectedPlatform == SpectraPlatformKind.Android)
            {
                ApplyBudget(androidMaxActiveBeams, androidMaxLaserSegments, androidMaxDiscoPoints, androidMaxProjectionReceivers);
                return;
            }

            ApplyBudget(pcMaxActiveBeams, pcMaxLaserSegments, pcMaxDiscoPoints, pcMaxProjectionReceivers);
        }

        private void ApplyBudget(int beams, int lasers, int disco, int receivers)
        {
            maxActiveBeams = Mathf.Max(0, beams);
            maxLaserSegments = Mathf.Max(0, lasers);
            maxDiscoPoints = Mathf.Max(0, disco);
            maxProjectionReceivers = Mathf.Max(0, receivers);
        }
    }
}
