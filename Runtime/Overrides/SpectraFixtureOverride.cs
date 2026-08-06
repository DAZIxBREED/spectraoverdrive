using UdonSharp;
using UnityEngine;

namespace SpectraOverdrive
{
    [UdonBehaviourSyncMode(BehaviourSyncMode.None)]
    public class SpectraFixtureOverride : UdonSharpBehaviour
    {
        public SpectraFixtureRuntime fixture;

        [Header("Override switches")]
        public bool overrideColor;
        public bool overrideIntensity;
        public bool overridePan;
        public bool overrideTilt;
        public bool overrideMovementScale;
        public bool muteFixture;

        [Header("Override values")]
        public Color color = Color.white;
        [Range(0f, 2f)] public float intensity = 1f;
        [Range(-1f, 1f)] public float panBias;
        [Range(-1f, 1f)] public float tiltBias;
        [Range(0f, 2f)] public float movementScale = 1f;

        public void Apply()
        {
            if (fixture == null) return;

            if (overrideColor) fixture.groupColorMultiplier = color;
            if (overrideIntensity) fixture.groupIntensityMultiplier = intensity;
            if (overridePan) fixture.groupPanBias = panBias;
            if (overrideTilt) fixture.groupTiltBias = tiltBias;
            if (overrideMovementScale) fixture.groupMovementScale = movementScale;
            if (muteFixture) fixture.groupIntensityMultiplier = 0f;

            fixture.PublishFixtureProperties();
        }

        public void Clear()
        {
            if (fixture == null) return;

            fixture.groupColorMultiplier = Color.white;
            fixture.groupIntensityMultiplier = 1f;
            fixture.groupPanBias = 0f;
            fixture.groupTiltBias = 0f;
            fixture.groupMovementScale = 1f;
            fixture.PublishFixtureProperties();
        }
    }
}
