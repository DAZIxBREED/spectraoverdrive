#if UNITY_EDITOR
using UnityEditor;
using UnityEditor.Build;
using UnityEditor.Build.Reporting;
using UnityEngine;
using SpectraOverdrive;

namespace SpectraOverdrive.Editor
{
    public class SpectraPlatformStripper : IPreprocessBuildWithReport
    {
        public int callbackOrder => 0;

        public void OnPreprocessBuild(BuildReport report)
        {
            SpectraPlatformMarker[] markers = Object.FindObjectsOfType<SpectraPlatformMarker>(true);

            for (int i = 0; i < markers.Length; i++)
            {
                SpectraPlatformMarker marker = markers[i];
                if (marker == null) continue;

                bool enabled = marker.ShouldEnableFor(report.summary.platform);
                marker.gameObject.SetActive(enabled);
            }

            Debug.Log("[SpectraOverdrive] Applied platform stripping markers for " + report.summary.platform + ".");
        }
    }
}
#endif
