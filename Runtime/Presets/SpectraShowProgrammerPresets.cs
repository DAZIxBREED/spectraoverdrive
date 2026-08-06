using System;
using UnityEngine;

namespace SpectraOverdrive
{
    [CreateAssetMenu(fileName = "SpectraCueTemplate", menuName = "SpectraOverdrive/Show Programmer/Cue Template")]
    public class SpectraCueTemplateAsset : ScriptableObject
    {
        public string templateName = "Cue Template";
        [TextArea] public string description;
        public SpectraCueBlock cue = new SpectraCueBlock();

        public SpectraCueBlock InstantiateCue()
        {
            SpectraCueBlock instance = cue == null
                ? new SpectraCueBlock()
                : JsonUtility.FromJson<SpectraCueBlock>(JsonUtility.ToJson(cue));
            instance.id = Guid.NewGuid().ToString("N");
            instance.enabled = true;
            instance.name = string.IsNullOrWhiteSpace(templateName) ? instance.name : templateName;
            return instance;
        }
    }

    [CreateAssetMenu(fileName = "SpectraMovementPreset", menuName = "SpectraOverdrive/Show Programmer/Movement Preset")]
    public class SpectraMovementPresetAsset : ScriptableObject
    {
        public string presetName = "Movement Preset";
        public SpectraMovementPatternKind pattern = SpectraMovementPatternKind.Circle;
        [Range(-1f, 1f)] public float pan;
        [Range(-1f, 1f)] public float tilt;
        [Range(0f, 8f)] public float speed = 1f;
        [Range(0f, 2f)] public float amplitude = 1f;
        [Range(0f, 2f)] public float spread = 1f;
        [Range(-8f, 8f)] public float phase;
        [Range(-1f, 1f)] public float direction = 1f;
        [Range(0f, 1f)] public float smoothing = 0.5f;
        public int randomSeed = 1337;

        public void ApplyTo(SpectraCueBlock cue)
        {
            if (cue == null) return;
            cue.valueType = SpectraCueValueType.Movement;
            cue.movementPattern = pattern;
            cue.pan = pan;
            cue.tilt = tilt;
            cue.movementSpeed = speed;
            cue.movementAmplitude = amplitude;
            cue.movementSpread = spread;
            cue.movementPhase = phase;
            cue.movementDirection = direction;
            cue.movementSmoothing = smoothing;
            cue.randomSeed = randomSeed;
        }
    }

    [CreateAssetMenu(fileName = "SpectraColorPalette", menuName = "SpectraOverdrive/Show Programmer/Color Palette")]
    public class SpectraColorPaletteAsset : ScriptableObject
    {
        public string paletteName = "Palette";
        public Color[] colors = new[] { Color.magenta, Color.cyan };
        [Range(0.1f, 4f)] public float mobileBrightnessScale = 0.85f;

        public Color Evaluate(float normalizedPosition, SpectraPlatformKind platform)
        {
            if (colors == null || colors.Length == 0) return Color.white;
            if (colors.Length == 1) return ApplyPlatform(colors[0], platform);
            float scaled = Mathf.Repeat(normalizedPosition, 1f) * (colors.Length - 1);
            int left = Mathf.Clamp(Mathf.FloorToInt(scaled), 0, colors.Length - 1);
            int right = Mathf.Min(left + 1, colors.Length - 1);
            return ApplyPlatform(Color.Lerp(colors[left], colors[right], scaled - left), platform);
        }

        private Color ApplyPlatform(Color color, SpectraPlatformKind platform)
        {
            if (platform == SpectraPlatformKind.Quest || platform == SpectraPlatformKind.IOS || platform == SpectraPlatformKind.Android)
            {
                float scale = Mathf.Clamp(mobileBrightnessScale, 0.1f, 4f);
                color.r *= scale;
                color.g *= scale;
                color.b *= scale;
            }
            return color;
        }
    }

    [Serializable]
    public class SpectraSectionTemplateCue
    {
        [Min(0f)] public float startBeat;
        [Min(0.01f)] public float durationBeats = 1f;
        public SpectraCueBlock cue = new SpectraCueBlock();
    }

    [CreateAssetMenu(fileName = "SpectraSectionTemplate", menuName = "SpectraOverdrive/Show Programmer/Section Template")]
    public class SpectraSectionTemplateAsset : ScriptableObject
    {
        public string templateName = "8 Bar Section";
        [Min(1)] public int lengthBars = 8;
        public SpectraSectionTemplateCue[] cues = new SpectraSectionTemplateCue[0];

        public SpectraCueBlock[] InstantiateCues(SpectraBeatGrid grid, float startSeconds)
        {
            if (grid == null) throw new ArgumentNullException("grid");
            int count = cues == null ? 0 : cues.Length;
            SpectraCueBlock[] result = new SpectraCueBlock[count];
            double startBeat = grid.SecondsToBeat(startSeconds);
            for (int i = 0; i < count; i++)
            {
                SpectraSectionTemplateCue source = cues[i] ?? new SpectraSectionTemplateCue();
                SpectraCueBlock instance = source.cue == null
                    ? new SpectraCueBlock()
                    : JsonUtility.FromJson<SpectraCueBlock>(JsonUtility.ToJson(source.cue));
                instance.id = Guid.NewGuid().ToString("N");
                instance.enabled = true;
                instance.timingMode = SpectraTimingMode.Musical;
                instance.startMusical = grid.SecondsToMusical(grid.BeatToSeconds(startBeat + source.startBeat));
                instance.durationBeats = Mathf.Max(0.01f, source.durationBeats);
                result[i] = instance;
            }
            return result;
        }
    }
}
