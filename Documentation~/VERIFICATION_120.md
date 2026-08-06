# SpectraOverdrive 1.2 Verification

## Automated editor self-test

The one-click test covers:

- beat-grid and variable-tempo conversion
- schema-v5 signed JSON round trip
- legacy migration path availability
- compiler array consistency and content signature
- keyframe automation flattening
- beat-based procedural pulse evaluation
- synchronized macro binding
- server-time macro interpolation
- ordered scene metadata
- hot-cue quantization
- capability fallbacks
- platform policies
- waveform analysis and assisted generation
- runtime optics, overrides, snapshots, loops, and blackout

## Static release audit

The release archive should not be produced unless:

- every C# source has balanced lexical delimiters
- every compiled runtime data field maps to the player
- every content-bearing field participates in the runtime signature
- all four platform policies exist
- package metadata reports `1.2.0`
- schema version is `5`
- no `TODO`, `FIXME`, `NotImplementedException`, or empty method body remains
- shader/include delimiters and local includes validate
- ZIP CRC validation passes

## Required external verification

This source environment cannot execute Unity, UdonSharp, VRChat clients, or
physical mobile hardware. Import into Unity `2022.3.22f1`, compile UdonSharp,
build each target, and complete `DEVICE_TEST_MATRIX_120.md`.
