using UdonSharp;
using UnityEngine;
using VRC.SDKBase;

namespace SpectraOverdrive
{
    [UdonBehaviourSyncMode(BehaviourSyncMode.None)]
    public class SpectraDmxVideoSource : UdonSharpBehaviour
    {
        [Header("Decoded DMX texture")]
        public RenderTexture dmxTexture;
        public SpectraDmxLayoutProfile layoutProfile;

        [Header("Signal behavior")]
        public bool holdLastGoodFrame = true;
        public float signalTimeoutSeconds = 2f;
        public float streamLossFadeSeconds = 1f;

        [Header("Diagnostics")]
        public bool signalValid;
        public float lastFrameTime;
        public float signalAge;
        public int publishedWidth;
        public int publishedHeight;

        private int _textureId;
        private int _layoutId;
        private int _layoutFlagsId;
        private int _signalId;

        private void Start()
        {
            _textureId = Shader.PropertyToID("_SpectraDmxTexture");
            _layoutId = Shader.PropertyToID("_SpectraDmxLayout");
            _layoutFlagsId = Shader.PropertyToID("_SpectraDmxLayoutFlags");
            _signalId = Shader.PropertyToID("_SpectraDmxSignal");
            Publish();
        }

        private void Update()
        {
            signalAge = Time.time - lastFrameTime;
            signalValid = dmxTexture != null && signalAge <= signalTimeoutSeconds;
            Publish();
        }

        public void MarkFrameReceived()
        {
            lastFrameTime = Time.time;
            signalValid = true;
        }

        public void Publish()
        {
            if (dmxTexture != null)
            {
                VRCShader.SetGlobalTexture(_textureId, dmxTexture);
                publishedWidth = dmxTexture.width;
                publishedHeight = dmxTexture.height;
            }
            else
            {
                publishedWidth = 0;
                publishedHeight = 0;
            }

            SpectraDmxGridMode mode = SpectraDmxGridMode.Horizontal;
            SpectraDmxColorPacking packing = SpectraDmxColorPacking.RedOnly;
            int universes = 1;
            int sectorWidth = 16;
            int sectorHeight = 16;
            int sectorsPerRow = 4;
            bool flipX = false;
            bool flipY = false;
            bool halfTexel = true;

            if (layoutProfile != null)
            {
                mode = layoutProfile.gridMode;
                packing = layoutProfile.colorPacking;
                universes = layoutProfile.universeCount;
                sectorWidth = layoutProfile.sectorWidth;
                sectorHeight = layoutProfile.sectorHeight;
                sectorsPerRow = layoutProfile.sectorsPerRow;
                flipX = layoutProfile.flipX;
                flipY = layoutProfile.flipY;
                halfTexel = layoutProfile.halfTexelOffset;
            }

            VRCShader.SetGlobalVector(
                _layoutId,
                new Vector4((float)mode, universes, publishedWidth, publishedHeight)
            );

            VRCShader.SetGlobalVector(
                _layoutFlagsId,
                new Vector4(
                    (float)packing,
                    sectorWidth,
                    sectorHeight,
                    sectorsPerRow
                )
            );

            VRCShader.SetGlobalVector(
                Shader.PropertyToID("_SpectraDmxUvFlags"),
                new Vector4(flipX ? 1f : 0f, flipY ? 1f : 0f, halfTexel ? 1f : 0f, 0f)
            );

            float fade = 1f;
            if (!signalValid && streamLossFadeSeconds > 0.0001f)
            {
                fade = Mathf.Clamp01(1f - ((signalAge - signalTimeoutSeconds) / streamLossFadeSeconds));
            }

            VRCShader.SetGlobalVector(
                _signalId,
                new Vector4(signalValid ? 1f : 0f, fade, holdLastGoodFrame ? 1f : 0f, signalAge)
            );
        }
    }
}
