#if UNITY_EDITOR
using System.IO;
using UnityEditor;
using UnityEngine;
using SpectraOverdrive;

namespace SpectraOverdrive.Editor
{
    public static class SpectraFixtureProfileJson
    {
        [MenuItem("SpectraOverdrive/Profiles/Export Selected Profile JSON")]
        public static void ExportSelected()
        {
            SpectraFixtureProfileData profile = Selection.activeObject as SpectraFixtureProfileData;
            if (profile == null)
            {
                Debug.LogWarning("[SpectraOverdrive] Select a fixture profile asset first.");
                return;
            }

            string path = EditorUtility.SaveFilePanel(
                "Export Fixture Profile",
                "",
                profile.name + ".json",
                "json"
            );

            if (string.IsNullOrEmpty(path)) return;

            File.WriteAllText(path, JsonUtility.ToJson(profile, true));
            Debug.Log("[SpectraOverdrive] Exported fixture profile: " + path);
        }

        [MenuItem("SpectraOverdrive/Profiles/Import Fixture Profile JSON")]
        public static void ImportProfile()
        {
            string path = EditorUtility.OpenFilePanel(
                "Import Fixture Profile",
                "",
                "json"
            );

            if (string.IsNullOrEmpty(path)) return;

            string json = File.ReadAllText(path);
            SpectraFixtureProfileData profile = ScriptableObject.CreateInstance<SpectraFixtureProfileData>();
            JsonUtility.FromJsonOverwrite(json, profile);

            string assetPath = AssetDatabase.GenerateUniqueAssetPath(
                "Assets/" + Path.GetFileNameWithoutExtension(path) + ".asset"
            );

            AssetDatabase.CreateAsset(profile, assetPath);
            AssetDatabase.SaveAssets();
            Selection.activeObject = profile;

            Debug.Log("[SpectraOverdrive] Imported fixture profile to " + assetPath);
        }
    }
}
#endif
