using UdonSharp;
using UnityEngine;

namespace SpectraOverdrive
{
    [UdonBehaviourSyncMode(BehaviourSyncMode.None)]
    public class SpectraReceiverZone : UdonSharpBehaviour
    {
        [Range(0, 7)] public int zoneIndex;
        public Color color = Color.black;
        [Range(0f, 4f)] public float intensity;
        [Range(0f, 1f)] public float pulse;
        [Range(0f, 1f)] public float priority;

        public Vector4 ToShaderVector()
        {
            float scale = Mathf.Max(0f, intensity);
            return new Vector4(color.r * scale, color.g * scale, color.b * scale, pulse);
        }
    }
}
