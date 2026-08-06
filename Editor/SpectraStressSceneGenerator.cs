#if UNITY_EDITOR
using UnityEditor;
using UnityEngine;
using SpectraOverdrive;

namespace SpectraOverdrive.Editor
{
    public class SpectraStressSceneGenerator : EditorWindow
    {
        private int movingHeads = 24;
        private int washes = 12;
        private int pars = 16;
        private int lasers = 8;
        private float spacing = 2f;

        [MenuItem("SpectraOverdrive/Stress Scene Generator")]
        public static void Open()
        {
            GetWindow<SpectraStressSceneGenerator>("Spectra Stress");
        }

        private void OnGUI()
        {
            EditorGUILayout.LabelField("Stress Scene Generator", EditorStyles.boldLabel);
            movingHeads = EditorGUILayout.IntSlider("Moving Heads", movingHeads, 0, 128);
            washes = EditorGUILayout.IntSlider("Washes", washes, 0, 128);
            pars = EditorGUILayout.IntSlider("PARs", pars, 0, 256);
            lasers = EditorGUILayout.IntSlider("Lasers", lasers, 0, 64);
            spacing = EditorGUILayout.Slider("Spacing", spacing, 0.5f, 10f);

            if (GUILayout.Button("Generate Stress Rig"))
            {
                Generate();
            }
        }

        private void Generate()
        {
            GameObject root = new GameObject("SpectraOverdrive Stress Rig");
            Undo.RegisterCreatedObjectUndo(root, "Generate Spectra Stress Rig");

            int index = 0;
            index = CreateRow(root.transform, SpectraFixtureType.MovingSpot, movingHeads, index, spacing, 0f);
            index = CreateRow(root.transform, SpectraFixtureType.MovingWash, washes, index, spacing, 3f);
            index = CreateRow(root.transform, SpectraFixtureType.Par, pars, index, spacing, 6f);
            CreateRow(root.transform, SpectraFixtureType.Laser, lasers, index, spacing, 9f);

            Selection.activeGameObject = root;
        }

        private int CreateRow(
            Transform parent,
            SpectraFixtureType type,
            int count,
            int startIndex,
            float xSpacing,
            float z
        )
        {
            for (int i = 0; i < count; i++)
            {
                GameObject go = GameObject.CreatePrimitive(PrimitiveType.Cube);
                Undo.RegisterCreatedObjectUndo(go, "Create Stress Fixture");
                go.name = type + " " + i;
                go.transform.SetParent(parent, false);
                go.transform.localPosition = new Vector3(i * xSpacing, 0f, z);

                Collider collider = go.GetComponent<Collider>();
                if (collider != null) Object.DestroyImmediate(collider);

                SpectraFixtureIdentity identity = go.AddComponent<SpectraFixtureIdentity>();
                identity.fixtureId = startIndex + i;
                identity.fixtureType = type;
                identity.universe = 1 + ((startIndex + i) / 32);
                identity.startAddress = 1 + (((startIndex + i) % 32) * 13);
                identity.channelCount = 13;
            }

            return startIndex + count;
        }
    }
}
#endif
