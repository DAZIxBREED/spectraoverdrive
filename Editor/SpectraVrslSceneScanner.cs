#if UNITY_EDITOR
using System.Text;
using UnityEditor;
using UnityEngine;
using SpectraOverdrive;

namespace SpectraOverdrive.Editor
{
    public class SpectraVrslSceneScanner : EditorWindow
    {
        private Vector2 _scroll;
        private string _report = "No scan has been run.";

        [MenuItem("SpectraOverdrive/VRSL Scene Scanner")]
        public static void Open()
        {
            GetWindow<SpectraVrslSceneScanner>("VRSL Scanner");
        }

        private void OnGUI()
        {
            EditorGUILayout.LabelField("VRSL Compatibility Scanner", EditorStyles.boldLabel);
            EditorGUILayout.HelpBox(
                "This scanner does not modify the scene. It finds likely VRSL objects, materials, and components and creates a migration report.",
                MessageType.Info
            );

            if (GUILayout.Button("Scan Open Scene"))
            {
                RunScan();
            }

            _scroll = EditorGUILayout.BeginScrollView(_scroll);
            EditorGUILayout.TextArea(_report, GUILayout.ExpandHeight(true));
            EditorGUILayout.EndScrollView();
        }

        private void RunScan()
        {
            Component[] allComponents = FindObjectsOfType<Component>(true);
            Renderer[] allRenderers = FindObjectsOfType<Renderer>(true);
            StringBuilder report = new StringBuilder();

            int componentMatches = 0;
            int materialMatches = 0;

            report.AppendLine("SpectraOverdrive VRSL migration scan");
            report.AppendLine("-------------------------------------");

            for (int i = 0; i < allComponents.Length; i++)
            {
                Component component = allComponents[i];
                if (component == null) continue;

                string typeName = component.GetType().FullName;
                string lower = typeName.ToLowerInvariant();

                if (lower.Contains("vrsl") || lower.Contains("stagelight") || lower.Contains("dmx"))
                {
                    componentMatches++;
                    report.AppendLine(
                        "Component: " + typeName +
                        " on " + GetHierarchyPath(component.transform)
                    );
                }
            }

            for (int i = 0; i < allRenderers.Length; i++)
            {
                Renderer renderer = allRenderers[i];
                if (renderer == null) continue;

                Material[] materials = renderer.sharedMaterials;
                for (int j = 0; j < materials.Length; j++)
                {
                    Material material = materials[j];
                    if (material == null || material.shader == null) continue;

                    string shaderName = material.shader.name;
                    string lower = shaderName.ToLowerInvariant();

                    if (lower.Contains("vrsl") || lower.Contains("stage light") || lower.Contains("dmx"))
                    {
                        materialMatches++;
                        report.AppendLine(
                            "Material: " + material.name +
                            " / Shader: " + shaderName +
                            " on " + GetHierarchyPath(renderer.transform)
                        );
                    }
                }
            }

            report.AppendLine();
            report.AppendLine("Likely VRSL-related components: " + componentMatches);
            report.AppendLine("Likely VRSL-related materials: " + materialMatches);
            report.AppendLine();
            report.AppendLine("This is a discovery report only. Automatic conversion comes after exact component-property mappings are implemented.");

            _report = report.ToString();
            Debug.Log(_report);
        }

        private string GetHierarchyPath(Transform target)
        {
            if (target == null) return "<null>";

            string path = target.name;
            Transform current = target.parent;

            while (current != null)
            {
                path = current.name + "/" + path;
                current = current.parent;
            }

            return path;
        }
    }
}
#endif
