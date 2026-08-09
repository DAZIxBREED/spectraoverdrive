# SpectraOverdrive 1.5.1

SpectraOverdrive is a cross-platform Unity and VRChat stage-lighting authoring,
playback, synchronization, and live-operation system written by DAZIxBREED.
A single compiled show preserves timing, cue intent, safety, operator state, and
deterministic routing across PCVR, Quest, iOS, and Android while each client
uses its own bounded rendering policy.

## What 1.5.1 fixes

- Canonicalizes cue-layer enabled and solo masks so a disabled layer can never remain as the active solo target.
- Soloing a disabled layer now enables and solos it atomically instead of suppressing every layered cue.
- Disabling or toggling off a soloed layer clears its solo bit in the same synchronized transaction.
- Late joiners keep compiled macro defaults and cue-layer defaults until the first authoritative revision is received, avoiding a brief all-layers/all-macros fallback state.
- Runtime arbitration now rejects malformed candidates whose mode, clock, phase, cycle length, or seed disagrees with the prepared group configuration.
- Adds `arbitrationConfigurationMismatchCount` diagnostics for corrupted or hand-edited baked players.
- Expands the editor self-test with layer-lockout, pre-deserialization, and malformed-arbitration regression coverage.
- Strengthens `Tools/validate_package.py` so package, README, portable-show, and release-report versions must agree.

1.5.1 keeps **schema v8**. Shows authored in 1.5.0 do not require a data migration, but baked runtime players should be rebuilt so the runtime fixes and diagnostics are present.

## What 1.5 adds

### Synchronized cue layers

Shows can define up to sixteen named cue layers. Each layer has:

- a display name, description, and operator color
- an authored default enabled state
- independent PCVR, Quest, iOS, and Android availability
- a priority bias applied before global cue-budget admission
- an optional per-layer active-cue ceiling

Cues bind to a layer by a flattened integer index. The authoritative network
controller synchronizes enabled and solo bitmasks, so operators can mute, restore,
or isolate entire programmed looks without seeking, recompiling, or changing the
show clock. Late joiners receive the same layer state.

### Deterministic cue arbitration

Up to sixteen arbitration groups resolve overlapping alternatives before they
consume the platform's active-cue budget. Available policies are:

- **Highest Priority** — chooses the cue with the highest authored priority plus
  its layer bias.
- **Latest Start** — lets the newest overlapping cue take control.
- **Earliest Start** — preserves the oldest active cue.
- **Deterministic Cycle** — rotates through active candidates from seconds,
  beats, or bars using an authored seed and phase.

Ties are resolved from compiled cue order, not client-local random state. The
same show time therefore produces the same winner on PCVR, Quest, iOS, Android,
and late-joining clients.

### Authoring and operation

- new `SpectraCueLayerController` for desktop or VR UI binding
- production-rig generation now creates and wires the layer controller
- one-click starter layers: Base, Movement, Accents, and Safety
- timeline `L#` and `ARB` badges
- one-click layer assignment and priority/cycle arbitration presets
- Neon Drop demo content with synchronized layer routing and an alternating
  second-drop color arbitration group
- schema-v7 to schema-v8 migration that leaves old shows behaviorally unchanged
  until layers or arbitration are explicitly authored
- compiler, portable JSON, content signatures, release reports, platform
  analysis, and runtime self-tests extended for all new fields

All earlier systems remain available: cue conditions, synchronized variation
groups, macro snapshots, rhythm gates, dynamic palettes, procedural modulation,
four synchronized macro buses, ordered scene stacks, flattened automation,
variable-tempo hot cues, fixture capability fallbacks, late-join recovery, live
overrides, recording, optics, AudioLink, world events, adaptive quality,
accessibility controls, and emergency blackout.

## Runtime order of operations

For every evaluation tick, SpectraOverdrive performs the following bounded,
allocation-free routing sequence:

1. Resolve cue time, conditions, variation selection, and rhythm-gate state.
2. Reject cues whose layer is muted, solo-excluded, or unavailable on the local
   platform.
3. Resolve one winner for each arbitration group.
4. Sort remaining cues by authored priority plus layer bias.
5. Enforce per-layer admission ceilings.
6. Enforce the global platform active-cue budget.
7. Evaluate and publish the selected cues.

This prevents inactive alternatives from stealing mobile cue capacity.

## Default runtime ceilings

| Platform | Active cues | Fixtures | Evaluation | Transparent beams | Default fallback |
| --- | ---: | ---: | ---: | ---: | --- |
| PCVR | 128 | 128 | 60 Hz | 64 | Full |
| Quest | 48 | 64 | 36 Hz | 20 | Simplified |
| iOS | 32 | 48 | 30 Hz | 12 | Emissive Only |
| Android phone/tablet | 32 | 48 | 30 Hz | 10 | Emissive Only |

Adaptive quality may lower local concurrency, fixture count, beam count, shader
tier, and evaluation cadence. It does not alter show time, layer masks,
arbitration winners, deterministic conditions, variation choices, macro targets,
scene targets, hot-cue timing, safety state, or another user's local comfort
settings.

## Install or upgrade

1. Back up the Unity project.
2. Replace the previous embedded package, or copy this folder to
   `Packages/com.dazixbreed.spectraoverdrive`.
3. Open the project with Unity 2022.3 LTS and allow scripts to compile.
4. Open each existing show once so schema migration can run.
5. Re-bake every production runtime player after moving to schema v8.
6. Run **SpectraOverdrive > Show Programmer > Run Runtime Self-Test**.
7. Run **Run 1.5 Release Readiness Check** on every production show.
8. Complete the included PCVR, Quest, iOS, and Android device matrix.

Do not mix a schema-v8 show with an older baked runtime player. Re-baking is
required because cue-layer and arbitration arrays participate in the content
signature.

## Recommended first run

1. Create the **Neon Drop Demo**.
2. Inspect the `cueLayers` section on the show asset.
3. Inspect the second-drop color cues carrying the `ARB` badge.
4. Create a **SpectraOverdrive 1.5 Production Rig**.
5. Bind operator buttons to `SpectraCueLayerController` toggle, solo, reset,
   next, and previous methods.
6. Test the same show with all four platform policies in the simulator.

## Important folders

- `Runtime/Shows` — editable and compiled schemas, migration, and validation
- `Runtime/Playback` — allocation-free cue evaluation and arbitration
- `Runtime/Network` — server-time playback and synchronized operator state
- `Runtime/Operators` — layer, macro, hot-cue, and scene-stack controls
- `Editor/ShowProgrammer` — timeline, compiler, authoring helpers, and demo
- `Editor/Validation` — platform, release, and runtime self-tests
- `Documentation~` — architecture, upgrade, compatibility, and verification

## Compatibility

Designed for:

- Unity 2022.3 LTS
- VRChat Worlds SDK 3.10 or newer
- UdonSharp 1.1.9 or newer
- Windows and Linux Unity editor workflows
- PCVR, Quest, iOS, and Android runtime policies

A Unity project still needs VRChat-supported build targets and appropriately
optimized world content. SpectraOverdrive cannot turn unsupported shaders,
video systems, or excessive geometry into mobile-compatible assets by itself.

## Verification boundary

The package includes executable Unity editor self-tests and static source checks.
The source was not compiled inside your complete Unity/VRChat project and was not
run on physical PCVR, Quest, iOS, or Android hardware in this environment. Run
the included checks and device matrix before a production event.
