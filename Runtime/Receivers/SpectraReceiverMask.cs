using UdonSharp;
using UnityEngine;

namespace SpectraOverdrive
{
    [UdonBehaviourSyncMode(BehaviourSyncMode.None)]
    public class SpectraReceiverMask : UdonSharpBehaviour
    {
        public Renderer targetRenderer;
        [Range(0, 7)] public int zoneIndex;
        [Range(0f, 1f)] public float receiveBeam = 1f;
        [Range(0f, 1f)] public float receiveProjection = 1f;
        [Range(0f, 1f)] public float receiveLaser = 1f;
        [Range(0f, 1f)] public float receiveDisco = 1f;

        public void Start()
        {
            Publish();
        }

        public void Publish()
        {
            if (targetRenderer == null)
            {
                return;
            }

            Material material = targetRenderer.material;
            if (material == null)
            {
                return;
            }

            material.SetFloat("_ZoneIndex", zoneIndex);
            material.SetVector(
                "_SpectraReceiverMask",
                new Vector4(receiveBeam, receiveProjection, receiveLaser, receiveDisco)
            );
        }
    }
}
