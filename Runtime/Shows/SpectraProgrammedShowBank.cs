using UnityEngine;

namespace SpectraOverdrive
{
    [CreateAssetMenu(
        fileName = "SpectraProgrammedShowBank",
        menuName = "SpectraOverdrive/Programmed Show Bank",
        order = 5)]
    public class SpectraProgrammedShowBank : ScriptableObject
    {
        public string bankName = "SpectraOverdrive Show Bank";
        public SpectraShowAsset[] shows = new SpectraShowAsset[0];
        public int defaultShowIndex;
        public bool preserveOverridesBetweenShows;

        public SpectraShowAsset GetShow(int index)
        {
            if (shows == null || index < 0 || index >= shows.Length) return null;
            return shows[index];
        }
    }
}
