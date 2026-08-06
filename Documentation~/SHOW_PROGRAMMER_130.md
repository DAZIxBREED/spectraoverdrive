# SpectraOverdrive 1.3 Show Programmer

## Rhythm gates

Every cue can carry an optional deterministic rhythm gate. The gate multiplies
the cue's normal fade and easing weight before the cue is applied.

### Patterns

- **Pulse** — every step is available, with the duty control determining its on-time.
- **Alternating** — even-numbered steps are active.
- **Euclidean** — distributes `gateActiveSteps` over `gateStepCount`.
- **Seeded Random** — selects steps deterministically from the cue seed.
- **Custom Mask** — reads up to 32 steps from `gateCustomMask`.

### Time bases

- **Seconds** uses elapsed cue seconds.
- **Beats** uses the variable-tempo runtime beat map.
- **Bars** divides elapsed beats by the active time-signature numerator.

`gateStepLength` is measured in the selected time base. `gatePhase` is measured
in steps. Attack and release are normalized portions of the active duty window.
All calculations are stateless and repeatable after seeking, pausing, late join,
or ownership transfer.

### Quick tools

Select a cue in the timeline and use:

- **Euclid 5/8**
- **Syncopate**
- **Seeded**
- **Clear Gate**

The complete cue inspector remains available for custom values.

## Dynamic palettes

A show can contain up to 16 `SpectraColorPalette` entries with one through 16
colors each. Palette descriptions remain editor metadata; names, offsets,
counts, and colors are flattened into the runtime player.

### Playback modes

- **Fixed** uses `palettePrimaryIndex`.
- **Step** advances through the palette.
- **PingPong** traverses forward and backward without duplicating endpoints.
- **SeededRandom** chooses a deterministic color for each step.
- **MacroMorph** blends primary and secondary colors from a synchronized macro.

Step-based modes use `paletteTimeBase`, `paletteStepLength`, and `palettePhase`.
`paletteBlend` mixes the resolved palette color with the cue's authored fallback
color. This allows a palette binding to be reduced or disabled without losing
the original cue color.

### Quick tools

The show inspector can create the built-in starter palettes. With a Color cue
selected, use:

- **Palette Step**
- **Macro Morph**
- **Clear Palette**

Macro Morph requires an existing performance macro. Palette bindings are valid
only on Color cues.

## Runtime and synchronization behavior

Rhythm gates and palettes are compiled into primitive arrays. No palette object,
list, curve, JSON parser, or editor class reaches Udon playback. The runtime
selection depends on show time, tempo map, cue seed, and synchronized macro
values, so visual results remain deterministic across different frame rates.

Gated-off cues are excluded from active-cue selection. This means rhythmic
programming can reduce work on mobile targets instead of merely multiplying an
already-selected cue by zero.
