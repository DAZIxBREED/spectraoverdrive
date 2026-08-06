using System;
using UnityEngine;

namespace SpectraOverdrive
{
    [Serializable]
    public class SpectraShowEntry
    {
        public string showName = "Show";
        public SpectraCueSequence sequence;
        public Color houseColor = Color.white;
        [Range(0f, 2f)] public float masterIntensity = 1f;
        public bool enableLasers = true;
        public bool enableStrobes = true;
        public string notes;
    }

    [CreateAssetMenu(
        fileName = "SpectraShowBank",
        menuName = "SpectraOverdrive/Show Bank",
        order = 4
    )]
    public class SpectraShowBank : ScriptableObject
    {
        public string bankName = "Show Bank";
        public SpectraShowEntry[] shows;
    }
}
