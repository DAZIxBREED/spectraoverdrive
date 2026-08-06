# SpectraOverdrive 1.5.0 Source Audit

This audit records the static checks performed on the 1.5.0 source tree before
release packaging and GitHub publication.

## Package totals

- 111 C# source files
- 17,314 lines of C# source
- 14 shader, HLSL, or include files
- 171 declared C# types
- 176 public fields in `SpectraCompiledShow`
- 135 flattened compiled arrays
- 97 cue-parallel compiled arrays

## Integration checks

- Every flattened array declared by `SpectraCompiledShow` is represented by
  `SpectraShowRuntimePlayer`.
- All 135 compiled arrays are copied by
  `SpectraShowCompiler.ApplyToRuntimePlayer`.
- All compiled arrays are initialized by the compiler before cue compilation.
- Layer metadata, cue-layer bindings, arbitration configuration, and existing
  schema-v7 systems all participate in the deterministic content signature.
- Layer filtering and arbitration execute before per-layer and global admission
  budgets.
- The synchronized network state uses compact enabled and solo integer masks.
- The production-rig builder creates and wires `SpectraCueLayerController`.
- Schema-v8 migration retains the complete historical migration chain and
  initializes layers and arbitration as disabled unless explicitly authored.

## Structural checks

- Comment-, string-, and character-aware delimiter scanning passed for every C#
  source file.
- No duplicate declared C# type names were found.
- No executable `TODO`, `FIXME`, `HACK`, or `NotImplementedException` marker was
  found.
- All package-local shader include paths resolve.
- All JSON and assembly-definition files parse successfully.
- `Tools/validate_package.py` contains the same reproducible repository audit
  used for the 1.5.0 release verification.
- `package.json` reports version `1.5.0` and Unity `2022.3`.
- No source file from the 1.4.0 baseline was removed.

## 1.5 self-test coverage

The Unity editor self-test now covers:

- schema-v8 creation and schema-v4 through schema-v8 migration defaults
- portable JSON export/import and content integrity
- complete layer and arbitration compiler arrays
- platform-specific layer filtering
- synchronized layer-mask disable and default restoration
- highest-priority arbitration with layer bias
- deterministic-cycle arbitration from the absolute show clock
- updated platform and release-readiness counts
- all earlier rhythm-gate, palette, condition, variation, macro, scene,
  automation, networking, capability, and safety checks

## Limits of this audit

This is a source and archive integrity audit, not a replacement for Unity and
UdonSharp compilation. Validate the package in Unity 2022.3 with the supported
VRChat SDK and UdonSharp versions, then complete the physical PCVR, Quest,
Android, and iOS device matrix before production deployment.
