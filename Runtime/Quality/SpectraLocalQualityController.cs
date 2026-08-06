using UdonSharp;
using UnityEngine;
using VRC.SDKBase;

namespace SpectraOverdrive
{
    [UdonBehaviourSyncMode(BehaviourSyncMode.None)]
    public class SpectraLocalQualityController : UdonSharpBehaviour
    {
        [Range(0, 3)] public int qualityLevel = 1;
        [Range(0, 3)] public int qualityFloor;
        [Range(0, 3)] public int qualityCeiling = 3;
        public GameObject[] enhancedOnly;
        public GameObject[] standardAndAbove;
        public GameObject[] safeAndAbove;

        private void Start()
        {
            ApplyQuality();
        }

        public void SetSafe()
        {
            qualityLevel = 0;
            ApplyQuality();
        }

        public void SetStandard()
        {
            qualityLevel = 1;
            ApplyQuality();
        }

        public void SetEnhanced()
        {
            qualityLevel = 2;
            ApplyQuality();
        }

        public void SetMaximum()
        {
            qualityLevel = 3;
            ApplyQuality();
        }

        public void ApplyQuality()
        {
            qualityLevel = Mathf.Clamp(qualityLevel,
                Mathf.Min(qualityFloor, qualityCeiling),
                Mathf.Max(qualityFloor, qualityCeiling));
            SetArray(safeAndAbove, qualityLevel >= 0);
            SetArray(standardAndAbove, qualityLevel >= 1);
            SetArray(enhancedOnly, qualityLevel >= 2);

            VRCShader.SetGlobalFloat(Shader.PropertyToID("_SpectraQualityLevel"), qualityLevel);
        }

        private void SetArray(GameObject[] objects, bool enabled)
        {
            if (objects == null) return;

            for (int i = 0; i < objects.Length; i++)
            {
                if (objects[i] != null)
                {
                    objects[i].SetActive(enabled);
                }
            }
        }
    }
}
