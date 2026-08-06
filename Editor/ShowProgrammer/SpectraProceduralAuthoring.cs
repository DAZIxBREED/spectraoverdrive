using UnityEngine;

namespace SpectraOverdrive.Editor
{
    public static class SpectraProceduralAuthoring
    {
        public static void ApplyBeatPulse(SpectraCueBlock cue)
        {
            if (cue == null) return;
            cue.modulationWaveform = SpectraModulationWaveform.Pulse;
            cue.modulationTimeBase = SpectraModulationTimeBase.Beats;
            cue.modulationMode = SpectraAutomationMode.Multiply;
            cue.modulationCycleLength = 1f;
            cue.modulationPhase = 0f;
            cue.modulationDutyCycle = 0.22f;
            cue.modulationQuantizeSteps = 0;
            cue.modulationOffset = new Vector4(0.25f, 0.25f, 0.25f, 1f);
            cue.modulationDepth = new Vector4(0.75f, 0.75f, 0.75f, 0f);
        }

        public static void ApplyEightBarBreathing(SpectraCueBlock cue)
        {
            if (cue == null) return;
            cue.modulationWaveform = SpectraModulationWaveform.Sine;
            cue.modulationTimeBase = SpectraModulationTimeBase.Bars;
            cue.modulationMode = SpectraAutomationMode.Multiply;
            cue.modulationCycleLength = 8f;
            cue.modulationPhase = 0.75f;
            cue.modulationDutyCycle = 0.5f;
            cue.modulationQuantizeSteps = 0;
            cue.modulationOffset = new Vector4(0.55f, 0.55f, 0.55f, 1f);
            cue.modulationDepth = new Vector4(0.45f, 0.45f, 0.45f, 0f);
        }

        public static void ApplyDeterministicFlicker(SpectraCueBlock cue)
        {
            if (cue == null) return;
            cue.modulationWaveform = SpectraModulationWaveform.SampleAndHold;
            cue.modulationTimeBase = SpectraModulationTimeBase.Beats;
            cue.modulationMode = SpectraAutomationMode.Multiply;
            cue.modulationCycleLength = 0.25f;
            cue.modulationPhase = 0f;
            cue.modulationDutyCycle = 0.5f;
            cue.modulationQuantizeSteps = 6;
            cue.modulationOffset = new Vector4(0.55f, 0.55f, 0.55f, 1f);
            cue.modulationDepth = new Vector4(0.45f, 0.45f, 0.45f, 0f);
            if (cue.randomSeed == 0) cue.randomSeed = 120120;
        }

        public static void Clear(SpectraCueBlock cue)
        {
            if (cue == null) return;
            cue.modulationWaveform = SpectraModulationWaveform.Disabled;
            cue.modulationTimeBase = SpectraModulationTimeBase.Beats;
            cue.modulationMode = SpectraAutomationMode.Multiply;
            cue.modulationCycleLength = 1f;
            cue.modulationPhase = 0f;
            cue.modulationDutyCycle = 0.5f;
            cue.modulationQuantizeSteps = 0;
            cue.modulationOffset = Vector4.one;
            cue.modulationDepth = Vector4.zero;
        }
    }
}
