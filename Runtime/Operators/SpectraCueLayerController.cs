using UdonSharp;
using UnityEngine;

namespace SpectraOverdrive
{
    [UdonBehaviourSyncMode(BehaviourSyncMode.None)]
    public class SpectraCueLayerController : UdonSharpBehaviour
    {
        public SpectraShowNetworkController networkController;
        [Range(0, 15)] public int selectedLayerIndex;
        public string selectedLayerName;
        public Color selectedLayerColor = Color.white;
        public bool selectedLayerEnabled;
        public bool selectedLayerSoloed;
        public int layerCount;

        private void Start()
        {
            RefreshSelection();
        }

        public void SelectNext()
        {
            RefreshCount();
            if (layerCount <= 0) return;
            selectedLayerIndex = (selectedLayerIndex + 1) % layerCount;
            RefreshSelection();
        }

        public void SelectPrevious()
        {
            RefreshCount();
            if (layerCount <= 0) return;
            selectedLayerIndex = (selectedLayerIndex - 1 + layerCount) % layerCount;
            RefreshSelection();
        }

        public void ToggleSelected()
        {
            SpectraShowRuntimePlayer player = ActivePlayer();
            if (networkController == null || player == null
                || !player.IsCueLayerUsable(selectedLayerIndex)) return;
            networkController.ToggleCueLayer(selectedLayerIndex);
            RefreshSelection();
        }

        public void EnableSelected()
        {
            if (networkController == null) return;
            networkController.SetCueLayerEnabled(selectedLayerIndex, true);
            RefreshSelection();
        }

        public void DisableSelected()
        {
            if (networkController == null) return;
            networkController.SetCueLayerEnabled(selectedLayerIndex, false);
            RefreshSelection();
        }

        public void SoloSelected()
        {
            if (networkController == null) return;
            networkController.SoloCueLayer(selectedLayerIndex);
            RefreshSelection();
        }

        public void ClearSolo()
        {
            if (networkController == null) return;
            networkController.ClearCueLayerSolo();
            RefreshSelection();
        }

        public void ResetLayers()
        {
            if (networkController == null) return;
            networkController.ResetCueLayers();
            RefreshSelection();
        }

        public void SelectLayer0() { SelectLayer(0); }
        public void SelectLayer1() { SelectLayer(1); }
        public void SelectLayer2() { SelectLayer(2); }
        public void SelectLayer3() { SelectLayer(3); }
        public void SelectLayer4() { SelectLayer(4); }
        public void SelectLayer5() { SelectLayer(5); }
        public void SelectLayer6() { SelectLayer(6); }
        public void SelectLayer7() { SelectLayer(7); }
        public void SelectLayer8() { SelectLayer(8); }
        public void SelectLayer9() { SelectLayer(9); }
        public void SelectLayer10() { SelectLayer(10); }
        public void SelectLayer11() { SelectLayer(11); }
        public void SelectLayer12() { SelectLayer(12); }
        public void SelectLayer13() { SelectLayer(13); }
        public void SelectLayer14() { SelectLayer(14); }
        public void SelectLayer15() { SelectLayer(15); }

        public void SelectLayer(int index)
        {
            RefreshCount();
            if (layerCount <= 0) return;
            selectedLayerIndex = Mathf.Clamp(index, 0, layerCount - 1);
            RefreshSelection();
        }

        public void RefreshSelection()
        {
            RefreshCount();
            SpectraShowRuntimePlayer player = ActivePlayer();
            if (player == null || layerCount <= 0)
            {
                selectedLayerName = string.Empty;
                selectedLayerColor = Color.white;
                selectedLayerEnabled = false;
                selectedLayerSoloed = false;
                return;
            }
            selectedLayerIndex = Mathf.Clamp(selectedLayerIndex, 0, layerCount - 1);
            selectedLayerName = player.GetCueLayerName(selectedLayerIndex);
            selectedLayerColor = player.GetCueLayerColor(selectedLayerIndex);
            int bit = 1 << selectedLayerIndex;
            selectedLayerEnabled = (player.cueLayerEnabledMask & bit) != 0;
            selectedLayerSoloed = (player.cueLayerSoloMask & bit) != 0;
        }

        private void RefreshCount()
        {
            SpectraShowRuntimePlayer player = ActivePlayer();
            layerCount = player == null ? 0 : player.GetCueLayerCount();
        }

        private SpectraShowRuntimePlayer ActivePlayer()
        {
            return networkController == null ? null : networkController.GetActivePlayer();
        }
    }
}
