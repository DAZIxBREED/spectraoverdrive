using UdonSharp;
using UnityEngine;

namespace SpectraOverdrive
{
    [UdonBehaviourSyncMode(BehaviourSyncMode.None)]
    public class SpectraLaserRibbon : UdonSharpBehaviour
    {
        public SpectraFixtureRuntime fixture;
        public Renderer ribbonRenderer;

        [Range(1, 32)] public int segmentCount = 8;
        [Range(0f, 4f)] public float laserPower = 1f;
        [Range(0f, 2f)] public float scanSpeed = 1f;
        [Range(0f, 1f)] public float spread = 0.5f;
        [Range(0f, 1f)] public float jitter;

        public void Start()
        {
            Publish();
        }

        public void Publish()
        {
            if (fixture == null || ribbonRenderer == null)
            {
                return;
            }

            fixture.controlledRenderers = new Renderer[] { ribbonRenderer };
            fixture.PublishFixtureProperties();

            Material material = ribbonRenderer.material;
            if (material != null)
            {
                material.SetFloat("_LaserSegments", segmentCount);
                material.SetFloat("_LaserPower", laserPower);
                material.SetFloat("_LaserSpeed", scanSpeed);
                material.SetFloat("_LaserSpread", spread);
                material.SetFloat("_LaserJitter", jitter);
            }
        }
    }
}
