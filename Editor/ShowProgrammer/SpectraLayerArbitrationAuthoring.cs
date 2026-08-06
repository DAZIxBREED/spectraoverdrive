using UnityEditor;
using UnityEngine;

namespace SpectraOverdrive.Editor
{
    public static class SpectraLayerArbitrationAuthoring
    {
        public static void EnsureStarterLayers(SpectraShowAsset show)
        {
            if (show == null) return;
            if (show.cueLayers != null && show.cueLayers.Length > 0) return;
            show.cueLayers = new[]
            {
                CreateLayer("Base", new Color(0.25f, 0.65f, 1f, 1f), 0, 0,
                    true, true, true, true),
                CreateLayer("Movement", new Color(0.55f, 0.25f, 1f, 1f), 5, 8,
                    true, true, true, true),
                CreateLayer("Accents", new Color(1f, 0.25f, 0.75f, 1f), 20, 8,
                    true, true, true, true),
                CreateLayer("Safety", new Color(1f, 0.75f, 0.15f, 1f), 80, 4,
                    true, true, true, true)
            };
        }

        public static void AssignLayer(SpectraCueBlock cue, int layerIndex)
        {
            if (cue == null) return;
            cue.layerIndex = Mathf.Clamp(layerIndex, -1, 15);
        }

        public static void ApplyHighestPriorityArbitration(
            SpectraCueBlock cue,
            int group)
        {
            if (cue == null) return;
            cue.arbitrationMode = SpectraCueArbitrationMode.HighestPriority;
            cue.arbitrationGroup = Mathf.Clamp(group, 0, 15);
            cue.arbitrationTimeBase = SpectraModulationTimeBase.Bars;
            cue.arbitrationCycleLength = 1f;
            cue.arbitrationPhase = 0f;
            cue.arbitrationSeed = 0;
        }

        public static void ApplyLatestStartArbitration(
            SpectraCueBlock cue,
            int group)
        {
            if (cue == null) return;
            cue.arbitrationMode = SpectraCueArbitrationMode.LatestStart;
            cue.arbitrationGroup = Mathf.Clamp(group, 0, 15);
            cue.arbitrationTimeBase = SpectraModulationTimeBase.Bars;
            cue.arbitrationCycleLength = 1f;
            cue.arbitrationPhase = 0f;
            cue.arbitrationSeed = 0;
        }

        public static void ApplyDeterministicCycleArbitration(
            SpectraCueBlock cue,
            int group,
            float bars,
            int seed)
        {
            if (cue == null) return;
            cue.arbitrationMode = SpectraCueArbitrationMode.DeterministicCycle;
            cue.arbitrationGroup = Mathf.Clamp(group, 0, 15);
            cue.arbitrationTimeBase = SpectraModulationTimeBase.Bars;
            cue.arbitrationCycleLength = Mathf.Max(0.0001f, bars);
            cue.arbitrationPhase = 0f;
            cue.arbitrationSeed = seed;
        }

        public static void ClearArbitration(SpectraCueBlock cue)
        {
            if (cue == null) return;
            cue.arbitrationMode = SpectraCueArbitrationMode.Disabled;
            cue.arbitrationGroup = -1;
            cue.arbitrationTimeBase = SpectraModulationTimeBase.Bars;
            cue.arbitrationCycleLength = 1f;
            cue.arbitrationPhase = 0f;
            cue.arbitrationSeed = 0;
        }

        [MenuItem("SpectraOverdrive/Show Programmer/Add Starter Cue Layers")]
        private static void AddStarterLayersToSelectedShow()
        {
            SpectraShowAsset show = Selection.activeObject as SpectraShowAsset;
            if (show == null)
            {
                EditorUtility.DisplayDialog("SpectraOverdrive",
                    "Select a SpectraShowAsset first.", "OK");
                return;
            }
            Undo.RecordObject(show, "Add SpectraOverdrive cue layers");
            EnsureStarterLayers(show);
            EditorUtility.SetDirty(show);
        }

        private static SpectraCueLayer CreateLayer(
            string name,
            Color color,
            int priorityBias,
            int maximumActiveCues,
            bool pc,
            bool quest,
            bool ios,
            bool android)
        {
            return new SpectraCueLayer
            {
                name = name,
                displayColor = color,
                defaultEnabled = true,
                priorityBias = priorityBias,
                maximumActiveCues = maximumActiveCues,
                pcEnabled = pc,
                questEnabled = quest,
                iosEnabled = ios,
                androidEnabled = android
            };
        }
    }
}
