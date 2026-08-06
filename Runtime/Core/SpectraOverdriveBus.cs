using UdonSharp;
using UnityEngine;
using VRC.SDKBase;

namespace SpectraOverdrive
{
    [UdonBehaviourSyncMode(BehaviourSyncMode.None)]
    public class SpectraOverdriveBus : UdonSharpBehaviour
    {
        [Header("Global shader names")]
        public string controlTextureProperty = "_SpectraControlTexture";
        public string globalStateProperty = "_SpectraGlobalState";
        public string globalTimeProperty = "_SpectraShowTime";

        [Header("Control data")]
        public RenderTexture controlTexture;
        [Range(0f, 2f)] public float masterIntensity = 1f;
        public bool blackout;
        public SpectraControlSource activeSource = SpectraControlSource.Idle;

        [Header("Diagnostics")]
        public bool publishEveryFrame = true;
        public bool externalShowClock;
        public float externalShowTime;
        public float lastPublishTime;
        public int publishCount;

        private int _controlTextureId;
        private int _globalStateId;
        private int _globalTimeId;

        private void Start()
        {
            _controlTextureId = Shader.PropertyToID(controlTextureProperty);
            _globalStateId = Shader.PropertyToID(globalStateProperty);
            _globalTimeId = Shader.PropertyToID(globalTimeProperty);
            Publish();
        }

        private void Update()
        {
            if (publishEveryFrame)
            {
                Publish();
            }
        }

        public void Publish()
        {
            if (controlTexture != null)
            {
                VRCShader.SetGlobalTexture(_controlTextureId, controlTexture);
            }

            float effectiveIntensity = blackout ? 0f : Mathf.Max(0f, masterIntensity);
            VRCShader.SetGlobalVector(
                _globalStateId,
                new Vector4(effectiveIntensity, blackout ? 1f : 0f, (float)activeSource, 0f)
            );

            float publishedTime = externalShowClock
                ? externalShowTime
                : (float)Networking.GetServerTimeInSeconds();
            VRCShader.SetGlobalFloat(_globalTimeId, publishedTime);

            lastPublishTime = Time.time;
            publishCount++;
        }

        public void SetBlackout(bool enabled)
        {
            blackout = enabled;
            Publish();
        }

        public void SetMasterIntensity(float value)
        {
            masterIntensity = Mathf.Clamp(value, 0f, 2f);
            Publish();
        }
    }
}
