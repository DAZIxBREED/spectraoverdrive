# SpectraOverdrive Show Programmer 0.8.0

> Historical 0.8.0 milestone notes. See `SHOW_PROGRAMMER_090.md` for the
> implemented visual timeline and current runtime contract.

This release contains executable SP1 show data and a minimum runtime playback
bridge. It is not placeholder scaffolding.

## Implemented

- Stable show, group, track, cue, marker, and loop identifiers
- Fixed and variable-tempo beat-grid conversion in both directions
- Second-based and musical cue timing
- Fixture groups, timeline tracks, cue blocks, markers, and loop-region data
- Cue validation with fatal errors and non-fatal warnings
- Deterministic compilation into flat arrays suitable for constrained runtimes
- Intensity, color, movement, and blackout evaluation against real
  `SpectraFixtureGroup` instances
- Replace, add, multiply, maximum, and minimum blending
- Play, pause, stop, and seek
- Emergency blackout as an independent highest-priority runtime layer
- Portable `.spectrashow.json` import and export
- Executable Unity editor self-test

## Proving the code works

1. Import the package into Unity 2022.3.22f1 with VRChat Worlds and UdonSharp.
2. Allow scripts to compile.
3. Run **SpectraOverdrive > Show Programmer > Run Runtime Self-Test**.
4. A passing run tests beat conversion, a BPM change, validation, JSON
   round-trip, deterministic compilation, fixture-group evaluation, and
   emergency blackout. Any failed assertion throws with the exact failed
   subsystem instead of reporting a false success.

## Authoring

Create a show with **Assets > Create > SpectraOverdrive > Show Asset**. Select
the show and use the Show Programmer menu to validate/export or compile it.
The visual drag-and-drop timeline remains SP4; show data is editable through the
Unity inspector in this SP1 release.
