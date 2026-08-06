#if UNITY_EDITOR
using System;
using System.Reflection;
using UnityEditor;
using UnityEngine;
using SpectraOverdrive;

namespace SpectraOverdrive.Editor
{
    public class SpectraVrslAssistedConverter : EditorWindow
    {
        private bool duplicateBeforeConversion = true;
        private bool addCompatibilityMarkers = true;

        [MenuItem("SpectraOverdrive/VRSL/Assisted Converter")]
        public static void Open()
        {
            GetWindow<SpectraVrslAssistedConverter>("VRSL Converter");
        }

        private void OnGUI()
        {
            EditorGUILayout.LabelField("Assisted VRSL Conversion", EditorStyles.boldLabel);
            EditorGUILayout.HelpBox(
                "This tool only converts selected objects. It can duplicate them first and adds migration markers. It does not delete the original VRSL components.",
                MessageType.Warning
            );

            duplicateBeforeConversion = EditorGUILayout.Toggle("Duplicate First", duplicateBeforeConversion);
            addCompatibilityMarkers = EditorGUILayout.Toggle("Add Migration Markers", addCompatibilityMarkers);

            if (GUILayout.Button("Convert Selected Objects"))
            {
                ConvertSelection();
            }
        }

        private void ConvertSelection()
        {
            GameObject[] selection = Selection.gameObjects;
            int converted = 0;

            for (int i = 0; i < selection.Length; i++)
            {
                GameObject source = selection[i];
                if (source == null) continue;

                GameObject target = source;

                if (duplicateBeforeConversion)
                {
                    target = Instantiate(source, source.transform.parent);
                    target.name = source.name + " [Spectra Copy]";
                    Undo.RegisterCreatedObjectUndo(target, "Duplicate for Spectra Conversion");
                }

                SpectraFixtureIdentity identity = target.GetComponent<SpectraFixtureIdentity>();
                if (identity == null)
                {
                    identity = Undo.AddComponent<SpectraFixtureIdentity>(target);
                }

                SpectraFixtureChannelMap map = target.GetComponent<SpectraFixtureChannelMap>();
                if (map == null)
                {
                    map = Undo.AddComponent<SpectraFixtureChannelMap>(target);
                }

                SpectraFixtureRuntime runtime = target.GetComponent<SpectraFixtureRuntime>();
                if (runtime == null)
                {
                    runtime = Undo.AddComponent<SpectraFixtureRuntime>(target);
                }

                runtime.identity = identity;
                runtime.channels = map;
                runtime.capabilities = SpectraFixtureCapabilities.ForType(identity.fixtureType);
                runtime.controlledRenderers = target.GetComponentsInChildren<Renderer>(true);

                Component[] components = target.GetComponents<Component>();
                for (int j = 0; j < components.Length; j++)
                {
                    Component component = components[j];
                    if (component == null) continue;

                    string typeName = component.GetType().FullName;
                    string lower = typeName.ToLowerInvariant();

                    if (!lower.Contains("vrsl") && !lower.Contains("dmx") && !lower.Contains("stagelight"))
                    {
                        continue;
                    }

                    TryCopyNumericProperty(component, "universe", value => identity.universe = Mathf.Clamp(value, 1, 9));
                    TryCopyNumericProperty(component, "channel", value => identity.startAddress = Mathf.Clamp(value, 1, 512));
                    TryCopyNumericProperty(component, "address", value => identity.startAddress = Mathf.Clamp(value, 1, 512));

                    if (addCompatibilityMarkers)
                    {
                        SpectraVrslCompatibilityMarker marker = target.GetComponent<SpectraVrslCompatibilityMarker>();
                        if (marker == null)
                        {
                            marker = Undo.AddComponent<SpectraVrslCompatibilityMarker>(target);
                        }

                        marker.originalComponentType = typeName;
                        marker.originalFixtureName = source.name;
                        marker.originalUniverse = identity.universe;
                        marker.originalAddress = identity.startAddress;
                        marker.conversionNotes = "Assisted conversion; original source component retained.";
                    }
                }

                runtime.PublishFixtureProperties();
                converted++;
            }

            Debug.Log("[SpectraOverdrive] Assisted conversion completed for " + converted + " selected object(s).");
        }

        private void TryCopyNumericProperty(Component component, string propertyName, Action<int> setter)
        {
            Type type = component.GetType();

            FieldInfo field = type.GetField(
                propertyName,
                BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.Instance | BindingFlags.IgnoreCase
            );

            if (field != null)
            {
                object value = field.GetValue(component);
                if (value is int intValue)
                {
                    setter(intValue);
                    return;
                }
                if (value is float floatValue)
                {
                    setter(Mathf.RoundToInt(floatValue));
                    return;
                }
            }

            PropertyInfo property = type.GetProperty(
                propertyName,
                BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.Instance | BindingFlags.IgnoreCase
            );

            if (property != null && property.CanRead)
            {
                object value = property.GetValue(component, null);
                if (value is int intValue)
                {
                    setter(intValue);
                }
                else if (value is float floatValue)
                {
                    setter(Mathf.RoundToInt(floatValue));
                }
            }
        }
    }
}
#endif
