using UnityEditor;
using UnityEngine;

namespace SpectraOverdrive.Editor
{
    public sealed class SpectraPlatformSimulationWindow : EditorWindow
    {
        private SpectraShowRuntimePlayer player;
        private SpectraPlatformKind platform = SpectraPlatformKind.Quest;
        private int qualityLevel = 1;
        private float previewTime;

        [MenuItem("SpectraOverdrive/Show Programmer/Open Platform Simulator")]
        private static void Open()
        {
            SpectraPlatformSimulationWindow window =
                GetWindow<SpectraPlatformSimulationWindow>("Spectra Platform Simulator");
            window.minSize = new Vector2(430f, 285f);
        }

        private void OnGUI()
        {
            EditorGUILayout.LabelField("SpectraOverdrive 1.5 Platform Simulator", EditorStyles.boldLabel);
            EditorGUILayout.HelpBox(
                "Evaluates the baked player with the exact PCVR, Quest, iOS, or Android cue, fixture, shader, beam, fallback, capability, and automation path. This is an editor preview; device testing is still required.",
                MessageType.Info);
            player = (SpectraShowRuntimePlayer)EditorGUILayout.ObjectField(
                "Runtime Player", player, typeof(SpectraShowRuntimePlayer), true);
            platform = (SpectraPlatformKind)EditorGUILayout.EnumPopup("Simulated Platform", platform);
            qualityLevel = EditorGUILayout.IntSlider("Quality Tier", qualityLevel, 0, 3);
            float maximum = player == null ? 60f : Mathf.Max(0.01f, player.durationSeconds);
            previewTime = EditorGUILayout.Slider("Show Time", previewTime, 0f, maximum);

            EditorGUI.BeginDisabledGroup(player == null);
            if (GUILayout.Button("Apply Platform Simulation", GUILayout.Height(32f)))
                ApplySimulation();
            EditorGUI.EndDisabledGroup();

            if (player == null) return;
            EditorGUILayout.Space();
            EditorGUILayout.LabelField("Active cues", player.activeCueCount.ToString());
            EditorGUILayout.LabelField("Dropped by budget", player.droppedCueCount.ToString());
            EditorGUILayout.LabelField("Fixture budget", player.activeFixtureBudget.ToString());
            EditorGUILayout.LabelField("Transparent beam budget", player.activeTransparentBeamBudget.ToString());
            EditorGUILayout.LabelField("Shader tier", player.activeShaderQualityTier.ToString());
            EditorGUILayout.LabelField("Audio update divider", player.activeAudioReactiveUpdateDivider.ToString());
        }

        private void ApplySimulation()
        {
            Undo.RecordObject(player, "Simulate Spectra Platform");
            player.localPlatform = platform == SpectraPlatformKind.Unknown
                ? SpectraPlatformKind.PC : platform;
            if (player.qualityController != null)
            {
                Undo.RecordObject(player.qualityController, "Simulate Spectra Quality");
                player.qualityController.qualityLevel = qualityLevel;
                player.qualityController.ApplyQuality();
                EditorUtility.SetDirty(player.qualityController);
            }
            player.ApplyAtTime(Mathf.Clamp(previewTime, 0f, player.durationSeconds));
            EditorUtility.SetDirty(player);
            SceneView.RepaintAll();
            Repaint();
        }
    }
}
