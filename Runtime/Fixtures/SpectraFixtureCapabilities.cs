namespace SpectraOverdrive
{
    public static class SpectraFixtureCapabilities
    {
        public static SpectraFixtureCapability ForType(SpectraFixtureType type)
        {
            SpectraFixtureCapability basic = SpectraFixtureCapability.Intensity
                | SpectraFixtureCapability.Color
                | SpectraFixtureCapability.AudioReactive;
            if (type == SpectraFixtureType.MovingSpot
                || type == SpectraFixtureType.MovingBeam)
                return basic | SpectraFixtureCapability.Movement
                    | SpectraFixtureCapability.Gobo
                    | SpectraFixtureCapability.Prism
                    | SpectraFixtureCapability.ZoomFocus
                    | SpectraFixtureCapability.Strobe;
            if (type == SpectraFixtureType.MovingWash)
                return basic | SpectraFixtureCapability.Movement
                    | SpectraFixtureCapability.ZoomFocus
                    | SpectraFixtureCapability.Strobe;
            if (type == SpectraFixtureType.Laser)
                return basic | SpectraFixtureCapability.Movement
                    | SpectraFixtureCapability.Laser;
            if (type == SpectraFixtureType.Disco)
                return basic | SpectraFixtureCapability.Movement;
            if (type == SpectraFixtureType.Par
                || type == SpectraFixtureType.Blinder
                || type == SpectraFixtureType.Strobe
                || type == SpectraFixtureType.LightBar)
                return basic | SpectraFixtureCapability.Strobe;
            return SpectraFixtureCapability.All;
        }
    }
}
