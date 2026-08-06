using UdonSharp;
using UnityEngine;

namespace SpectraOverdrive
{
    [UdonBehaviourSyncMode(BehaviourSyncMode.None)]
    public class SpectraFixtureGroup : UdonSharpBehaviour
    {
        public int groupId;
        public string groupName = "Fixture Group";
        public SpectraFixtureRuntime[] fixtures;
        public SpectraFixtureSelection selection = SpectraFixtureSelection.All;
        public int selectionSeed;
        [Tooltip("-1 enables every fixture; the runtime player writes a local platform budget when a show is active.")]
        public int fixtureBudget = -1;

        [Header("Group output")]
        public Color colorMultiplier = Color.white;
        [Range(0f, 2f)] public float intensityMultiplier = 1f;
        [Range(-1f, 1f)] public float panBias;
        [Range(-1f, 1f)] public float tiltBias;
        [Range(0f, 2f)] public float movementScale = 1f;
        public SpectraMovementPatternKind movementPattern;
        public float movementPatternTime;
        [Range(0f, 8f)] public float movementPatternSpeed = 1f;
        [Range(0f, 2f)] public float movementPatternAmplitude = 1f;
        [Range(0f, 2f)] public float movementPatternSpread = 1f;
        [Range(-8f, 8f)] public float movementPatternPhase;
        [Range(-1f, 1f)] public float movementPatternDirection = 1f;
        [Range(0f, 1f)] public float movementPatternSmoothing = 0.5f;
        public int movementPatternSeed;
        [HideInInspector] public float movementPatternWeight;
        [Header("Optics and effects")]
        public float goboIndex = -1f;
        [Range(-8f, 8f)] public float goboRotation;
        [Range(0f, 1f)] public float prismAmount;
        public float zoom = -1f;
        public float focus = -1f;
        [Range(0f, 30f)] public float strobeHz;
        public bool laserEnabled;
        public int audioReactiveBand = -1;
        [Range(-2f, 2f)] public float audioReactiveAmount;
        [Range(0f, 1f)] public float audioReactiveFloor = 1f;
        [HideInInspector] public float audioReactiveWeight;

        public void ApplyToFixtures()
        {
            if (fixtures == null) return;
            int allowed = fixtureBudget < 0
                ? fixtures.Length
                : Mathf.Clamp(fixtureBudget, 0, fixtures.Length);

            for (int i = 0; i < fixtures.Length; i++)
            {
                SpectraFixtureRuntime fixture = fixtures[i];
                if (fixture == null) continue;

                int orderedIndex = ResolveOrderedIndex(i, fixtures.Length);
                bool selected = i < allowed && IsFixtureSelected(orderedIndex, fixtures.Length);
                Vector2 patternOffset = selected ? EvaluatePattern(orderedIndex, fixtures.Length) : Vector2.zero;
                int capabilityMask = (int)fixture.capabilities;
                bool intensityCapable = (capabilityMask & (int)SpectraFixtureCapability.Intensity) != 0;
                bool colorCapable = (capabilityMask & (int)SpectraFixtureCapability.Color) != 0;
                bool movementCapable = (capabilityMask & (int)SpectraFixtureCapability.Movement) != 0;
                bool goboCapable = (capabilityMask & (int)SpectraFixtureCapability.Gobo) != 0;
                bool prismCapable = (capabilityMask & (int)SpectraFixtureCapability.Prism) != 0;
                bool zoomCapable = (capabilityMask & (int)SpectraFixtureCapability.ZoomFocus) != 0;
                bool strobeCapable = (capabilityMask & (int)SpectraFixtureCapability.Strobe) != 0;
                bool laserCapable = (capabilityMask & (int)SpectraFixtureCapability.Laser) != 0;
                bool audioCapable = (capabilityMask & (int)SpectraFixtureCapability.AudioReactive) != 0;
                fixture.groupColorMultiplier = selected && colorCapable ? colorMultiplier : Color.white;
                fixture.groupIntensityMultiplier = selected && intensityCapable ? intensityMultiplier : 1f;
                fixture.groupPanBias = selected && movementCapable ? Mathf.Clamp(panBias + patternOffset.x, -1f, 1f) : 0f;
                fixture.groupTiltBias = selected && movementCapable ? Mathf.Clamp(tiltBias + patternOffset.y, -1f, 1f) : 0f;
                fixture.groupMovementScale = selected && movementCapable ? movementScale : 1f;
                fixture.groupGoboIndex = selected && goboCapable ? goboIndex : -1f;
                fixture.groupGoboRotation = selected && goboCapable ? goboRotation : 0f;
                fixture.groupPrismAmount = selected && prismCapable ? prismAmount : 0f;
                fixture.groupZoom = selected && zoomCapable ? zoom : -1f;
                fixture.groupFocus = selected && zoomCapable ? focus : -1f;
                fixture.groupStrobeHz = selected && strobeCapable ? strobeHz : 0f;
                fixture.groupLaserEnabled = selected && laserCapable && laserEnabled;
                fixture.groupAudioReactiveBand = selected && audioCapable ? audioReactiveBand : -1;
                fixture.groupAudioReactiveAmount = selected && audioCapable ? audioReactiveAmount : 0f;
                fixture.groupAudioReactiveFloor = selected && audioCapable ? audioReactiveFloor : 1f;
                fixture.PublishFixtureProperties();
            }
        }

        private int ResolveOrderedIndex(int fixtureIndex, int fixtureCount)
        {
            if (selection == SpectraFixtureSelection.Reverse)
                return Mathf.Max(0, fixtureCount - 1 - fixtureIndex);
            if (selection == SpectraFixtureSelection.CenterOut && fixtureCount > 1)
            {
                float center = (fixtureCount - 1) * 0.5f;
                float distance = Mathf.Abs(fixtureIndex - center);
                float maximum = Mathf.Max(0.5f, center);
                return Mathf.RoundToInt(distance / maximum * (fixtureCount - 1));
            }
            return fixtureIndex;
        }

        private bool IsFixtureSelected(int fixtureIndex, int fixtureCount)
        {
            if (selection == SpectraFixtureSelection.Odd) return fixtureIndex % 2 == 0;
            if (selection == SpectraFixtureSelection.Even) return fixtureIndex % 2 != 0;
            if (selection == SpectraFixtureSelection.Alternating)
            {
                int step = Mathf.FloorToInt(movementPatternTime * Mathf.Max(0.25f, movementPatternSpeed));
                return (fixtureIndex + step) % 2 == 0;
            }
            if (selection == SpectraFixtureSelection.SeededRandom)
            {
                int step = Mathf.FloorToInt(movementPatternTime * Mathf.Max(0.25f, movementPatternSpeed));
                return HashSigned(selectionSeed, fixtureIndex, step) >= 0f;
            }
            return true;
        }

        private Vector2 EvaluatePattern(int fixtureIndex, int fixtureCount)
        {
            float pan = 0f;
            float tilt = 0f;
            if (movementPattern == SpectraMovementPatternKind.Static || movementPatternWeight <= 0f)
                return Vector2.zero;

            float count = Mathf.Max(1f, fixtureCount - 1f);
            float normalized = fixtureCount <= 1 ? 0.5f : fixtureIndex / count;
            float centered = normalized * 2f - 1f;
            float direction = movementPatternDirection < 0f ? -1f : 1f;
            float cycle = movementPatternTime * movementPatternSpeed * 6.2831853f * direction
                + movementPatternPhase;
            float fixturePhase = centered * movementPatternSpread * 3.1415926f;
            float amplitude = movementPatternAmplitude * movementPatternWeight;
            float wave = ApplySmoothing(Mathf.Sin(cycle + fixturePhase));
            float waveQuarter = ApplySmoothing(Mathf.Cos(cycle + fixturePhase));

            if (movementPattern == SpectraMovementPatternKind.HorizontalSweep)
                pan = wave * amplitude;
            else if (movementPattern == SpectraMovementPatternKind.VerticalSweep)
                tilt = wave * amplitude;
            else if (movementPattern == SpectraMovementPatternKind.Circle)
            {
                pan = wave * amplitude;
                tilt = waveQuarter * amplitude;
            }
            else if (movementPattern == SpectraMovementPatternKind.FigureEight)
            {
                pan = wave * amplitude;
                tilt = Mathf.Sin((cycle + fixturePhase) * 2f) * amplitude * 0.6f;
            }
            else if (movementPattern == SpectraMovementPatternKind.Fan)
            {
                pan = centered * amplitude * movementPatternSpread;
                tilt = waveQuarter * amplitude * 0.35f;
            }
            else if (movementPattern == SpectraMovementPatternKind.ReverseFan)
            {
                pan = -centered * amplitude * movementPatternSpread;
                tilt = waveQuarter * amplitude * 0.35f;
            }
            else if (movementPattern == SpectraMovementPatternKind.CenterOutFan)
            {
                float side = centered < 0f ? -1f : 1f;
                pan = side * Mathf.Abs(centered) * amplitude * movementPatternSpread;
                tilt = Mathf.Cos(cycle) * amplitude * 0.3f;
            }
            else if (movementPattern == SpectraMovementPatternKind.Wave)
            {
                pan = wave * amplitude;
                tilt = Mathf.Sin(cycle + fixturePhase * 1.7f) * amplitude * 0.45f;
            }
            else if (movementPattern == SpectraMovementPatternKind.AlternatingWave)
            {
                float sign = fixtureIndex % 2 == 0 ? 1f : -1f;
                pan = wave * amplitude * sign;
                tilt = waveQuarter * amplitude * 0.5f;
            }
            else if (movementPattern == SpectraMovementPatternKind.Bounce)
                tilt = (Mathf.Abs(wave) * 2f - 1f) * amplitude;
            else if (movementPattern == SpectraMovementPatternKind.Spiral)
            {
                float radius = 0.25f + 0.75f * (0.5f + 0.5f * Mathf.Sin(cycle * 0.25f));
                pan = wave * amplitude * radius;
                tilt = waveQuarter * amplitude * radius;
            }
            else if (movementPattern == SpectraMovementPatternKind.Cross)
            {
                pan = wave * amplitude;
                tilt = (fixtureIndex % 2 == 0 ? wave : -wave) * amplitude;
            }
            else if (movementPattern == SpectraMovementPatternKind.AudienceSweep)
            {
                pan = wave * amplitude;
                tilt = -0.45f * amplitude;
            }
            else if (movementPattern == SpectraMovementPatternKind.StageSweep)
            {
                pan = wave * amplitude;
                tilt = 0.45f * amplitude;
            }
            else if (movementPattern == SpectraMovementPatternKind.DjFocus)
            {
                pan = -centered * amplitude * 0.75f;
                tilt = 0.2f * amplitude;
            }
            else if (movementPattern == SpectraMovementPatternKind.Mirrored)
            {
                float sign = centered < 0f ? -1f : 1f;
                pan = wave * sign * amplitude;
                tilt = waveQuarter * amplitude * 0.5f;
            }
            else if (movementPattern == SpectraMovementPatternKind.FollowTheLeader)
            {
                pan = Mathf.Sin(cycle - normalized * movementPatternSpread * 6.2831853f) * amplitude;
                tilt = Mathf.Cos(cycle - normalized * movementPatternSpread * 6.2831853f) * amplitude * 0.6f;
            }
            else if (movementPattern == SpectraMovementPatternKind.Chase)
            {
                float chase = Mathf.Max(0f, Mathf.Sin(cycle - normalized * movementPatternSpread * 6.2831853f));
                pan = (chase * 2f - 1f) * amplitude;
                tilt = chase * amplitude * 0.5f;
            }
            else if (movementPattern == SpectraMovementPatternKind.SeededRandom)
            {
                int step = Mathf.FloorToInt(movementPatternTime * Mathf.Max(0.05f, movementPatternSpeed));
                pan = HashSigned(movementPatternSeed, fixtureIndex, step) * amplitude;
                tilt = HashSigned(movementPatternSeed + 7919, fixtureIndex, step) * amplitude;
            }
            return new Vector2(pan, tilt);
        }

        private float HashSigned(int seed, int fixtureIndex, int step)
        {
            int value = seed;
            value = value * 1103515245 + 12345 + fixtureIndex * 374761393;
            value = value ^ (step * 668265263);
            value = value ^ (value >> 13);
            int positive = value & 0x7fffffff;
            return positive / 1073741823.5f - 1f;
        }

        private float ApplySmoothing(float value)
        {
            float normalized = value * 0.5f + 0.5f;
            float smoothed = normalized * normalized * (3f - 2f * normalized);
            return Mathf.Lerp(value, smoothed * 2f - 1f, movementPatternSmoothing);
        }
    }
}
