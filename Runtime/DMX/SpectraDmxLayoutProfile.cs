using UdonSharp;
using UnityEngine;

namespace SpectraOverdrive
{
    [UdonBehaviourSyncMode(BehaviourSyncMode.None)]
    public class SpectraDmxLayoutProfile : UdonSharpBehaviour
    {
        public SpectraDmxGridMode gridMode = SpectraDmxGridMode.Horizontal;
        public SpectraDmxColorPacking colorPacking = SpectraDmxColorPacking.RedOnly;
        [Range(1, 9)] public int universeCount = 1;

        [Header("Legacy sector layout")]
        [Range(1, 64)] public int sectorWidth = 16;
        [Range(1, 64)] public int sectorHeight = 16;
        [Range(1, 16)] public int sectorsPerRow = 4;

        [Header("Texture mapping")]
        public bool flipX;
        public bool flipY;
        public bool halfTexelOffset = true;
    }
}
