#if UNITY_EDITOR
using System.IO;
using System.Text;
using UnityEditor;
using UnityEngine;
using SpectraOverdrive;

namespace SpectraOverdrive.Editor
{
    public static class SpectraVrslConversionReport
    {
        [MenuItem("SpectraOverdrive/VRSL/Export Conversion Report")]
        public static void ExportReport()
        {
            string path = EditorUtility.SaveFilePanel(
                "Export VRSL Conversion Report",
                "",
                "SpectraOverdrive-VRSL-Conversion.txt",
                "txt"
            );

            if (string.IsNullOrEmpty(path)) return;

            Component[] components = Object.FindObjectsOfType<Component>(true);
            Renderer[] renderers = Object.FindObjectsOfType<Renderer>(true);

            StringBuilder sb = new StringBuilder();
            sb.AppendLine("SpectraOverdrive VRSL Conversion Report");
            sb.AppendLine("=====================================");
            sb.AppendLine();

            for (int i = 0; i < components.Length; i++)
            {
                Component component = components[i];
                if (component == null) continue;

                string typeName = component.GetType().FullName;
                string lower = typeName.ToLowerInvariant();

                if (lower.Contains("vrsl") || lower.Contains("dmx") || lower.Contains("stagelight"))
                {
                    sb.AppendLine("Component candidate: " + typeName + " on " + GetPath(component.transform));
                }
            }

            for (int i = 0; i < renderers.Length; i++)
            {
                Renderer renderer = renderers[i];
                if (renderer == null) continue;

                Material[] mats = renderer.sharedMaterials;
                for (int j = 0; j < mats.Length; j++)
                {
                    Material mat = mats[j];
                    if (mat == null || mat.shader == null) continue;

                    string shader = mat.shader.name;
                    string lower = shader.ToLowerInvariant();

                    if (lower.Contains("vrsl") || lower.Contains("dmx") || lower.Contains("stage"))
                    {
                        sb.AppendLine("Material candidate: " + mat.name + " / " + shader + " on " + GetPath(renderer.transform));
                    }
                }
            }

            File.WriteAllText(path, sb.ToString());
            Debug.Log("[SpectraOverdrive] Exported VRSL conversion report to " + path);
        }

        private static string GetPath(Transform target)
        {
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
