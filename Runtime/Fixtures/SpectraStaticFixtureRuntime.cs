using UdonSharp;
using UnityEngine;

namespace SpectraOverdrive
{
    [UdonBehaviourSyncMode(BehaviourSyncMode.None)]
    public class SpectraStaticFixtureRuntime : UdonSharpBehaviour
    {
        public SpectraFixtureRuntime fixture;
        public Renderer[] emitters;
        [Range(0f, 4f)] public float localPower = 1f;

        public void Start()
        {
            Publish();
        }

        public void Publish()
        {
            if (fixture == null || emitters == null)
            {
                return;
            }

            fixture.controlledRenderers = emitters;
            fixture.intensityMultiplier = localPower;
            fixture.PublishFixtureProperties();
        }
    }
}
