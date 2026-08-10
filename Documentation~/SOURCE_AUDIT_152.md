# SpectraOverdrive 1.5.2 Source Audit

This audit records the static package checks for the 1.5.2 schema-v8 maintenance release.

## Package totals

- 111 C# source files
- 17,519 lines of C# source
- 14 shader, HLSL, or include files
- 171 declared C# types
- 135 flattened compiled arrays

## 1.5.2 integration checks

- Synchronized loop requests are normalized against the active compiled player before serialization.
- Authoritative loop state is applied through `SpectraShowRuntimePlayer.SetLoop` rather than raw index assignment.
- Malformed non-negative loop indices are locally rejected and counted.
- Redundant loop, blackout, strobe, laser, master-intensity, and cue-layer writes are suppressed before revision/serialization work.
- Cue-layer reset comparisons use the same compiled default-mask calculation used by the runtime reset itself.
- The self-test contains regression assertions for valid loops, invalid requested loops, invalid deserialized loops, and redundant operator writes.

## Structural checks

- `package.json`, README, portable-show metadata, release-readiness version, and validator target all report 1.5.2.
- All JSON and assembly-definition files parse successfully.
- Comment-, string-, and character-aware delimiter validation passes for all C# sources.
- No duplicate declared C# types are present.
- All package-local shader include paths resolve.
- Every flattened compiled-show array remains represented by the runtime player and compiler bake path.
- No executable `TODO`, `FIXME`, `HACK`, or `NotImplementedException` marker is present.

## Validation boundary

The repository audit is static validation. Unity 2022.3 compilation, UdonSharp compilation, and physical PCVR, Quest, iOS, and Android tests remain required before production deployment.
