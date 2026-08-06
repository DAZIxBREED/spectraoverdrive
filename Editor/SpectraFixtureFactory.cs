#if UNITY_EDITOR
using UnityEditor;
using UnityEngine;
using SpectraOverdrive;

namespace SpectraOverdrive.Editor
{
    public static class SpectraFixtureFactory
    {
        [MenuItem("GameObject/SpectraOverdrive/Create Generic Moving Head", false, 10)]
        public static void CreateMovingHead()
        {
            GameObject root = new GameObject("Spectra Moving Head");
            Undo.RegisterCreatedObjectUndo(root, "Create Spectra Moving Head");

            SpectraFixtureIdentity identity = root.AddComponent<SpectraFixtureIdentity>();
            identity.fixtureType = SpectraFixtureType.MovingSpot;
            identity.fixtureProfile = "Spectra 13ch Moving Spot";
            identity.channelCount = 13;

            SpectraFixtureChannelMap channels = root.AddComponent<SpectraFixtureChannelMap>();
            SpectraFixtureRuntime runtime = root.AddComponent<SpectraFixtureRuntime>();
            runtime.identity = identity;
            runtime.channels = channels;
            runtime.capabilities = SpectraFixtureCapabilities.ForType(identity.fixtureType);

            GameObject pan = new GameObject("Pan");
            Undo.RegisterCreatedObjectUndo(pan, "Create Pan Transform");
            pan.transform.SetParent(root.transform, false);

            GameObject tilt = new GameObject("Tilt");
            Undo.RegisterCreatedObjectUndo(tilt, "Create Tilt Transform");
            tilt.transform.SetParent(pan.transform, false);

            GameObject beam = GameObject.CreatePrimitive(PrimitiveType.Cylinder);
            Undo.RegisterCreatedObjectUndo(beam, "Create Beam");
            beam.name = "Beam";
            beam.transform.SetParent(tilt.transform, false);
            beam.transform.localPosition = new Vector3(0f, 0f, 1f);
            beam.transform.localScale = new Vector3(0.15f, 1f, 0.15f);

            Collider collider = beam.GetComponent<Collider>();
            if (collider != null)
            {
                Object.DestroyImmediate(collider);
            }

            Renderer renderer = beam.GetComponent<Renderer>();
            runtime.controlledRenderers = new Renderer[] { renderer };

            SpectraMovingHeadRig rig = root.AddComponent<SpectraMovingHeadRig>();
            rig.fixture = runtime;
            rig.panTransform = pan.transform;
            rig.tiltTransform = tilt.transform;

            identity.beam = beam;
            Selection.activeGameObject = root;
        }
    }
}
#endif
