# SpectraOverdrive Show Programmer 0.9.0

## What is real in this release

The timeline window edits the same `SpectraShowAsset` that the compiler consumes.
It is not a disconnected mock-up. Every edit goes through serialized show data,
Unity Undo, stable-ID repair, validation, and deterministic compilation.

The visual editor implements:

- scrollable and zoomable time/beat grid
- variable-tempo beat conversion
- frame, seconds, beats, bars, and phrase-scale snapping
- Select, Draw, and Razor tools
- cue move, end-resize, split, duplicate, copy, paste, and delete
- track add, mute, lock, reorder, and delete
- waveform generation from real audio samples
- draggable structure markers
- movable and resizable loop regions
- tap-tempo and first-downbeat alignment
- scene fixture preview through `SpectraShowRuntimePlayer`
- cue, movement, palette, and section preset application
- platform-budget status beside selected cues

## Runtime/compiler boundary

Authoring data may contain names, notes, display colors, an editor-only audio
reference, and preset references. The compiler emits:

- stable group lookup tables
- sorted cue arrays
- easing and blending metadata
- Quest/iOS/Android fallbacks
- movement pattern IDs and packed parameters
- deterministic seeds
- marker lookup arrays
- loop lookup arrays
- platform update and concurrency budgets

The editor waveform audio is intentionally absent from portable JSON and
compiled shows.

Use **Bake Show Into Runtime Player** for the final scene bridge. It copies the
compiled primitive fields and arrays directly onto the UdonSharp behaviour and
can rebuild the player group array by matching scene `groupId` values. The
runtime behaviour never holds the editable asset or a custom C# object graph.

## Movement patterns

The fixture-group evaluator includes:

- horizontal and vertical sweeps
- circle and figure eight
- fan, reverse fan, and center-out fan
- wave and alternating wave
- bounce, spiral, and cross
- audience and stage sweeps
- DJ focus
- mirrored motion
- follow-the-leader
- chase
- deterministic seeded random

Pattern phase is spread over fixture order. Seeded random uses show data rather
than frame-local randomness, so all clients evaluate the same structural
motion.

## Presets

Run **Generate Built-In Preset Library** to create editable Unity assets under
`Assets/SpectraOverdriveGenerated/Presets`. Re-running the command updates the
built-ins without creating duplicate files.

The included generators create:

- 8 cue templates
- 10 movement presets
- 8 color palettes
- 2 section templates

Preset application clones the cue data and assigns a new stable ID. Editing an
inserted cue never mutates its source preset.

## Demo

Run **Create Neon Drop Demo** for a complete 174 BPM, 80-bar show with fixture
groups, colors, movement, PAR intensity, lasers, blinders, blackouts, markers,
loops, mobile fallbacks, accessibility metadata, and platform budgets.
