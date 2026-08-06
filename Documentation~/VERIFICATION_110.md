# SpectraOverdrive 1.1 Verification

## Automated Unity checks

Run **SpectraOverdrive > Show Programmer > Run Runtime Self-Test**.

The test executes:

- schema-4 signed JSON round trip and corruption rejection
- editor and runtime variable-tempo conversion
- compiler consistency and deterministic signature
- capability mask compilation
- automation flattening and runtime interpolation
- hot-cue metadata and beat quantization
- PCVR, Quest, iOS, and Android policy compilation
- normal cue, gobo, zoom/focus, loop, and blackout evaluation
- live overrides and snapshot restoration
- offline network-state reconstruction
- waveform generation and assisted show generation
- release-readiness validation

## Release check

Run **Run 1.1 Release Readiness Check** for every shipped show. A ready report
requires all four explicit platform policies, valid cue arrays, valid automation
keys, valid markers and loops, successful compile, signed JSON round trip, and
platform cue concurrency within budget.

## Required external checks

The package cannot prove Unity compilation, UdonSharp translation, VRChat build,
shader compilation, networking, thermal behavior, or device GPU cost outside a
configured Unity project. Complete `DEVICE_TEST_MATRIX_110.md` before calling a
world release-ready.
