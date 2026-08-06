using UdonSharp;
using UnityEngine;

namespace SpectraOverdrive
{
    [UdonBehaviourSyncMode(BehaviourSyncMode.None)]
    public class SpectraFixtureIdentity : UdonSharpBehaviour
    {
        [Header("Identity")]
        public int fixtureId;
        public string fixtureName = "Fixture";
        public string fixtureProfile = "Generic";
        public SpectraFixtureType fixtureType = SpectraFixtureType.Generic;

        [Header("DMX patch")]
        [Range(1, 9)] public int universe = 1;
        [Range(1, 512)] public int startAddress = 1;
        [Range(1, 64)] public int channelCount = 13;

        [Header("Groups")]
        public int primaryGroup;
        public int receiverZone;

        [Header("Mounting")]
        public bool invertPan;
        public bool invertTilt;
        public bool upsideDown;
        [Range(-180f, 180f)] public float panOffset;
        [Range(-180f, 180f)] public float tiltOffset;

        [Header("Render components")]
        public GameObject fixtureBody;
        public GameObject lens;
        public GameObject beam;
        public GameObject projection;
        public GameObject halo;

        public void SetVisualLayerEnabled(int layer, bool enabled)
        {
            GameObject target = null;

            if (layer == 0) target = fixtureBody;
            else if (layer == 1) target = lens;
            else if (layer == 2) target = beam;
            else if (layer == 3) target = projection;
            else if (layer == 4) target = halo;

            if (target != null)
            {
                target.SetActive(enabled);
            }
        }
    }
}
