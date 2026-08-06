# Upgrade 1.3.0 to 1.4.0

## Before upgrading

Back up the Unity project and commit or archive the 1.3 package. Existing
schema-v6 show assets and `.spectrashow.json` files are supported.

## Upgrade steps

1. Replace the old package with SpectraOverdrive 1.4.0.
2. Open the project in Unity 2022.3 LTS and allow scripts to compile.
3. Select each `SpectraShowAsset`; `EnsureStableIds`/`OnValidate` upgrades it to
   schema v7.
4. Save the upgraded asset.
5. Re-export portable shows if you need a schema-v7 integrity signature.
6. Re-bake every runtime show player or regenerate the production rig.
7. Run the runtime self-test and release-readiness check.
8. Test PCVR, Quest, iOS, and Android using the device matrix.

## Migration behavior

Schema-v6 cues are migrated with conditions and variation routing disabled.
No existing cue is probabilistically hidden or placed into a variation group.
Existing performance macros, rhythm gates, dynamic palettes, scene stacks, and
network state retain their prior behavior.

`performanceMacroSnapshots` starts as an empty array. Add snapshots manually or
use **Create 1.4 Starter Palettes and Snapshots** in the Show Programmer.

## Network compatibility

Do not mix baked 1.3/schema-v6 and 1.4/schema-v7 runtime players for the same
show instance. Content signatures include all condition, variation, and macro
snapshot fields, so mismatched clients correctly enter show-mismatch safety.

## Audio conditions

Audio Above/Below reads CPU-side values exposed by `SpectraAudioLinkAdapter`.
Populate its manual/published band values through your world integration.
AudioLink texture data used only by shaders is not read back to the CPU.
