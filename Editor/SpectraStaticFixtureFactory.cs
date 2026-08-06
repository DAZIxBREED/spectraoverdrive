#if UNITY_EDITOR
using UnityEditor;
using UnityEngine;
using SpectraOverdrive;

namespace SpectraOverdrive.Editor
{
    public static class SpectraStaticFixtureFactory
    {
        [MenuItem("GameObject/SpectraOverdrive/Create PAR", false, 11)]
        public static void CreatePar()
        {
            CreateStaticFixture("Spectra PAR", SpectraFixtureType.Par, 5, "Spectra 5ch PAR");
        }

        [MenuItem("GameObject/SpectraOverdrive/Create Blinder", false, 12)]
        public static void CreateBlinder()
        {
            CreateStaticFixture("Spectra Blinder", SpectraFixtureType.Blinder, 2, "Spectra 2ch Blinder");
        }

        [MenuItem("GameObject/SpectraOverdrive/Create Strobe", false, 13)]
        public static void CreateStrobe()
        {
            CreateStaticFixture("Spectra Strobe", SpectraFixtureType.Strobe, 3, "Spectra 3ch Strobe");
        }

        [MenuItem("GameObject/SpectraOverdrive/Create Light Bar", false, 14)]
        public static void CreateLightBar()
        {
            CreateStaticFixture("Spectra Light Bar", SpectraFixtureType.LightBar, 5, "Spectra 5ch Light Bar");
        }

        private static void CreateStaticFixture(string objectName, SpectraFixtureType type, int channels, string profile)
        {
            GameObject root = GameObject.CreatePrimitive(PrimitiveType.Cube);
            Undo.RegisterCreatedObjectUndo(root, "Create " + objectName);
            root.name = objectName;

            Collider collider = root.GetComponent<Collider>();
            if (collider != null) Object.DestroyImmediate(collider);

            SpectraFixtureIdentity identity = root.AddComponent<SpectraFixtureIdentity>();
            identity.fixtureType = type;
            identity.fixtureProfile = profile;
            identity.channelCount = channels;

            SpectraFixtureChannelMap map = root.AddComponent<SpectraFixtureChannelMap>();
            SpectraFixtureRuntime runtime = root.AddComponent<SpectraFixtureRuntime>();
            runtime.identity = identity;
            runtime.channels = map;
            runtime.capabilities = SpectraFixtureCapabilities.ForType(identity.fixtureType);
            runtime.controlledRenderers = new Renderer[] { root.GetComponent<Renderer>() };

            SpectraStaticFixtureRuntime staticRuntime = root.AddComponent<SpectraStaticFixtureRuntime>();
            staticRuntime.fixture = runtime;
            staticRuntime.emitters = runtime.controlledRenderers;

            Selection.activeGameObject = root;
        }
    }
}
#endif
