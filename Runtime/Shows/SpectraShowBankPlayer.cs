using UdonSharp;
using UnityEngine;
using VRC.SDKBase;

namespace SpectraOverdrive
{
    [UdonBehaviourSyncMode(BehaviourSyncMode.Manual)]
    public class SpectraShowBankPlayer : UdonSharpBehaviour
    {
        public SpectraShowBank bank;
        public SpectraCueSequencePlayer sequencePlayer;
        public SpectraOverdriveBus bus;
        public SpectraLaserSafetyController laserSafety;

        [UdonSynced] public int selectedShowIndex;
        [UdonSynced] public string activeShowName;

        public void PlaySelectedShow()
        {
            if (bank == null || bank.shows == null || bank.shows.Length == 0)
            {
                return;
            }

            selectedShowIndex = Mathf.Clamp(selectedShowIndex, 0, bank.shows.Length - 1);
            SpectraShowEntry show = bank.shows[selectedShowIndex];
            if (show == null) return;

            Networking.SetOwner(Networking.LocalPlayer, gameObject);

            activeShowName = show.showName;

            if (bus != null)
            {
                bus.masterIntensity = show.masterIntensity;
            }

            if (laserSafety != null)
            {
                laserSafety.lasersEnabled = show.enableLasers;
                laserSafety.ApplySafety();
            }

            if (sequencePlayer != null)
            {
                sequencePlayer.sequence = show.sequence;
                sequencePlayer.StartSequence();
            }

            RequestSerialization();
        }

        public void StopShow()
        {
            if (sequencePlayer != null)
            {
                sequencePlayer.StopSequence();
            }

            activeShowName = "";
            RequestSerialization();
        }

        public void SelectNextShow()
        {
            if (bank == null || bank.shows == null || bank.shows.Length == 0) return;
            selectedShowIndex = (selectedShowIndex + 1) % bank.shows.Length;
        }

        public void SelectPreviousShow()
        {
            if (bank == null || bank.shows == null || bank.shows.Length == 0) return;
            selectedShowIndex--;
            if (selectedShowIndex < 0) selectedShowIndex = bank.shows.Length - 1;
        }
    }
}
