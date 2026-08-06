using System.Text;
using UnityEditor;
using UnityEngine;

namespace SpectraOverdrive.Editor
{
    public sealed class SpectraRuntimeShowBakerWindow : EditorWindow
    {
        [SerializeField] private SpectraShowAsset show;
        [SerializeField] private SpectraShowRuntimePlayer player;
        [SerializeField] private bool mapGroupsByRuntimeId = true;

        [MenuItem("SpectraOverdrive/Show Programmer/Bake Show Into Runtime Player")]
        public static void Open()
        {
            SpectraRuntimeShowBakerWindow window = GetWindow<SpectraRuntimeShowBakerWindow>();
            window.titleContent = new GUIContent("Spectra Runtime Baker");
            window.minSize = new Vector2(430f, 190f);
            window.Show();
        }

        private void OnGUI()
        {
            EditorGUILayout.LabelField("Bake Udon-Safe Runtime Show", EditorStyles.boldLabel);
            EditorGUILayout.HelpBox(
                "Compiles the editable show and serializes primitive fields and flat arrays directly onto the UdonSharp runtime player.",
                MessageType.Info);
            show = (SpectraShowAsset)EditorGUILayout.ObjectField("Show Asset", show, typeof(SpectraShowAsset), false);
            player = (SpectraShowRuntimePlayer)EditorGUILayout.ObjectField("Runtime Player", player, typeof(SpectraShowRuntimePlayer), true);
            mapGroupsByRuntimeId = EditorGUILayout.Toggle("Map Groups By Runtime ID", mapGroupsByRuntimeId);
            EditorGUILayout.Space();
            using (new EditorGUI.DisabledScope(show == null || player == null))
            {
                if (GUILayout.Button("Validate, Compile, and Bake", GUILayout.Height(34f)))
                    Bake(show, player, mapGroupsByRuntimeId, true);
            }
        }

        public static bool Bake(SpectraShowAsset source, SpectraShowRuntimePlayer target, bool autoMapGroups, bool showDialog)
        {
            if (source == null || target == null) return false;
            SpectraCompiledShow compiled;
            try { compiled = SpectraShowCompiler.Compile(source); }
            catch (System.Exception exception)
            {
                if (showDialog) EditorUtility.DisplayDialog("SpectraOverdrive Bake Failed", exception.Message, "OK");
                return false;
            }

            Undo.RecordObject(target, "Bake SpectraOverdrive Runtime Show");
            SpectraShowCompiler.ApplyToRuntimePlayer(compiled, target);
            StringBuilder warnings = new StringBuilder();
            if (autoMapGroups)
            {
                SpectraFixtureGroup[] sceneGroups = Object.FindObjectsOfType<SpectraFixtureGroup>(true);
                SpectraFixtureGroup[] mapped = new SpectraFixtureGroup[compiled.runtimeGroupIds.Length];
                for (int index = 0; index < mapped.Length; index++)
                {
                    int runtimeId = compiled.runtimeGroupIds[index];
                    for (int candidate = 0; candidate < sceneGroups.Length; candidate++)
                        if (sceneGroups[candidate] != null && sceneGroups[candidate].groupId == runtimeId)
                        {
                            mapped[index] = sceneGroups[candidate];
                            break;
                        }
                    if (mapped[index] == null)
                        warnings.AppendLine("- Missing scene fixture group with runtime ID " + runtimeId + ".");
                }
                target.groups = mapped;
            }
            EditorUtility.SetDirty(target);
            if (target.gameObject.scene.IsValid()) UnityEditor.SceneManagement.EditorSceneManager.MarkSceneDirty(target.gameObject.scene);

            if (showDialog)
            {
                string message = "Baked " + compiled.CueCount + " cues, " + compiled.markerTimes.Length
                    + " markers, and " + compiled.loopStarts.Length + " loops into " + target.name + ".";
                if (warnings.Length > 0) message += "\n\nGroup mapping warnings:\n" + warnings;
                EditorUtility.DisplayDialog("SpectraOverdrive Runtime Bake", message, "OK");
            }
            return true;
        }
    }
}
