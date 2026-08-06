using UdonSharp;
using UnityEngine;

namespace SpectraOverdrive
{
    [UdonBehaviourSyncMode(BehaviourSyncMode.None)]
    public class SpectraWashRuntime : UdonSharpBehaviour
    {
        public SpectraFixtureRuntime fixture;
        public Renderer washRenderer;
        [Range(0f, 4f)] public float washPower = 1f;
        [Range(0f, 1f)] public float softness = 0.5f;

        public void Start()
        {
            Publish();
        }

        public void Publish()
        {
            if (fixture == null || washRenderer == null)
            {
                return;
            }

            fixture.controlledRenderers = new Renderer[] { washRenderer };
            fixture.projectionMultiplier = washPower;
            fixture.PublishFixtureProperties();

            Material material = washRenderer.material;
            if (material != null)
            {
                material.SetFloat("_WashSoftness", softness);
                material.SetFloat("_WashPower", washPower);
            }
        }
    }
}
