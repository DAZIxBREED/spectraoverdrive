using System;
using UnityEngine;

namespace SpectraOverdrive
{
    [Serializable]
    public class SpectraFixtureChannelDefinition
    {
        public SpectraChannelFunction function = SpectraChannelFunction.Unused;
        [Range(0, 63)] public int offset;
        public bool fineChannel;
        public float minimum = 0f;
        public float maximum = 1f;
        public bool invert;
        public string notes;
    }

    [CreateAssetMenu(
        fileName = "SpectraFixtureProfile",
        menuName = "SpectraOverdrive/Fixture Profile",
        order = 1
    )]
    public class SpectraFixtureProfileData : ScriptableObject
    {
        public string manufacturer = "Generic";
        public string model = "Fixture";
        public string modeName = "Default";
        public SpectraFixtureType fixtureType = SpectraFixtureType.Generic;
        [Range(1, 64)] public int channelCount = 1;

        [Header("Physical movement")]
        [Range(0f, 720f)] public float panRangeDegrees = 540f;
        [Range(0f, 360f)] public float tiltRangeDegrees = 270f;

        [Header("Optics")]
        [Range(0f, 180f)] public float minimumBeamAngle = 5f;
        [Range(0f, 180f)] public float maximumBeamAngle = 30f;
        [Range(1, 64)] public int goboCount = 1;
        [Range(1, 16)] public int prismFacetCount = 1;

        [Header("Channel map")]
        public SpectraFixtureChannelDefinition[] channels;

        public int FindOffset(SpectraChannelFunction function)
        {
            if (channels == null) return -1;

            for (int i = 0; i < channels.Length; i++)
            {
                SpectraFixtureChannelDefinition channel = channels[i];
                if (channel != null && channel.function == function)
                {
                    return channel.offset;
                }
            }

            return -1;
        }
    }
}
