# Upgrade from 1.1 to 1.2

1. Back up the Unity project.
2. Replace the 1.1 package with 1.2.
3. Allow Unity to recompile.
4. Select each `SpectraShowAsset`; validation upgrades schema 4 to schema 5.
5. Run **Run Runtime Self-Test**.
6. Run **Run 1.2 Release Readiness Check** on every show.
7. Re-bake every `SpectraShowRuntimePlayer`.
8. Recreate or manually add the performance-macro and scene-stack controllers
   to older production rigs.
9. Open **Platform Simulator** and validate all four policies.
10. Complete the 1.2 physical-device matrix.

No 1.1 cue is automatically assigned modulation, macros, or scene behavior.
Existing show behavior therefore remains unchanged until the creator opts in.

Portable schema-v4 files may log a legacy integrity warning because schema-v5
adds fields to the canonical serializer. They are migrated, validated, and
signed with a new schema-v5 hash when exported again.
