using UdonSharp;
using UnityEngine;
using VRC.SDKBase;

namespace SpectraOverdrive
{
    [UdonBehaviourSyncMode(BehaviourSyncMode.None)]
    public class SpectraAudioCoordinatePublisher : UdonSharpBehaviour
    {
        public SpectraAudioCoordinateProfile profile;
        public bool useComponentDefaultsWhenProfileMissing = true;
        public Vector2 defaultBassUv = new Vector2(0.0625f, 0.125f);
        public Vector2 defaultLowMidUv = new Vector2(0.1875f, 0.125f);
        public Vector2 defaultHighMidUv = new Vector2(0.3125f, 0.125f);
        public Vector2 defaultTrebleUv = new Vector2(0.4375f, 0.125f);
        public Vector2 defaultOverallUv = new Vector2(0.5625f, 0.125f);
        public Vector2 defaultColorUv = new Vector2(0.0625f, 0.375f);
        public Vector2 defaultThemeColorUv = new Vector2(0.1875f, 0.375f);

        public void Start()
        {
            Publish();
        }

        public void Publish()
        {
            if (profile == null && !useComponentDefaultsWhenProfileMissing) return;
            Vector2 bass = profile == null ? defaultBassUv : profile.bassUv;
            Vector2 lowMid = profile == null ? defaultLowMidUv : profile.lowMidUv;
            Vector2 highMid = profile == null ? defaultHighMidUv : profile.highMidUv;
            Vector2 treble = profile == null ? defaultTrebleUv : profile.trebleUv;
            Vector2 overall = profile == null ? defaultOverallUv : profile.overallUv;
            Vector2 color = profile == null ? defaultColorUv : profile.colorUv;
            Vector2 theme = profile == null ? defaultThemeColorUv : profile.themeColorUv;
            SpectraAudioLayout layout = profile == null ? SpectraAudioLayout.GenericFallback : profile.layout;

            VRCShader.SetGlobalVector(
                Shader.PropertyToID("_SpectraAudioUv0"),
                new Vector4(bass.x, bass.y, lowMid.x, lowMid.y)
            );

            VRCShader.SetGlobalVector(
                Shader.PropertyToID("_SpectraAudioUv1"),
                new Vector4(highMid.x, highMid.y, treble.x, treble.y)
            );

            VRCShader.SetGlobalVector(
                Shader.PropertyToID("_SpectraAudioUv2"),
                new Vector4(overall.x, overall.y, color.x, color.y)
            );

            VRCShader.SetGlobalVector(
                Shader.PropertyToID("_SpectraAudioUv3"),
                new Vector4(theme.x, theme.y, (float)layout, 0f)
            );
        }
    }
}
