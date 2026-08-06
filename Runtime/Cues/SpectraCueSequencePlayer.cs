using UdonSharp;
using UnityEngine;
using VRC.SDKBase;

namespace SpectraOverdrive
{
    [UdonBehaviourSyncMode(BehaviourSyncMode.None)]
    public class SpectraCueSequencePlayer : UdonSharpBehaviour
    {
        public SpectraCueController controller;
        public SpectraCueSequence sequence;
        public SpectraFixtureGroupEffect groupEffect;

        public bool playing;
        public int currentStep;
        public double stepStartServerTime;

        public void StartSequence()
        {
            if (sequence == null || sequence.steps == null || sequence.steps.Length == 0)
            {
                return;
            }

            playing = true;
            currentStep = 0;
            stepStartServerTime = Networking.GetServerTimeInSeconds();
            ApplyCurrentStep();
        }

        public void StopSequence()
        {
            playing = false;
        }

        public void Update()
        {
            if (!playing || sequence == null || sequence.steps == null || sequence.steps.Length == 0)
            {
                return;
            }

            SpectraCueStep step = sequence.steps[currentStep];
            double now = Networking.GetServerTimeInSeconds();

            if (now - stepStartServerTime >= step.duration)
            {
                currentStep++;

                if (currentStep >= sequence.steps.Length)
                {
                    if (sequence.loop)
                    {
                        currentStep = 0;
                    }
                    else
                    {
                        playing = false;
                        return;
                    }
                }

                stepStartServerTime = now;
                ApplyCurrentStep();
            }
        }

        private void ApplyCurrentStep()
        {
            SpectraCueStep step = sequence.steps[currentStep];

            if (controller != null)
            {
                controller.cueDuration = step.duration;
                controller.StartCue(step.cueId);
            }

            if (groupEffect != null)
            {
                groupEffect.pattern = step.effectPattern;
                groupEffect.speed = step.effectSpeed;
            }
        }
    }
}
