using UdonSharp;
using UnityEngine;
using VRC.SDKBase;
using VRC.Udon.Common.Interfaces;

namespace SpectraOverdrive
{
    [UdonBehaviourSyncMode(BehaviourSyncMode.Manual)]
    public class SpectraOperatorConsole : UdonSharpBehaviour
    {
        public SpectraOverdriveBus bus;
        public SpectraCueController cueController;
        public SpectraCueSequencePlayer sequencePlayer;
        public SpectraShowNetworkController networkController;
        public SpectraLiveOverrideLayer liveOverrides;

        [Header("Permissions")]
        public SpectraOperatorRole requiredRole = SpectraOperatorRole.LightingTech;
        public string[] allowedDisplayNames;
        public bool ownerOnly;

        [Header("Synced operator state")]
        [UdonSynced] public string activeOperator;
        [UdonSynced] public int selectedCue;
        [UdonSynced] public bool liveOverrideEnabled;

        public bool CanOperate()
        {
            VRCPlayerApi player = Networking.LocalPlayer;
            if (player == null) return false;

            if (ownerOnly && !Networking.IsOwner(gameObject))
            {
                return false;
            }

            if (allowedDisplayNames == null || allowedDisplayNames.Length == 0)
            {
                return true;
            }

            for (int i = 0; i < allowedDisplayNames.Length; i++)
            {
                if (allowedDisplayNames[i] == player.displayName)
                {
                    return true;
                }
            }

            return false;
        }

        public void TakeConsole()
        {
            if (!CanOperate()) return;

            Networking.SetOwner(Networking.LocalPlayer, gameObject);
            if (networkController != null) networkController.TakeControl();
            activeOperator = Networking.LocalPlayer.displayName;
            RequestSerialization();
        }

        public void TriggerSelectedCue()
        {
            if (!CanOperate() || cueController == null) return;

            TakeConsole();
            cueController.StartCue(selectedCue);
            liveOverrideEnabled = true;
            RequestSerialization();
        }

        public void StartSequence()
        {
            if (!CanOperate() || sequencePlayer == null) return;

            TakeConsole();
            sequencePlayer.StartSequence();
            liveOverrideEnabled = false;
            RequestSerialization();
        }

        public void StopSequence()
        {
            if (!CanOperate() || sequencePlayer == null) return;

            TakeConsole();
            sequencePlayer.StopSequence();
            RequestSerialization();
        }

        public void Blackout()
        {
            if (!CanOperate()) return;

            TakeConsole();
            if (networkController != null)
            {
                networkController.EnableEmergencyBlackout();
                return;
            }
            if (bus == null) return;
            bus.SetBlackout(true);
            SendCustomNetworkEvent(NetworkEventTarget.All, nameof(ApplyEmergencyBlackout));
        }

        public void ClearBlackout()
        {
            if (!CanOperate()) return;

            TakeConsole();
            if (networkController != null)
            {
                networkController.DisableEmergencyBlackout();
                return;
            }
            if (bus == null) return;
            bus.SetBlackout(false);
            SendCustomNetworkEvent(NetworkEventTarget.All, nameof(ApplyClearBlackout));
        }

        public void ApplyEmergencyBlackout()
        {
            if (bus != null) bus.SetBlackout(true);
        }

        public void ApplyClearBlackout()
        {
            if (bus != null) bus.SetBlackout(false);
        }

        public void PlayProgrammedShow()
        {
            if (!CanOperate() || networkController == null) return;
            TakeConsole();
            networkController.PlaySynchronized();
        }

        public void PauseProgrammedShow()
        {
            if (!CanOperate() || networkController == null) return;
            TakeConsole();
            networkController.PauseSynchronized();
        }

        public void StopProgrammedShow()
        {
            if (!CanOperate() || networkController == null) return;
            TakeConsole();
            networkController.StopSynchronized();
        }

        public void PreviousMarker()
        {
            if (!CanOperate() || networkController == null) return;
            TakeConsole();
            networkController.SeekPreviousMarker();
        }

        public void NextMarker()
        {
            if (!CanOperate() || networkController == null) return;
            TakeConsole();
            networkController.SeekNextMarker();
        }

        public void JumpToDrop()
        {
            if (!CanOperate() || networkController == null) return;
            TakeConsole();
            networkController.JumpToNextDrop();
        }

        public void TriggerHotCue()
        {
            if (!CanOperate() || networkController == null) return;
            TakeConsole();
            networkController.TriggerRequestedHotCue();
        }

        public void CancelPendingHotCue()
        {
            if (!CanOperate() || networkController == null) return;
            TakeConsole();
            networkController.CancelHotCue();
        }

        public void ClearLiveOverrides()
        {
            if (!CanOperate() || liveOverrides == null) return;
            TakeConsole();
            liveOverrides.ClearAll();
        }
    }
}
