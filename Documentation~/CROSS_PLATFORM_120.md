# SpectraOverdrive 1.2 Cross-Platform Policy

## Shared behavior

PCVR, Quest, iOS, and Android use the same:

- show time and variable-tempo map
- cue ordering, priority, fade, and blend rules
- automation keys
- procedural waveform, seed, phase, and cycle
- performance-macro target and server timestamp
- scene and hot-cue target
- loop and playback state
- safety and emergency blackout state

Platform policies may simplify presentation. They may not change musical timing
or synchronized control state.

## PCVR

PCVR retains the full shader tier, optical cues, higher fixture and transparent
beam ceilings, 60 Hz default evaluation, and full platform fallbacks.

## Quest

Quest uses a 36 Hz default evaluator, capped fixture/beam counts, simplified
movement fallbacks, shared mobile materials, capability fallbacks, and adaptive
quality. Procedural modulation runs on the CPU as a few scalar operations per
active cue and does not introduce shader variants.

## iOS

iOS uses a 30 Hz emissive-first path, bounded transparent beams, simplified
optics, thermal scaling, and local quality reduction. Fast procedural clocks
remain deterministic but are intentionally sampled at the platform update rate.

## Android phone/tablet

Android uses a policy distinct from Quest: 30 Hz evaluation, a lower default
transparent-beam ceiling, emissive-first fallback, thermal scaling, and
phone/tablet quality limits.

## Compatibility guarantees

- schema-v1 through schema-v4 imports migrate to schema v5
- existing cue IDs, group IDs, timing, fallbacks, and capability policies survive
- 1.1 players must be re-baked because schema-v5 adds flat arrays
- old shows without macros receive no macro bindings
- old shows without modulation use the Disabled waveform
- old markers do not become scenes automatically
- emergency blackout remains above macros, modulation, automation, overrides,
  AudioLink, scene jumps, and platform fallbacks
