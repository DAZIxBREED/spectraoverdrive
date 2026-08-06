#if UNITY_EDITOR
using UnityEditor;
using UnityEngine;
using SpectraOverdrive;

namespace SpectraOverdrive.Editor
{
    public class SpectraOverdriveValidatorWindow : EditorWindow
    {
        [MenuItem("SpectraOverdrive/Validator")]
        public static void Open()
        {
            GetWindow<SpectraOverdriveValidatorWindow>("SpectraOverdrive");
        }

        private Vector2 _scroll;

        private void OnGUI()
        {
            EditorGUILayout.LabelField("SpectraOverdrive Validator", EditorStyles.boldLabel);
            EditorGUILayout.HelpBox(
                "Pre-alpha validator. It checks core scene components and basic fixture patch conflicts.",
                MessageType.Info
            );

            if (GUILayout.Button("Validate Open Scene"))
            {
                ValidateScene();
            }
        }

        private void ValidateScene()
        {
            var bus = FindObjectOfType<SpectraOverdriveBus>();
            var platform = FindObjectOfType<SpectraPlatformManager>();
            var cues = FindObjectOfType<SpectraCueController>();
            var fixtures = FindObjectsOfType<SpectraFixtureIdentity>();

            if (bus == null) Debug.LogWarning("[SpectraOverdrive] Missing SpectraOverdriveBus.");
            if (platform == null) Debug.LogWarning("[SpectraOverdrive] Missing SpectraPlatformManager.");
            if (cues == null) Debug.LogWarning("[SpectraOverdrive] Missing SpectraCueController.");

            int conflicts = 0;

            for (int i = 0; i < fixtures.Length; i++)
            {
                var a = fixtures[i];
                int aStart = a.startAddress;
                int aEnd = a.startAddress + a.channelCount - 1;

                if (aEnd > 512)
                {
                    Debug.LogError(
                        "[SpectraOverdrive] Fixture '" + a.fixtureName +
                        "' exceeds universe " + a.universe + " channel 512.",
                        a
                    );
                }

                for (int j = i + 1; j < fixtures.Length; j++)
                {
                    var b = fixtures[j];
                    if (a.universe != b.universe) continue;

                    int bStart = b.startAddress;
                    int bEnd = b.startAddress + b.channelCount - 1;
                    bool overlaps = aStart <= bEnd && bStart <= aEnd;

                    if (overlaps)
                    {
                        conflicts++;
                        Debug.LogWarning(
                            "[SpectraOverdrive] DMX overlap: '" + a.fixtureName +
                            "' and '" + b.fixtureName + "' in universe " + a.universe + "."
                        );
                    }
                }
            }

            Debug.Log(
                "[SpectraOverdrive] Validation complete. Fixtures: " +
                fixtures.Length + ", overlaps: " + conflicts + "."
            );
        }
    }
}
#endif
