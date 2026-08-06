using UdonSharp;
using UnityEngine;
using VRC.SDKBase;

namespace SpectraOverdrive
{
    [UdonBehaviourSyncMode(BehaviourSyncMode.None)]
    public class SpectraReceiverZoneManager : UdonSharpBehaviour
    {
        public SpectraReceiverZone[] zones;
        public bool publishEveryFrame = true;

        private void Start()
        {
            PublishZones();
        }

        private void Update()
        {
            if (publishEveryFrame)
            {
                PublishZones();
            }
        }

        public void PublishZones()
        {
            for (int i = 0; i < 8; i++)
            {
                Vector4 value = Vector4.zero;

                if (zones != null)
                {
                    for (int j = 0; j < zones.Length; j++)
                    {
                        SpectraReceiverZone zone = zones[j];
                        if (zone != null && zone.zoneIndex == i)
                        {
                            value = zone.ToShaderVector();
                            break;
                        }
                    }
                }

                VRCShader.SetGlobalVector(Shader.PropertyToID("_SpectraZone" + i), value);
            }
        }
    }
}
