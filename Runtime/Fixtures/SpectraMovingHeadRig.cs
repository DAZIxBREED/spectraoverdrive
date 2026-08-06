using UdonSharp;
using UnityEngine;

namespace SpectraOverdrive
{
    [UdonBehaviourSyncMode(BehaviourSyncMode.None)]
    public class SpectraMovingHeadRig : UdonSharpBehaviour
    {
        public SpectraFixtureRuntime fixture;
        public Transform panTransform;
        public Transform tiltTransform;

        [Header("Editor preview only")]
        [Range(0f, 1f)] public float previewPan = 0.5f;
        [Range(0f, 1f)] public float previewTilt = 0.5f;
        public bool previewInPlayMode;

        private void Update()
        {
            if (!previewInPlayMode || fixture == null)
            {
                return;
            }

            ApplyPreview();
        }

        public void ApplyPreview()
        {
            float panRange = fixture.panRangeDegrees;
            float tiltRange = fixture.tiltRangeDegrees;

            float pan = (previewPan - 0.5f) * panRange;
            float tilt = (previewTilt - 0.5f) * tiltRange;

            if (fixture.identity != null)
            {
                if (fixture.identity.invertPan) pan = -pan;
                if (fixture.identity.invertTilt) tilt = -tilt;
                pan += fixture.identity.panOffset;
                tilt += fixture.identity.tiltOffset;
            }

            if (panTransform != null)
            {
                panTransform.localRotation = Quaternion.Euler(0f, pan, 0f);
            }

            if (tiltTransform != null)
            {
                tiltTransform.localRotation = Quaternion.Euler(tilt, 0f, 0f);
            }
        }
    }
}
