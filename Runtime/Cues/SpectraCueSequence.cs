using System;
using UdonSharp;
using UnityEngine;

namespace SpectraOverdrive
{
    [Serializable]
    public class SpectraCueStep
    {
        public int cueId;
        [Range(0.05f, 60f)] public float duration = 1f;
        [Range(0f, 10f)] public float fade = 0.25f;
        public SpectraEffectPattern effectPattern = SpectraEffectPattern.Static;
        [Range(0.05f, 20f)] public float effectSpeed = 1f;
    }

    [UdonBehaviourSyncMode(BehaviourSyncMode.None)]
    public class SpectraCueSequence : UdonSharpBehaviour
    {
        public string sequenceName = "Sequence";
        public SpectraCueStep[] steps;
        public bool loop = true;
    }
}
