#if UNITY_EDITOR
using UnityEditor;
using UnityEngine;
using SpectraOverdrive;

namespace SpectraOverdrive.Editor
{
    [CustomEditor(typeof(SpectraFixtureProfileData))]
    public class SpectraFixtureProfileEditor : UnityEditor.Editor
    {
        public override void OnInspectorGUI()
        {
            DrawDefaultInspector();

            SpectraFixtureProfileData profile = (SpectraFixtureProfileData)target;

            EditorGUILayout.Space();
            EditorGUILayout.LabelField("Profile Tools", EditorStyles.boldLabel);

            if (GUILayout.Button("Create Generic Channel Layout"))
            {
                Undo.RecordObject(profile, "Create Generic Channel Layout");
                profile.channels = new SpectraFixtureChannelDefinition[profile.channelCount];

                for (int i = 0; i < profile.channels.Length; i++)
                {
                    profile.channels[i] = new SpectraFixtureChannelDefinition
                    {
                        offset = i,
                        function = SpectraChannelFunction.Unused
                    };
                }

                EditorUtility.SetDirty(profile);
            }

            if (GUILayout.Button("Validate Profile"))
            {
                Validate(profile);
            }
        }

        private void Validate(SpectraFixtureProfileData profile)
        {
            int errors = 0;

            if (profile.channels == null || profile.channels.Length == 0)
            {
                Debug.LogError("[SpectraOverdrive] Profile has no channel definitions.", profile);
                return;
            }

            for (int i = 0; i < profile.channels.Length; i++)
            {
                SpectraFixtureChannelDefinition a = profile.channels[i];
                if (a == null) continue;

                if (a.offset >= profile.channelCount)
                {
                    errors++;
                    Debug.LogError(
                        "[SpectraOverdrive] Channel offset " + a.offset +
                        " exceeds mode size " + profile.channelCount + ".",
                        profile
                    );
                }

                for (int j = i + 1; j < profile.channels.Length; j++)
                {
                    SpectraFixtureChannelDefinition b = profile.channels[j];
                    if (b != null && a.offset == b.offset)
                    {
                        errors++;
                        Debug.LogWarning(
                            "[SpectraOverdrive] Duplicate channel offset " + a.offset + ".",
                            profile
                        );
                    }
                }
            }

            Debug.Log("[SpectraOverdrive] Profile validation complete. Issues: " + errors, profile);
        }
    }
}
#endif
