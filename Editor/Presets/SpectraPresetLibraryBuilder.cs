using UnityEditor;
using UnityEngine;

namespace SpectraOverdrive.Editor
{
    public static class SpectraPresetLibraryBuilder
    {
        private const string Root = "Assets/SpectraOverdriveGenerated";
        private const string Presets = Root + "/Presets";
        private const string Cues = Presets + "/Cues";
        private const string Movement = Presets + "/Movement";
        private const string Palettes = Presets + "/Palettes";
        private const string Sections = Presets + "/Sections";

        [MenuItem("SpectraOverdrive/Show Programmer/Generate Built-In Preset Library")]
        public static void Generate()
        {
            EnsureFolder("Assets", "SpectraOverdriveGenerated");
            EnsureFolder(Root, "Presets");
            EnsureFolder(Presets, "Cues");
            EnsureFolder(Presets, "Movement");
            EnsureFolder(Presets, "Palettes");
            EnsureFolder(Presets, "Sections");

            CreateCue("White Impact", SpectraCueValueType.Intensity, Color.white, 1f, 0.25f, SpectraCueBlendMode.Maximum);
            CreateCue("Purple Wash", SpectraCueValueType.Color, new Color(0.55f, 0.05f, 1f), 0.8f, 8f, SpectraCueBlendMode.ColorMix);
            CreateMovementCue("Bass Chase", SpectraMovementPatternKind.Chase, new Color(0.6f, 0.1f, 1f), 1.5f, 4f);
            CreateMovementCue("Drop Sweep", SpectraMovementPatternKind.HorizontalSweep, new Color(0.1f, 0.9f, 1f), 2f, 8f);
            CreateMovementCue("Slow Breakdown", SpectraMovementPatternKind.Circle, new Color(0.15f, 0.35f, 1f), 0.25f, 16f);
            CreateCue("Laser Fan", SpectraCueValueType.LaserEnable, new Color(0.1f, 1f, 0.55f), 1f, 8f, SpectraCueBlendMode.PriorityOverride);
            CreateCue("Audience Blinder", SpectraCueValueType.Intensity, new Color(1f, 0.82f, 0.6f), 1f, 1f, SpectraCueBlendMode.Maximum);
            SpectraCueTemplateAsset blackout = CreateCue("Emergency Blackout", SpectraCueValueType.Blackout, Color.black, 1f, 1f, SpectraCueBlendMode.PriorityOverride);
            blackout.cue.priority = 100000;
            blackout.cue.boolValue = true;
            EditorUtility.SetDirty(blackout);

            CreateMovement("Horizontal Sweep", SpectraMovementPatternKind.HorizontalSweep, 1f, 1f, 1f);
            CreateMovement("Vertical Sweep", SpectraMovementPatternKind.VerticalSweep, 1f, 1f, 1f);
            CreateMovement("Circle", SpectraMovementPatternKind.Circle, 0.75f, 0.8f, 1f);
            CreateMovement("Figure Eight", SpectraMovementPatternKind.FigureEight, 0.6f, 0.9f, 1f);
            CreateMovement("Fan", SpectraMovementPatternKind.Fan, 0.4f, 0.8f, 1.3f);
            CreateMovement("Alternating Wave", SpectraMovementPatternKind.AlternatingWave, 1.25f, 0.8f, 1.4f);
            CreateMovement("Audience Sweep", SpectraMovementPatternKind.AudienceSweep, 0.5f, 0.8f, 1f);
            CreateMovement("DJ Focus", SpectraMovementPatternKind.DjFocus, 0.2f, 0.7f, 1f);
            CreateMovement("Follow The Leader", SpectraMovementPatternKind.FollowTheLeader, 1f, 0.8f, 1.5f);
            CreateMovement("Seeded Random", SpectraMovementPatternKind.SeededRandom, 0.5f, 0.65f, 1f);

            CreatePalette("Cyberpunk Purple", new Color(0.48f, 0.02f, 1f), new Color(1f, 0.05f, 0.65f), new Color(0.05f, 0.9f, 1f));
            CreatePalette("Neon Mixtape", new Color(1f, 0.08f, 0.55f), new Color(0.05f, 1f, 0.75f), new Color(1f, 0.85f, 0.1f));
            CreatePalette("Blood and Chrome", new Color(0.75f, 0.01f, 0.02f), new Color(0.85f, 0.9f, 1f), new Color(0.12f, 0.12f, 0.16f));
            CreatePalette("Industrial Amber", new Color(1f, 0.28f, 0.02f), new Color(1f, 0.72f, 0.12f), new Color(0.18f, 0.08f, 0.02f));
            CreatePalette("Ice Blue", new Color(0.05f, 0.35f, 1f), new Color(0.15f, 1f, 1f), Color.white);
            CreatePalette("Pride Spectrum", Color.red, new Color(1f, 0.5f, 0f), Color.yellow, Color.green, Color.blue, new Color(0.55f, 0f, 0.9f));
            CreatePalette("Emergency Red", new Color(0.25f, 0f, 0f), Color.red, new Color(1f, 0.65f, 0.15f));
            CreatePalette("UV Nightclub", new Color(0.18f, 0f, 0.55f), new Color(0.75f, 0f, 1f), new Color(0.05f, 0.25f, 1f));

            CreateBuildSection();
            CreateDropSection();
            AssetDatabase.SaveAssets();
            AssetDatabase.Refresh();
            Object selection = AssetDatabase.LoadAssetAtPath<Object>(Presets);
            Selection.activeObject = selection;
            EditorGUIUtility.PingObject(selection);
            EditorUtility.DisplayDialog("SpectraOverdrive", "Generated or updated 8 cue templates, 10 movement presets, 8 palettes, and 2 section templates.", "OK");
        }

        private static SpectraCueTemplateAsset CreateCue(string name, SpectraCueValueType type, Color color, float intensity, float durationBeats, SpectraCueBlendMode blend)
        {
            SpectraCueTemplateAsset asset = LoadOrCreate<SpectraCueTemplateAsset>(Cues + "/" + FileName(name) + ".asset");
            asset.templateName = name;
            asset.description = "Built-in SpectraOverdrive 1.5.0 cue template.";
            asset.cue = new SpectraCueBlock
            {
                name = name,
                enabled = true,
                timingMode = SpectraTimingMode.Musical,
                durationBeats = durationBeats,
                valueType = type,
                color = color,
                intensity = intensity,
                boolValue = true,
                blendMode = blend,
                easing = SpectraCueEasing.SmoothStep,
                movementDirection = 1f,
                movementAmplitude = 1f,
                movementSpread = 1f,
                questFallback = SpectraPlatformFallback.Simplified,
                iosFallback = SpectraPlatformFallback.EmissiveOnly,
                androidFallback = SpectraPlatformFallback.EmissiveOnly
            };
            EditorUtility.SetDirty(asset);
            return asset;
        }

        private static void CreateMovementCue(string name, SpectraMovementPatternKind pattern, Color color, float speed, float durationBeats)
        {
            SpectraCueTemplateAsset asset = CreateCue(name, SpectraCueValueType.Movement, color, 1f, durationBeats, SpectraCueBlendMode.Replace);
            asset.cue.movementPattern = pattern;
            asset.cue.movementSpeed = speed;
            asset.cue.movementAmplitude = 0.8f;
            asset.cue.movementSpread = 1.2f;
            EditorUtility.SetDirty(asset);
        }

        private static void CreateMovement(string name, SpectraMovementPatternKind pattern, float speed, float amplitude, float spread)
        {
            SpectraMovementPresetAsset asset = LoadOrCreate<SpectraMovementPresetAsset>(Movement + "/" + FileName(name) + ".asset");
            asset.presetName = name;
            asset.pattern = pattern;
            asset.speed = speed;
            asset.amplitude = amplitude;
            asset.spread = spread;
            asset.direction = 1f;
            asset.smoothing = 0.5f;
            asset.randomSeed = 1337;
            EditorUtility.SetDirty(asset);
        }

        private static void CreatePalette(string name, params Color[] colors)
        {
            SpectraColorPaletteAsset asset = LoadOrCreate<SpectraColorPaletteAsset>(Palettes + "/" + FileName(name) + ".asset");
            asset.paletteName = name;
            asset.colors = colors;
            asset.mobileBrightnessScale = 0.82f;
            EditorUtility.SetDirty(asset);
        }

        private static void CreateBuildSection()
        {
            SpectraSectionTemplateAsset asset = LoadOrCreate<SpectraSectionTemplateAsset>(Sections + "/EightBarBuild.asset");
            asset.templateName = "8-Bar Build";
            asset.lengthBars = 8;
            asset.cues = new[]
            {
                SectionCue(0f, 32f, SpectraCueValueType.Color, "Build Wash", new Color(0.45f, 0.05f, 1f), 0.55f),
                SectionMovement(0f, 32f, "Expanding Fan", SpectraMovementPatternKind.CenterOutFan, 0.5f),
                SectionCue(24f, 8f, SpectraCueValueType.Strobe, "Build Accents", Color.white, 1f),
                SectionCue(31f, 1f, SpectraCueValueType.Intensity, "White Impact", Color.white, 1f)
            };
            asset.cues[2].cue.strobeHz = 8f;
            EditorUtility.SetDirty(asset);
        }

        private static void CreateDropSection()
        {
            SpectraSectionTemplateAsset asset = LoadOrCreate<SpectraSectionTemplateAsset>(Sections + "/SixteenBarDrop.asset");
            asset.templateName = "16-Bar Drop";
            asset.lengthBars = 16;
            asset.cues = new[]
            {
                SectionCue(0f, 64f, SpectraCueValueType.Color, "Purple Cyan Drop", new Color(0.1f, 0.85f, 1f), 1f),
                SectionMovement(0f, 64f, "Drop Sweep", SpectraMovementPatternKind.AlternatingWave, 1.75f),
                SectionCue(0f, 64f, SpectraCueValueType.LaserEnable, "Laser Fan", Color.green, 1f),
                SectionCue(0f, 1f, SpectraCueValueType.Intensity, "Drop Impact", Color.white, 1f)
            };
            EditorUtility.SetDirty(asset);
        }

        private static SpectraSectionTemplateCue SectionCue(float startBeat, float durationBeats, SpectraCueValueType type, string name, Color color, float intensity)
        {
            return new SpectraSectionTemplateCue
            {
                startBeat = startBeat,
                durationBeats = durationBeats,
                cue = new SpectraCueBlock
                {
                    name = name,
                    enabled = true,
                    valueType = type,
                    color = color,
                    intensity = intensity,
                    boolValue = true,
                    easing = SpectraCueEasing.SmoothStep,
                    movementDirection = 1f,
                    movementAmplitude = 1f,
                    movementSpread = 1f,
                    questFallback = SpectraPlatformFallback.Simplified,
                    iosFallback = SpectraPlatformFallback.EmissiveOnly,
                    androidFallback = SpectraPlatformFallback.EmissiveOnly
                }
            };
        }

        private static SpectraSectionTemplateCue SectionMovement(float startBeat, float durationBeats, string name, SpectraMovementPatternKind pattern, float speed)
        {
            SpectraSectionTemplateCue cue = SectionCue(startBeat, durationBeats, SpectraCueValueType.Movement, name, Color.white, 1f);
            cue.cue.movementPattern = pattern;
            cue.cue.movementSpeed = speed;
            cue.cue.movementAmplitude = 0.85f;
            cue.cue.movementSpread = 1.25f;
            return cue;
        }

        private static T LoadOrCreate<T>(string path) where T : ScriptableObject
        {
            T asset = AssetDatabase.LoadAssetAtPath<T>(path);
            if (asset != null) return asset;
            asset = ScriptableObject.CreateInstance<T>();
            AssetDatabase.CreateAsset(asset, path);
            return asset;
        }

        private static void EnsureFolder(string parent, string name)
        {
            string path = parent + "/" + name;
            if (!AssetDatabase.IsValidFolder(path)) AssetDatabase.CreateFolder(parent, name);
        }

        private static string FileName(string name)
        {
            return name.Replace(" ", string.Empty).Replace("-", string.Empty);
        }
    }
}
