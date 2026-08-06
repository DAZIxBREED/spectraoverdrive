#if UNITY_EDITOR
using UnityEditor;
using UnityEngine;
using SpectraOverdrive;

namespace SpectraOverdrive.Editor
{
    [CustomEditor(typeof(SpectraCueSequence))]
    public class SpectraCueSequenceEditor : UnityEditor.Editor
    {
        public override void OnInspectorGUI()
        {
            DrawDefaultInspector();

            SpectraCueSequence sequence = (SpectraCueSequence)target;

            EditorGUILayout.Space();
            EditorGUILayout.LabelField("Sequence Tools", EditorStyles.boldLabel);

            if (GUILayout.Button("Normalize Invalid Durations"))
            {
                Undo.RecordObject(sequence, "Normalize Cue Durations");

                if (sequence.steps != null)
                {
                    for (int i = 0; i < sequence.steps.Length; i++)
                    {
                        SpectraCueStep step = sequence.steps[i];
                        if (step == null) continue;

                        step.duration = Mathf.Max(0.05f, step.duration);
                        step.fade = Mathf.Clamp(step.fade, 0f, step.duration);
                    }
                }

                EditorUtility.SetDirty(sequence);
            }
        }
    }
}
#endif
