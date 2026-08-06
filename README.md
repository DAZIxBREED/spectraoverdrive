# SpectraOverdrive 1.4.0

SpectraOverdrive is a cross-platform Unity and VRChat stage-lighting authoring,
playback, synchronization, and live-operation system written by DAZIxBREED.
One compiled show preserves timing, cue intent, safety, macro state, generative
choices, and scene transitions across PCVR, Quest, iOS, and Android while each
client applies its own bounded rendering policy.

## What 1.4 adds

- schema-v7 editable and portable shows
- deterministic per-cue conditions:
  - Probability
  - Every Nth Cycle
  - Macro Above / Below
  - Audio Above / Below
- synchronized variation groups with Cycle, Ping-Pong, Seeded Random, and
  Macro Select routing
- up to 16 variation groups and eight options per group
- absolute show-clock variation selection, keeping clients and overlapping cues
  on the same option without extra network messages
- conditioned and non-selected variation cues rejected before local active-cue
  budget allocation
- up to 16 show-defined performance macro snapshots
- synchronized recall of all four performance macros with one transition
- desktop/VR-friendly `SpectraMacroSnapshotController`
- timeline `COND` and `VAR` badges and one-click generative authoring presets
- schema-v6 to schema-v7 migration, JSON support, deterministic signatures,
  release reporting, platform analysis, and expanded runtime self-tests
- updated Neon Drop demo containing real conditional accents, synchronized
  movement variations, and macro snapshot looks

All 1.3 features remain: deterministic rhythm gates, dynamic color palettes,
procedural modulation, four synchronized macro buses, ordered scene stacks,
flattened keyframe automation, variable-tempo hot cues, fixture capability
fallbacks, server-time playback, late-join recovery, show banks, live overrides,
recording, snapshots, optics, AudioLink, world events, adaptive quality, signed
portable shows, VRSL migration, accessibility controls, and emergency blackout.

## Deterministic cue conditions

Conditions are compiled into primitive arrays and evaluated before a cue enters
the platform's active-cue budget. Probability and Every-N conditions use the
cue-relative clock and deterministic integer hashing. Macro conditions use the
four synchronized performance macro buses. Audio conditions use the runtime
AudioLink adapter's published/manual CPU-side band values; shader-only texture
sampling is intentionally not pulled back to the CPU.

No condition requires runtime allocation, collections, reflection, JSON parsing,
or client-local random state.

## Synchronized variation groups

Cues in one variation group share the same mode, option count, clock, phase,
seed, and macro binding. Each cue declares which option it belongs to. The
runtime selects one option from the absolute show clock, so overlapping cues
with different start times still agree on the same look.

- **Cycle:** advances through options in order.
- **Ping-Pong:** walks forward and backward through the group.
- **Seeded Random:** picks one deterministic option per cycle.
- **Macro Select:** maps a synchronized macro value to an option.

Multiple cues may use the same option, allowing one variation to contain a
layered movement, color, intensity, and optics look.

## Performance macro snapshots

A snapshot stores four normalized macro values, a display name/color, and a
transition time. Recalling a snapshot writes one compact synchronized state:
the four targets, their common server-time transition, the snapshot index, and
a revision. Late joiners reconstruct the same transition from server time.

The production-rig generator automatically adds and wires a
`SpectraMacroSnapshotController`.

## Default runtime ceilings

| Platform | Active cues | Fixtures | Evaluation | Transparent beams | Default fallback |
| --- | ---: | ---: | ---: | ---: | --- |
| PCVR | 128 | 128 | 60 Hz | 64 | Full |
| Quest | 48 | 64 | 36 Hz | 20 | Simplified |
| iOS | 32 | 48 | 30 Hz | 12 | Emissive Only |
| Android phone/tablet | 32 | 48 | 30 Hz | 10 | Emissive Only |

Adaptive quality may lower local concurrency, fixture count, beam count, shader
tier, and evaluation cadence. It does not change show time, deterministic
condition results, variation selection, rhythm steps, palette selection, macro
targets, scene targets, hot-cue timing, safety state, or another user's local
accessibility settings.

## Install

1. Back up the Unity project.
2. Remove an older embedded SpectraOverdrive package or replace its folder.
3. Copy this folder under `Packages/com.dazixbreed.spectraoverdrive`, or add it
   through the Unity Package Manager as a local package.
4. Open the project with Unity 2022.3 LTS and allow scripts to compile.
5. Run **SpectraOverdrive > Show Programmer > Run Runtime Self-Test**.
6. Run **Run 1.4 Release Readiness Check** on each show.
7. Re-bake production runtime players after upgrading a show to schema v7.

## Recommended first run

1. Open **SpectraOverdrive > Show Programmer > Create Neon Drop Demo**.
2. Inspect the movement variation pair in the first drop and the conditioned
   accent cues.
3. Open the Show Programmer and inspect `performanceMacroSnapshots`.
4. Create a production runtime rig.
5. Bind UI buttons to the generated macro snapshot controller's selection and
   recall methods.
6. Use the platform simulator and release-readiness validator.

## Important folders

- `Runtime/Shows` — editable/compiled show types and schema migration
- `Runtime/Playback` — allocation-free runtime evaluation
- `Runtime/Network` — server-time synchronized authoritative state
- `Runtime/Operators` — hot-cue, scene-stack, macro, and snapshot controls
- `Editor/ShowProgrammer` — timeline, compiler, authoring helpers, and demo
- `Editor/Validation` — platform, release, and runtime self-tests
- `Documentation~` — architecture, upgrade, compatibility, and verification

## Compatibility

Designed for:

- Unity 2022.3 LTS
- VRChat Worlds SDK 3.10 or newer
- UdonSharp 1.1.9 or newer
- Windows editor and Linux editor authoring workflows
- PCVR, Quest, iOS, and Android runtime policies

A single Unity project still needs VRChat-supported platform build targets and
platform-appropriate content. SpectraOverdrive cannot make unsupported shaders,
video systems, or excessive world geometry mobile-compatible by itself.

## Verification boundary

The package includes executable editor self-tests and extensive static source
checks. This archive was not compiled inside the user's complete Unity/VRChat
project and was not run on physical PCVR, Quest, iOS, or Android hardware here.
Run the included checks and device matrix before production deployment.
