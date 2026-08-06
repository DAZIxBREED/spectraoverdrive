using UdonSharp;
using UnityEngine;
using TMPro;
using UnityEngine.UI;

namespace SpectraOverdrive
{
    [UdonBehaviourSyncMode(BehaviourSyncMode.None)]
    public class SpectraOperatorUiController : UdonSharpBehaviour
    {
        public SpectraOperatorConsole console;
        public SpectraShowBankPlayer showPlayer;
        public TMP_Text statusText;
        public TMP_Text selectedShowText;
        public Slider masterIntensitySlider;
        public Toggle blackoutToggle;
        public Toggle laserToggle;
        public Toggle strobeToggle;

        public void RefreshUi()
        {
            if (console != null && statusText != null)
            {
                string operatorName = string.IsNullOrEmpty(console.activeOperator)
                    ? "No operator"
                    : console.activeOperator;

                statusText.text =
                    "Operator: " + operatorName + "\n" +
                    "Live override: " + (console.liveOverrideEnabled ? "ON" : "OFF");
            }

            if (showPlayer != null && selectedShowText != null)
            {
                selectedShowText.text =
                    "Show " + showPlayer.selectedShowIndex + "\n" +
                    showPlayer.activeShowName;
            }
            if (console != null && console.networkController != null && statusText != null)
            {
                SpectraShowNetworkController network = console.networkController;
                statusText.text =
                    "Operator: " + (string.IsNullOrEmpty(network.activeOperatorDisplayName)
                        ? "No operator" : network.activeOperatorDisplayName) + "\n"
                    + "State: " + ((SpectraShowPlaybackState)network.playbackState) + "\n"
                    + "Time: " + network.calculatedShowTime.ToString("0.000") + "\n"
                    + "Sync: " + network.syncStatus;
            }
        }

        public void ApplyMasterIntensity()
        {
            if (console == null || masterIntensitySlider == null) return;
            if (console.networkController != null)
            {
                console.networkController.SetSynchronizedMasterIntensity(masterIntensitySlider.value);
                return;
            }
            if (console.bus != null) console.bus.SetMasterIntensity(masterIntensitySlider.value);
        }

        public void ApplyBlackoutToggle()
        {
            if (console == null || blackoutToggle == null) return;

            if (blackoutToggle.isOn) console.Blackout();
            else console.ClearBlackout();
        }

        public void ApplyLaserToggle()
        {
            if (console != null && console.networkController != null && laserToggle != null)
            {
                console.networkController.SetSynchronizedLasers(laserToggle.isOn);
                return;
            }
            if (showPlayer == null || showPlayer.laserSafety == null || laserToggle == null) return;
            showPlayer.laserSafety.lasersEnabled = laserToggle.isOn;
            showPlayer.laserSafety.ApplySafety();
        }

        public void ApplyStrobeToggle()
        {
            if (console == null || console.networkController == null || strobeToggle == null) return;
            console.networkController.SetSynchronizedStrobes(strobeToggle.isOn);
        }

        public void NextShow()
        {
            if (showPlayer != null)
            {
                showPlayer.SelectNextShow();
                RefreshUi();
            }
        }

        public void PreviousShow()
        {
            if (showPlayer != null)
            {
                showPlayer.SelectPreviousShow();
                RefreshUi();
            }
        }

        public void PlayShow()
        {
            if (showPlayer != null)
            {
                showPlayer.PlaySelectedShow();
                RefreshUi();
            }
        }

        public void StopShow()
        {
            if (showPlayer != null)
            {
                showPlayer.StopShow();
                RefreshUi();
            }
        }
    }
}
