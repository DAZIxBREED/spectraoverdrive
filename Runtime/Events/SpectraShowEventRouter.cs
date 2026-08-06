using UdonSharp;
using UnityEngine;
using VRC.Udon;
using VRC.Udon.Common.Interfaces;
using VRC.SDKBase;

namespace SpectraOverdrive
{
    [UdonBehaviourSyncMode(BehaviourSyncMode.None)]
    public class SpectraShowEventRouter : UdonSharpBehaviour
    {
        public UdonBehaviour[] targets = new UdonBehaviour[0];
        public string[] eventNames = new string[0];
        public bool[] networked = new bool[0];
        [Tooltip("Only this object's owner emits networked timeline events, preventing every client from rebroadcasting the same cue.")]
        public GameObject networkAuthority;
        public int lastTriggeredChannel = -1;
        public int triggerCount;

        public void TriggerChannel(int channel)
        {
            if (channel < 0 || targets == null || eventNames == null) return;
            if (channel >= targets.Length || channel >= eventNames.Length) return;
            UdonBehaviour target = targets[channel];
            string eventName = eventNames[channel];
            if (target == null || string.IsNullOrEmpty(eventName)) return;
            if (networked != null && channel < networked.Length && networked[channel])
            {
                GameObject authority = networkAuthority == null ? gameObject : networkAuthority;
                if (Networking.LocalPlayer != null && !Networking.IsOwner(authority)) return;
                target.SendCustomNetworkEvent(NetworkEventTarget.All, eventName);
            }
            else
                target.SendCustomEvent(eventName);
            lastTriggeredChannel = channel;
            triggerCount++;
        }
    }
}
