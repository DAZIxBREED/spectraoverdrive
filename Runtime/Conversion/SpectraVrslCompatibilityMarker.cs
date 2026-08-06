using UdonSharp;
using UnityEngine;

namespace SpectraOverdrive
{
    [UdonBehaviourSyncMode(BehaviourSyncMode.None)]
    public class SpectraVrslCompatibilityMarker : UdonSharpBehaviour
    {
        [Header("Migration metadata")]
        public string originalComponentType;
        public string originalFixtureName;
        public int originalUniverse = 1;
        public int originalAddress = 1;
        public bool importedAudioLink;
        public bool importedMovementLimits;
        public string conversionNotes;
    }
}
