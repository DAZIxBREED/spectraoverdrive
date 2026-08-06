using System;
using UnityEngine;

namespace SpectraOverdrive
{
    [Serializable]
    public class SpectraVrslPropertyRule
    {
        public string sourceTypeContains;
        public string sourceProperty;
        public string targetProperty;
        public float scale = 1f;
        public float offset;
        public bool invert;
        public string notes;
    }

    [CreateAssetMenu(
        fileName = "SpectraVrslPropertyMap",
        menuName = "SpectraOverdrive/VRSL Property Map",
        order = 2
    )]
    public class SpectraVrslPropertyMap : ScriptableObject
    {
        public string vrslVersion = "Unknown";
        public SpectraVrslPropertyRule[] rules;
    }
}
