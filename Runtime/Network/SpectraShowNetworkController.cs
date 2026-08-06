using UdonSharp;
using UnityEngine;
using VRC.SDKBase;

namespace SpectraOverdrive
{
    [UdonBehaviourSyncMode(BehaviourSyncMode.Manual)]
    public class SpectraShowNetworkController : UdonSharpBehaviour
    {
        [Header("Baked show players")]
        public SpectraShowRuntimePlayer[] showPlayers = new SpectraShowRuntimePlayer[0];
        public SpectraLiveOverrideLayer overrideLayer;
        public bool masterMayTakeControl = true;
        public bool ownerMayTakeControl = true;
        [Range(5f, 60f)] public float ownerHeartbeatSeconds = 15f;

        [Header("Operator input")]
        public int requestedShowIndex;
        public int requestedLoopIndex = -1;
        public float requestedSeekSeconds;
        [Range(0.25f, 4f)] public float requestedPlaybackSpeed = 1f;
        public int requestedHotCueMarkerIndex = -1;
        [Range(0, 3)] public int requestedPerformanceMacroIndex;
        [Range(0f, 1f)] public float requestedPerformanceMacroValue = 1f;
        public int requestedPerformanceMacroSnapshotIndex = -1;

        [Header("Synchronized authoritative state")]
        [UdonSynced] public int revision;
        [UdonSynced] public int activeShowIndex;
        [UdonSynced] public int activeContentSignature;
        [UdonSynced] public int playbackState;
        [UdonSynced] public double playStartedServerTime;
        [UdonSynced] public float pausedOffset;
        [UdonSynced] public float synchronizedPlaybackSpeed = 1f;
        [UdonSynced] public int activeLoopIndex = -1;
        [UdonSynced] public bool emergencyBlackout;
        [UdonSynced] public bool synchronizedStrobesEnabled = true;
        [UdonSynced] public bool synchronizedLasersEnabled = true;
        [UdonSynced] public float synchronizedMasterIntensity = 1f;
        [UdonSynced] public string activeOperatorDisplayName;
        [UdonSynced] public int hotCueRevision;
        [UdonSynced] public double hotCueExecuteServerTime;
        [UdonSynced] public float hotCueTargetOffset;
        [UdonSynced] public float hotCueTransitionSeconds;
        [UdonSynced] public int performanceMacroRevision;
        [UdonSynced] public int activePerformanceMacroSnapshotIndex = -1;
        [UdonSynced] public double performanceMacroChangeServerTime;
        [UdonSynced] public float performanceMacroTransitionSeconds;
        [UdonSynced] public float performanceMacroStart0 = 1f;
        [UdonSynced] public float performanceMacroStart1 = 1f;
        [UdonSynced] public float performanceMacroStart2 = 1f;
        [UdonSynced] public float performanceMacroStart3 = 1f;
        [UdonSynced] public float performanceMacroTarget0 = 1f;
        [UdonSynced] public float performanceMacroTarget1 = 1f;
        [UdonSynced] public float performanceMacroTarget2 = 1f;
        [UdonSynced] public float performanceMacroTarget3 = 1f;

        [Header("Local diagnostics")]
        public SpectraNetworkSyncStatus syncStatus = SpectraNetworkSyncStatus.Offline;
        public float calculatedShowTime;
        public float localClockDriftMilliseconds;
        public int deserializationCount;
        public int rejectedStateCount;
        public int executedHotCueCount;
        public double lastNetworkUpdateServerTime;

        private float _nextHeartbeat;
        private float _lastAppliedShowTime;

        private void Start()
        {
            ConfigurePlayersForExternalClock();
            if (showPlayers == null || showPlayers.Length == 0)
            {
                syncStatus = SpectraNetworkSyncStatus.InvalidState;
                return;
            }
            activeShowIndex = Mathf.Clamp(activeShowIndex, 0, showPlayers.Length - 1);
            if (Networking.LocalPlayer == null)
            {
                syncStatus = SpectraNetworkSyncStatus.Offline;
                InitializePerformanceMacros(ActivePlayer(), 0d);
                ApplyAuthoritativeState();
                return;
            }
            if (Networking.IsOwner(gameObject) && activeContentSignature == 0)
            {
                SpectraShowRuntimePlayer player = ActivePlayer();
                if (player != null) activeContentSignature = player.contentSignature;
                playbackState = (int)SpectraShowPlaybackState.Stopped;
                pausedOffset = 0f;
                revision++;
                RequestSerialization();
            }
            if (Networking.IsOwner(gameObject) && performanceMacroRevision == 0)
            {
                InitializePerformanceMacros(
                    ActivePlayer(),
                    Networking.GetServerTimeInSeconds());
                performanceMacroRevision++;
                revision++;
                RequestSerialization();
            }
            ApplyAuthoritativeState();
        }

        private void Update()
        {
            SpectraShowRuntimePlayer player = ActivePlayer();
            if (player == null) return;
            double now = Networking.GetServerTimeInSeconds();
            ApplyPerformanceMacros(player, now);
            SpectraShowPlaybackState resolvedState = (SpectraShowPlaybackState)playbackState;
            if (resolvedState == SpectraShowPlaybackState.Playing)
            {
                calculatedShowTime = ResolveAuthoritativeTime(now);
                float before = player.showTime;
                player.ApplyExternalClock(calculatedShowTime, resolvedState, synchronizedPlaybackSpeed);
                ApplyTransitionIntensity(player, now);
                localClockDriftMilliseconds = Mathf.Abs(before - calculatedShowTime) * 1000f;
                _lastAppliedShowTime = calculatedShowTime;
                if (player.showTime >= player.durationSeconds
                    && !HasInfiniteActiveLoop(player)
                    && Networking.IsOwner(gameObject))
                    StopSynchronized();
            }
            else if (performanceMacroTransitionSeconds > 0.0001f
                && now <= performanceMacroChangeServerTime
                    + performanceMacroTransitionSeconds + 0.05d)
            {
                player.ApplyExternalClock(
                    pausedOffset,
                    resolvedState,
                    synchronizedPlaybackSpeed);
            }
            if (Networking.IsOwner(gameObject) && Time.time >= _nextHeartbeat)
            {
                _nextHeartbeat = Time.time + ownerHeartbeatSeconds;
                lastNetworkUpdateServerTime = Networking.GetServerTimeInSeconds();
                RequestSerialization();
            }
        }

        public void TakeControl()
        {
            VRCPlayerApi local = Networking.LocalPlayer;
            if (local == null) return;
            if (!ownerMayTakeControl && !(masterMayTakeControl && Networking.IsMaster)) return;
            Networking.SetOwner(local, gameObject);
            activeOperatorDisplayName = local.displayName;
            revision++;
            RequestSerialization();
        }

        public void SelectRequestedShow()
        {
            SelectShow(requestedShowIndex);
        }

        public void SelectShow(int showIndex)
        {
            if (!AcquireControl() || showPlayers == null || showPlayers.Length == 0) return;
            int next = Mathf.Clamp(showIndex, 0, showPlayers.Length - 1);
            SpectraShowRuntimePlayer previous = ActivePlayer();
            if (previous != null) previous.ApplyExternalClock(0f, SpectraShowPlaybackState.Stopped, 1f);
            activeShowIndex = next;
            SpectraShowRuntimePlayer player = ActivePlayer();
            activeContentSignature = player == null ? 0 : player.contentSignature;
            if (overrideLayer != null && player != null)
            {
                overrideLayer.player = player;
                overrideLayer.configuredGroupCount = player.groups == null
                    ? 1 : Mathf.Max(1, player.groups.Length);
                if (overrideLayer.recorder != null) overrideLayer.recorder.StopRecording();
                overrideLayer.ClearAll();
            }
            playbackState = (int)SpectraShowPlaybackState.Stopped;
            pausedOffset = 0f;
            activeLoopIndex = -1;
            ClearHotCueSchedule();
            InitializePerformanceMacros(player, Networking.GetServerTimeInSeconds());
            performanceMacroRevision++;
            revision++;
            CommitAndApply();
        }

        public void PlaySynchronized()
        {
            if (!AcquireControl()) return;
            SpectraShowRuntimePlayer player = ActivePlayer();
            if (player == null || player.durationSeconds <= 0f) return;
            synchronizedPlaybackSpeed = Mathf.Clamp(requestedPlaybackSpeed, 0.25f, 4f);
            playStartedServerTime = Networking.GetServerTimeInSeconds()
                - pausedOffset / synchronizedPlaybackSpeed;
            playbackState = (int)SpectraShowPlaybackState.Playing;
            activeContentSignature = player.contentSignature;
            ClearHotCueSchedule();
            revision++;
            CommitAndApply();
        }

        public void PauseSynchronized()
        {
            if (!AcquireControl()) return;
            if ((SpectraShowPlaybackState)playbackState != SpectraShowPlaybackState.Playing) return;
            pausedOffset = ResolveAuthoritativeTime(Networking.GetServerTimeInSeconds());
            playbackState = (int)SpectraShowPlaybackState.Paused;
            ClearHotCueSchedule();
            revision++;
            CommitAndApply();
        }

        public void StopSynchronized()
        {
            if (!AcquireControl()) return;
            playbackState = (int)SpectraShowPlaybackState.Stopped;
            pausedOffset = 0f;
            playStartedServerTime = Networking.GetServerTimeInSeconds();
            ClearHotCueSchedule();
            revision++;
            CommitAndApply();
        }

        public void SeekRequestedTime()
        {
            SeekSynchronized(requestedSeekSeconds);
        }

        public void SeekNextMarker()
        {
            SpectraShowRuntimePlayer player = ActivePlayer();
            if (player == null || player.markerTimes == null) return;
            float current = ResolveAuthoritativeTime(Networking.GetServerTimeInSeconds());
            for (int i = 0; i < player.markerTimes.Length; i++)
                if (player.markerTimes[i] > current + 0.001f)
                {
                    SeekSynchronized(player.markerTimes[i]);
                    return;
                }
        }

        public void SeekPreviousMarker()
        {
            SpectraShowRuntimePlayer player = ActivePlayer();
            if (player == null || player.markerTimes == null) return;
            float current = ResolveAuthoritativeTime(Networking.GetServerTimeInSeconds());
            for (int i = player.markerTimes.Length - 1; i >= 0; i--)
                if (player.markerTimes[i] < current - 0.001f)
                {
                    SeekSynchronized(player.markerTimes[i]);
                    return;
                }
            SeekSynchronized(0f);
        }

        public void JumpToNextDrop()
        {
            SpectraShowRuntimePlayer player = ActivePlayer();
            if (player == null || player.markerTimes == null || player.markerKinds == null) return;
            float current = ResolveAuthoritativeTime(Networking.GetServerTimeInSeconds());
            for (int i = 0; i < player.markerTimes.Length && i < player.markerKinds.Length; i++)
                if (player.markerTimes[i] > current + 0.001f
                    && player.markerKinds[i] == (int)SpectraMarkerKind.Drop)
                {
                    SeekSynchronized(player.markerTimes[i]);
                    return;
                }
        }

        public void TriggerRequestedHotCue()
        {
            ScheduleHotCue(requestedHotCueMarkerIndex);
        }

        public void ScheduleHotCue(int markerIndex)
        {
            if (!AcquireControl()) return;
            SpectraShowRuntimePlayer player = ActivePlayer();
            if (player == null || !player.IsHotCueUsable(markerIndex)) return;
            float target = player.GetHotCueTargetTime(markerIndex);
            if ((SpectraShowPlaybackState)playbackState != SpectraShowPlaybackState.Playing)
            {
                SeekSynchronized(target);
                return;
            }

            double now = Networking.GetServerTimeInSeconds();
            float current = ResolveAuthoritativeTime(now);
            playStartedServerTime = now
                - current / Mathf.Max(0.25f, synchronizedPlaybackSpeed);
            float executionShowTime = player.ResolveHotCueExecutionShowTime(markerIndex, current);
            float delayShowSeconds = Mathf.Max(0f, executionShowTime - current);
            hotCueExecuteServerTime = now
                + delayShowSeconds / Mathf.Max(0.25f, synchronizedPlaybackSpeed);
            hotCueTargetOffset = target;
            hotCueTransitionSeconds = player.GetHotCueTransitionSeconds(markerIndex);
            hotCueRevision++;
            executedHotCueCount++;
            revision++;
            CommitAndApply();
        }

        public void CancelHotCue()
        {
            if (!AcquireControl()) return;
            float current = ResolveAuthoritativeTime(Networking.GetServerTimeInSeconds());
            RebasePlaybackClock(current);
            ClearHotCueSchedule();
            revision++;
            CommitAndApply();
        }

        public void SeekSynchronized(float seconds)
        {
            if (!AcquireControl()) return;
            SpectraShowRuntimePlayer player = ActivePlayer();
            if (player == null) return;
            pausedOffset = Mathf.Clamp(seconds, 0f, player.durationSeconds);
            if ((SpectraShowPlaybackState)playbackState == SpectraShowPlaybackState.Playing)
                playStartedServerTime = Networking.GetServerTimeInSeconds()
                    - pausedOffset / Mathf.Max(0.25f, synchronizedPlaybackSpeed);
            ClearHotCueSchedule();
            revision++;
            CommitAndApply();
        }

        public void SetRequestedLoop()
        {
            SetSynchronizedLoop(requestedLoopIndex);
        }

        public void SetSynchronizedLoop(int loopIndex)
        {
            if (!AcquireControl()) return;
            activeLoopIndex = loopIndex;
            revision++;
            CommitAndApply();
        }

        public void ClearSynchronizedLoop()
        {
            SetSynchronizedLoop(-1);
        }

        public void SetRequestedPlaybackSpeed()
        {
            if (!AcquireControl()) return;
            float current = ResolveAuthoritativeTime(Networking.GetServerTimeInSeconds());
            synchronizedPlaybackSpeed = Mathf.Clamp(requestedPlaybackSpeed, 0.25f, 4f);
            pausedOffset = current;
            if ((SpectraShowPlaybackState)playbackState == SpectraShowPlaybackState.Playing)
                playStartedServerTime = Networking.GetServerTimeInSeconds()
                    - pausedOffset / synchronizedPlaybackSpeed;
            ClearHotCueSchedule();
            revision++;
            CommitAndApply();
        }

        public void EnableEmergencyBlackout()
        {
            SetEmergencyBlackout(true);
        }

        public void DisableEmergencyBlackout()
        {
            SetEmergencyBlackout(false);
        }

        public void SetEmergencyBlackout(bool enabled)
        {
            if (!AcquireControl()) return;
            emergencyBlackout = enabled;
            revision++;
            CommitAndApply();
        }

        public void EnableSynchronizedStrobes()
        {
            SetSynchronizedStrobes(true);
        }

        public void DisableSynchronizedStrobes()
        {
            SetSynchronizedStrobes(false);
        }

        public void SetSynchronizedStrobes(bool enabled)
        {
            if (!AcquireControl()) return;
            synchronizedStrobesEnabled = enabled;
            revision++;
            CommitAndApply();
        }

        public void EnableSynchronizedLasers()
        {
            SetSynchronizedLasers(true);
        }

        public void DisableSynchronizedLasers()
        {
            SetSynchronizedLasers(false);
        }

        public void SetSynchronizedLasers(bool enabled)
        {
            if (!AcquireControl()) return;
            synchronizedLasersEnabled = enabled;
            revision++;
            CommitAndApply();
        }

        public void SetSynchronizedMasterIntensity(float value)
        {
            if (!AcquireControl()) return;
            synchronizedMasterIntensity = Mathf.Clamp(value, 0f, 2f);
            revision++;
            CommitAndApply();
        }

        public void SetRequestedPerformanceMacro()
        {
            SetPerformanceMacro(
                requestedPerformanceMacroIndex,
                requestedPerformanceMacroValue);
        }

        public void SetPerformanceMacro(int index, float value)
        {
            if (!AcquireControl() || index < 0 || index > 3) return;
            SpectraShowRuntimePlayer player = ActivePlayer();
            if (player == null) return;
            double now = Networking.GetServerTimeInSeconds();
            float current0 = ResolvePerformanceMacro(0, now);
            float current1 = ResolvePerformanceMacro(1, now);
            float current2 = ResolvePerformanceMacro(2, now);
            float current3 = ResolvePerformanceMacro(3, now);
            performanceMacroStart0 = current0;
            performanceMacroStart1 = current1;
            performanceMacroStart2 = current2;
            performanceMacroStart3 = current3;
            performanceMacroTarget0 = index == 0
                ? Mathf.Clamp01(value) : performanceMacroTarget0;
            performanceMacroTarget1 = index == 1
                ? Mathf.Clamp01(value) : performanceMacroTarget1;
            performanceMacroTarget2 = index == 2
                ? Mathf.Clamp01(value) : performanceMacroTarget2;
            performanceMacroTarget3 = index == 3
                ? Mathf.Clamp01(value) : performanceMacroTarget3;
            performanceMacroChangeServerTime = now;
            performanceMacroTransitionSeconds = player.GetPerformanceMacroSmoothing(index);
            activePerformanceMacroSnapshotIndex = -1;
            performanceMacroRevision++;
            revision++;
            CommitAndApply();
        }

        public void RecallRequestedPerformanceMacroSnapshot()
        {
            RecallPerformanceMacroSnapshot(requestedPerformanceMacroSnapshotIndex);
        }

        public void RecallPerformanceMacroSnapshot(int snapshotIndex)
        {
            if (!AcquireControl()) return;
            SpectraShowRuntimePlayer player = ActivePlayer();
            if (player == null || !player.IsPerformanceMacroSnapshotUsable(snapshotIndex)) return;
            double now = Networking.GetServerTimeInSeconds();
            performanceMacroStart0 = ResolvePerformanceMacro(0, now);
            performanceMacroStart1 = ResolvePerformanceMacro(1, now);
            performanceMacroStart2 = ResolvePerformanceMacro(2, now);
            performanceMacroStart3 = ResolvePerformanceMacro(3, now);
            Vector4 targets = player.GetPerformanceMacroSnapshotValues(snapshotIndex);
            performanceMacroTarget0 = targets.x;
            performanceMacroTarget1 = targets.y;
            performanceMacroTarget2 = targets.z;
            performanceMacroTarget3 = targets.w;
            performanceMacroChangeServerTime = now;
            performanceMacroTransitionSeconds =
                player.GetPerformanceMacroSnapshotTransitionSeconds(snapshotIndex);
            activePerformanceMacroSnapshotIndex = snapshotIndex;
            performanceMacroRevision++;
            revision++;
            CommitAndApply();
        }

        public void ResetPerformanceMacros()
        {
            if (!AcquireControl()) return;
            SpectraShowRuntimePlayer player = ActivePlayer();
            if (player == null) return;
            InitializePerformanceMacros(player, Networking.GetServerTimeInSeconds());
            performanceMacroRevision++;
            revision++;
            CommitAndApply();
        }

        public float ResolvePerformanceMacro(int index, double serverTime)
        {
            float start = PerformanceMacroStart(index);
            float target = PerformanceMacroTarget(index);
            if (performanceMacroTransitionSeconds <= 0.0001f) return target;
            float t = Mathf.Clamp01((float)(serverTime - performanceMacroChangeServerTime)
                / performanceMacroTransitionSeconds);
            t = t * t * (3f - 2f * t);
            return Mathf.Lerp(start, target, t);
        }

        public override void OnDeserialization()
        {
            deserializationCount++;
            lastNetworkUpdateServerTime = Networking.GetServerTimeInSeconds();
            ApplyAuthoritativeState();
        }

        public override void OnOwnershipTransferred(VRCPlayerApi player)
        {
            syncStatus = SpectraNetworkSyncStatus.Recovering;
            if (player == null || !player.isLocal) return;
            float current = ResolveAuthoritativeTime(Networking.GetServerTimeInSeconds());
            pausedOffset = current;
            if ((SpectraShowPlaybackState)playbackState == SpectraShowPlaybackState.Playing)
                playStartedServerTime = Networking.GetServerTimeInSeconds()
                    - current / Mathf.Max(0.25f, synchronizedPlaybackSpeed);
            ClearHotCueSchedule();
            activeOperatorDisplayName = player.displayName;
            revision++;
            RequestSerialization();
            ApplyAuthoritativeState();
        }

        public float ResolveAuthoritativeTime(double serverTime)
        {
            if ((SpectraShowPlaybackState)playbackState != SpectraShowPlaybackState.Playing)
                return pausedOffset;
            if (hotCueExecuteServerTime > 0d && serverTime >= hotCueExecuteServerTime)
            {
                double afterJump = serverTime - hotCueExecuteServerTime;
                return Mathf.Max(0f, hotCueTargetOffset
                    + (float)(afterJump * Mathf.Max(0.25f, synchronizedPlaybackSpeed)));
            }
            double elapsed = serverTime - playStartedServerTime;
            return Mathf.Max(0f, (float)(elapsed * Mathf.Max(0.25f, synchronizedPlaybackSpeed)));
        }

        public float ResolveHotCueTransitionIntensity(double serverTime)
        {
            if (hotCueExecuteServerTime <= 0d || hotCueTransitionSeconds <= 0.0001f)
                return 1f;
            float half = Mathf.Max(0.01f, hotCueTransitionSeconds * 0.5f);
            float distance = Mathf.Abs((float)(serverTime - hotCueExecuteServerTime));
            return distance >= half ? 1f : Mathf.Clamp01(distance / half);
        }

        public void ApplyAuthoritativeState()
        {
            ConfigurePlayersForExternalClock();
            SpectraShowRuntimePlayer player = ActivePlayer();
            if (player == null)
            {
                syncStatus = SpectraNetworkSyncStatus.InvalidState;
                rejectedStateCount++;
                return;
            }
            if (activeContentSignature != 0 && player.contentSignature != activeContentSignature)
            {
                syncStatus = SpectraNetworkSyncStatus.ShowMismatch;
                player.state = SpectraShowPlaybackState.Invalid;
                player.SetEmergencyBlackout(true);
                rejectedStateCount++;
                return;
            }
            DeactivateInactivePlayers();
            if (overrideLayer != null)
            {
                overrideLayer.player = player;
                if (overrideLayer.recorder != null) overrideLayer.recorder.player = player;
            }
            player.selectedLoopIndex = activeLoopIndex;
            player.showStrobesEnabled = synchronizedStrobesEnabled;
            player.showLasersEnabled = synchronizedLasersEnabled;
            player.emergencyBlackout = emergencyBlackout;
            double serverTime = Networking.GetServerTimeInSeconds();
            ApplyPerformanceMacros(player, serverTime);
            if (player.bus != null)
                player.bus.SetMasterIntensity(synchronizedMasterIntensity
                    * ResolveHotCueTransitionIntensity(serverTime));
            float time = ResolveAuthoritativeTime(serverTime);
            SpectraShowPlaybackState resolvedState = (SpectraShowPlaybackState)playbackState;
            player.ApplyExternalClock(time, resolvedState, synchronizedPlaybackSpeed);
            calculatedShowTime = time;
            _lastAppliedShowTime = time;
            syncStatus = Networking.LocalPlayer == null
                ? SpectraNetworkSyncStatus.Offline
                : SpectraNetworkSyncStatus.Synchronized;
        }

        private bool AcquireControl()
        {
            VRCPlayerApi local = Networking.LocalPlayer;
            if (local == null) return true;
            if (Networking.IsOwner(gameObject)) return true;
            if (!ownerMayTakeControl && !(masterMayTakeControl && Networking.IsMaster)) return false;
            Networking.SetOwner(local, gameObject);
            activeOperatorDisplayName = local.displayName;
            return Networking.IsOwner(gameObject);
        }

        private void CommitAndApply()
        {
            lastNetworkUpdateServerTime = Networking.GetServerTimeInSeconds();
            RequestSerialization();
            ApplyAuthoritativeState();
        }

        private void ApplyTransitionIntensity(SpectraShowRuntimePlayer player, double serverTime)
        {
            if (player == null || player.bus == null) return;
            player.bus.SetMasterIntensity(
                synchronizedMasterIntensity * ResolveHotCueTransitionIntensity(serverTime));
        }

        private void InitializePerformanceMacros(
            SpectraShowRuntimePlayer player,
            double serverTime)
        {
            if (player == null) return;
            performanceMacroStart0 = performanceMacroTarget0 = player.GetPerformanceMacroDefault(0);
            performanceMacroStart1 = performanceMacroTarget1 = player.GetPerformanceMacroDefault(1);
            performanceMacroStart2 = performanceMacroTarget2 = player.GetPerformanceMacroDefault(2);
            performanceMacroStart3 = performanceMacroTarget3 = player.GetPerformanceMacroDefault(3);
            performanceMacroChangeServerTime = serverTime;
            performanceMacroTransitionSeconds = 0f;
            activePerformanceMacroSnapshotIndex = -1;
            ApplyPerformanceMacros(player, serverTime);
        }

        private void ApplyPerformanceMacros(
            SpectraShowRuntimePlayer player,
            double serverTime)
        {
            if (player == null) return;
            player.SetPerformanceMacroValues(
                ResolvePerformanceMacro(0, serverTime),
                ResolvePerformanceMacro(1, serverTime),
                ResolvePerformanceMacro(2, serverTime),
                ResolvePerformanceMacro(3, serverTime));
        }

        private float PerformanceMacroStart(int index)
        {
            if (index == 0) return performanceMacroStart0;
            if (index == 1) return performanceMacroStart1;
            if (index == 2) return performanceMacroStart2;
            if (index == 3) return performanceMacroStart3;
            return 1f;
        }

        private float PerformanceMacroTarget(int index)
        {
            if (index == 0) return performanceMacroTarget0;
            if (index == 1) return performanceMacroTarget1;
            if (index == 2) return performanceMacroTarget2;
            if (index == 3) return performanceMacroTarget3;
            return 1f;
        }

        private void RebasePlaybackClock(float current)
        {
            pausedOffset = Mathf.Max(0f, current);
            if ((SpectraShowPlaybackState)playbackState == SpectraShowPlaybackState.Playing)
                playStartedServerTime = Networking.GetServerTimeInSeconds()
                    - pausedOffset / Mathf.Max(0.25f, synchronizedPlaybackSpeed);
        }

        private void ClearHotCueSchedule()
        {
            hotCueExecuteServerTime = 0d;
            hotCueTargetOffset = 0f;
            hotCueTransitionSeconds = 0f;
        }

        private SpectraShowRuntimePlayer ActivePlayer()
        {
            if (showPlayers == null || activeShowIndex < 0 || activeShowIndex >= showPlayers.Length) return null;
            return showPlayers[activeShowIndex];
        }

        public SpectraShowRuntimePlayer GetActivePlayer()
        {
            return ActivePlayer();
        }

        private void ConfigurePlayersForExternalClock()
        {
            if (showPlayers == null) return;
            for (int i = 0; i < showPlayers.Length; i++)
                if (showPlayers[i] != null) showPlayers[i].externalClock = true;
        }

        private void DeactivateInactivePlayers()
        {
            if (showPlayers == null) return;
            for (int i = 0; i < showPlayers.Length; i++)
            {
                SpectraShowRuntimePlayer player = showPlayers[i];
                if (player == null || i == activeShowIndex) continue;
                player.state = SpectraShowPlaybackState.Stopped;
                player.showTime = 0f;
            }
        }

        private bool HasInfiniteActiveLoop(SpectraShowRuntimePlayer player)
        {
            int loopIndex = player == null ? -1 : player.selectedLoopIndex;
            return loopIndex >= 0
                && player.loopEnabled != null
                && player.loopRepeatCounts != null
                && loopIndex < player.loopEnabled.Length
                && loopIndex < player.loopRepeatCounts.Length
                && player.loopEnabled[loopIndex]
                && player.loopRepeatCounts[loopIndex] <= 0;
        }
    }
}
