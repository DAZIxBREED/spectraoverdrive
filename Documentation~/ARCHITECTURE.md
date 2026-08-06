# SpectraOverdrive Architecture 0.9

## Layer 1 — Show state

The synchronized truth:

- current source
- cue list
- cue
- cue start server time
- BPM
- deterministic seed
- global override
- blackout

## Layer 2 — Signal adapters

Future adapters:

- VRSL-compatible DMX video
- AudioLink
- internal cues
- manual operator UI
- Beowulf
- recorded shows

## Layer 3 — Logical fixtures

A logical fixture owns:

- identity
- DMX patch
- channel profile
- groups
- mount calibration
- receiver zones

It does not own platform quality.

## Layer 4 — Platform renderers

- PC renderer
- Quest renderer
- iOS renderer
- Android renderer

All receive the same logical fixture state.

## Layer 5 — Local comfort and quality

Local-only controls can reduce:

- beam intensity
- projection intensity
- lasers
- disco
- strobe behavior
- movement speed
- effect density

They never modify the synchronized show for other users.
