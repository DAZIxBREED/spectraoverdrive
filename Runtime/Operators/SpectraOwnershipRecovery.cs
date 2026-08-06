using UdonSharp;
using UnityEngine;
using VRC.SDKBase;

namespace SpectraOverdrive
{
    [UdonBehaviourSyncMode(BehaviourSyncMode.None)]
    public class SpectraOwnershipRecovery : UdonSharpBehaviour
    {
        public GameObject[] controlledObjects;
        [Range(1f, 60f)] public float checkInterval = 5f;
        public bool masterClaimsUnownedObjects = true;

        private float _nextCheck;

        private void Update()
        {
            if (Time.time < _nextCheck) return;
            _nextCheck = Time.time + checkInterval;
            Recover();
        }

        public void Recover()
        {
            VRCPlayerApi local = Networking.LocalPlayer;
            if (local == null || controlledObjects == null)
            {
                return;
            }

            bool mayClaim = Networking.IsMaster || !masterClaimsUnownedObjects;
            if (!mayClaim) return;

            for (int i = 0; i < controlledObjects.Length; i++)
            {
                GameObject target = controlledObjects[i];
                if (target == null) continue;

                VRCPlayerApi owner = Networking.GetOwner(target);
                if (owner == null || !owner.IsValid())
                {
                    Networking.SetOwner(local, target);
                }
            }
        }
    }
}
