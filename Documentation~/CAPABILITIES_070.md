# SpectraOverdrive 0.7 Capabilities

## Show banks

A `SpectraShowBank` stores complete named show entries:

- cue sequence
- house color
- master intensity
- laser policy
- strobe policy
- operator notes

`SpectraShowBankPlayer` selects, starts, stops, and synchronizes show entries.

## Operator UI

`SpectraOperatorUiController` connects standard Unity UI and TextMeshPro controls to:

- show selection
- play
- stop
- blackout
- laser enable
- master intensity
- active operator state

## Ownership recovery

`SpectraOwnershipRecovery` allows the current instance master to reclaim abandoned synchronized objects after an operator leaves.

## Multi-zone receiver blending

`SpectraMultiZoneReceiver` blends four receiver zones into one material. This supports rooms, walls, stages, and props spanning multiple lighting areas.

## Audio coordinate profiles

`SpectraAudioCoordinateProfile` makes AudioLink texture coordinates configurable instead of baking guessed coordinates into shaders.

## Assisted VRSL conversion

The assisted converter:

- operates only on selected objects
- can duplicate objects first
- retains original components
- adds SpectraOverdrive runtime components
- searches likely universe/address fields
- writes compatibility markers and notes

It is intentionally non-destructive.

## PC renderer

`SpectraOverdrive/PC/MovingHeadBeam` adds a higher-target shader with smoother transparency, volumetric-style depth fading, gobo support, zoom, movement, DMX color, and strobe.
