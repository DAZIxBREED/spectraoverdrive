#if UNITY_EDITOR
using UnityEditor;
using UnityEngine;
using SpectraOverdrive;

namespace SpectraOverdrive.Editor
{
    [CustomEditor(typeof(SpectraShowBank))]
    public class SpectraShowBankEditor : UnityEditor.Editor
    {
        public override void OnInspectorGUI()
        {
            DrawDefaultInspector();

            SpectraShowBank bank = (SpectraShowBank)target;

            EditorGUILayout.Space();
            EditorGUILayout.LabelField("Show Bank Tools", EditorStyles.boldLabel);

            if (GUILayout.Button("Validate Show Bank"))
            {
                Validate(bank);
            }
        }

        private void Validate(SpectraShowBank bank)
        {
            int issues = 0;

            if (bank.shows == null || bank.shows.Length == 0)
            {
                Debug.LogWarning("[SpectraOverdrive] Show bank is empty.", bank);
                return;
            }

            for (int i = 0; i < bank.shows.Length; i++)
            {
                SpectraShowEntry show = bank.shows[i];

                if (show == null)
                {
                    issues++;
                    Debug.LogWarning("[SpectraOverdrive] Null show entry at index " + i + ".", bank);
                    continue;
                }

                if (show.sequence == null)
                {
                    issues++;
                    Debug.LogWarning("[SpectraOverdrive] Show '" + show.showName + "' has no sequence.", bank);
                }
            }

            Debug.Log("[SpectraOverdrive] Show bank validation complete. Issues: " + issues, bank);
        }
    }
}
#endif
