# SpectraOverdrive 1.3 Source Audit

Audit target: the exact source tree packaged as `SpectraOverdrive-1.3.0`.

## Results

- 107 C# files
- approximately 14,900 C# lines
- 14 shader/include files
- 136 compiled public runtime fields
- 95 compiled runtime array fields
- every compiled field maps to the flat runtime player
- every compiled array is initialized by a compiler path
- every new rhythm/palette content field participates in the deterministic signature
- all new cue arrays participate in compiled and runtime consistency checks
- lexical delimiter scan passes for all C#, ShaderLab, CGINC, and HLSL files
- no duplicate compiler-to-player assignments
- no unresolved local shader includes
- no `TODO`, `FIXME`, `HACK`, `NotImplementedException`, placeholder, or known stub marker
- package metadata reports version `1.3.0`
- editable/portable show schema reports version `6`

## Runtime boundary

Rhythm gating and palette playback perform scalar, integer, vector, and color
math only for relevant cues. Palette data is one contiguous `Color[]` with
integer offsets and counts. Seeded choices use the existing deterministic hash.
No runtime list construction, JSON parsing, reflection, LINQ, polymorphic graph,
or ScriptableObject lookup is required.

## External boundary

This is compile-oriented static verification, not a substitute for Unity,
UdonSharp, VRChat client, or physical PCVR/Quest/iOS/Android execution. Complete
the included editor self-test and device matrix in the destination project.
