using UnityEditor;
using UnityEngine;

namespace SpectraOverdrive.Editor
{
    [CustomEditor(typeof(SpectraShowAsset))]
    public sealed class SpectraShowAssetEditor : UnityEditor.Editor
    {
        public override void OnInspectorGUI()
        {
            EditorGUILayout.HelpBox(
                "Use the Show Programmer for timeline editing. The inspector remains available for complete low-level data access.",
                MessageType.Info);
            if (GUILayout.Button("Open Visual Show Programmer", GUILayout.Height(30f)))
                SpectraShowProgrammerWindow.Open();
            EditorGUILayout.BeginHorizontal();
            if (GUILayout.Button("Validate"))
            {
                Selection.activeObject = target;
                EditorApplication.ExecuteMenuItem("SpectraOverdrive/Show Programmer/Validate Cross-Platform Budgets");
            }
            if (GUILayout.Button("Compile"))
            {
                Selection.activeObject = target;
                EditorApplication.ExecuteMenuItem("SpectraOverdrive/Show Programmer/Compile Selected Show");
            }
            EditorGUILayout.EndHorizontal();
            EditorGUILayout.Space();
            DrawDefaultInspector();
        }
    }
}
