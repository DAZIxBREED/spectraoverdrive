using UdonSharp;
using UnityEngine;

namespace SpectraOverdrive
{
    [UdonBehaviourSyncMode(BehaviourSyncMode.None)]
    public class SpectraAudioReactiveProfile : UdonSharpBehaviour
    {
        public SpectraAudioBand band = SpectraAudioBand.Bass;
        [Range(0f, 4f)] public float gain = 1f;
        [Range(0f, 1f)] public float floor = 0.05f;
        [Range(0f, 1f)] public float ceiling = 1f;
        [Range(0f, 2f)] public float smoothing = 0.2f;
        public bool driveIntensity = true;
        public bool driveColor;
        public bool driveMovement;
        public bool driveStrobe;
    }
}
