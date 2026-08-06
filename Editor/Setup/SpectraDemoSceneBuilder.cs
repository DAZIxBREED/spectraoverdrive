using System.Collections.Generic;
using System.IO;
using UnityEditor;
using UnityEditor.SceneManagement;
using UnityEngine;
using UnityEngine.SceneManagement;

namespace SpectraOverdrive.Editor
{
    public class SpectraDemoSceneBuilder : EditorWindow
    {
        private SpectraShowAsset _show;
        private int _fixturesPerGroup = 6;

        [MenuItem("SpectraOverdrive/Show Programmer/Create Playable Demo Scene")]
        private static void Open()
        {
            GetWindow<SpectraDemoSceneBuilder>("Spectra Demo Scene");
        }

        private void OnGUI()
        {
            EditorGUILayout.LabelField("Playable Cross-Platform Demo Scene", EditorStyles.boldLabel);
            EditorGUILayout.HelpBox(
                "Creates a clean scene with real Spectra fixture groups, mobile-safe emissive visualizers, a camera, and the complete synchronized 1.5 production rig. Unity will ask before replacing an unsaved scene.",
                MessageType.Info);
            _show = (SpectraShowAsset)EditorGUILayout.ObjectField(
                "Show", _show, typeof(SpectraShowAsset), false);
            _fixturesPerGroup = EditorGUILayout.IntSlider("Fixtures Per Group", _fixturesPerGroup, 1, 16);
            EditorGUI.BeginDisabledGroup(_show == null);
            if (GUILayout.Button("Create and Save Demo Scene", GUILayout.Height(38f)))
                CreateDemoScene(_show, _fixturesPerGroup);
            EditorGUI.EndDisabledGroup();
        }

        public static void CreateDemoScene(SpectraShowAsset show, int fixturesPerGroup)
        {
            if (show == null || show.fixtureGroups == null || show.fixtureGroups.Length == 0)
                throw new System.InvalidOperationException("The show needs at least one fixture group.");
            if (!EditorSceneManager.SaveCurrentModifiedScenesIfUserWantsTo()) return;
            string scenePath = EditorUtility.SaveFilePanelInProject(
                "Save SpectraOverdrive Demo Scene", "SpectraOverdriveDemo", "unity",
                "Choose where to save the generated demo scene.");
            if (string.IsNullOrEmpty(scenePath)) return;

            Scene scene = EditorSceneManager.NewScene(NewSceneSetup.EmptyScene, NewSceneMode.Single);
            GameObject stage = GameObject.CreatePrimitive(PrimitiveType.Plane);
            stage.name = "Demo Stage";
            stage.transform.localScale = new Vector3(2.5f, 1f, 1.5f);

            Camera camera = new GameObject("Demo Camera").AddComponent<Camera>();
            camera.tag = "MainCamera";
            camera.transform.position = new Vector3(0f, 6f, -14f);
            camera.transform.LookAt(new Vector3(0f, 1.5f, 0f));
            camera.clearFlags = CameraClearFlags.SolidColor;
            camera.backgroundColor = new Color(0.005f, 0.005f, 0.015f, 1f);

            string directory = Path.GetDirectoryName(scenePath).Replace("\\", "/");
            string sceneName = Path.GetFileNameWithoutExtension(scenePath);
            string materialFolderName = sceneName + "_Materials";
            string materialFolder = directory + "/" + materialFolderName;
            if (!AssetDatabase.IsValidFolder(materialFolder))
                AssetDatabase.CreateFolder(directory, materialFolderName);
            Shader shader = Shader.Find("SpectraOverdrive/Demo/FixtureEmissive");
            if (shader == null) throw new System.InvalidOperationException("Spectra demo shader is missing.");

            GameObject fixturesRoot = new GameObject("Spectra Fixture Groups");
            int fixtureId = 1;
            int countPerGroup = Mathf.Clamp(fixturesPerGroup, 1, 16);
            for (int groupIndex = 0; groupIndex < show.fixtureGroups.Length; groupIndex++)
            {
                SpectraShowFixtureGroup showGroup = show.fixtureGroups[groupIndex];
                if (showGroup == null) continue;
                GameObject groupObject = new GameObject(showGroup.name);
                groupObject.transform.SetParent(fixturesRoot.transform, false);
                SpectraFixtureGroup group = groupObject.AddComponent<SpectraFixtureGroup>();
                group.groupId = showGroup.runtimeGroupId;
                group.groupName = showGroup.name;
                group.selection = showGroup.selection;
                group.selectionSeed = showGroup.randomSeed;
                List<SpectraFixtureRuntime> runtimes = new List<SpectraFixtureRuntime>();
                Color baseColor = Color.HSVToRGB(
                    Mathf.Repeat(groupIndex * 0.173f + 0.72f, 1f), 0.82f, 1f);
                Material material = new Material(shader);
                material.name = showGroup.name + " Demo Material";
                material.SetColor("_BaseColor", baseColor);
                string materialPath = AssetDatabase.GenerateUniqueAssetPath(
                    materialFolder + "/" + Sanitize(showGroup.name) + ".mat");
                AssetDatabase.CreateAsset(material, materialPath);

                for (int fixtureIndex = 0; fixtureIndex < countPerGroup; fixtureIndex++)
                {
                    GameObject fixtureObject = GameObject.CreatePrimitive(PrimitiveType.Cube);
                    fixtureObject.name = showGroup.name + " " + (fixtureIndex + 1);
                    fixtureObject.transform.SetParent(groupObject.transform, false);
                    float x = (fixtureIndex - (countPerGroup - 1) * 0.5f) * 1.35f;
                    float y = 0.6f + groupIndex * 0.85f;
                    fixtureObject.transform.position = new Vector3(x, y, groupIndex * 1.1f - 2f);
                    fixtureObject.transform.localScale = new Vector3(0.75f, 0.45f, 0.45f);
                    Renderer renderer = fixtureObject.GetComponent<Renderer>();
                    renderer.sharedMaterial = material;
                    SpectraFixtureIdentity identity = fixtureObject.AddComponent<SpectraFixtureIdentity>();
                    identity.fixtureId = fixtureId++;
                    identity.fixtureName = fixtureObject.name;
                    identity.fixtureProfile = "Spectra Demo Emissive";
                    identity.fixtureType = SpectraFixtureType.MovingWash;
                    identity.primaryGroup = showGroup.runtimeGroupId;
                    identity.universe = 1;
                    identity.startAddress = 1 + ((identity.fixtureId - 1) * 13) % 500;
                    identity.fixtureBody = fixtureObject;
                    SpectraFixtureChannelMap channels = fixtureObject.AddComponent<SpectraFixtureChannelMap>();
                    SpectraFixtureRuntime runtime = fixtureObject.AddComponent<SpectraFixtureRuntime>();
                    runtime.identity = identity;
                    runtime.channels = channels;
                    runtime.capabilities = SpectraFixtureCapabilities.ForType(identity.fixtureType);
                    runtime.controlledRenderers = new[] { renderer };
                    runtimes.Add(runtime);
                }
                group.fixtures = runtimes.ToArray();
            }

            SpectraProductionRigBuilder.CreateRig(show, null, true, true);
            AssetDatabase.SaveAssets();
            EditorSceneManager.SaveScene(scene, scenePath);
            EditorUtility.DisplayDialog("SpectraOverdrive",
                "Created a playable demo scene with " + show.fixtureGroups.Length
                + " fixture groups and the synchronized 1.5 runtime rig.", "OK");
        }

        private static string Sanitize(string value)
        {
            if (string.IsNullOrEmpty(value)) return "SpectraGroup";
            char[] invalid = Path.GetInvalidFileNameChars();
            for (int i = 0; i < invalid.Length; i++) value = value.Replace(invalid[i], '_');
            return value;
        }
    }
}
