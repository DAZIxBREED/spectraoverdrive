# SpectraOverdrive 0.5 Authoring

## Fixture profiles

Create profiles through:

`Assets > Create > SpectraOverdrive > Fixture Profile`

Profiles store:

- manufacturer
- model
- mode name
- fixture type
- channel count
- movement ranges
- beam angles
- gobo count
- prism facets
- channel-function assignments

Attach `SpectraFixtureProfileBinder` to a fixture to copy profile settings into its runtime components.

## Cue sequences

A `SpectraCueSequence` stores timed cue steps. Each step can select:

- cue ID
- duration
- fade
- chase pattern
- effect speed

`SpectraCueSequencePlayer` executes the sequence using VRChat server time.

## Receiver masks

`SpectraReceiverMask` controls how strongly a material accepts:

- beams
- projections
- lasers
- disco effects

The mobile receiver shader currently consumes the projection mask. Additional renderer-specific mask use will follow.

## VRSL mapping

`SpectraVrslPropertyMap` is the data foundation for explicit source-to-target property migration. It does not yet mutate scenes.
