# SpectraOverdrive 1.1 Source Audit

Audit date: 2026-07-26

## Source state

- 103 C# files parsed with the C# tree-sitter grammar
- zero parse failures
- 10 shader files and 4 shader includes passed delimiter checks
- all local Spectra shader includes resolve
- C# preprocessor directives are balanced
- package and assembly-definition JSON parse successfully
- no empty method bodies
- no `TODO`, `FIXME`, `NotImplementedException`, placeholder, or intentional
  unsupported-operation bodies

## Compiler and runtime bake

- 91 compiled-show public data fields
- every field has a same-name flat field on `SpectraShowRuntimePlayer`
- every field has an explicit compiler-to-player bake assignment
- every content-bearing field participates in the deterministic signature
- no `SpectraCompiledShow` object graph is stored in the Udon behavior

## 1.1 integration paths checked

- schema-v4 JSON export/import and v3 migration
- cue automation offsets, counts, key arrays, and interpolation arrays
- group and cue capability arrays
- hot-cue marker metadata
- variable-tempo arrays
- PCVR, Quest, iOS, and Android arrays
- network hot-cue state and transition reconstruction
- release validator and self-test coverage

This is a source and packaging audit, not a substitute for Unity compilation,
UdonSharp translation, VRChat build validation, or physical-device testing.
