# Upgrade from 1.2 to 1.3

1. Back up the Unity project and existing `.spectrashow.json` exports.
2. Replace the 1.2 package with 1.3.
3. Allow Unity to reimport scripts and assets.
4. Select each `SpectraShowAsset`; validation upgrades schema 5 to schema 6.
5. Confirm existing cue colors and timing are unchanged.
6. Run **Run Runtime Self-Test**.
7. Run **Run 1.3 Release Readiness Check** on every production show.
8. Re-bake every `SpectraShowRuntimePlayer` or recreate the production rig.
9. Test seeking, pause/resume, late join, and ownership transfer.
10. Complete `DEVICE_TEST_MATRIX_130.md` on PCVR, Quest, iOS, and Android.

## Migration defaults

The migration does not opt existing cues into new behavior. It creates an empty
palette list, disables every rhythm gate, sets palette indices to `-1`, and
disables palette playback. Existing procedural modulation, macro bindings,
scenes, automation, fallbacks, and cue colors remain intact.

## Portable files

Schema-v5 portable files may log a legacy integrity warning because schema-v6
serialization contains additional fields. The imported show is migrated,
validated, and receives a new schema-v6 hash on export.
