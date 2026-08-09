# SpectraOverdrive 1.5.1 Source Audit

This audit records the static package checks for the 1.5.1 schema-v8 maintenance release.

## Package totals

- 111 C# source files
- 17,430 lines of C# source after the live-controller refresh addition
- 14 shader, HLSL, or include files
- 171 declared C# types
- 135 flattened compiled arrays

## 1.5.1 integration checks

- `soloMask` is canonicalized against both the valid-layer mask and enabled mask.
- Network disable/toggle operations clear stale solo bits.
- Network solo operations atomically enable the selected layer.
- Pre-deserialization network state leaves compiled macro and cue-layer defaults intact until the first authoritative revision arrives.
- Arbitration groups reject runtime candidates that disagree with the prepared mode, time base, cycle length, phase, or seed.
- `arbitrationConfigurationMismatchCount` exposes malformed baked-data rejections.
- `SpectraCueLayerController` periodically refreshes remote/show-switch state at a bounded default cadence of four times per second.
- No schema or compiled-array layout change was introduced.

## Structural checks

`Tools/validate_package.py` verifies release-version consistency, JSON and assembly definitions, C# delimiters, unique types, local shader includes, compiled/runtime array parity, compiler initialization and bake mappings, and executable stub markers.

Unity/UdonSharp compilation and physical PCVR, Quest, iOS, and Android testing remain required before production deployment.
