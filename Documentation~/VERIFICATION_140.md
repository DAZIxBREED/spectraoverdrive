# SpectraOverdrive 1.4 Verification

## Unity checks

1. Import under Unity 2022.3 LTS with VRChat Worlds SDK and UdonSharp.
2. Confirm zero C# and UdonSharp compiler errors.
3. Run **Run Runtime Self-Test**.
4. Create the Neon Drop demo and validate it.
5. Generate a production runtime rig and verify all object references.
6. Export/import a show and compare the compiled content signature.
7. Run the platform simulator and release-readiness validator.

## Schema-v7 checks

- schema-v6 assets migrate with condition/variation disabled
- portable schema-v7 JSON round trips with an integrity hash
- condition and variation arrays match cue count
- snapshot arrays share one count and clamp values
- all new arrays copy from compiled show to runtime player
- content signatures change when any condition, variation, or snapshot changes

## Runtime checks

- Cycle variation alternates options from absolute show time
- two overlapping cues in one variation option activate together
- Every-N and Probability conditions remain deterministic after seek
- late joiners reconstruct variation and macro snapshot state
- manually changing one macro clears the active snapshot index
- non-selected cues do not consume active-cue budget
- emergency blackout overrides all generated choices

## Verification boundary

Static package checks cannot replace Unity compilation, UdonSharp compilation,
VRChat client tests, or physical device testing. Complete the device matrix
before production use.
