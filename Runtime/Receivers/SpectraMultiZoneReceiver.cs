using UdonSharp;
using UnityEngine;

namespace SpectraOverdrive
{
    [UdonBehaviourSyncMode(BehaviourSyncMode.None)]
    public class SpectraMultiZoneReceiver : UdonSharpBehaviour
    {
        public Renderer targetRenderer;

        [Header("Zone blend")]
        [Range(0, 7)] public int zoneA;
        [Range(0, 7)] public int zoneB = 1;
        [Range(0, 7)] public int zoneC = 2;
        [Range(0, 7)] public int zoneD = 3;

        [Range(0f, 1f)] public float weightA = 1f;
        [Range(0f, 1f)] public float weightB;
        [Range(0f, 1f)] public float weightC;
        [Range(0f, 1f)] public float weightD;

        public void Start()
        {
            Publish();
        }

        public void Publish()
        {
            if (targetRenderer == null) return;

            Material material = targetRenderer.material;
            if (material == null) return;

            material.SetVector("_SpectraReceiverZones", new Vector4(zoneA, zoneB, zoneC, zoneD));
            material.SetVector("_SpectraReceiverWeights", new Vector4(weightA, weightB, weightC, weightD));
        }
    }
}
