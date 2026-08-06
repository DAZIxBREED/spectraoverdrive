# SpectraOverdrive 1.4.0 Source Audit

This audit describes the static checks performed on the packaged 1.4.0 source tree before release packaging.

## Package totals

- 109 C# source files
- 16,008 lines of C# source
- 14 shader, HLSL, or include files
- 167 declared C# types
- 161 public fields in `SpectraCompiledShow`
- 119 flattened compiled arrays
- 81 cue-parallel compiled arrays

## Integration checks

- Every flattened array declared by `SpectraCompiledShow` is represented by the runtime player.
- All 119 compiled arrays are copied by `SpectraShowCompiler.ApplyToRuntimePlayer`.
- All 81 cue-parallel arrays are allocated to the resolved cue count before compilation.
- Condition, variation, and macro-snapshot arrays participate in the compiled content signature.
- The macro snapshot controller exposes direct select and recall entry points for snapshot indices 0 through 15.
- Schema-v7 JSON migration retains the historical v3-v6 migration chain and initializes 1.4 behavior as disabled unless authored.

## Structural checks

- Comment- and string-aware delimiter scan passed for every C# file.
- No duplicate declared C# type names were found.
- No `TODO`, `FIXME`, `HACK`, `NotImplementedException`, or known placeholder implementation marker was found in executable source or shaders.
- All package-local shader include paths resolve.
- `package.json` parses successfully and reports version `1.4.0` with Unity `2022.3`.
- No source file or directory from the 1.3.0 baseline was removed.

## 1.4 self-test coverage

The Unity editor self-test now covers:

- schema-v7 creation and v6-to-v7 migration defaults
- JSON export/import round-trip
- condition, variation, and snapshot compiler arrays
- deterministic A/B variation selection
- every-N-cycle condition selection
- synchronized four-macro snapshot recall
- updated release-readiness counts

## Limits of this audit

This is a source and archive integrity audit, not a substitute for Unity compilation. The package still requires validation in Unity 2022.3 with the supported VRChat SDK and UdonSharp versions, followed by physical PCVR, Quest, Android, and iOS device testing using the included verification and device-matrix documents.
