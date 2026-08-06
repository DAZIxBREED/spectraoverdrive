using UdonSharp;
using UnityEngine;

namespace SpectraOverdrive
{
    [UdonBehaviourSyncMode(BehaviourSyncMode.None)]
    public class SpectraFixtureChannelMap : UdonSharpBehaviour
    {
        [Header("Offsets from fixture start address")]
        public int dimmer = 0;
        public int red = 1;
        public int green = 2;
        public int blue = 3;
        public int white = -1;
        public int pan = 4;
        public int panFine = 5;
        public int tilt = 6;
        public int tiltFine = 7;
        public int strobe = 8;
        public int zoom = 9;
        public int gobo = 10;
        public int goboRotate = 11;
        public int prism = 12;

        public int ResolveAbsoluteChannel(int startAddress, int offset)
        {
            if (offset < 0)
            {
                return -1;
            }

            return Mathf.Clamp(startAddress + offset, 1, 512);
        }
    }
}
