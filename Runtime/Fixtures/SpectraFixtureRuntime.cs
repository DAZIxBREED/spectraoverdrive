using UdonSharp;
using UnityEngine;

namespace SpectraOverdrive
{
    [UdonBehaviourSyncMode(BehaviourSyncMode.None)]
    public class SpectraFixtureRuntime : UdonSharpBehaviour
    {
        public SpectraFixtureIdentity identity;
        public SpectraFixtureChannelMap channels;
        public Renderer[] controlledRenderers;

        [Header("Local calibration")]
        [Range(0f, 2f)] public float intensityMultiplier = 1f;
        [Range(0f, 2f)] public float beamMultiplier = 1f;
        [Range(0f, 2f)] public float projectionMultiplier = 1f;
        [Range(0f, 720f)] public float panRangeDegrees = 540f;
        [Range(0f, 360f)] public float tiltRangeDegrees = 270f;
        [Range(1, 64)] public int goboCount = 8;
        [Tooltip("Runtime capabilities used by capability-aware cue evaluation.")]
        public SpectraFixtureCapability capabilities = SpectraFixtureCapability.All;

        [Header("Group contribution")]
        public Color groupColorMultiplier = Color.white;
        [Range(0f, 2f)] public float groupIntensityMultiplier = 1f;
        [Range(-1f, 1f)] public float groupPanBias;
        [Range(-1f, 1f)] public float groupTiltBias;
        [Range(0f, 2f)] public float groupMovementScale = 1f;
        public float groupGoboIndex = -1f;
        public float groupGoboRotation;
        [Range(0f, 1f)] public float groupPrismAmount;
        public float groupZoom = -1f;
        public float groupFocus = -1f;
        [Range(0f, 30f)] public float groupStrobeHz;
        public bool groupLaserEnabled;
        public int groupAudioReactiveBand = -1;
        public float groupAudioReactiveAmount;
        public float groupAudioReactiveFloor = 1f;

        private Material[] _controlledMaterials;
        private int _universeId;
        private int _startAddressId;
        private int _channelMap0Id;
        private int _channelMap1Id;
        private int _channelMap2Id;
        private int _fixtureCalibrationId;
        private int _movementCalibrationId;
        private int _groupMotionId;
        private int _groupColorId;
        private int _groupOpticsId;
        private int _groupEffectsId;
        private int _groupAudioId;
        private int _goboCountId;

        private void Start()
        {
            EnsurePropertyIds();
            CacheControlledMaterials();
            PublishFixtureProperties();
        }

        public bool Supports(SpectraFixtureCapability capability)
        {
            int available = (int)capabilities;
            int required = (int)capability;
            return (available & required) == required;
        }

        public void PublishFixtureProperties()
        {
            if (identity == null || channels == null || controlledRenderers == null)
            {
                return;
            }
            EnsurePropertyIds();
            CacheControlledMaterials();

            int start = identity.startAddress;

            for (int i = 0; i < controlledRenderers.Length; i++)
            {
                Material materialTarget = _controlledMaterials[i];
                if (materialTarget == null) continue;

                materialTarget.SetFloat(_universeId, identity.universe);
                materialTarget.SetFloat(_startAddressId, start);
                materialTarget.SetVector(_channelMap0Id, new Vector4(
                    channels.ResolveAbsoluteChannel(start, channels.dimmer),
                    channels.ResolveAbsoluteChannel(start, channels.red),
                    channels.ResolveAbsoluteChannel(start, channels.green),
                    channels.ResolveAbsoluteChannel(start, channels.blue)
                ));
                materialTarget.SetVector(_channelMap1Id, new Vector4(
                    channels.ResolveAbsoluteChannel(start, channels.pan),
                    channels.ResolveAbsoluteChannel(start, channels.panFine),
                    channels.ResolveAbsoluteChannel(start, channels.tilt),
                    channels.ResolveAbsoluteChannel(start, channels.tiltFine)
                ));
                materialTarget.SetVector(_channelMap2Id, new Vector4(
                    channels.ResolveAbsoluteChannel(start, channels.strobe),
                    channels.ResolveAbsoluteChannel(start, channels.zoom),
                    channels.ResolveAbsoluteChannel(start, channels.gobo),
                    channels.ResolveAbsoluteChannel(start, channels.goboRotate)
                ));
                materialTarget.SetVector(_fixtureCalibrationId, new Vector4(
                    intensityMultiplier,
                    beamMultiplier,
                    projectionMultiplier,
                    identity.upsideDown ? 1f : 0f
                ));
                materialTarget.SetVector(_movementCalibrationId, new Vector4(
                    panRangeDegrees,
                    tiltRangeDegrees,
                    identity.invertPan ? 1f : 0f,
                    identity.invertTilt ? 1f : 0f
                ));
                materialTarget.SetVector(_groupMotionId, new Vector4(
                    groupPanBias,
                    groupTiltBias,
                    groupMovementScale,
                    groupIntensityMultiplier
                ));
                materialTarget.SetColor(_groupColorId, groupColorMultiplier);
                materialTarget.SetVector(_groupOpticsId, new Vector4(
                    groupGoboIndex,
                    groupGoboRotation,
                    groupPrismAmount,
                    groupZoom
                ));
                materialTarget.SetVector(_groupEffectsId, new Vector4(
                    groupStrobeHz,
                    groupLaserEnabled ? 1f : 0f,
                    groupFocus,
                    0f
                ));
                materialTarget.SetVector(_groupAudioId, new Vector4(
                    groupAudioReactiveBand,
                    groupAudioReactiveAmount,
                    groupAudioReactiveFloor,
                    groupAudioReactiveBand >= 0 ? 1f : 0f
                ));
                materialTarget.SetFloat(_goboCountId, goboCount);
            }
        }

        private void EnsurePropertyIds()
        {
            if (_universeId != 0) return;
            _universeId = Shader.PropertyToID("_SpectraUniverse");
            _startAddressId = Shader.PropertyToID("_SpectraStartAddress");
            _channelMap0Id = Shader.PropertyToID("_SpectraChannelMap0");
            _channelMap1Id = Shader.PropertyToID("_SpectraChannelMap1");
            _channelMap2Id = Shader.PropertyToID("_SpectraChannelMap2");
            _fixtureCalibrationId = Shader.PropertyToID("_SpectraFixtureCalibration");
            _movementCalibrationId = Shader.PropertyToID("_SpectraMovementCalibration");
            _groupMotionId = Shader.PropertyToID("_SpectraGroupMotion");
            _groupColorId = Shader.PropertyToID("_SpectraGroupColor");
            _groupOpticsId = Shader.PropertyToID("_SpectraGroupOptics");
            _groupEffectsId = Shader.PropertyToID("_SpectraGroupEffects");
            _groupAudioId = Shader.PropertyToID("_SpectraGroupAudio");
            _goboCountId = Shader.PropertyToID("_SpectraGoboCount");
        }

        private void CacheControlledMaterials()
        {
            if (controlledRenderers == null) return;
            if (_controlledMaterials != null
                && _controlledMaterials.Length == controlledRenderers.Length)
                return;
            _controlledMaterials = new Material[controlledRenderers.Length];
            for (int i = 0; i < controlledRenderers.Length; i++)
            {
                Renderer rendererTarget = controlledRenderers[i];
                if (rendererTarget != null)
                    _controlledMaterials[i] = rendererTarget.material;
            }
        }
    }
}
