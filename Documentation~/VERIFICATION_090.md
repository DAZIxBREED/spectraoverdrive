# SpectraOverdrive 0.9.0 Verification

## Static package checks

Before packaging, the source tree is checked for:

- valid `package.json`
- balanced source delimiters
- C# parser errors across every `.cs` file
- incomplete compiled-array assignments
- runtime array reads without length guards
- `TODO`, `FIXME`, `NotImplementedException`, and placeholder bodies
- archive CRC/integrity errors

These checks detect packaging and source-structure failures. They are not a
substitute for Unity's C# compiler or the UdonSharp compiler.

## Unity executable self-test

Run:

**SpectraOverdrive > Show Programmer > Run Runtime Self-Test**

The test throws immediately on failure and covers:

1. fixed and variable-tempo beat conversion
2. beat-grid snapping
3. cue duplication, splitting, and clipboard round-trip
4. waveform sample reduction
5. independent cue-template instantiation
6. schema-v2 JSON round-trip
7. platform-budget analysis
8. deterministic compilation
9. flat-field baking onto the Udon runtime player
10. marker and loop compilation
11. runtime loop-time mapping
12. fixture-group intensity output
13. emergency blackout

## Required project verification

1. Import into the target Unity 2022.3 project with VRChat Worlds and
   UdonSharp installed.
2. Allow normal C# and UdonSharp compilation to finish with zero errors.
3. Run the self-test.
4. Generate the Neon Drop demo and preset library.
5. Bake the demo into a runtime player.
6. Test Windows/PCVR.
7. Switch to Android and test both Quest VR and non-VR Android behavior.
8. Switch to iOS and test a physical device for thermal stability.
9. Join with a second client and verify deterministic visual structure.
10. Exercise local accessibility and emergency blackout during playback.
