using UdonSharp;
using UnityEngine;
using VRC.SDKBase;

namespace SpectraOverdrive
{
    [UdonBehaviourSyncMode(BehaviourSyncMode.None)]
    public class SpectraPlatformMarker : UdonSharpBehaviour
    {
        public bool includeOnPC = true;
        public bool includeOnQuest = true;
        public bool includeOnIOS = true;
        public bool includeOnAndroid = true;

        private void Start()
        {
#if UNITY_ANDROID
            bool isQuest = Networking.LocalPlayer != null && Networking.LocalPlayer.IsUserInVR();
            gameObject.SetActive(isQuest ? includeOnQuest : includeOnAndroid);
#elif UNITY_IOS
            gameObject.SetActive(includeOnIOS);
#else
            gameObject.SetActive(includeOnPC);
#endif
        }

#if UNITY_EDITOR
        public bool ShouldEnableFor(UnityEditor.BuildTarget target)
        {
            if (target == UnityEditor.BuildTarget.Android) return includeOnQuest || includeOnAndroid;
            if (target == UnityEditor.BuildTarget.iOS) return includeOnIOS;
            return includeOnPC;
        }
#endif
    }
}
