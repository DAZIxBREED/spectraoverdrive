using System;
using UnityEditor;
using UnityEngine;

namespace SpectraOverdrive.Editor
{
    public sealed class SpectraShowProgrammerWindow : EditorWindow
    {
        private enum ToolMode { Select, Draw, Razor }
        private enum DragMode { None, MoveCue, ResizeCueEnd, MovePlayhead, MoveMarker, MoveLoop, ResizeLoopStart, ResizeLoopEnd }

        private const float TrackListWidth = 220f;
        private const float InspectorWidth = 310f;
        private const float RulerHeight = 24f;
        private const float MarkerHeight = 28f;
        private const float LoopHeight = 22f;
        private const float WaveformHeight = 68f;
        private const float TrackHeight = 50f;
        private const float BottomBarHeight = 18f;

        [SerializeField] private SpectraShowAsset show;
        [SerializeField] private SpectraShowRuntimePlayer previewPlayer;
        [SerializeField] private float pixelsPerSecond = 100f;
        [SerializeField] private float scrollSeconds;
        [SerializeField] private float playhead;
        [SerializeField] private SpectraTimelineSnap snap = SpectraTimelineSnap.QuarterBeat;
        [SerializeField] private float frameRate = 60f;
        [SerializeField] private bool showWaveform = true;
        [SerializeField] private int selectedTrack = -1;
        [SerializeField] private int selectedCue = -1;
        [SerializeField] private int selectedMarker = -1;
        [SerializeField] private int selectedLoop = -1;
        [SerializeField] private ToolMode toolMode;
        [SerializeField] private SpectraCueTemplateAsset cueTemplate;
        [SerializeField] private SpectraMovementPresetAsset movementPreset;
        [SerializeField] private SpectraColorPaletteAsset colorPalette;
        [SerializeField] private SpectraSectionTemplateAsset sectionTemplate;

        private SpectraWaveformData waveform;
        private AudioClip waveformClip;
        private DragMode dragMode;
        private Vector2 dragMouseStart;
        private float dragOriginalStart;
        private float dragOriginalDuration;
        private float dragOriginalEnd;
        private bool playing;
        private double lastEditorTime;
        private double lastTapTime;
        private float tapBpm;
        private string cueClipboard;
        private SpectraCompiledShow previewCompiled;
        private SpectraPlatformCompatibilityResult[] compatibilityCache;
        private Vector2 inspectorScroll;
        private GUIStyle centeredMini;
        private GUIStyle cueLabel;

        [MenuItem("SpectraOverdrive/Show Programmer/Open Timeline")]
        public static void Open()
        {
            SpectraShowProgrammerWindow window = GetWindow<SpectraShowProgrammerWindow>();
            window.titleContent = new GUIContent("Spectra Show Programmer");
            window.minSize = new Vector2(900f, 520f);
            window.Show();
        }

        private void OnEnable()
        {
            EditorApplication.update += EditorTick;
            Undo.undoRedoPerformed += OnUndoRedo;
            if (show == null && Selection.activeObject is SpectraShowAsset)
                SetShow((SpectraShowAsset)Selection.activeObject);
        }

        private void OnDisable()
        {
            EditorApplication.update -= EditorTick;
            Undo.undoRedoPerformed -= OnUndoRedo;
            StopPreview(false);
        }

        private void OnSelectionChange()
        {
            SpectraShowAsset selected = Selection.activeObject as SpectraShowAsset;
            if (selected != null && selected != show)
            {
                SetShow(selected);
                Repaint();
            }
        }

        private void OnUndoRedo()
        {
            ClampSelection();
            previewCompiled = null;
            compatibilityCache = null;
            Repaint();
        }

        private void OnGUI()
        {
            EnsureStyles();
            DrawTitleBar();
            if (show == null)
            {
                DrawEmptyState();
                return;
            }
            DrawTransport();
            DrawAuthoringBar();
            Rect workspace = GUILayoutUtility.GetRect(100f, 100000f, 180f, 100000f, GUILayout.ExpandWidth(true), GUILayout.ExpandHeight(true));
            DrawWorkspace(workspace);
            HandleKeyboard();
        }

        private void DrawTitleBar()
        {
            EditorGUILayout.BeginHorizontal(EditorStyles.toolbar);
            SpectraShowAsset selected = (SpectraShowAsset)EditorGUILayout.ObjectField(
                show, typeof(SpectraShowAsset), false, GUILayout.MinWidth(220f));
            if (selected != show) SetShow(selected);
            GUILayout.FlexibleSpace();
            if (show != null)
                GUILayout.Label(show.showName + "  •  schema " + show.schemaVersion, EditorStyles.miniLabel);
            EditorGUILayout.EndHorizontal();
        }

        private void DrawEmptyState()
        {
            GUILayout.FlexibleSpace();
            EditorGUILayout.BeginHorizontal();
            GUILayout.FlexibleSpace();
            EditorGUILayout.BeginVertical(GUILayout.Width(430f));
            GUILayout.Label("SpectraOverdrive Show Programmer", EditorStyles.boldLabel);
            GUILayout.Label("Assign or select a SpectraShowAsset to open its timeline.", EditorStyles.wordWrappedLabel);
            if (GUILayout.Button("Create New Show Asset", GUILayout.Height(32f))) CreateShowAsset();
            EditorGUILayout.EndVertical();
            GUILayout.FlexibleSpace();
            EditorGUILayout.EndHorizontal();
            GUILayout.FlexibleSpace();
        }

        private void DrawTransport()
        {
            EditorGUILayout.BeginHorizontal(EditorStyles.toolbar);
            if (GUILayout.Button(playing ? "❚❚" : "▶", EditorStyles.toolbarButton, GUILayout.Width(34f)))
            {
                if (playing) StopPreview(false); else StartPreview();
            }
            if (GUILayout.Button("■", EditorStyles.toolbarButton, GUILayout.Width(30f)))
            {
                StopPreview(true);
                SetPlayhead(0f, true);
            }
            if (GUILayout.Button("◀ Marker", EditorStyles.toolbarButton, GUILayout.Width(74f))) SeekMarker(-1);
            if (GUILayout.Button("Marker ▶", EditorStyles.toolbarButton, GUILayout.Width(74f))) SeekMarker(1);
            GUILayout.Space(8f);
            GUILayout.Label(FormatTime(playhead), EditorStyles.toolbarButton, GUILayout.Width(82f));
            SpectraMusicalPosition musical = show.beatGrid == null
                ? new SpectraMusicalPosition(1, 1, 0f)
                : show.beatGrid.SecondsToMusical(playhead);
            GUILayout.Label("Bar " + musical.bar + "  Beat " + musical.beat + "  +" + musical.beatFraction.ToString("0.00"),
                EditorStyles.miniLabel, GUILayout.Width(150f));
            GUILayout.FlexibleSpace();
            previewPlayer = (SpectraShowRuntimePlayer)EditorGUILayout.ObjectField(
                previewPlayer, typeof(SpectraShowRuntimePlayer), true, GUILayout.Width(180f));
            if (GUILayout.Button("Compile", EditorStyles.toolbarButton, GUILayout.Width(58f))) CompilePreview(true);
            if (GUILayout.Button("Validate", EditorStyles.toolbarButton, GUILayout.Width(60f))) ShowValidation();
            EditorGUILayout.EndHorizontal();
        }

        private void DrawAuthoringBar()
        {
            EditorGUILayout.BeginHorizontal(EditorStyles.toolbar);
            toolMode = (ToolMode)GUILayout.Toolbar((int)toolMode, new[] { "Select", "Draw", "Razor" },
                EditorStyles.toolbarButton, GUILayout.Width(170f));
            snap = (SpectraTimelineSnap)EditorGUILayout.EnumPopup(snap, EditorStyles.toolbarPopup, GUILayout.Width(100f));
            GUILayout.Label("Zoom", GUILayout.Width(34f));
            pixelsPerSecond = GUILayout.HorizontalSlider(pixelsPerSecond, 20f, 800f, GUILayout.Width(105f));
            GUILayout.Label(Mathf.RoundToInt(pixelsPerSecond) + " px/s", EditorStyles.miniLabel, GUILayout.Width(58f));
            showWaveform = GUILayout.Toggle(showWaveform, "Waveform", EditorStyles.toolbarButton, GUILayout.Width(74f));
            if (GUILayout.Button("Add Track", EditorStyles.toolbarDropDown, GUILayout.Width(78f))) ShowAddTrackMenu();
            if (GUILayout.Button("+ Marker", EditorStyles.toolbarButton, GUILayout.Width(66f))) AddMarkerAtPlayhead();
            if (GUILayout.Button("+ 8-Beat Loop", EditorStyles.toolbarButton, GUILayout.Width(92f))) AddLoopAtPlayhead();
            GUILayout.FlexibleSpace();
            if (GUILayout.Button("Downbeat Here", EditorStyles.toolbarButton, GUILayout.Width(92f))) SetDownbeatHere();
            if (GUILayout.Button(tapBpm > 0f ? "Tap " + tapBpm.ToString("0.0") : "Tap BPM",
                EditorStyles.toolbarButton, GUILayout.Width(78f))) TapTempo();
            EditorGUILayout.EndHorizontal();

            EditorGUILayout.BeginHorizontal(EditorStyles.toolbar);
            AudioClip clip = show.authoringAudio;
            AudioClip changed = (AudioClip)EditorGUILayout.ObjectField("Waveform Audio", clip, typeof(AudioClip), false, GUILayout.MinWidth(260f));
            if (changed != clip)
            {
                Undo.RecordObject(show, "Change Spectra Waveform Audio");
                show.authoringAudio = changed;
                EditorUtility.SetDirty(show);
                RebuildWaveform();
            }
            if (GUILayout.Button("Rebuild", EditorStyles.toolbarButton, GUILayout.Width(58f))) RebuildWaveform();
            if (waveform != null && !string.IsNullOrEmpty(waveform.error))
                GUILayout.Label(waveform.error, EditorStyles.miniLabel);
            GUILayout.FlexibleSpace();
            frameRate = EditorGUILayout.FloatField("FPS", frameRate, GUILayout.Width(90f));
            EditorGUILayout.EndHorizontal();
        }

        private void DrawWorkspace(Rect workspace)
        {
            float rightWidth = Mathf.Min(InspectorWidth, Mathf.Max(250f, workspace.width * 0.28f));
            Rect trackList = new Rect(workspace.x, workspace.y, TrackListWidth, workspace.height);
            Rect canvas = new Rect(trackList.xMax, workspace.y, Mathf.Max(120f, workspace.width - TrackListWidth - rightWidth), workspace.height);
            Rect inspector = new Rect(canvas.xMax, workspace.y, rightWidth, workspace.height);
            EditorGUI.DrawRect(trackList, new Color(0.085f, 0.075f, 0.11f));
            EditorGUI.DrawRect(canvas, new Color(0.045f, 0.04f, 0.06f));
            EditorGUI.DrawRect(inspector, new Color(0.105f, 0.095f, 0.125f));
            DrawTrackList(trackList);
            DrawTimeline(canvas);
            DrawInspector(inspector);
        }

        private void DrawTrackList(Rect rect)
        {
            float top = rect.y;
            DrawTrackHeader(new Rect(rect.x, top, rect.width, RulerHeight), "TRACKS");
            top += RulerHeight;
            if (showWaveform)
            {
                GUI.Label(new Rect(rect.x + 8f, top + 8f, rect.width - 16f, 20f), "REFERENCE WAVEFORM", EditorStyles.miniBoldLabel);
                top += WaveformHeight;
            }
            DrawTrackHeader(new Rect(rect.x, top, rect.width, MarkerHeight), "STRUCTURE MARKERS");
            top += MarkerHeight;
            DrawTrackHeader(new Rect(rect.x, top, rect.width, LoopHeight), "LOOP REGIONS");
            top += LoopHeight;

            int count = show.tracks == null ? 0 : show.tracks.Length;
            for (int i = 0; i < count; i++)
            {
                SpectraTimelineTrack track = show.tracks[i];
                Rect row = new Rect(rect.x, top + i * TrackHeight, rect.width, TrackHeight);
                if (i == selectedTrack) EditorGUI.DrawRect(row, new Color(0.33f, 0.13f, 0.45f, 0.45f));
                if (track == null)
                {
                    GUI.Label(row, "Missing track");
                    continue;
                }
                EditorGUI.DrawRect(new Rect(row.x, row.y, 4f, row.height), track.displayColor);
                if (GUI.Button(new Rect(row.x + 8f, row.y + 5f, 22f, 18f), track.muted ? "M" : "●", EditorStyles.miniButton))
                {
                    Undo.RecordObject(show, "Toggle Track Mute");
                    track.muted = !track.muted;
                    Dirty();
                }
                if (GUI.Button(new Rect(row.x + 32f, row.y + 5f, 22f, 18f), track.locked ? "L" : "○", EditorStyles.miniButton))
                {
                    Undo.RecordObject(show, "Toggle Track Lock");
                    track.locked = !track.locked;
                    Dirty();
                }
                GUI.Label(new Rect(row.x + 59f, row.y + 3f, row.width - 64f, 20f), track.name, EditorStyles.boldLabel);
                GUI.Label(new Rect(row.x + 59f, row.y + 23f, row.width - 64f, 18f),
                    track.trackType + "  •  " + (track.cues == null ? 0 : track.cues.Length) + " cues", EditorStyles.miniLabel);
                if (Event.current.type == EventType.MouseDown && row.Contains(Event.current.mousePosition))
                {
                    selectedTrack = i;
                    selectedCue = -1;
                    selectedMarker = -1;
                    selectedLoop = -1;
                    Event.current.Use();
                    Repaint();
                }
            }
        }

        private void DrawTrackHeader(Rect rect, string label)
        {
            EditorGUI.DrawRect(rect, new Color(0.13f, 0.11f, 0.16f));
            GUI.Label(new Rect(rect.x + 8f, rect.y + 2f, rect.width - 16f, rect.height - 4f), label, centeredMini);
        }

        private void DrawTimeline(Rect rect)
        {
            float bottom = rect.yMax - BottomBarHeight;
            Rect view = new Rect(rect.x, rect.y, rect.width, rect.height - BottomBarHeight);
            GUI.BeginClip(view);
            Rect local = new Rect(0f, 0f, view.width, view.height);
            DrawGrid(local);
            float top = 0f;
            DrawRuler(new Rect(0f, top, local.width, RulerHeight));
            top += RulerHeight;
            if (showWaveform)
            {
                DrawWaveform(new Rect(0f, top, local.width, WaveformHeight));
                top += WaveformHeight;
            }
            DrawMarkers(new Rect(0f, top, local.width, MarkerHeight));
            top += MarkerHeight;
            DrawLoops(new Rect(0f, top, local.width, LoopHeight));
            top += LoopHeight;
            DrawCueTracks(new Rect(0f, top, local.width, Mathf.Max(0f, local.height - top)));
            DrawPlayhead(local);
            HandleTimelineEvents(local, top);
            GUI.EndClip();

            float visible = rect.width / Mathf.Max(1f, pixelsPerSecond);
            float maximum = Mathf.Max(0f, show.durationSeconds - visible);
            scrollSeconds = GUI.HorizontalScrollbar(
                new Rect(rect.x, bottom, rect.width, BottomBarHeight),
                Mathf.Clamp(scrollSeconds, 0f, maximum), visible, 0f, Mathf.Max(show.durationSeconds, visible));
        }

        private void DrawGrid(Rect rect)
        {
            float visibleEnd = scrollSeconds + rect.width / pixelsPerSecond;
            if (show.beatGrid != null)
            {
                int firstBeat = Mathf.Max(0, Mathf.FloorToInt((float)show.beatGrid.SecondsToBeat(scrollSeconds)) - 1);
                int lastBeat = Mathf.CeilToInt((float)show.beatGrid.SecondsToBeat(visibleEnd)) + 1;
                int stride = Mathf.Max(1, Mathf.CeilToInt((lastBeat - firstBeat) / 2500f));
                for (int beat = firstBeat; beat <= lastBeat; beat += stride)
                {
                    float time = (float)show.beatGrid.BeatToSeconds(beat);
                    float x = TimeToX(time);
                    bool bar = beat % Mathf.Max(1, show.beatGrid.beatsPerBar) == 0;
                    EditorGUI.DrawRect(new Rect(x, 0f, bar ? 1.5f : 1f, rect.height),
                        bar ? new Color(0.6f, 0.25f, 0.85f, 0.24f) : new Color(1f, 1f, 1f, 0.055f));
                }
            }
            else
            {
                int start = Mathf.FloorToInt(scrollSeconds);
                int end = Mathf.CeilToInt(visibleEnd);
                for (int second = start; second <= end; second++)
                    EditorGUI.DrawRect(new Rect(TimeToX(second), 0f, 1f, rect.height), new Color(1f, 1f, 1f, 0.08f));
            }
        }

        private void DrawRuler(Rect rect)
        {
            EditorGUI.DrawRect(rect, new Color(0.105f, 0.09f, 0.13f));
            float visibleEnd = scrollSeconds + rect.width / pixelsPerSecond;
            float interval = ChooseRulerInterval();
            float first = Mathf.Floor(scrollSeconds / interval) * interval;
            for (float time = first; time <= visibleEnd + interval; time += interval)
            {
                float x = TimeToX(time);
                EditorGUI.DrawRect(new Rect(x, rect.yMax - 7f, 1f, 7f), new Color(1f, 1f, 1f, 0.4f));
                GUI.Label(new Rect(x + 3f, rect.y + 2f, 70f, 18f), FormatTime(time), EditorStyles.miniLabel);
            }
        }

        private void DrawWaveform(Rect rect)
        {
            EditorGUI.DrawRect(rect, new Color(0.065f, 0.055f, 0.085f));
            EnsureWaveform();
            if (waveform != null && waveform.BucketCount > 0)
                waveform.Draw(rect, scrollSeconds - show.audioStartOffset,
                    scrollSeconds - show.audioStartOffset + rect.width / pixelsPerSecond,
                    new Color(0.75f, 0.25f, 1f, 0.72f));
            else
                GUI.Label(rect, "Assign an AudioClip and click Rebuild", centeredMini);
        }

        private void DrawMarkers(Rect rect)
        {
            EditorGUI.DrawRect(rect, new Color(0.08f, 0.065f, 0.1f));
            if (show.markers == null) return;
            for (int i = 0; i < show.markers.Length; i++)
            {
                SpectraTimelineMarker marker = show.markers[i];
                if (marker == null) continue;
                float x = TimeToX(marker.ResolveSeconds(show.beatGrid));
                EditorGUI.DrawRect(new Rect(x, rect.y, 2f, rect.height), marker.color);
                Rect labelRect = new Rect(x + 3f, rect.y + 4f, 92f, 18f);
                string markerPrefix = marker.scene ? "▶ " : marker.hotCue ? "◆ " : string.Empty;
                GUI.Label(labelRect, markerPrefix + marker.name,
                    marker.hotCue ? EditorStyles.miniBoldLabel : EditorStyles.miniLabel);
            }
        }

        private void DrawLoops(Rect rect)
        {
            EditorGUI.DrawRect(rect, new Color(0.06f, 0.075f, 0.1f));
            if (show.loopRegions == null) return;
            for (int i = 0; i < show.loopRegions.Length; i++)
            {
                SpectraLoopRegion loop = show.loopRegions[i];
                if (loop == null) continue;
                float x = TimeToX(loop.startSeconds);
                float width = Mathf.Max(2f, (loop.endSeconds - loop.startSeconds) * pixelsPerSecond);
                Color color = loop.enabled ? loop.color : new Color(loop.color.r, loop.color.g, loop.color.b, 0.08f);
                EditorGUI.DrawRect(new Rect(x, rect.y + 2f, width, rect.height - 4f), color);
                GUI.Label(new Rect(x + 4f, rect.y + 2f, Mathf.Max(0f, width - 8f), rect.height - 4f), loop.name, EditorStyles.miniLabel);
            }
        }

        private void DrawCueTracks(Rect rect)
        {
            int count = show.tracks == null ? 0 : show.tracks.Length;
            for (int trackIndex = 0; trackIndex < count; trackIndex++)
            {
                SpectraTimelineTrack track = show.tracks[trackIndex];
                Rect row = new Rect(rect.x, rect.y + trackIndex * TrackHeight, rect.width, TrackHeight);
                if (trackIndex % 2 == 0) EditorGUI.DrawRect(row, new Color(1f, 1f, 1f, 0.018f));
                if (track == null || track.cues == null) continue;
                for (int cueIndex = 0; cueIndex < track.cues.Length; cueIndex++)
                {
                    SpectraCueBlock cue = track.cues[cueIndex];
                    if (cue == null) continue;
                    Rect cueRect = CueRect(row, cue);
                    Color color = track.displayColor;
                    color.a = cue.enabled && !track.muted ? 0.86f : 0.28f;
                    EditorGUI.DrawRect(cueRect, color);
                    if (selectedTrack == trackIndex && selectedCue == cueIndex)
                    {
                        EditorGUI.DrawRect(new Rect(cueRect.x, cueRect.y, cueRect.width, 2f), Color.white);
                        EditorGUI.DrawRect(new Rect(cueRect.x, cueRect.yMax - 2f, cueRect.width, 2f), Color.white);
                    }
                    if (cue.fadeIn > 0f)
                        EditorGUI.DrawRect(new Rect(cueRect.x, cueRect.yMax - 3f,
                            Mathf.Min(cueRect.width, cue.fadeIn * pixelsPerSecond), 2f), new Color(1f, 1f, 1f, 0.7f));
                    if (cue.fadeOut > 0f)
                        EditorGUI.DrawRect(new Rect(cueRect.xMax - Mathf.Min(cueRect.width, cue.fadeOut * pixelsPerSecond),
                            cueRect.y + 1f, Mathf.Min(cueRect.width, cue.fadeOut * pixelsPerSecond), 2f), new Color(0f, 0f, 0f, 0.7f));
                    GUI.Label(new Rect(cueRect.x + 5f, cueRect.y + 4f, Mathf.Max(0f, cueRect.width - 10f), 18f),
                        cue.name, cueLabel);
                    GUI.Label(new Rect(cueRect.x + 5f, cueRect.y + 22f, Mathf.Max(0f, cueRect.width - 10f), 15f),
                        cue.valueType + "  P" + cue.priority
                        + (cue.modulationWaveform != SpectraModulationWaveform.Disabled ? "  MOD" : "")
                        + (cue.performanceMacroIndex >= 0 ? "  M" + (cue.performanceMacroIndex + 1) : "")
                        + (cue.gatePattern != SpectraCueGatePattern.Disabled ? "  GATE" : "")
                        + (cue.paletteMode != SpectraPalettePlaybackMode.Disabled ? "  PAL" : "")
                        + (cue.conditionMode != SpectraCueConditionMode.Disabled ? "  COND" : "")
                        + (cue.variationMode != SpectraVariationSelectionMode.Disabled ? "  VAR" : ""),
                        EditorStyles.miniLabel);
                    if (cue.automationMode != SpectraAutomationMode.Disabled
                        && cue.automationKeys != null)
                    {
                        for (int keyIndex = 0; keyIndex < cue.automationKeys.Length; keyIndex++)
                        {
                            SpectraAutomationKey key = cue.automationKeys[keyIndex];
                            if (key == null) continue;
                            float keyX = Mathf.Lerp(cueRect.x + 3f, cueRect.xMax - 3f,
                                Mathf.Clamp01(key.normalizedTime));
                            float keyY = Mathf.Lerp(cueRect.yMax - 5f, cueRect.y + 5f,
                                Mathf.Clamp01(key.value.x));
                            EditorGUI.DrawRect(new Rect(keyX - 1.5f, keyY - 1.5f, 3f, 3f),
                                new Color(1f, 0.95f, 0.35f, 1f));
                        }
                    }
                }
            }
        }

        private void DrawPlayhead(Rect rect)
        {
            float x = TimeToX(playhead);
            EditorGUI.DrawRect(new Rect(x, 0f, 2f, rect.height), new Color(1f, 0.22f, 0.72f, 1f));
            EditorGUI.DrawRect(new Rect(x - 4f, 0f, 10f, 5f), new Color(1f, 0.22f, 0.72f, 1f));
        }

        private void HandleTimelineEvents(Rect local, float tracksTop)
        {
            Event evt = Event.current;
            if (evt.type == EventType.ScrollWheel && local.Contains(evt.mousePosition))
            {
                if (evt.control || evt.command)
                {
                    float anchorTime = XToTime(evt.mousePosition.x);
                    pixelsPerSecond = Mathf.Clamp(pixelsPerSecond * Mathf.Pow(1.12f, -evt.delta.y), 20f, 800f);
                    scrollSeconds = Mathf.Max(0f, anchorTime - evt.mousePosition.x / pixelsPerSecond);
                }
                else
                    scrollSeconds = Mathf.Clamp(scrollSeconds + evt.delta.y * 0.35f, 0f, Mathf.Max(0f, show.durationSeconds));
                evt.Use();
                Repaint();
                return;
            }

            if (evt.type == EventType.MouseDown && evt.button == 0)
            {
                if (evt.mousePosition.y < RulerHeight)
                {
                    dragMode = DragMode.MovePlayhead;
                    SetPlayhead(Snap(XToTime(evt.mousePosition.x)), true);
                    evt.Use();
                    return;
                }
                float markerTop = RulerHeight + (showWaveform ? WaveformHeight : 0f);
                if (evt.mousePosition.y >= markerTop && evt.mousePosition.y < markerTop + MarkerHeight)
                {
                    int markerIndex = FindNearestMarker(evt.mousePosition.x, 8f);
                    if (markerIndex >= 0)
                    {
                        selectedMarker = markerIndex;
                        selectedLoop = -1;
                        selectedTrack = -1;
                        selectedCue = -1;
                        dragMode = DragMode.MoveMarker;
                        dragMouseStart = evt.mousePosition;
                        dragOriginalStart = show.markers[markerIndex].ResolveSeconds(show.beatGrid);
                        Undo.RecordObject(show, "Move Spectra Marker");
                    }
                    else SetPlayhead(Snap(XToTime(evt.mousePosition.x)), true);
                    evt.Use();
                    return;
                }
                float loopTop = markerTop + MarkerHeight;
                if (evt.mousePosition.y >= loopTop && evt.mousePosition.y < loopTop + LoopHeight)
                {
                    int loopIndex = FindLoopAt(evt.mousePosition.x);
                    if (loopIndex >= 0)
                    {
                        SpectraLoopRegion loop = show.loopRegions[loopIndex];
                        selectedLoop = loopIndex;
                        selectedMarker = -1;
                        selectedTrack = -1;
                        selectedCue = -1;
                        dragMouseStart = evt.mousePosition;
                        dragOriginalStart = loop.startSeconds;
                        dragOriginalEnd = loop.endSeconds;
                        float startX = TimeToX(loop.startSeconds);
                        float endX = TimeToX(loop.endSeconds);
                        dragMode = Mathf.Abs(evt.mousePosition.x - startX) <= 6f ? DragMode.ResizeLoopStart
                            : Mathf.Abs(evt.mousePosition.x - endX) <= 6f ? DragMode.ResizeLoopEnd
                            : DragMode.MoveLoop;
                        Undo.RecordObject(show, "Edit Spectra Loop");
                    }
                    evt.Use();
                    return;
                }
                int trackIndex = Mathf.FloorToInt((evt.mousePosition.y - tracksTop) / TrackHeight);
                if (trackIndex < 0 || show.tracks == null || trackIndex >= show.tracks.Length) return;
                SpectraTimelineTrack track = show.tracks[trackIndex];
                if (track == null || track.locked) return;
                int hitCue = FindCueAt(trackIndex, evt.mousePosition, tracksTop);
                selectedTrack = trackIndex;
                selectedCue = hitCue;
                selectedMarker = -1;
                selectedLoop = -1;
                if (hitCue >= 0)
                {
                    SpectraCueBlock cue = track.cues[hitCue];
                    if (toolMode == ToolMode.Razor)
                    {
                        float split = Snap(XToTime(evt.mousePosition.x));
                        Undo.RecordObject(show, "Split Spectra Cue");
                        try
                        {
                            SpectraTimelineEditing.SplitCue(show, trackIndex, hitCue, split);
                            Dirty();
                        }
                        catch (ArgumentOutOfRangeException) { }
                        evt.Use();
                        return;
                    }
                    if (evt.clickCount == 2)
                    {
                        selectedCue = hitCue;
                        evt.Use();
                        return;
                    }
                    Rect row = new Rect(0f, tracksTop + trackIndex * TrackHeight, local.width, TrackHeight);
                    Rect cueRect = CueRect(row, cue);
                    dragMode = cueRect.xMax - evt.mousePosition.x <= 7f ? DragMode.ResizeCueEnd : DragMode.MoveCue;
                    dragMouseStart = evt.mousePosition;
                    dragOriginalStart = cue.ResolveStartSeconds(show.beatGrid);
                    dragOriginalDuration = cue.ResolveDurationSeconds(show.beatGrid);
                    Undo.RecordObject(show, dragMode == DragMode.MoveCue ? "Move Spectra Cue" : "Resize Spectra Cue");
                    evt.Use();
                }
                else if (toolMode == ToolMode.Draw)
                {
                    float start = Snap(XToTime(evt.mousePosition.x));
                    float duration = show.beatGrid == null ? 1f : (float)(show.beatGrid.BeatToSeconds(
                        show.beatGrid.SecondsToBeat(start) + 1d) - start);
                    Undo.RecordObject(show, "Create Spectra Cue");
                    SpectraCueBlock cue = SpectraTimelineEditing.CreateCue(show, trackIndex, start, Mathf.Max(0.05f, duration));
                    selectedCue = Array.IndexOf(track.cues, cue);
                    Dirty();
                    evt.Use();
                }
                Repaint();
            }
            else if (evt.type == EventType.MouseDrag && evt.button == 0)
            {
                if (dragMode == DragMode.MovePlayhead)
                {
                    SetPlayhead(Snap(XToTime(evt.mousePosition.x)), true);
                    evt.Use();
                }
                else if ((dragMode == DragMode.MoveCue || dragMode == DragMode.ResizeCueEnd) && TryGetSelectedCue(out SpectraCueBlock cue))
                {
                    float delta = (evt.mousePosition.x - dragMouseStart.x) / pixelsPerSecond;
                    if (dragMode == DragMode.MoveCue)
                        SpectraTimelineEditing.SetCueStart(show, cue, Snap(dragOriginalStart + delta));
                    else
                        SpectraTimelineEditing.SetCueDuration(show, cue,
                            Mathf.Max(0.01f, Snap(dragOriginalStart + dragOriginalDuration + delta) - dragOriginalStart));
                    Dirty(false);
                    evt.Use();
                    Repaint();
                }
                else if (dragMode == DragMode.MoveMarker && selectedMarker >= 0 && show.markers != null && selectedMarker < show.markers.Length)
                {
                    float target = Snap(dragOriginalStart + (evt.mousePosition.x - dragMouseStart.x) / pixelsPerSecond);
                    SpectraTimelineMarker marker = show.markers[selectedMarker];
                    if (marker.timingMode == SpectraTimingMode.Musical && show.beatGrid != null)
                        marker.musicalPosition = show.beatGrid.SecondsToMusical(target);
                    else marker.timeSeconds = target;
                    Dirty(false);
                    evt.Use();
                }
                else if ((dragMode == DragMode.MoveLoop || dragMode == DragMode.ResizeLoopStart || dragMode == DragMode.ResizeLoopEnd)
                    && selectedLoop >= 0 && show.loopRegions != null && selectedLoop < show.loopRegions.Length)
                {
                    SpectraLoopRegion loop = show.loopRegions[selectedLoop];
                    float delta = (evt.mousePosition.x - dragMouseStart.x) / pixelsPerSecond;
                    if (dragMode == DragMode.MoveLoop)
                    {
                        float duration = dragOriginalEnd - dragOriginalStart;
                        loop.startSeconds = Snap(dragOriginalStart + delta);
                        loop.endSeconds = Mathf.Min(show.durationSeconds, loop.startSeconds + duration);
                    }
                    else if (dragMode == DragMode.ResizeLoopStart)
                        loop.startSeconds = Mathf.Min(loop.endSeconds - 0.01f, Snap(dragOriginalStart + delta));
                    else
                        loop.endSeconds = Mathf.Max(loop.startSeconds + 0.01f, Snap(dragOriginalEnd + delta));
                    Dirty(false);
                    evt.Use();
                }
            }
            else if (evt.type == EventType.MouseUp)
            {
                if (dragMode != DragMode.None)
                {
                    dragMode = DragMode.None;
                    Dirty();
                    evt.Use();
                }
            }
            else if (evt.type == EventType.ContextClick)
            {
                int trackIndex = Mathf.FloorToInt((evt.mousePosition.y - tracksTop) / TrackHeight);
                if (trackIndex >= 0 && show.tracks != null && trackIndex < show.tracks.Length)
                {
                    int cueIndex = FindCueAt(trackIndex, evt.mousePosition, tracksTop);
                    if (cueIndex >= 0)
                    {
                        selectedTrack = trackIndex;
                        selectedCue = cueIndex;
                        ShowCueContextMenu();
                        evt.Use();
                    }
                }
            }
        }

        private void DrawInspector(Rect rect)
        {
            GUILayout.BeginArea(new Rect(rect.x + 8f, rect.y + 6f, rect.width - 16f, rect.height - 12f));
            inspectorScroll = EditorGUILayout.BeginScrollView(inspectorScroll);
            if (selectedTrack >= 0 && show.tracks != null && selectedTrack < show.tracks.Length)
            {
                SerializedObject serialized = new SerializedObject(show);
                serialized.Update();
                SerializedProperty trackProperty = serialized.FindProperty("tracks").GetArrayElementAtIndex(selectedTrack);
                SpectraTimelineTrack track = show.tracks[selectedTrack];
                GUILayout.Label("TRACK", EditorStyles.miniBoldLabel);
                EditorGUILayout.PropertyField(trackProperty.FindPropertyRelative("name"));
                EditorGUILayout.PropertyField(trackProperty.FindPropertyRelative("trackType"));
                EditorGUILayout.PropertyField(trackProperty.FindPropertyRelative("fixtureGroupId"));
                EditorGUILayout.PropertyField(trackProperty.FindPropertyRelative("displayColor"));
                EditorGUILayout.BeginHorizontal();
                if (GUILayout.Button("Move Up")) { serialized.ApplyModifiedProperties(); MoveSelectedTrack(-1); }
                if (GUILayout.Button("Move Down")) { serialized.ApplyModifiedProperties(); MoveSelectedTrack(1); }
                if (GUILayout.Button("Delete")) { serialized.ApplyModifiedProperties(); DeleteSelectedTrack(); }
                EditorGUILayout.EndHorizontal();
                EditorGUILayout.Space();

                if (selectedCue >= 0 && track != null && track.cues != null && selectedCue < track.cues.Length)
                {
                    GUILayout.Label("CUE", EditorStyles.miniBoldLabel);
                    SerializedProperty cueProperty = trackProperty.FindPropertyRelative("cues").GetArrayElementAtIndex(selectedCue);
                    EditorGUILayout.PropertyField(cueProperty, true);
                    EditorGUILayout.Space();
                    DrawPresetTools();
                    EditorGUILayout.Space();
                    DrawCueQuickTools(track.cues[selectedCue]);
                }
                ApplySerializedChanges(serialized);
            }
            else if (selectedMarker >= 0 && show.markers != null && selectedMarker < show.markers.Length)
            {
                GUILayout.Label("MARKER", EditorStyles.miniBoldLabel);
                SerializedObject serialized = new SerializedObject(show);
                serialized.Update();
                SerializedProperty marker = serialized.FindProperty("markers").GetArrayElementAtIndex(selectedMarker);
                EditorGUILayout.PropertyField(marker, true);
                ApplySerializedChanges(serialized);
                if (GUILayout.Button("Delete Marker"))
                {
                    Undo.RecordObject(show, "Delete Spectra Marker");
                    show.markers = RemoveAt(show.markers, selectedMarker);
                    selectedMarker = -1;
                    Dirty();
                    GUIUtility.ExitGUI();
                }
            }
            else if (selectedLoop >= 0 && show.loopRegions != null && selectedLoop < show.loopRegions.Length)
            {
                GUILayout.Label("LOOP REGION", EditorStyles.miniBoldLabel);
                SerializedObject serialized = new SerializedObject(show);
                serialized.Update();
                SerializedProperty loop = serialized.FindProperty("loopRegions").GetArrayElementAtIndex(selectedLoop);
                EditorGUILayout.PropertyField(loop, true);
                ApplySerializedChanges(serialized);
                if (GUILayout.Button("Delete Loop Region"))
                {
                    Undo.RecordObject(show, "Delete Spectra Loop");
                    show.loopRegions = RemoveAt(show.loopRegions, selectedLoop);
                    selectedLoop = -1;
                    Dirty();
                    GUIUtility.ExitGUI();
                }
            }
            else
            {
                GUILayout.Label("SHOW", EditorStyles.miniBoldLabel);
                SerializedObject serialized = new SerializedObject(show);
                serialized.Update();
                EditorGUILayout.PropertyField(serialized.FindProperty("showName"));
                EditorGUILayout.PropertyField(serialized.FindProperty("artist"));
                EditorGUILayout.PropertyField(serialized.FindProperty("songName"));
                EditorGUILayout.PropertyField(serialized.FindProperty("durationSeconds"));
                EditorGUILayout.PropertyField(serialized.FindProperty("audioStartOffset"));
                EditorGUILayout.PropertyField(serialized.FindProperty("audioReference"));
                EditorGUILayout.PropertyField(serialized.FindProperty("beatGrid"), true);
                EditorGUILayout.PropertyField(serialized.FindProperty("fixtureGroups"), true);
                EditorGUILayout.PropertyField(serialized.FindProperty("colorPalettes"), true);
                EditorGUILayout.PropertyField(serialized.FindProperty("performanceMacros"), true);
                EditorGUILayout.PropertyField(serialized.FindProperty("performanceMacroSnapshots"), true);
                if (((show.colorPalettes == null || show.colorPalettes.Length == 0)
                        || (show.performanceMacros == null || show.performanceMacros.Length == 0)
                        || (show.performanceMacroSnapshots == null
                            || show.performanceMacroSnapshots.Length == 0))
                    && GUILayout.Button("Create 1.4 Starter Palettes and Snapshots"))
                {
                    serialized.ApplyModifiedProperties();
                    Undo.RecordObject(show, "Create Spectra Starter Palettes");
                    SpectraRhythmPaletteAuthoring.EnsureStarterPalettes(show);
                    SpectraGenerativeAuthoring.EnsureStarterSnapshots(show);
                    Dirty();
                }
                EditorGUILayout.PropertyField(serialized.FindProperty("platformPolicies"), true);
                EditorGUILayout.PropertyField(serialized.FindProperty("accessibility"), true);
                ApplySerializedChanges(serialized);
            }
            EditorGUILayout.EndScrollView();
            GUILayout.EndArea();
        }

        private void DrawPresetTools()
        {
            GUILayout.Label("PRESETS", EditorStyles.miniBoldLabel);
            cueTemplate = (SpectraCueTemplateAsset)EditorGUILayout.ObjectField("Cue", cueTemplate, typeof(SpectraCueTemplateAsset), false);
            if (cueTemplate != null && GUILayout.Button("Replace Cue Values From Template")) ApplyCueTemplate();
            movementPreset = (SpectraMovementPresetAsset)EditorGUILayout.ObjectField("Movement", movementPreset, typeof(SpectraMovementPresetAsset), false);
            if (movementPreset != null && GUILayout.Button("Apply Movement Preset")) ApplyMovementPreset();
            colorPalette = (SpectraColorPaletteAsset)EditorGUILayout.ObjectField("Palette", colorPalette, typeof(SpectraColorPaletteAsset), false);
            if (colorPalette != null && GUILayout.Button("Apply First Palette Color")) ApplyPalette();
            sectionTemplate = (SpectraSectionTemplateAsset)EditorGUILayout.ObjectField("Section", sectionTemplate, typeof(SpectraSectionTemplateAsset), false);
            if (sectionTemplate != null && GUILayout.Button("Insert Section At Playhead")) ApplySectionTemplate();
        }

        private void DrawCueQuickTools(SpectraCueBlock cue)
        {
            GUILayout.Label("EDIT", EditorStyles.miniBoldLabel);
            EditorGUILayout.BeginHorizontal();
            if (GUILayout.Button("Copy")) cueClipboard = SpectraTimelineEditing.CopyCueToJson(cue);
            if (GUILayout.Button("Duplicate")) DuplicateSelectedCue();
            if (GUILayout.Button("Split Here")) SplitSelectedCue();
            if (GUILayout.Button("Delete")) DeleteSelectedCue();
            EditorGUILayout.EndHorizontal();
            if (compatibilityCache == null) compatibilityCache = SpectraPlatformCompatibilityValidator.AnalyzeAll(show);
            SpectraPlatformCompatibilityResult[] results = compatibilityCache;
            for (int i = 0; i < results.Length; i++)
            {
                SpectraPlatformCompatibilityResult result = results[i];
                Color old = GUI.color;
                GUI.color = result.FitsBudget ? new Color(0.65f, 1f, 0.75f) : new Color(1f, 0.55f, 0.45f);
                GUILayout.Label(result.platform + "  " + result.maximumConcurrentCues + "/" + result.cueBudget
                    + " cues @ " + result.updateRate + " Hz", EditorStyles.miniLabel);
                GUI.color = old;
            }
            GUILayout.Label("AUTOMATION", EditorStyles.miniBoldLabel);
            EditorGUILayout.BeginHorizontal();
            if (GUILayout.Button("Pulse"))
            {
                Undo.RecordObject(show, "Add Pulse Automation");
                SpectraAutomationAuthoring.ApplyPulseEnvelope(cue);
                Dirty();
            }
            if (GUILayout.Button("Riser"))
            {
                Undo.RecordObject(show, "Add Riser Automation");
                SpectraAutomationAuthoring.ApplyRiserEnvelope(cue);
                Dirty();
            }
            if (GUILayout.Button("4-Beat Gate"))
            {
                Undo.RecordObject(show, "Add Gate Automation");
                SpectraAutomationAuthoring.ApplyFourBeatGate(cue);
                Dirty();
            }
            if (GUILayout.Button("Clear"))
            {
                Undo.RecordObject(show, "Clear Cue Automation");
                SpectraAutomationAuthoring.Clear(cue);
                Dirty();
            }
            EditorGUILayout.EndHorizontal();
            GUILayout.Label("PROCEDURAL", EditorStyles.miniBoldLabel);
            EditorGUILayout.BeginHorizontal();
            if (GUILayout.Button("Beat Pulse"))
            {
                Undo.RecordObject(show, "Add Beat Pulse Modulation");
                SpectraProceduralAuthoring.ApplyBeatPulse(cue);
                Dirty();
            }
            if (GUILayout.Button("8-Bar Breathe"))
            {
                Undo.RecordObject(show, "Add Breathing Modulation");
                SpectraProceduralAuthoring.ApplyEightBarBreathing(cue);
                Dirty();
            }
            if (GUILayout.Button("Flicker"))
            {
                Undo.RecordObject(show, "Add Deterministic Flicker");
                SpectraProceduralAuthoring.ApplyDeterministicFlicker(cue);
                Dirty();
            }
            if (GUILayout.Button("Clear Mod"))
            {
                Undo.RecordObject(show, "Clear Procedural Modulation");
                SpectraProceduralAuthoring.Clear(cue);
                Dirty();
            }
            EditorGUILayout.EndHorizontal();
            GUILayout.Label("RHYTHM GATE", EditorStyles.miniBoldLabel);
            EditorGUILayout.BeginHorizontal();
            if (GUILayout.Button("Euclid 5/8"))
            {
                Undo.RecordObject(show, "Add Euclidean Rhythm Gate");
                SpectraRhythmPaletteAuthoring.ApplyEuclideanGate(cue, 5, 8);
                Dirty();
            }
            if (GUILayout.Button("Syncopate"))
            {
                Undo.RecordObject(show, "Add Syncopated Rhythm Gate");
                SpectraRhythmPaletteAuthoring.ApplySyncopatedMask(cue);
                Dirty();
            }
            if (GUILayout.Button("Seeded"))
            {
                Undo.RecordObject(show, "Add Seeded Rhythm Gate");
                SpectraRhythmPaletteAuthoring.ApplySeededGate(cue);
                Dirty();
            }
            if (GUILayout.Button("Clear Gate"))
            {
                Undo.RecordObject(show, "Clear Rhythm Gate");
                SpectraRhythmPaletteAuthoring.ClearGate(cue);
                Dirty();
            }
            EditorGUILayout.EndHorizontal();
            GUILayout.Label("GENERATIVE CONDITIONS", EditorStyles.miniBoldLabel);
            EditorGUILayout.BeginHorizontal();
            if (GUILayout.Button("50% Chance"))
            {
                Undo.RecordObject(show, "Add Probability Condition");
                SpectraGenerativeAuthoring.ApplyProbabilityCondition(cue, 0.5f);
                Dirty();
            }
            if (GUILayout.Button("Every 4 Bars"))
            {
                Undo.RecordObject(show, "Add Every-Four Condition");
                SpectraGenerativeAuthoring.ApplyEveryNthCondition(cue, 4);
                Dirty();
            }
            if (GUILayout.Button("Energy > 50%"))
            {
                Undo.RecordObject(show, "Add Macro Condition");
                SpectraGenerativeAuthoring.ApplyMacroCondition(cue, 0, 0.5f, true);
                Dirty();
            }
            if (GUILayout.Button("Clear Cond"))
            {
                Undo.RecordObject(show, "Clear Cue Condition");
                SpectraGenerativeAuthoring.ClearCondition(cue);
                Dirty();
            }
            EditorGUILayout.EndHorizontal();
            GUILayout.Label("VARIATION ROUTING", EditorStyles.miniBoldLabel);
            EditorGUILayout.BeginHorizontal();
            if (GUILayout.Button("Cycle A"))
            {
                Undo.RecordObject(show, "Assign Cycle Variation A");
                SpectraGenerativeAuthoring.ApplyCycleVariation(cue, 0, 0, 2);
                Dirty();
            }
            if (GUILayout.Button("Cycle B"))
            {
                Undo.RecordObject(show, "Assign Cycle Variation B");
                SpectraGenerativeAuthoring.ApplyCycleVariation(cue, 0, 1, 2);
                Dirty();
            }
            if (GUILayout.Button("Seeded A"))
            {
                Undo.RecordObject(show, "Assign Seeded Variation");
                SpectraGenerativeAuthoring.ApplySeededVariation(cue, 0, 0, 2);
                Dirty();
            }
            if (GUILayout.Button("Clear Var"))
            {
                Undo.RecordObject(show, "Clear Cue Variation");
                SpectraGenerativeAuthoring.ClearVariation(cue);
                Dirty();
            }
            EditorGUILayout.EndHorizontal();
            GUILayout.Label("DYNAMIC PALETTE", EditorStyles.miniBoldLabel);
            EditorGUILayout.BeginHorizontal();
            GUI.enabled = show.colorPalettes != null && show.colorPalettes.Length > 0;
            if (GUILayout.Button("Palette Step"))
            {
                Undo.RecordObject(show, "Bind Palette Step");
                SpectraRhythmPaletteAuthoring.ApplyPaletteStep(cue, 0);
                Dirty();
            }
            GUI.enabled = show.colorPalettes != null && show.colorPalettes.Length > 0
                && show.performanceMacros != null && show.performanceMacros.Length > 0;
            if (GUILayout.Button("Macro Morph"))
            {
                Undo.RecordObject(show, "Bind Palette Macro Morph");
                SpectraRhythmPaletteAuthoring.ApplyPaletteMacroMorph(cue, 0, 0);
                Dirty();
            }
            GUI.enabled = true;
            if (GUILayout.Button("Clear Palette"))
            {
                Undo.RecordObject(show, "Clear Dynamic Palette");
                SpectraRhythmPaletteAuthoring.ClearPalette(cue);
                Dirty();
            }
            EditorGUILayout.EndHorizontal();
        }

        private void HandleKeyboard()
        {
            Event evt = Event.current;
            if (evt.type != EventType.KeyDown || EditorGUIUtility.editingTextField) return;
            if (evt.keyCode == KeyCode.Space)
            {
                if (playing) StopPreview(false); else StartPreview();
                evt.Use();
            }
            else if (evt.keyCode == KeyCode.Delete || evt.keyCode == KeyCode.Backspace)
            {
                DeleteSelectedCue();
                evt.Use();
            }
            else if ((evt.control || evt.command) && evt.keyCode == KeyCode.C)
            {
                if (TryGetSelectedCue(out SpectraCueBlock cue)) cueClipboard = SpectraTimelineEditing.CopyCueToJson(cue);
                evt.Use();
            }
            else if ((evt.control || evt.command) && evt.keyCode == KeyCode.V)
            {
                PasteCue();
                evt.Use();
            }
            else if ((evt.control || evt.command) && evt.keyCode == KeyCode.D)
            {
                DuplicateSelectedCue();
                evt.Use();
            }
            else if (evt.keyCode == KeyCode.M)
            {
                AddMarkerAtPlayhead();
                evt.Use();
            }
        }

        private void EditorTick()
        {
            if (!playing || show == null) return;
            double now = EditorApplication.timeSinceStartup;
            double delta = Math.Max(0d, now - lastEditorTime);
            lastEditorTime = now;
            float next = playhead + (float)delta;
            SpectraLoopRegion loop = FirstEnabledLoopContaining(playhead);
            if (loop != null && next >= loop.endSeconds)
                next = loop.startSeconds + Mathf.Repeat(next - loop.startSeconds, loop.endSeconds - loop.startSeconds);
            if (next >= show.durationSeconds)
            {
                next = show.durationSeconds;
                StopPreview(false);
            }
            SetPlayhead(next, true);
            Repaint();
        }

        private void StartPreview()
        {
            if (show == null) return;
            if (!CompilePreview(false)) return;
            playing = true;
            lastEditorTime = EditorApplication.timeSinceStartup;
        }

        private void StopPreview(bool resetPlayer)
        {
            playing = false;
            if (resetPlayer && previewPlayer != null)
            {
                previewPlayer.Stop();
                EditorUtility.SetDirty(previewPlayer);
            }
        }

        private bool CompilePreview(bool notify)
        {
            try
            {
                previewCompiled = SpectraShowCompiler.Compile(show);
                if (previewPlayer != null)
                {
                    Undo.RecordObject(previewPlayer, "Assign Spectra Preview Show");
                    SpectraShowCompiler.ApplyToRuntimePlayer(previewCompiled, previewPlayer);
                    previewPlayer.ApplyAtTime(playhead);
                    EditorUtility.SetDirty(previewPlayer);
                }
                if (notify) EditorUtility.DisplayDialog("SpectraOverdrive", "Compiled " + previewCompiled.CueCount
                    + " cues, " + previewCompiled.markerTimes.Length + " markers, and "
                    + previewCompiled.loopStarts.Length + " loops.", "OK");
                return true;
            }
            catch (Exception exception)
            {
                EditorUtility.DisplayDialog("SpectraOverdrive Compile Failed", exception.Message, "OK");
                return false;
            }
        }

        private void SetPlayhead(float value, bool applyPreview)
        {
            playhead = Mathf.Clamp(value, 0f, show == null ? float.MaxValue : show.durationSeconds);
            if (applyPreview && previewPlayer != null)
            {
                if (previewCompiled == null && !CompilePreview(false)) return;
                SpectraShowCompiler.ApplyToRuntimePlayer(previewCompiled, previewPlayer);
                previewPlayer.ApplyAtTime(playhead);
                SceneView.RepaintAll();
            }
        }

        private void SetShow(SpectraShowAsset value)
        {
            StopPreview(false);
            show = value;
            selectedTrack = -1;
            selectedCue = -1;
            selectedMarker = -1;
            selectedLoop = -1;
            playhead = 0f;
            scrollSeconds = 0f;
            previewCompiled = null;
            compatibilityCache = null;
            waveform = null;
            waveformClip = null;
            if (show != null)
            {
                show.EnsureStableIds();
                EnsureWaveform();
            }
        }

        private void CreateShowAsset()
        {
            string path = EditorUtility.SaveFilePanelInProject(
                "Create SpectraOverdrive Show", "SpectraShow", "asset", "Choose where to save the show.");
            if (string.IsNullOrEmpty(path)) return;
            SpectraShowAsset asset = CreateInstance<SpectraShowAsset>();
            asset.platformPolicies = new[]
            {
                SpectraPlatformPolicy.CreateDefault(SpectraPlatformKind.PC),
                SpectraPlatformPolicy.CreateDefault(SpectraPlatformKind.Quest),
                SpectraPlatformPolicy.CreateDefault(SpectraPlatformKind.IOS),
                SpectraPlatformPolicy.CreateDefault(SpectraPlatformKind.Android)
            };
            asset.EnsureStableIds();
            AssetDatabase.CreateAsset(asset, path);
            AssetDatabase.SaveAssets();
            Selection.activeObject = asset;
            SetShow(asset);
        }

        private void ShowAddTrackMenu()
        {
            GenericMenu menu = new GenericMenu();
            foreach (SpectraTrackType type in Enum.GetValues(typeof(SpectraTrackType)))
            {
                SpectraTrackType captured = type;
                menu.AddItem(new GUIContent(captured.ToString()), false, delegate
                {
                    Undo.RecordObject(show, "Add Spectra Track");
                    string groupId = show.fixtureGroups != null && show.fixtureGroups.Length > 0
                        ? show.fixtureGroups[0].id : string.Empty;
                    SpectraTimelineTrack track = SpectraTimelineEditing.AddTrack(show, captured, groupId);
                    selectedTrack = Array.IndexOf(show.tracks, track);
                    selectedCue = -1;
                    Dirty();
                });
            }
            menu.ShowAsContext();
        }

        private void ShowCueContextMenu()
        {
            GenericMenu menu = new GenericMenu();
            menu.AddItem(new GUIContent("Copy"), false, delegate
            {
                if (TryGetSelectedCue(out SpectraCueBlock cue)) cueClipboard = SpectraTimelineEditing.CopyCueToJson(cue);
            });
            menu.AddItem(new GUIContent("Paste At Playhead"), false, PasteCue);
            menu.AddItem(new GUIContent("Duplicate"), false, DuplicateSelectedCue);
            menu.AddItem(new GUIContent("Split At Playhead"), false, SplitSelectedCue);
            menu.AddSeparator("");
            menu.AddItem(new GUIContent("Delete"), false, DeleteSelectedCue);
            menu.ShowAsContext();
        }

        private void AddMarkerAtPlayhead()
        {
            Undo.RecordObject(show, "Add Spectra Marker");
            SpectraTimelineEditing.AddMarker(show, Snap(playhead), "Marker " + (show.markers.Length + 1), SpectraMarkerKind.Generic);
            Dirty();
        }

        private void AddLoopAtPlayhead()
        {
            float start = Snap(playhead);
            double startBeat = show.beatGrid == null ? start : show.beatGrid.SecondsToBeat(start);
            float end = show.beatGrid == null ? start + 4f : (float)show.beatGrid.BeatToSeconds(startBeat + 8d);
            Undo.RecordObject(show, "Add Spectra Loop");
            SpectraTimelineEditing.AddLoop(show, start, Mathf.Min(show.durationSeconds, end), "8-Beat Loop");
            Dirty();
        }

        private void SetDownbeatHere()
        {
            if (show.beatGrid == null) show.beatGrid = new SpectraBeatGrid();
            Undo.RecordObject(show, "Set Spectra Downbeat");
            show.beatGrid.firstDownbeatSeconds = playhead;
            Dirty();
        }

        private void TapTempo()
        {
            double now = EditorApplication.timeSinceStartup;
            if (lastTapTime > 0d)
            {
                double interval = now - lastTapTime;
                if (interval > 0.15d && interval < 2d)
                {
                    float instant = (float)(60d / interval);
                    tapBpm = tapBpm <= 0f ? instant : Mathf.Lerp(tapBpm, instant, 0.35f);
                    Undo.RecordObject(show, "Tap Spectra BPM");
                    if (show.beatGrid == null) show.beatGrid = new SpectraBeatGrid();
                    show.beatGrid.bpm = Mathf.Clamp(tapBpm, 30f, 300f);
                    Dirty();
                }
                else tapBpm = 0f;
            }
            lastTapTime = now;
        }

        private void SeekMarker(int direction)
        {
            if (show.markers == null || show.markers.Length == 0) return;
            float best = direction > 0 ? float.MaxValue : float.MinValue;
            for (int i = 0; i < show.markers.Length; i++)
            {
                SpectraTimelineMarker marker = show.markers[i];
                if (marker == null) continue;
                float time = marker.ResolveSeconds(show.beatGrid);
                if (direction > 0 && time > playhead + 0.001f && time < best) best = time;
                if (direction < 0 && time < playhead - 0.001f && time > best) best = time;
            }
            if (best != float.MaxValue && best != float.MinValue) SetPlayhead(best, true);
            else if (direction < 0) SetPlayhead(0f, true);
        }

        private void DuplicateSelectedCue()
        {
            if (!TryGetSelectedCue(out SpectraCueBlock cue)) return;
            Undo.RecordObject(show, "Duplicate Spectra Cue");
            float beatLength = show.beatGrid == null ? 1f : (float)(show.beatGrid.BeatToSeconds(
                show.beatGrid.SecondsToBeat(cue.ResolveStartSeconds(show.beatGrid)) + 1d) - cue.ResolveStartSeconds(show.beatGrid));
            SpectraCueBlock duplicate = SpectraTimelineEditing.DuplicateCue(show, selectedTrack, selectedCue, Mathf.Max(0.05f, beatLength));
            selectedCue = Array.IndexOf(show.tracks[selectedTrack].cues, duplicate);
            Dirty();
        }

        private void DeleteSelectedCue()
        {
            if (!TryGetSelectedCue(out _)) return;
            Undo.RecordObject(show, "Delete Spectra Cue");
            SpectraTimelineEditing.DeleteCue(show, selectedTrack, selectedCue);
            selectedCue = -1;
            Dirty();
            GUIUtility.ExitGUI();
        }

        private void SplitSelectedCue()
        {
            if (!TryGetSelectedCue(out SpectraCueBlock cue)) return;
            float start = cue.ResolveStartSeconds(show.beatGrid);
            float end = start + cue.ResolveDurationSeconds(show.beatGrid);
            float split = playhead > start && playhead < end ? playhead : (start + end) * 0.5f;
            Undo.RecordObject(show, "Split Spectra Cue");
            try
            {
                SpectraCueBlock right = SpectraTimelineEditing.SplitCue(show, selectedTrack, selectedCue, Snap(split));
                selectedCue = Array.IndexOf(show.tracks[selectedTrack].cues, right);
                Dirty();
            }
            catch (ArgumentOutOfRangeException) { }
        }

        private void PasteCue()
        {
            if (selectedTrack < 0 || string.IsNullOrEmpty(cueClipboard)) return;
            Undo.RecordObject(show, "Paste Spectra Cue");
            SpectraCueBlock cue = SpectraTimelineEditing.PasteCueFromJson(show, selectedTrack, cueClipboard, Snap(playhead));
            selectedCue = Array.IndexOf(show.tracks[selectedTrack].cues, cue);
            Dirty();
        }

        private void MoveSelectedTrack(int direction)
        {
            if (selectedTrack < 0) return;
            int target = Mathf.Clamp(selectedTrack + direction, 0, show.tracks.Length - 1);
            Undo.RecordObject(show, "Move Spectra Track");
            SpectraTimelineEditing.MoveTrack(show, selectedTrack, target);
            selectedTrack = target;
            Dirty();
        }

        private void DeleteSelectedTrack()
        {
            if (selectedTrack < 0) return;
            if (!EditorUtility.DisplayDialog("Delete Track", "Delete this track and all of its cues?", "Delete", "Cancel")) return;
            Undo.RecordObject(show, "Delete Spectra Track");
            SpectraTimelineEditing.DeleteTrack(show, selectedTrack);
            selectedTrack = -1;
            selectedCue = -1;
            Dirty();
            GUIUtility.ExitGUI();
        }

        private void ApplyCueTemplate()
        {
            if (cueTemplate == null || !TryGetSelectedCue(out SpectraCueBlock target)) return;
            SpectraCueBlock source = cueTemplate.InstantiateCue();
            string id = target.id;
            float start = target.ResolveStartSeconds(show.beatGrid);
            float duration = target.ResolveDurationSeconds(show.beatGrid);
            Undo.RecordObject(show, "Apply Spectra Cue Template");
            EditorJsonUtility.FromJsonOverwrite(EditorJsonUtility.ToJson(source), target);
            target.id = id;
            SpectraTimelineEditing.SetCueStart(show, target, start);
            SpectraTimelineEditing.SetCueDuration(show, target, duration);
            Dirty();
        }

        private void ApplyMovementPreset()
        {
            if (movementPreset == null || !TryGetSelectedCue(out SpectraCueBlock cue)) return;
            Undo.RecordObject(show, "Apply Spectra Movement Preset");
            movementPreset.ApplyTo(cue);
            Dirty();
        }

        private void ApplyPalette()
        {
            if (colorPalette == null || !TryGetSelectedCue(out SpectraCueBlock cue)) return;
            Undo.RecordObject(show, "Apply Spectra Palette");
            cue.valueType = SpectraCueValueType.Color;
            cue.color = colorPalette.Evaluate(0f, SpectraPlatformKind.PC);
            Dirty();
        }

        private void ApplySectionTemplate()
        {
            if (sectionTemplate == null || selectedTrack < 0) return;
            Undo.RecordObject(show, "Insert Spectra Section Template");
            SpectraCueBlock[] cues = sectionTemplate.InstantiateCues(show.beatGrid, playhead);
            SpectraTimelineTrack track = show.tracks[selectedTrack];
            for (int i = 0; i < cues.Length; i++)
            {
                SpectraCueBlock[] expanded = new SpectraCueBlock[(track.cues == null ? 0 : track.cues.Length) + 1];
                if (track.cues != null) Array.Copy(track.cues, expanded, track.cues.Length);
                expanded[expanded.Length - 1] = cues[i];
                track.cues = expanded;
            }
            Dirty();
        }

        private void ShowValidation()
        {
            SpectraValidationIssue[] issues = show.ValidateShow();
            string report = issues.Length == 0 ? "No show-data issues found.\n\n" : "";
            for (int i = 0; i < issues.Length; i++)
                report += (issues[i].isError ? "ERROR: " : "Warning: ") + issues[i].path + " — " + issues[i].message + "\n";
            report += "\n" + SpectraPlatformCompatibilityValidator.Format(SpectraPlatformCompatibilityValidator.AnalyzeAll(show));
            EditorUtility.DisplayDialog("SpectraOverdrive Validation", report, "OK");
        }

        private void RebuildWaveform()
        {
            waveformClip = show == null ? null : show.authoringAudio;
            waveform = SpectraWaveformCache.Build(waveformClip, 32768);
            Repaint();
        }

        private void EnsureWaveform()
        {
            AudioClip clip = show == null ? null : show.authoringAudio;
            if (waveform == null || clip != waveformClip) RebuildWaveform();
        }

        private int FindCueAt(int trackIndex, Vector2 mouse, float tracksTop)
        {
            SpectraTimelineTrack track = show.tracks[trackIndex];
            if (track == null || track.cues == null) return -1;
            Rect row = new Rect(0f, tracksTop + trackIndex * TrackHeight, position.width, TrackHeight);
            for (int cueIndex = track.cues.Length - 1; cueIndex >= 0; cueIndex--)
                if (track.cues[cueIndex] != null && CueRect(row, track.cues[cueIndex]).Contains(mouse)) return cueIndex;
            return -1;
        }

        private int FindNearestMarker(float mouseX, float maximumDistance)
        {
            if (show.markers == null) return -1;
            int nearest = -1;
            float distance = maximumDistance;
            for (int i = 0; i < show.markers.Length; i++)
            {
                SpectraTimelineMarker marker = show.markers[i];
                if (marker == null) continue;
                float candidate = Mathf.Abs(TimeToX(marker.ResolveSeconds(show.beatGrid)) - mouseX);
                if (candidate <= distance)
                {
                    distance = candidate;
                    nearest = i;
                }
            }
            return nearest;
        }

        private int FindLoopAt(float mouseX)
        {
            if (show.loopRegions == null) return -1;
            for (int i = show.loopRegions.Length - 1; i >= 0; i--)
            {
                SpectraLoopRegion loop = show.loopRegions[i];
                if (loop != null && mouseX >= TimeToX(loop.startSeconds) - 6f && mouseX <= TimeToX(loop.endSeconds) + 6f)
                    return i;
            }
            return -1;
        }

        private Rect CueRect(Rect row, SpectraCueBlock cue)
        {
            float start = cue.ResolveStartSeconds(show.beatGrid);
            float duration = cue.ResolveDurationSeconds(show.beatGrid);
            return new Rect(TimeToX(start), row.y + 5f, Mathf.Max(8f, duration * pixelsPerSecond), row.height - 10f);
        }

        private bool TryGetSelectedCue(out SpectraCueBlock cue)
        {
            cue = null;
            if (show == null || show.tracks == null || selectedTrack < 0 || selectedTrack >= show.tracks.Length) return false;
            SpectraTimelineTrack track = show.tracks[selectedTrack];
            if (track == null || track.cues == null || selectedCue < 0 || selectedCue >= track.cues.Length) return false;
            cue = track.cues[selectedCue];
            return cue != null;
        }

        private SpectraLoopRegion FirstEnabledLoopContaining(float time)
        {
            if (show.loopRegions == null) return null;
            for (int i = 0; i < show.loopRegions.Length; i++)
            {
                SpectraLoopRegion loop = show.loopRegions[i];
                if (loop != null && loop.enabled && time >= loop.startSeconds && time < loop.endSeconds) return loop;
            }
            return null;
        }

        private float Snap(float seconds)
        {
            return Mathf.Clamp(SpectraTimelineEditing.SnapTime(show, seconds, snap, frameRate), 0f, show.durationSeconds);
        }

        private float TimeToX(float time) { return (time - scrollSeconds) * pixelsPerSecond; }
        private float XToTime(float x) { return scrollSeconds + x / pixelsPerSecond; }

        private float ChooseRulerInterval()
        {
            float target = 90f / pixelsPerSecond;
            float[] choices = { 0.1f, 0.25f, 0.5f, 1f, 2f, 5f, 10f, 15f, 30f, 60f, 120f, 300f };
            for (int i = 0; i < choices.Length; i++) if (choices[i] >= target) return choices[i];
            return 600f;
        }

        private static string FormatTime(float time)
        {
            time = Mathf.Max(0f, time);
            int minutes = Mathf.FloorToInt(time / 60f);
            float seconds = time - minutes * 60f;
            return minutes.ToString("00") + ":" + seconds.ToString("00.000");
        }

        private void Dirty(bool invalidatePreview = true)
        {
            if (show == null) return;
            if (invalidatePreview) show.EnsureStableIds();
            EditorUtility.SetDirty(show);
            if (invalidatePreview) previewCompiled = null;
            compatibilityCache = null;
            Repaint();
        }

        private void ClampSelection()
        {
            if (show == null || show.tracks == null || selectedTrack >= show.tracks.Length)
            {
                selectedTrack = -1;
                selectedCue = -1;
                selectedMarker = -1;
                selectedLoop = -1;
                return;
            }
            if (selectedTrack >= 0)
            {
                SpectraTimelineTrack track = show.tracks[selectedTrack];
                if (track == null || track.cues == null || selectedCue >= track.cues.Length) selectedCue = -1;
            }
            if (show.markers == null || selectedMarker >= show.markers.Length) selectedMarker = -1;
            if (show.loopRegions == null || selectedLoop >= show.loopRegions.Length) selectedLoop = -1;
        }

        private static T[] RemoveAt<T>(T[] source, int index)
        {
            T[] result = new T[source.Length - 1];
            if (index > 0) Array.Copy(source, 0, result, 0, index);
            if (index < source.Length - 1) Array.Copy(source, index + 1, result, index, source.Length - index - 1);
            return result;
        }

        private void ApplySerializedChanges(SerializedObject serialized)
        {
            if (!serialized.ApplyModifiedProperties()) return;
            previewCompiled = null;
            compatibilityCache = null;
            EditorUtility.SetDirty(show);
        }

        private void EnsureStyles()
        {
            if (centeredMini == null)
            {
                centeredMini = new GUIStyle(EditorStyles.miniLabel)
                {
                    alignment = TextAnchor.MiddleCenter,
                    fontStyle = FontStyle.Bold
                };
            }
            if (cueLabel == null)
            {
                cueLabel = new GUIStyle(EditorStyles.whiteMiniLabel)
                {
                    fontStyle = FontStyle.Bold,
                    clipping = TextClipping.Clip
                };
            }
        }
    }
}
