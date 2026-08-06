using UnityEngine;

namespace SpectraOverdrive
{
    [CreateAssetMenu(
        fileName = "SpectraAudioCoordinateProfile",
        menuName = "SpectraOverdrive/Audio Coordinate Profile",
        order = 3
    )]
    public class SpectraAudioCoordinateProfile : ScriptableObject
    {
        public SpectraAudioLayout layout = SpectraAudioLayout.GenericFallback;
        public Vector2 bassUv = new Vector2(0.0625f, 0.125f);
        public Vector2 lowMidUv = new Vector2(0.1875f, 0.125f);
        public Vector2 highMidUv = new Vector2(0.3125f, 0.125f);
        public Vector2 trebleUv = new Vector2(0.4375f, 0.125f);
        public Vector2 overallUv = new Vector2(0.5625f, 0.125f);
        public Vector2 colorUv = new Vector2(0.0625f, 0.375f);
        public Vector2 themeColorUv = new Vector2(0.1875f, 0.375f);
        public string notes;
    }
}
