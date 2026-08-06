# Changelog

## 1.4.0

- Added schema-v7 deterministic cue conditions: Probability, Every Nth Cycle,
  Macro Above/Below, and Audio Above/Below.
- Added synchronized variation groups with Cycle, Ping-Pong, Seeded Random,
  and Macro Select modes, up to 16 groups and eight options per group.
- Evaluated variation selection from absolute show time so overlapping cues and
  late joiners resolve the same option without extra synchronized state.
- Rejected conditioned-off and non-selected variation cues before active-cue
  budget allocation.
- Added up to 16 performance macro snapshots with synchronized four-macro recall
  and server-time transitions.
- Added `SpectraMacroSnapshotController` and production-rig wiring.
- Added timeline COND/VAR badges and one-click probability, every-N, macro,
  cycle, and seeded authoring helpers.
- Extended compiler arrays, Udon runtime baking, content signatures, JSON,
  schema migration, validation, platform reporting, and release reports.
- Expanded the Neon Drop demo and runtime self-test with real schema-v7 content.
- Preserved PCVR, Quest, iOS, and Android runtime policies and all schema-v6
  rhythm-gate/dynamic-palette behavior.

## 1.3.0

- Added schema-v6 deterministic cue rhythm gates.
- Added Pulse, Alternating, Euclidean, Seeded Random, and 32-step Custom Mask patterns.
- Added seconds-, beat-, and bar-based gate clocks with step length, pattern length, active-step count, duty, attack, release, phase, and inversion.
- Ensured gated-off cues do not consume the local active-cue budget.
- Added show-defined dynamic color palettes with up to 16 palettes and 16 colors per palette.
- Added Fixed, Step, Ping-Pong, Seeded Random, and synchronized Macro Morph palette modes.
- Flattened palette metadata and colors into Udon-safe primitive arrays.
- Added timeline GATE/PAL badges, starter palettes, and one-click rhythm/palette authoring tools.
- Extended portable JSON, schema migration, deterministic signatures, compiler consistency checks, runtime-player baking, platform analysis, release reports, and self-tests.
- Expanded the Neon Drop demo with real Euclidean, syncopated, stepped-palette, and macro-morph programming.
- Updated production-rig, demo-scene, platform-simulator, and preset-library version labels.
- Preserved all 1.2 behavior and PCVR, Quest, iOS, and Android policies.

## 1.2.0

- Added schema-v5 deterministic procedural modulation.
- Added Sine, Triangle, Saw Up, Saw Down, Square, Pulse, and seeded
  Sample-and-Hold waveforms.
- Added seconds-, beat-, and bar-based modulation clocks with phase, duty cycle,
  quantization, offset, depth, and Replace/Add/Multiply application.
- Added four show-defined performance macro buses with names, colors, defaults,
  smoothing, and per-cue vector ranges.
- Added compact synchronized macro state reconstructed from VRChat server time,
  including deterministic smoothing while a show is playing, paused, or stopped.
- Added desktop/VR performance-macro controls.
- Added ordered scene stacks with eight banks, quantized triggering, safe dip
  transitions, and optional selection advance.
- Extended the visual timeline with modulation/macro labels, scene markers, and
  procedural preset buttons.
- Added Beat Pulse, 8-Bar Breathe, and deterministic Flicker authoring presets.
- Extended the compiler signature, Udon-safe baker, portable JSON, migration,
  compatibility validator, release report, runtime self-test, production rig,
  and Neon Drop demo.
- Fixed the Neon Drop generator applying automation before its tracks existed.
- Preserved schema-v1 through schema-v4 import, all 1.1 functionality, legacy
  cue sequences, DMX, AudioLink, VRSL conversion, PCVR, Quest, iOS, and Android.

## 1.1.0

- Added schema-v4 cue automation with Step, Linear, and Smooth interpolation.
- Flattened automation into allocation-free primitive runtime arrays.
- Added variable-tempo runtime conversion and time-signature-aware quantization.
- Added marker-based live hot cues with Beat, Half-Bar, Bar, Two-Bar, Four-Bar,
  and Immediate scheduling.
- Added server-time synchronized dip transitions for deterministic hot-cue jumps.
- Added a VR/desktop hot-cue bank controller.
- Added fixture and fixture-group capability masks with deterministic best-effort,
  disabled, and emissive approximation policies.
- Added per-fixture capability gating for movement, optics, strobes, lasers,
  audio modulation, color, and intensity.
- Reworked adaptive quality around smoothed frame time, hysteresis, cooldowns,
  quality floors/ceilings, and device-pressure diagnostics.
- Added one-click Pulse, Riser, and Four-Beat Gate automation authoring.
- Added a PCVR/Quest/iOS/Android platform simulation window.
- Extended the compiler signature, JSON migration, cross-platform validator,
  release report, demo show, production rig, and executable self-test.

## 1.0.0

- Added server-time synchronized timeline playback and show banks.
- Added synchronized operator master intensity alongside playback and safety state.
- Added late-join reconstruction, owner recovery, drift diagnostics, and
  content-signature mismatch blackout.
- Added synchronized live group overrides, mute/solo, recording, and editor
  conversion to timeline cues.
- Added circular runtime fixture-state snapshots.
- Added gobo, rotation, prism, zoom/focus, AudioLink-modulated intensity, and
  routed world-event cues.
- Added shader-side AudioLink modulation with manual fallback bands.
- Added waveform-energy and transient assisted starter-show generation.
- Added SHA-256 integrity to portable shows and schema-v3 migration.
- Added production runtime-rig generation and programmed show-bank assets.
- Added local rapid-color comfort smoothing.
- Added adaptive cue and evaluation budgets tied to local quality.
- Baked and enforced per-platform fixture and transparent-beam ceilings, with
  runtime-published AudioLink cadence and shader-quality policies.
- Added group-free global tracks and owner-authoritative routed network events.
- Added a one-click playable demo-scene builder with mobile-safe fixture
  visualizers and the complete production rig.
- Added 1.0 release-readiness reports and expanded executable self-tests.
- Expanded the Neon Drop demo with optics, bass modulation, safe strobes, and
  world events.
- Preserved legacy cue sequences, DMX, AudioLink, VRSL conversion, receivers,
  fixture profiles, accessibility, safety, and stress tools.
