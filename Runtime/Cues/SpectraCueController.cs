using UdonSharp;
using UnityEngine;
using VRC.SDKBase;
using VRC.Udon.Common.Interfaces;

namespace SpectraOverdrive
{
    [UdonBehaviourSyncMode(BehaviourSyncMode.Manual)]
    public class SpectraCueController : UdonSharpBehaviour
    {
        [UdonSynced] public int cueListId;
        [UdonSynced] public int cueId;
        [UdonSynced] public double cueStartServerTime;
        [UdonSynced] public float cueDuration = 1f;
        [UdonSynced] public int deterministicSeed;
        [UdonSynced] public bool paused;

        [Header("Local derived state")]
        public float normalizedCueTime;
        public bool isOwner;
        public SpectraCueExecutor[] executors = new SpectraCueExecutor[0];
        public int appliedCueRevision;
        public double lastCueChangeServerTime;

        private void Update()
        {
            isOwner = Networking.IsOwner(gameObject);

            if (paused || cueDuration <= 0.0001f)
            {
                normalizedCueTime = 0f;
                return;
            }

            double now = Networking.GetServerTimeInSeconds();
            normalizedCueTime = Mathf.Repeat((float)((now - cueStartServerTime) / cueDuration), 1f);
        }

        public void TakeControl()
        {
            Networking.SetOwner(Networking.LocalPlayer, gameObject);
        }

        public void StartCue(int newCueId)
        {
            if (!Networking.IsOwner(gameObject))
            {
                TakeControl();
            }

            cueId = newCueId;
            cueStartServerTime = Networking.GetServerTimeInSeconds();
            paused = false;
            deterministicSeed = Random.Range(1, int.MaxValue);
            RequestSerialization();
            SendCustomNetworkEvent(NetworkEventTarget.All, nameof(OnCueChanged));
        }

        public void PauseCue()
        {
            if (!Networking.IsOwner(gameObject))
            {
                TakeControl();
            }

            paused = true;
            RequestSerialization();
        }

        public void ResumeCue()
        {
            if (!Networking.IsOwner(gameObject))
            {
                TakeControl();
            }

            cueStartServerTime = Networking.GetServerTimeInSeconds();
            paused = false;
            RequestSerialization();
        }

        public void OnCueChanged()
        {
            lastCueChangeServerTime = Networking.GetServerTimeInSeconds();
            normalizedCueTime = paused || cueDuration <= 0.0001f
                ? 0f
                : Mathf.Repeat(
                    (float)((lastCueChangeServerTime - cueStartServerTime)
                        / cueDuration),
                    1f);
            appliedCueRevision++;
            if (executors == null) return;
            for (int i = 0; i < executors.Length; i++)
            {
                SpectraCueExecutor executor = executors[i];
                if (executor == null) continue;
                executor.controller = this;
                executor.RefreshNow();
            }
        }
    }
}
