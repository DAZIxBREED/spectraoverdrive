# SpectraOverdrive 1.2 Source Audit

Audit target: the exact source tree packaged as `SpectraOverdrive-1.2.0`.

## Results

- 106 C# files
- 14 shader/include files
- 112 compiled runtime fields
- 71 compiled runtime array fields
- every compiled field maps to the flat runtime player
- every compiled array is initialized by the compiler
- every compiled content field participates in the deterministic signature
- all 41 cue-array fields participate in runtime consistency checks
- lexical delimiter scan passes for all C#, shader, CGINC, and HLSL files
- no duplicate compiler-to-player assignments
- no `TODO`, `FIXME`, `NotImplementedException`, placeholder, or stub marker
- package metadata reports version `1.2.0`
- editable/portable show schema reports version `5`

## Runtime boundary

The procedural evaluator performs scalar/vector math only for active cues.
Sample-and-Hold uses an integer seed/cycle hash. Macro networking serializes
start values, targets, one server timestamp, and one duration; it does not send
per-frame values. Scene stacks reuse the existing hot-cue network state.

## External boundary

This audit is compile-oriented static verification. Unity, UdonSharp, VRChat
client execution, and physical PCVR/Quest/iOS/Android builds must be run in the
destination Unity project.
