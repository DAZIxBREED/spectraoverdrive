using UdonSharp;
using UnityEngine;

namespace SpectraOverdrive
{
    [UdonBehaviourSyncMode(BehaviourSyncMode.None)]
    public class SpectraFixtureProfileBinder : UdonSharpBehaviour
    {
        public SpectraFixtureProfileData profile;
        public SpectraFixtureIdentity identity;
        public SpectraFixtureChannelMap channelMap;
        public SpectraFixtureRuntime runtime;

        public void Start()
        {
            ApplyProfile();
        }

        public void ApplyProfile()
        {
            if (profile == null || identity == null || channelMap == null || runtime == null)
            {
                return;
            }

            identity.fixtureProfile = profile.manufacturer + " " + profile.model + " / " + profile.modeName;
            identity.fixtureType = profile.fixtureType;
            identity.channelCount = profile.channelCount;

            runtime.panRangeDegrees = profile.panRangeDegrees;
            runtime.tiltRangeDegrees = profile.tiltRangeDegrees;
            runtime.goboCount = profile.goboCount;

            channelMap.dimmer = profile.FindOffset(SpectraChannelFunction.Dimmer);
            channelMap.red = profile.FindOffset(SpectraChannelFunction.Red);
            channelMap.green = profile.FindOffset(SpectraChannelFunction.Green);
            channelMap.blue = profile.FindOffset(SpectraChannelFunction.Blue);
            channelMap.white = profile.FindOffset(SpectraChannelFunction.White);
            channelMap.pan = profile.FindOffset(SpectraChannelFunction.Pan);
            channelMap.panFine = profile.FindOffset(SpectraChannelFunction.PanFine);
            channelMap.tilt = profile.FindOffset(SpectraChannelFunction.Tilt);
            channelMap.tiltFine = profile.FindOffset(SpectraChannelFunction.TiltFine);
            channelMap.strobe = profile.FindOffset(SpectraChannelFunction.Strobe);
            channelMap.zoom = profile.FindOffset(SpectraChannelFunction.Zoom);
            channelMap.gobo = profile.FindOffset(SpectraChannelFunction.Gobo);
            channelMap.goboRotate = profile.FindOffset(SpectraChannelFunction.GoboRotate);
            channelMap.prism = profile.FindOffset(SpectraChannelFunction.Prism);

            int capabilityMask = 0;
            if (channelMap.dimmer >= 0)
                capabilityMask |= (int)SpectraFixtureCapability.Intensity;
            if (channelMap.red >= 0 || channelMap.green >= 0
                || channelMap.blue >= 0 || channelMap.white >= 0)
                capabilityMask |= (int)SpectraFixtureCapability.Color;
            if (channelMap.pan >= 0 || channelMap.tilt >= 0)
                capabilityMask |= (int)SpectraFixtureCapability.Movement;
            if (channelMap.gobo >= 0)
                capabilityMask |= (int)SpectraFixtureCapability.Gobo;
            if (channelMap.prism >= 0)
                capabilityMask |= (int)SpectraFixtureCapability.Prism;
            if (channelMap.zoom >= 0)
                capabilityMask |= (int)SpectraFixtureCapability.ZoomFocus;
            if (channelMap.strobe >= 0)
                capabilityMask |= (int)SpectraFixtureCapability.Strobe;
            if (profile.fixtureType == SpectraFixtureType.Laser)
                capabilityMask |= (int)SpectraFixtureCapability.Laser;
            if ((capabilityMask & (int)SpectraFixtureCapability.Intensity) != 0)
                capabilityMask |= (int)SpectraFixtureCapability.AudioReactive;
            runtime.capabilities = (SpectraFixtureCapability)capabilityMask;

            runtime.PublishFixtureProperties();
        }
    }
}
