#if UNITY_EDITOR
using System.IO;
using System.Text;
using UnityEditor;
using UnityEngine;
using SpectraOverdrive;

namespace SpectraOverdrive.Editor
{
    public class SpectraPatchWindow : EditorWindow
    {
        private int _universe = 1;
        private int _startAddress = 1;
        private int _spacing = 0;

        [MenuItem("SpectraOverdrive/Patch Manager")]
        public static void Open()
        {
            GetWindow<SpectraPatchWindow>("Spectra Patch");
        }

        private void OnGUI()
        {
            EditorGUILayout.LabelField("Fixture Patching", EditorStyles.boldLabel);
            _universe = EditorGUILayout.IntSlider("Universe", _universe, 1, 9);
            _startAddress = EditorGUILayout.IntSlider("Start Address", _startAddress, 1, 512);
            _spacing = EditorGUILayout.IntSlider("Extra Spacing", _spacing, 0, 32);

            if (GUILayout.Button("Auto-Patch Selected Fixtures"))
            {
                AutoPatch();
            }

            if (GUILayout.Button("Export Scene Patch CSV"))
            {
                ExportCsv();
            }
        }

        private void AutoPatch()
        {
            GameObject[] selected = Selection.gameObjects;
            int address = _startAddress;
            int universe = _universe;

            for (int i = 0; i < selected.Length; i++)
            {
                SpectraFixtureIdentity fixture = selected[i].GetComponent<SpectraFixtureIdentity>();
                if (fixture == null) continue;

                if (address + fixture.channelCount - 1 > 512)
                {
                    universe++;
                    address = 1;
                }

                if (universe > 9)
                {
                    Debug.LogError("[SpectraOverdrive] Auto-patch exceeded universe 9.");
                    break;
                }

                Undo.RecordObject(fixture, "Auto Patch Spectra Fixture");
                fixture.universe = universe;
                fixture.startAddress = address;
                EditorUtility.SetDirty(fixture);

                address += fixture.channelCount + _spacing;
            }

            Debug.Log("[SpectraOverdrive] Auto-patched selected fixtures.");
        }

        private void ExportCsv()
        {
            string path = EditorUtility.SaveFilePanel(
                "Export SpectraOverdrive Patch",
                "",
                "SpectraOverdrive-Patch.csv",
                "csv"
            );

            if (string.IsNullOrEmpty(path)) return;

            SpectraFixtureIdentity[] fixtures = FindObjectsOfType<SpectraFixtureIdentity>();
            StringBuilder sb = new StringBuilder();
            sb.AppendLine("Fixture ID,Fixture Name,Profile,Universe,Start Address,Channel Count,Group,Receiver Zone");

            for (int i = 0; i < fixtures.Length; i++)
            {
                SpectraFixtureIdentity f = fixtures[i];
                sb.AppendLine(
                    f.fixtureId + "," +
                    Escape(f.fixtureName) + "," +
                    Escape(f.fixtureProfile) + "," +
                    f.universe + "," +
                    f.startAddress + "," +
                    f.channelCount + "," +
                    f.primaryGroup + "," +
                    f.receiverZone
                );
            }

            File.WriteAllText(path, sb.ToString(), Encoding.UTF8);
            Debug.Log("[SpectraOverdrive] Exported patch CSV: " + path);
        }

        private string Escape(string value)
        {
            if (value == null) return "";
            return "\"" + value.Replace("\"", "\"\"") + "\"";
        }
    }
}
#endif
