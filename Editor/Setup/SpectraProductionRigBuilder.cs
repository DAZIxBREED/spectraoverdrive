using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

namespace SpectraOverdrive.Editor
{
    public class SpectraProductionRigBuilder : EditorWindow
    {
        private SpectraShowAsset _singleShow;
        private SpectraProgrammedShowBank _showBank;
        private bool _mapFixtureGroups = true;
        private bool _addSnapshotCaches = true;

        [MenuItem("SpectraOverdrive/Show Programmer/Create Production Runtime Rig")]
        private static void Open()
        {
            SpectraProductionRigBuilder window = GetWindow<SpectraProductionRigBuilder>("Spectra Runtime Rig");
            window.minSize = new Vector2(460f, 260f);
        }

        private void OnGUI()
        {
            EditorGUILayout.LabelField("SpectraOverdrive 1.4 Production Rig", EditorStyles.boldLabel);
            EditorGUILayout.HelpBox(
                "Creates and wires the server-time network controller, baked show player(s), live override layer, recorder, snapshot cache, macro snapshot recall, event router, platform manager, AudioLink adapter, and emergency bus.",
                MessageType.Info);
            _singleShow = (SpectraShowAsset)EditorGUILayout.ObjectField(
                "Single Show", _singleShow, typeof(SpectraShowAsset), false);
            _showBank = (SpectraProgrammedShowBank)EditorGUILayout.ObjectField(
                "Or Show Bank", _showBank, typeof(SpectraProgrammedShowBank), false);
            _mapFixtureGroups = EditorGUILayout.Toggle("Map Fixture Groups", _mapFixtureGroups);
            _addSnapshotCaches = EditorGUILayout.Toggle("Add Snapshot Caches", _addSnapshotCaches);
            EditorGUILayout.Space();
            bool hasShows = _singleShow != null || (_showBank != null && _showBank.shows != null && _showBank.shows.Length > 0);
            EditorGUI.BeginDisabledGroup(!hasShows);
            if (GUILayout.Button("Create, Wire, Validate, and Bake Rig", GUILayout.Height(38f)))
                CreateRig(_singleShow, _showBank, _mapFixtureGroups, _addSnapshotCaches);
            EditorGUI.EndDisabledGroup();
        }

        public static GameObject CreateRig(
            SpectraShowAsset singleShow,
            SpectraProgrammedShowBank bank,
            bool mapFixtureGroups,
            bool addSnapshots)
        {
            List<SpectraShowAsset> shows = new List<SpectraShowAsset>();
            if (bank != null && bank.shows != null)
                for (int i = 0; i < bank.shows.Length; i++)
                {
                    if (bank.shows[i] == null)
                        throw new System.InvalidOperationException("Show bank entry " + i + " is empty.");
                    shows.Add(bank.shows[i]);
                }
            if (shows.Count == 0 && singleShow != null) shows.Add(singleShow);
            if (shows.Count == 0) return null;

            GameObject root = new GameObject("SpectraOverdrive 1.4 Production Rig");
            Undo.RegisterCreatedObjectUndo(root, "Create SpectraOverdrive production rig");
            SpectraPlatformManager platform = Undo.AddComponent<SpectraPlatformManager>(root);
            SpectraLocalQualityController quality = Undo.AddComponent<SpectraLocalQualityController>(root);
            SpectraAdaptiveBudgetAllocator allocator = Undo.AddComponent<SpectraAdaptiveBudgetAllocator>(root);
            allocator.platformManager = platform;
            allocator.qualityController = quality;
            SpectraOverdriveBus bus = Undo.AddComponent<SpectraOverdriveBus>(root);
            SpectraAudioLinkAdapter audio = Undo.AddComponent<SpectraAudioLinkAdapter>(root);
            Undo.AddComponent<SpectraAudioCoordinatePublisher>(root);
            SpectraShowEventRouter events = Undo.AddComponent<SpectraShowEventRouter>(root);
            SpectraShowNetworkController network = Undo.AddComponent<SpectraShowNetworkController>(root);
            events.networkAuthority = root;
            SpectraLiveOverrideLayer overrides = Undo.AddComponent<SpectraLiveOverrideLayer>(root);
            SpectraLiveOverrideRecorder recorder = Undo.AddComponent<SpectraLiveOverrideRecorder>(root);
            SpectraAccessibilityController accessibility = Undo.AddComponent<SpectraAccessibilityController>(root);
            SpectraOperatorConsole console = Undo.AddComponent<SpectraOperatorConsole>(root);
            SpectraHotCueBankController hotCues = Undo.AddComponent<SpectraHotCueBankController>(root);
            SpectraPerformanceMacroController macros = Undo.AddComponent<SpectraPerformanceMacroController>(root);
            SpectraMacroSnapshotController macroSnapshots = Undo.AddComponent<SpectraMacroSnapshotController>(root);
            SpectraSceneStackController scenes = Undo.AddComponent<SpectraSceneStackController>(root);
            overrides.recorder = recorder;
            network.overrideLayer = overrides;
            console.bus = bus;
            console.networkController = network;
            console.liveOverrides = overrides;
            hotCues.networkController = network;
            macros.networkController = network;
            macroSnapshots.networkController = network;
            scenes.networkController = network;

            SpectraShowRuntimePlayer[] players = new SpectraShowRuntimePlayer[shows.Count];
            for (int i = 0; i < shows.Count; i++)
            {
                GameObject playerObject = new GameObject("Show " + (i + 1) + " - " + shows[i].showName);
                Undo.RegisterCreatedObjectUndo(playerObject, "Create SpectraOverdrive show player");
                playerObject.transform.SetParent(root.transform, false);
                SpectraShowRuntimePlayer player = Undo.AddComponent<SpectraShowRuntimePlayer>(playerObject);
                player.platformManager = platform;
                player.qualityController = quality;
                player.bus = bus;
                player.audioLinkAdapter = audio;
                player.overrideLayer = overrides;
                player.eventRouter = events;
                player.externalClock = true;
                if (addSnapshots)
                {
                    SpectraShowSnapshotCache cache = Undo.AddComponent<SpectraShowSnapshotCache>(playerObject);
                    cache.capacity = shows[i].GetPlatformPolicy(SpectraPlatformKind.Quest).snapshotCapacity;
                    player.snapshotCache = cache;
                }
                if (!SpectraRuntimeShowBakerWindow.Bake(shows[i], player, mapFixtureGroups, false))
                    throw new System.InvalidOperationException("Failed to bake '" + shows[i].showName + "'.");
                players[i] = player;
            }
            network.showPlayers = players;
            network.activeShowIndex = bank == null
                ? 0 : Mathf.Clamp(bank.defaultShowIndex, 0, players.Length - 1);
            overrides.player = players[network.activeShowIndex];
            overrides.configuredGroupCount = players[network.activeShowIndex].groups == null
                ? 16 : Mathf.Max(1, players[network.activeShowIndex].groups.Length);
            recorder.player = players[network.activeShowIndex];
            accessibility.showPlayer = players[network.activeShowIndex];
            accessibility.networkController = network;
            bus.activeSource = SpectraControlSource.InternalCue;

            Selection.activeGameObject = root;
            UnityEditor.SceneManagement.EditorSceneManager.MarkSceneDirty(root.scene);
            EditorUtility.DisplayDialog("SpectraOverdrive",
                "Created a production rig with " + players.Length
                + " validated, signed, Udon-safe show player(s).", "OK");
            return root;
        }
    }
}
