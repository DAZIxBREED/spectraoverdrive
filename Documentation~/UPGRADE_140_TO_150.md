# Upgrade 1.4.0 to 1.5.0

SpectraOverdrive 1.5 upgrades editable and portable shows from schema v7 to
schema v8.

## Safe upgrade procedure

1. Back up the Unity project and existing `.spectrashow.json` exports.
2. Replace the 1.4 package with 1.5.
3. Open the project with Unity 2022.3 LTS and allow compilation to finish.
4. Open each `SpectraShowAsset` so `EnsureStableIds` can migrate it.
5. Save the migrated assets.
6. Recompile or re-bake every runtime player.
7. Recreate production rigs when you want the new cue-layer controller.
8. Run the runtime self-test and 1.5 release-readiness check.
9. Test all target devices.

## Migration behavior

Schema-v7 shows migrate with:

- `cueLayers` initialized to an empty array
- every cue `layerIndex` set to `-1`
- arbitration disabled
- arbitration group set to `-1`
- bars as the default arbitration time base
- cycle length `1`, phase `0`, and seed `0`

The migrated show therefore behaves like 1.4 until an author explicitly creates
layers or arbitration groups.

## Re-bake requirement

Do not use a schema-v8 editable show with a schema-v7 baked runtime player.
Layer metadata and arbitration fields are included in the deterministic content
signature. Re-baking keeps network mismatch protection meaningful.

## Operator rig changes

The 1.5 production-rig builder adds `SpectraCueLayerController` and connects it
to `SpectraShowNetworkController`. Existing rigs remain valid for shows without
layer controls, but they do not automatically gain layer UI access.
