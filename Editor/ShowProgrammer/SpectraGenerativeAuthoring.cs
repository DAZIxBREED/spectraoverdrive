using UnityEngine;

namespace SpectraOverdrive.Editor
{
    public static class SpectraGenerativeAuthoring
    {
        public static void ApplyProbabilityCondition(SpectraCueBlock cue, float probability)
        {
            if (cue == null) return;
            cue.conditionMode = SpectraCueConditionMode.Probability;
            cue.conditionTimeBase = SpectraModulationTimeBase.Bars;
            cue.conditionCycleLength = 1f;
            cue.conditionPhase = 0f;
            cue.conditionProbability = Mathf.Clamp01(probability);
            cue.conditionEveryN = 1;
            cue.conditionCycleOffset = 0;
            cue.conditionInvert = false;
            if (cue.randomSeed == 0) cue.randomSeed = 140140;
        }

        public static void ApplyEveryNthCondition(SpectraCueBlock cue, int everyN)
        {
            if (cue == null) return;
            cue.conditionMode = SpectraCueConditionMode.EveryNthCycle;
            cue.conditionTimeBase = SpectraModulationTimeBase.Bars;
            cue.conditionCycleLength = 1f;
            cue.conditionPhase = 0f;
            cue.conditionEveryN = Mathf.Clamp(everyN, 1, 32);
            cue.conditionCycleOffset = 0;
            cue.conditionInvert = false;
        }

        public static void ApplyMacroCondition(SpectraCueBlock cue, int macroIndex, float threshold, bool above)
        {
            if (cue == null) return;
            cue.conditionMode = above
                ? SpectraCueConditionMode.MacroAbove
                : SpectraCueConditionMode.MacroBelow;
            cue.conditionMacroIndex = Mathf.Clamp(macroIndex, 0, 3);
            cue.conditionThreshold = Mathf.Clamp01(threshold);
            cue.conditionInvert = false;
        }

        public static void ClearCondition(SpectraCueBlock cue)
        {
            if (cue == null) return;
            cue.conditionMode = SpectraCueConditionMode.Disabled;
            cue.conditionTimeBase = SpectraModulationTimeBase.Bars;
            cue.conditionCycleLength = 1f;
            cue.conditionPhase = 0f;
            cue.conditionProbability = 1f;
            cue.conditionEveryN = 1;
            cue.conditionCycleOffset = 0;
            cue.conditionMacroIndex = -1;
            cue.conditionAudioBand = SpectraAudioBand.Bass;
            cue.conditionThreshold = 0.5f;
            cue.conditionInvert = false;
        }

        public static void ApplyCycleVariation(
            SpectraCueBlock cue,
            int group,
            int option,
            int optionCount)
        {
            ApplyVariation(cue, SpectraVariationSelectionMode.Cycle,
                group, option, optionCount);
        }

        public static void ApplySeededVariation(
            SpectraCueBlock cue,
            int group,
            int option,
            int optionCount)
        {
            ApplyVariation(cue, SpectraVariationSelectionMode.SeededRandom,
                group, option, optionCount);
            if (cue.variationSeed == 0) cue.variationSeed = 140071;
        }

        public static void ApplyMacroVariation(
            SpectraCueBlock cue,
            int group,
            int option,
            int optionCount,
            int macroIndex)
        {
            ApplyVariation(cue, SpectraVariationSelectionMode.MacroSelect,
                group, option, optionCount);
            cue.variationMacroIndex = Mathf.Clamp(macroIndex, 0, 3);
        }

        public static void ClearVariation(SpectraCueBlock cue)
        {
            if (cue == null) return;
            cue.variationMode = SpectraVariationSelectionMode.Disabled;
            cue.variationGroup = -1;
            cue.variationOption = 0;
            cue.variationOptionCount = 2;
            cue.variationTimeBase = SpectraModulationTimeBase.Bars;
            cue.variationCycleLength = 1f;
            cue.variationPhase = 0f;
            cue.variationSeed = 0;
            cue.variationMacroIndex = -1;
        }

        public static void EnsureStarterSnapshots(SpectraShowAsset show)
        {
            if (show == null) return;
            if (show.performanceMacros == null || show.performanceMacros.Length == 0)
            {
                show.performanceMacros = new[]
                {
                    Macro("Energy", new Color(1f, 0.18f, 0.72f, 1f)),
                    Macro("Motion", new Color(0.08f, 0.82f, 1f, 1f)),
                    Macro("Impact", new Color(1f, 0.58f, 0.08f, 1f)),
                    Macro("Audio Drive", new Color(0.28f, 1f, 0.42f, 1f))
                };
            }
            if (show.performanceMacroSnapshots != null
                && show.performanceMacroSnapshots.Length > 0) return;
            show.performanceMacroSnapshots = new[]
            {
                Snapshot("Warmup", new Vector4(0.45f, 0.55f, 0.35f, 0.4f), 0.5f,
                    new Color(0.35f, 0.55f, 1f, 1f)),
                Snapshot("Full Send", Vector4.one, 0.35f,
                    new Color(1f, 0.18f, 0.72f, 1f)),
                Snapshot("Breakdown", new Vector4(0.28f, 0.22f, 0.12f, 0.3f), 1.2f,
                    new Color(0.36f, 0.12f, 0.75f, 1f)),
                Snapshot("No Motion", new Vector4(0.8f, 0f, 0.75f, 0.7f), 0.25f,
                    new Color(1f, 0.6f, 0.12f, 1f))
            };
        }

        private static void ApplyVariation(
            SpectraCueBlock cue,
            SpectraVariationSelectionMode mode,
            int group,
            int option,
            int optionCount)
        {
            if (cue == null) return;
            cue.variationMode = mode;
            cue.variationGroup = Mathf.Clamp(group, 0, 15);
            cue.variationOptionCount = Mathf.Clamp(optionCount, 2, 8);
            cue.variationOption = Mathf.Clamp(option, 0, cue.variationOptionCount - 1);
            cue.variationTimeBase = SpectraModulationTimeBase.Bars;
            cue.variationCycleLength = 1f;
            cue.variationPhase = 0f;
            cue.variationMacroIndex = 0;
        }


        private static SpectraPerformanceMacro Macro(string name, Color color)
        {
            return new SpectraPerformanceMacro
            {
                name = name,
                description = "Starter synchronized performance macro.",
                defaultValue = 1f,
                smoothingSeconds = 0.12f,
                displayColor = color
            };
        }

        private static SpectraPerformanceMacroSnapshot Snapshot(
            string name,
            Vector4 values,
            float transition,
            Color color)
        {
            return new SpectraPerformanceMacroSnapshot
            {
                name = name,
                description = "Cross-platform synchronized operator look.",
                values = values,
                transitionSeconds = transition,
                displayColor = color
            };
        }
    }
}
