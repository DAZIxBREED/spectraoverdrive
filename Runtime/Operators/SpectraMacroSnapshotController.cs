using UdonSharp;
using UnityEngine;

namespace SpectraOverdrive
{
    [UdonBehaviourSyncMode(BehaviourSyncMode.None)]
    public class SpectraMacroSnapshotController : UdonSharpBehaviour
    {
        public SpectraShowNetworkController networkController;
        public int selectedSnapshotIndex;
        public string selectedSnapshotName;
        public Color selectedSnapshotColor = Color.cyan;
        public Vector4 selectedSnapshotValues = Vector4.one;
        public float selectedTransitionSeconds;
        public int snapshotCount;

        private void Start()
        {
            Refresh();
        }

        public void SelectNext()
        {
            RefreshCount();
            if (snapshotCount <= 0) return;
            selectedSnapshotIndex = (selectedSnapshotIndex + 1) % snapshotCount;
            Refresh();
        }

        public void SelectPrevious()
        {
            RefreshCount();
            if (snapshotCount <= 0) return;
            selectedSnapshotIndex = (selectedSnapshotIndex - 1 + snapshotCount) % snapshotCount;
            Refresh();
        }

        public void RecallSelected()
        {
            Refresh();
            if (networkController == null || snapshotCount <= 0) return;
            networkController.requestedPerformanceMacroSnapshotIndex = selectedSnapshotIndex;
            networkController.RecallRequestedPerformanceMacroSnapshot();
        }

        public void SelectSnapshot0() { SelectSnapshot(0); }
        public void SelectSnapshot1() { SelectSnapshot(1); }
        public void SelectSnapshot2() { SelectSnapshot(2); }
        public void SelectSnapshot3() { SelectSnapshot(3); }
        public void SelectSnapshot4() { SelectSnapshot(4); }
        public void SelectSnapshot5() { SelectSnapshot(5); }
        public void SelectSnapshot6() { SelectSnapshot(6); }
        public void SelectSnapshot7() { SelectSnapshot(7); }
        public void SelectSnapshot8() { SelectSnapshot(8); }
        public void SelectSnapshot9() { SelectSnapshot(9); }
        public void SelectSnapshot10() { SelectSnapshot(10); }
        public void SelectSnapshot11() { SelectSnapshot(11); }
        public void SelectSnapshot12() { SelectSnapshot(12); }
        public void SelectSnapshot13() { SelectSnapshot(13); }
        public void SelectSnapshot14() { SelectSnapshot(14); }
        public void SelectSnapshot15() { SelectSnapshot(15); }

        public void RecallSnapshot0() { RecallSnapshot(0); }
        public void RecallSnapshot1() { RecallSnapshot(1); }
        public void RecallSnapshot2() { RecallSnapshot(2); }
        public void RecallSnapshot3() { RecallSnapshot(3); }
        public void RecallSnapshot4() { RecallSnapshot(4); }
        public void RecallSnapshot5() { RecallSnapshot(5); }
        public void RecallSnapshot6() { RecallSnapshot(6); }
        public void RecallSnapshot7() { RecallSnapshot(7); }
        public void RecallSnapshot8() { RecallSnapshot(8); }
        public void RecallSnapshot9() { RecallSnapshot(9); }
        public void RecallSnapshot10() { RecallSnapshot(10); }
        public void RecallSnapshot11() { RecallSnapshot(11); }
        public void RecallSnapshot12() { RecallSnapshot(12); }
        public void RecallSnapshot13() { RecallSnapshot(13); }
        public void RecallSnapshot14() { RecallSnapshot(14); }
        public void RecallSnapshot15() { RecallSnapshot(15); }

        public void Refresh()
        {
            RefreshCount();
            SpectraShowRuntimePlayer player = ActivePlayer();
            if (player == null || snapshotCount <= 0)
            {
                selectedSnapshotIndex = 0;
                selectedSnapshotName = "No macro snapshots";
                selectedSnapshotColor = Color.gray;
                selectedSnapshotValues = Vector4.one;
                selectedTransitionSeconds = 0f;
                return;
            }
            selectedSnapshotIndex = Mathf.Clamp(selectedSnapshotIndex, 0, snapshotCount - 1);
            selectedSnapshotName = player.GetPerformanceMacroSnapshotName(selectedSnapshotIndex);
            selectedSnapshotColor = player.GetPerformanceMacroSnapshotColor(selectedSnapshotIndex);
            selectedSnapshotValues = player.GetPerformanceMacroSnapshotValues(selectedSnapshotIndex);
            selectedTransitionSeconds =
                player.GetPerformanceMacroSnapshotTransitionSeconds(selectedSnapshotIndex);
        }

        private void SelectSnapshot(int index)
        {
            RefreshCount();
            if (snapshotCount <= 0) return;
            selectedSnapshotIndex = Mathf.Clamp(index, 0, snapshotCount - 1);
            Refresh();
        }

        private void RecallSnapshot(int index)
        {
            SelectSnapshot(index);
            RecallSelected();
        }

        private void RefreshCount()
        {
            SpectraShowRuntimePlayer player = ActivePlayer();
            snapshotCount = player == null ? 0 : player.GetPerformanceMacroSnapshotCount();
        }

        private SpectraShowRuntimePlayer ActivePlayer()
        {
            return networkController == null ? null : networkController.GetActivePlayer();
        }
    }
}
