# Upgrade from SpectraOverdrive 1.0.0 to 1.1.0

1. Back up the Unity project.
2. Replace the 1.0 package with 1.1.
3. Open each `SpectraShowAsset`; schema 3 migrates to schema 4.
4. Review fixture-group capability masks. Migrated groups default to All so
   existing behavior is preserved.
5. Re-bake every runtime player. Old baked players intentionally fail the 1.1
   parallel-array consistency check.
6. Re-export portable shows. Legacy schema-3 shows are validated and re-signed
   using the schema-4 serializer.
7. Rebuild the production rig if hot-cue bank controls are wanted.
8. Run the runtime self-test and 1.1 release-readiness check.
9. Run the platform simulator.
10. Repeat physical PCVR, Quest, iOS, and Android tests.

Existing cues default to automation Disabled and capability fallback Emissive
Approximation. Existing markers are not hot cues until explicitly enabled.
Playback, safety, override, snapshot, network, and JSON show identities remain
stable after migration, but compiled content signatures change because 1.1
includes capabilities, automation, tempo maps, and hot-cue metadata.
