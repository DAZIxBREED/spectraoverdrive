using UdonSharp;
using UnityEngine;
using VRC.SDKBase;

namespace SpectraOverdrive
{
    [UdonBehaviourSyncMode(BehaviourSyncMode.None)]
    public class SpectraAudioLinkAdapter : UdonSharpBehaviour
    {
        [Header("AudioLink texture")]
        public Texture audioTexture;
        public bool publishAudioTexture = true;

        [Header("AudioLink mapping")]
        [Range(0f, 4f)] public float gain = 1f;
        [Range(0f, 2f)] public float attack = 0.15f;
        [Range(0f, 2f)] public float release = 0.35f;
        [Range(0f, 1f)] public float beatThreshold = 0.55f;
        public bool useColorChord = true;
        public bool useThemeColors = true;

        [Header("Fallback input")]
        [Range(0f, 1f)] public float manualBass;
        [Range(0f, 1f)] public float manualLowMid;
        [Range(0f, 1f)] public float manualHighMid;
        [Range(0f, 1f)] public float manualTreble;
        [Range(0f, 1f)] public float manualOverall;

        private int _settingsId;
        private int _fallbackBandsId;
        private int _textureId;

        private void Start()
        {
            _settingsId = Shader.PropertyToID("_SpectraAudioSettings");
            _fallbackBandsId = Shader.PropertyToID("_SpectraAudioFallbackBands");
            _textureId = Shader.PropertyToID("_SpectraAudioTexture");
            Publish();
        }

        public void Publish()
        {
            if (publishAudioTexture && audioTexture != null)
            {
                VRCShader.SetGlobalTexture(_textureId, audioTexture);
            }

            VRCShader.SetGlobalVector(
                _settingsId,
                new Vector4(gain, attack, release, beatThreshold)
            );

            VRCShader.SetGlobalVector(
                _fallbackBandsId,
                new Vector4(manualBass, manualLowMid, manualHighMid, manualTreble)
            );

            VRCShader.SetGlobalFloat(
                Shader.PropertyToID("_SpectraAudioFallbackOverall"),
                manualOverall
            );

            VRCShader.SetGlobalFloat(
                Shader.PropertyToID("_SpectraUseColorChord"),
                useColorChord ? 1f : 0f
            );

            VRCShader.SetGlobalFloat(
                Shader.PropertyToID("_SpectraUseThemeColors"),
                useThemeColors ? 1f : 0f
            );
        }
    }
}
