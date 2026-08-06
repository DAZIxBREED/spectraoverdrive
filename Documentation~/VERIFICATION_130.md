# SpectraOverdrive 1.3 Verification

## Included editor self-test

The one-click test now covers:

- schema-v6 signed JSON round trip
- schema-v4 through schema-v6 migration behavior
- flattened palette metadata and colors
- alternating rhythm-gate suppression
- synchronized stepped-palette runtime output
- compiler array consistency and content signature
- release-report rhythm/palette counts
- all prior automation, modulation, macro, scene, hot-cue, capability, waveform,
  optics, override, snapshot, network, platform, loop, and blackout coverage

## Static source audit performed for this package

- 107 C# files and approximately 14,900 C# lines inspected
- 14 shader/include files inspected
- lexical delimiters balanced across C#, ShaderLab, CGINC, and HLSL
- 136 compiled public fields map one-to-one into the runtime player
- 95 compiled array fields are initialized by compiler paths
- all new palette and rhythm arrays are copied, consistency-checked, and signed
- no duplicate compiler-to-player assignments
- no `TODO`, `FIXME`, `HACK`, `NotImplementedException`, or known stub marker
- no unresolved local shader includes
- package metadata reports `1.3.0`
- editable/portable show schema reports version `6`

## Required external verification

This source environment cannot execute Unity, UdonSharp, VRChat clients, or
physical mobile hardware. Import into Unity `2022.3.22f1`, allow all scripts and
UdonSharp programs to compile, run the included self-test, build each target, and
complete `DEVICE_TEST_MATRIX_130.md`.
