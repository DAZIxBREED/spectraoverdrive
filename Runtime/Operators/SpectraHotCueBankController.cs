using UdonSharp;
using UnityEngine;

namespace SpectraOverdrive
{
    [UdonBehaviourSyncMode(BehaviourSyncMode.None)]
    public class SpectraHotCueBankController : UdonSharpBehaviour
    {
        public SpectraShowNetworkController networkController;
        public int selectedHotCueOrdinal;
        public int selectedMarkerIndex = -1;
        public string selectedHotCueName;
        public int hotCueCount;

        private void Start()
        {
            RefreshSelection();
        }

        public void SelectNext()
        {
            RefreshCount();
            if (hotCueCount <= 0) return;
            selectedHotCueOrdinal = (selectedHotCueOrdinal + 1) % hotCueCount;
            RefreshSelection();
        }

        public void SelectPrevious()
        {
            RefreshCount();
            if (hotCueCount <= 0) return;
            selectedHotCueOrdinal = (selectedHotCueOrdinal - 1 + hotCueCount) % hotCueCount;
            RefreshSelection();
        }

        public void TriggerSelected()
        {
            RefreshSelection();
            if (networkController == null || selectedMarkerIndex < 0) return;
            networkController.requestedHotCueMarkerIndex = selectedMarkerIndex;
            networkController.TriggerRequestedHotCue();
        }

        public void CancelPending()
        {
            if (networkController != null) networkController.CancelHotCue();
        }

        public void RefreshSelection()
        {
            RefreshCount();
            selectedMarkerIndex = -1;
            selectedHotCueName = string.Empty;
            if (hotCueCount <= 0) return;
            selectedHotCueOrdinal = Mathf.Clamp(selectedHotCueOrdinal, 0, hotCueCount - 1);
            SpectraShowRuntimePlayer player = ActivePlayer();
            if (player == null || player.markerHotCues == null) return;
            int ordinal = 0;
            for (int marker = 0; marker < player.markerHotCues.Length; marker++)
            {
                if (!player.markerHotCues[marker]) continue;
                if (ordinal == selectedHotCueOrdinal)
                {
                    selectedMarkerIndex = marker;
                    selectedHotCueName = player.markerNames != null
                        && marker < player.markerNames.Length
                        ? player.markerNames[marker] : "Hot Cue " + (ordinal + 1);
                    return;
                }
                ordinal++;
            }
        }

        private void RefreshCount()
        {
            hotCueCount = 0;
            SpectraShowRuntimePlayer player = ActivePlayer();
            if (player == null || player.markerHotCues == null) return;
            for (int i = 0; i < player.markerHotCues.Length; i++)
                if (player.markerHotCues[i]) hotCueCount++;
        }

        private SpectraShowRuntimePlayer ActivePlayer()
        {
            return networkController == null ? null : networkController.GetActivePlayer();
        }
    }
}
