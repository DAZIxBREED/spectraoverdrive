using UdonSharp;
using UnityEngine;

namespace SpectraOverdrive
{
    [UdonBehaviourSyncMode(BehaviourSyncMode.None)]
    public class SpectraCuePreset : UdonSharpBehaviour
    {
        public int cueId;
        public string cueName = "Cue";
        public Color primaryColor = Color.white;
        public Color secondaryColor = Color.black;
        [Range(0f, 2f)] public float masterIntensity = 1f;
        [Range(0f, 20f)] public float duration = 2f;
        [Range(0f, 20f)] public float fadeTime = 0.5f;
        [Range(0f, 20f)] public float movementSpeed = 1f;
        [Range(0f, 20f)] public float strobeRate;
        public bool loop;
    }
}
