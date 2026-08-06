using UdonSharp;
using UnityEngine;

namespace SpectraOverdrive
{
    [UdonBehaviourSyncMode(BehaviourSyncMode.None)]
    public class SpectraSceneStackController : UdonSharpBehaviour
    {
        public SpectraShowNetworkController networkController;
        [Range(0, 7)] public int sceneBank;
        public int selectedSceneOrdinal;
        public int selectedMarkerIndex = -1;
        public string selectedSceneName;
        public int sceneCount;

        private void Start()
        {
            RefreshSelection();
        }

        public void SelectNext()
        {
            RefreshCount();
            if (sceneCount <= 0) return;
            selectedSceneOrdinal = (selectedSceneOrdinal + 1) % sceneCount;
            RefreshSelection();
        }

        public void SelectPrevious()
        {
            RefreshCount();
            if (sceneCount <= 0) return;
            selectedSceneOrdinal = (selectedSceneOrdinal - 1 + sceneCount) % sceneCount;
            RefreshSelection();
        }

        public void TriggerSelected()
        {
            RefreshSelection();
            if (networkController == null || selectedMarkerIndex < 0) return;
            networkController.ScheduleHotCue(selectedMarkerIndex);
            SpectraShowRuntimePlayer player = networkController.GetActivePlayer();
            if (player != null
                && player.markerSceneAutoAdvance != null
                && selectedMarkerIndex < player.markerSceneAutoAdvance.Length
                && player.markerSceneAutoAdvance[selectedMarkerIndex])
                SelectNext();
        }

        public void SelectBank0() { SelectBank(0); }
        public void SelectBank1() { SelectBank(1); }
        public void SelectBank2() { SelectBank(2); }
        public void SelectBank3() { SelectBank(3); }
        public void SelectBank4() { SelectBank(4); }
        public void SelectBank5() { SelectBank(5); }
        public void SelectBank6() { SelectBank(6); }
        public void SelectBank7() { SelectBank(7); }

        public void SelectBank(int bank)
        {
            sceneBank = Mathf.Clamp(bank, 0, 7);
            selectedSceneOrdinal = 0;
            RefreshSelection();
        }

        public void RefreshSelection()
        {
            RefreshCount();
            selectedMarkerIndex = -1;
            selectedSceneName = string.Empty;
            if (sceneCount <= 0) return;
            selectedSceneOrdinal = Mathf.Clamp(selectedSceneOrdinal, 0, sceneCount - 1);
            SpectraShowRuntimePlayer player = ActivePlayer();
            if (player == null || player.markerTimes == null) return;
            for (int marker = 0; marker < player.markerTimes.Length; marker++)
            {
                if (!player.IsSceneUsable(marker, sceneBank)) continue;
                int rank = 0;
                int order = player.GetSceneOrder(marker);
                for (int other = 0; other < player.markerTimes.Length; other++)
                {
                    if (other == marker || !player.IsSceneUsable(other, sceneBank)) continue;
                    int otherOrder = player.GetSceneOrder(other);
                    if (otherOrder < order || (otherOrder == order && other < marker)) rank++;
                }
                if (rank != selectedSceneOrdinal) continue;
                selectedMarkerIndex = marker;
                selectedSceneName = player.markerNames != null
                    && marker < player.markerNames.Length
                    ? player.markerNames[marker]
                    : "Scene " + (selectedSceneOrdinal + 1);
                return;
            }
        }

        private void RefreshCount()
        {
            sceneCount = 0;
            SpectraShowRuntimePlayer player = ActivePlayer();
            if (player == null || player.markerTimes == null) return;
            for (int i = 0; i < player.markerTimes.Length; i++)
                if (player.IsSceneUsable(i, sceneBank)) sceneCount++;
        }

        private SpectraShowRuntimePlayer ActivePlayer()
        {
            return networkController == null ? null : networkController.GetActivePlayer();
        }
    }
}
