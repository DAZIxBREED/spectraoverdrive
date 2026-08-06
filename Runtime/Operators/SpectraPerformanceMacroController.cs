using UdonSharp;
using UnityEngine;

namespace SpectraOverdrive
{
    [UdonBehaviourSyncMode(BehaviourSyncMode.None)]
    public class SpectraPerformanceMacroController : UdonSharpBehaviour
    {
        public SpectraShowNetworkController networkController;
        [Range(0, 3)] public int selectedMacro;
        [Range(0f, 1f)] public float faderValue = 1f;
        [Range(0.01f, 0.5f)] public float nudgeAmount = 0.05f;
        public string selectedMacroName;
        public Color selectedMacroColor = Color.magenta;

        private void Start()
        {
            Refresh();
        }

        public void SelectNext()
        {
            selectedMacro = (selectedMacro + 1) % 4;
            Refresh();
        }

        public void SelectPrevious()
        {
            selectedMacro = (selectedMacro + 3) % 4;
            Refresh();
        }

        public void PushFader()
        {
            if (networkController == null) return;
            networkController.requestedPerformanceMacroIndex = selectedMacro;
            networkController.requestedPerformanceMacroValue = Mathf.Clamp01(faderValue);
            networkController.SetRequestedPerformanceMacro();
        }

        public void NudgeUp()
        {
            faderValue = Mathf.Clamp01(faderValue + nudgeAmount);
            PushFader();
        }

        public void NudgeDown()
        {
            faderValue = Mathf.Clamp01(faderValue - nudgeAmount);
            PushFader();
        }

        public void ResetAll()
        {
            if (networkController != null) networkController.ResetPerformanceMacros();
            Refresh();
        }

        public void Refresh()
        {
            SpectraShowRuntimePlayer player = networkController == null
                ? null : networkController.GetActivePlayer();
            if (player == null)
            {
                selectedMacroName = "Macro " + (selectedMacro + 1);
                selectedMacroColor = Color.magenta;
                faderValue = 1f;
                return;
            }
            selectedMacroName = player.performanceMacroNames != null
                && selectedMacro < player.performanceMacroNames.Length
                ? player.performanceMacroNames[selectedMacro]
                : "Macro " + (selectedMacro + 1);
            selectedMacroColor = player.performanceMacroColors != null
                && selectedMacro < player.performanceMacroColors.Length
                ? player.performanceMacroColors[selectedMacro]
                : Color.magenta;
            faderValue = networkController.ResolvePerformanceMacro(
                selectedMacro,
                VRC.SDKBase.Networking.GetServerTimeInSeconds());
        }
    }
}
