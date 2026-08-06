# Upgrade from SpectraOverdrive 0.8.0 to 0.9.0

## Existing Unity show assets

When a schema-v1 `SpectraShowAsset` is loaded, 0.9.0 upgrades it to schema v2:

- existing cues are enabled
- iOS fallback values seed the new Android fallback
- movement amplitude, spread, and direction receive safe defaults
- track display colors are repaired
- default PCVR, Quest, iOS, and Android policies are added when absent

Stable show, group, track, cue, marker, and loop IDs are preserved.

## Portable JSON

The importer accepts schema v1 and migrates it before validation. Exports use
schema v2 and omit the editor-only waveform `AudioClip`.

Keep a source-control or file backup before upgrading production assets. The
migration is non-destructive to fixture objects and VRSL components, but Unity
will serialize the upgraded show schema when the asset becomes dirty.

## Runtime player

Recompile shows so the new runtime arrays contain:

- easing
- per-mobile-platform fallback values
- movement pattern data
- markers
- loops
- platform budgets

Old compiled JSON does not contain these arrays and will fail the 0.9.0
consistency check rather than playing partially.
