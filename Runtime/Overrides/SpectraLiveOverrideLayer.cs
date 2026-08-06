using UdonSharp;
using UnityEngine;
using VRC.SDKBase;

namespace SpectraOverdrive
{
    [UdonBehaviourSyncMode(BehaviourSyncMode.Manual)]
    public class SpectraLiveOverrideLayer : UdonSharpBehaviour
    {
        [Header("Targets")]
        public SpectraShowRuntimePlayer player;
        public SpectraLiveOverrideRecorder recorder;
        [Range(1, 128)] public int configuredGroupCount = 16;

        [Header("Operator input")]
        public int selectedGroup;
        public SpectraOverrideMode selectedMode = SpectraOverrideMode.Replace;
        [Range(0f, 2f)] public float inputIntensity = 1f;
        public Color inputColor = Color.white;
        [Range(-1f, 1f)] public float inputPan;
        [Range(-1f, 1f)] public float inputTilt;
        [Range(0f, 4f)] public float inputMovement = 1f;
        public float inputGobo = -1f;
        [Range(-8f, 8f)] public float inputGoboRotation;
        [Range(0f, 1f)] public float inputPrism;
        public float inputZoom = -1f;
        public float inputFocus = -1f;
        [Range(0f, 30f)] public float inputStrobeHz;
        public bool inputLaser;

        [Header("Synchronized flat override state")]
        [UdonSynced] public int revision;
        [UdonSynced] public int[] modes = new int[0];
        [UdonSynced] public float[] intensities = new float[0];
        [UdonSynced] public float[] colorR = new float[0];
        [UdonSynced] public float[] colorG = new float[0];
        [UdonSynced] public float[] colorB = new float[0];
        [UdonSynced] public float[] colorA = new float[0];
        [UdonSynced] public float[] pans = new float[0];
        [UdonSynced] public float[] tilts = new float[0];
        [UdonSynced] public float[] movements = new float[0];
        [UdonSynced] public float[] gobos = new float[0];
        [UdonSynced] public float[] goboRotations = new float[0];
        [UdonSynced] public float[] prisms = new float[0];
        [UdonSynced] public float[] zooms = new float[0];
        [UdonSynced] public float[] focuses = new float[0];
        [UdonSynced] public float[] strobeRates = new float[0];
        [UdonSynced] public bool[] lasers = new bool[0];

        private void Start()
        {
            EnsureCapacity();
        }

        public void CommitSelectedGroup()
        {
            EnsureCapacity();
            int group = Mathf.Clamp(selectedGroup, 0, modes.Length - 1);
            AcquireOwnership();
            modes[group] = (int)selectedMode;
            intensities[group] = inputIntensity;
            colorR[group] = inputColor.r;
            colorG[group] = inputColor.g;
            colorB[group] = inputColor.b;
            colorA[group] = inputColor.a;
            pans[group] = inputPan;
            tilts[group] = inputTilt;
            movements[group] = inputMovement;
            gobos[group] = inputGobo;
            goboRotations[group] = inputGoboRotation;
            prisms[group] = inputPrism;
            zooms[group] = inputZoom;
            focuses[group] = inputFocus;
            strobeRates[group] = inputStrobeHz;
            lasers[group] = inputLaser;
            revision++;
            if (recorder != null) recorder.RecordCurrentOverride(group, this);
            RequestSerialization();
            RefreshPlayer();
        }

        public void MuteSelectedGroup()
        {
            selectedMode = SpectraOverrideMode.Mute;
            CommitSelectedGroup();
        }

        public void SoloSelectedGroup()
        {
            selectedMode = SpectraOverrideMode.Solo;
            CommitSelectedGroup();
        }

        public void ClearSelectedGroup()
        {
            EnsureCapacity();
            int group = Mathf.Clamp(selectedGroup, 0, modes.Length - 1);
            AcquireOwnership();
            ResetGroup(group);
            revision++;
            if (recorder != null) recorder.RecordClearGroup(group);
            RequestSerialization();
            RefreshPlayer();
        }

        public void ClearAll()
        {
            EnsureCapacity();
            AcquireOwnership();
            for (int i = 0; i < modes.Length; i++) ResetGroup(i);
            revision++;
            if (recorder != null) recorder.RecordClearAll();
            RequestSerialization();
            RefreshPlayer();
        }

        public void ApplyToGroups(SpectraFixtureGroup[] groups)
        {
            if (groups == null) return;
            EnsureCapacity();
            int soloGroup = -1;
            for (int i = 0; i < modes.Length && i < groups.Length; i++)
                if ((SpectraOverrideMode)modes[i] == SpectraOverrideMode.Solo) { soloGroup = i; break; }

            for (int i = 0; i < groups.Length; i++)
            {
                SpectraFixtureGroup group = groups[i];
                if (group == null) continue;
                if (soloGroup >= 0 && i != soloGroup)
                {
                    group.intensityMultiplier = 0f;
                    group.strobeHz = 0f;
                    group.laserEnabled = false;
                    continue;
                }
                if (i >= modes.Length) continue;
                SpectraOverrideMode mode = (SpectraOverrideMode)modes[i];
                if (mode == SpectraOverrideMode.None) continue;
                if (mode == SpectraOverrideMode.Mute)
                {
                    group.intensityMultiplier = 0f;
                    group.strobeHz = 0f;
                    group.laserEnabled = false;
                    continue;
                }

                Color overrideColor = new Color(colorR[i], colorG[i], colorB[i], colorA[i]);
                if (mode == SpectraOverrideMode.Add)
                {
                    group.intensityMultiplier += intensities[i];
                    group.colorMultiplier += overrideColor;
                    group.panBias += pans[i];
                    group.tiltBias += tilts[i];
                }
                else if (mode == SpectraOverrideMode.Multiply)
                {
                    group.intensityMultiplier *= intensities[i];
                    group.colorMultiplier *= overrideColor;
                    group.panBias *= pans[i];
                    group.tiltBias *= tilts[i];
                }
                else
                {
                    group.intensityMultiplier = intensities[i];
                    group.colorMultiplier = overrideColor;
                    group.panBias = pans[i];
                    group.tiltBias = tilts[i];
                }
                group.movementScale = movements[i];
                group.goboIndex = gobos[i];
                group.goboRotation = goboRotations[i];
                group.prismAmount = prisms[i];
                group.zoom = zooms[i];
                group.focus = focuses[i];
                group.strobeHz = strobeRates[i];
                group.laserEnabled = lasers[i];
            }
        }

        public override void OnDeserialization()
        {
            EnsureCapacity();
            RefreshPlayer();
        }

        private void EnsureCapacity()
        {
            int count = Mathf.Max(1, configuredGroupCount);
            if (player != null && player.groups != null) count = Mathf.Max(1, player.groups.Length);
            if (modes != null && modes.Length == count
                && intensities != null && intensities.Length == count
                && colorR != null && colorR.Length == count
                && colorG != null && colorG.Length == count
                && colorB != null && colorB.Length == count
                && colorA != null && colorA.Length == count
                && pans != null && pans.Length == count
                && tilts != null && tilts.Length == count
                && movements != null && movements.Length == count
                && gobos != null && gobos.Length == count
                && goboRotations != null && goboRotations.Length == count
                && prisms != null && prisms.Length == count
                && zooms != null && zooms.Length == count
                && focuses != null && focuses.Length == count
                && strobeRates != null && strobeRates.Length == count
                && lasers != null && lasers.Length == count) return;

            modes = new int[count];
            intensities = new float[count];
            colorR = new float[count];
            colorG = new float[count];
            colorB = new float[count];
            colorA = new float[count];
            pans = new float[count];
            tilts = new float[count];
            movements = new float[count];
            gobos = new float[count];
            goboRotations = new float[count];
            prisms = new float[count];
            zooms = new float[count];
            focuses = new float[count];
            strobeRates = new float[count];
            lasers = new bool[count];
            for (int i = 0; i < count; i++) ResetGroup(i);
        }

        private void ResetGroup(int group)
        {
            modes[group] = (int)SpectraOverrideMode.None;
            intensities[group] = 1f;
            colorR[group] = 1f;
            colorG[group] = 1f;
            colorB[group] = 1f;
            colorA[group] = 1f;
            pans[group] = 0f;
            tilts[group] = 0f;
            movements[group] = 1f;
            gobos[group] = -1f;
            goboRotations[group] = 0f;
            prisms[group] = 0f;
            zooms[group] = -1f;
            focuses[group] = -1f;
            strobeRates[group] = 0f;
            lasers[group] = false;
        }

        private void AcquireOwnership()
        {
            if (Networking.LocalPlayer != null && !Networking.IsOwner(gameObject))
                Networking.SetOwner(Networking.LocalPlayer, gameObject);
        }

        private void RefreshPlayer()
        {
            if (player != null) player.ApplyAtTime(player.showTime);
        }
    }
}
