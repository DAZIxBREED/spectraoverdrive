#if UNITY_EDITOR
using UnityEditor;
using UnityEngine;
using SpectraOverdrive;

namespace SpectraOverdrive.Editor
{
    public static class SpectraAdvancedFixtureFactory
    {
        [MenuItem("GameObject/SpectraOverdrive/Create Moving Wash", false, 15)]
        public static void CreateMovingWash()
        {
            GameObject root = new GameObject("Spectra Moving Wash");
            Undo.RegisterCreatedObjectUndo(root, "Create Spectra Moving Wash");

            SpectraFixtureIdentity identity = root.AddComponent<SpectraFixtureIdentity>();
            identity.fixtureType = SpectraFixtureType.MovingWash;
            identity.fixtureProfile = "Spectra 10ch Moving Wash";
            identity.channelCount = 10;

            SpectraFixtureChannelMap channels = root.AddComponent<SpectraFixtureChannelMap>();
            channels.gobo = -1;
            channels.goboRotate = -1;
            channels.prism = -1;

            SpectraFixtureRuntime runtime = root.AddComponent<SpectraFixtureRuntime>();
            runtime.identity = identity;
            runtime.channels = channels;
            runtime.capabilities = SpectraFixtureCapabilities.ForType(identity.fixtureType);

            GameObject wash = GameObject.CreatePrimitive(PrimitiveType.Quad);
            Undo.RegisterCreatedObjectUndo(wash, "Create Wash Surface");
            wash.name = "Wash";
            wash.transform.SetParent(root.transform, false);
            wash.transform.localPosition = new Vector3(0f, 0f, 1f);
            Object.DestroyImmediate(wash.GetComponent<Collider>());

            SpectraWashRuntime washRuntime = root.AddComponent<SpectraWashRuntime>();
            washRuntime.fixture = runtime;
            washRuntime.washRenderer = wash.GetComponent<Renderer>();

            runtime.controlledRenderers = new Renderer[] { washRuntime.washRenderer };
            identity.projection = wash;
            Selection.activeGameObject = root;
        }

        [MenuItem("GameObject/SpectraOverdrive/Create Laser Ribbon", false, 16)]
        public static void CreateLaserRibbon()
        {
            GameObject root = GameObject.CreatePrimitive(PrimitiveType.Quad);
            Undo.RegisterCreatedObjectUndo(root, "Create Spectra Laser Ribbon");
            root.name = "Spectra Laser Ribbon";

            Collider collider = root.GetComponent<Collider>();
            if (collider != null) Object.DestroyImmediate(collider);

            SpectraFixtureIdentity identity = root.AddComponent<SpectraFixtureIdentity>();
            identity.fixtureType = SpectraFixtureType.Laser;
            identity.fixtureProfile = "Spectra 8ch Laser";
            identity.channelCount = 8;

            SpectraFixtureChannelMap channels = root.AddComponent<SpectraFixtureChannelMap>();
            SpectraFixtureRuntime runtime = root.AddComponent<SpectraFixtureRuntime>();
            runtime.identity = identity;
            runtime.channels = channels;
            runtime.capabilities = SpectraFixtureCapabilities.ForType(identity.fixtureType);
            runtime.controlledRenderers = new Renderer[] { root.GetComponent<Renderer>() };

            SpectraLaserRibbon laser = root.AddComponent<SpectraLaserRibbon>();
            laser.fixture = runtime;
            laser.ribbonRenderer = root.GetComponent<Renderer>();

            Selection.activeGameObject = root;
        }
    }
}
#endif
