# SpectraOverdrive 0.6 Abilities

## Operator console

`SpectraOperatorConsole` provides:

- console ownership
- operator allow-list
- cue triggering
- sequence start and stop
- synchronized blackout
- emergency clear
- active operator display

## Per-fixture overrides

`SpectraFixtureOverride` can temporarily override:

- color
- intensity
- pan
- tilt
- movement scale
- mute state

## Laser safety

`SpectraLaserSafetyController` provides local controls for:

- complete laser disable
- audience-scan policy
- local intensity ceiling
- scan-speed ceiling
- minimum vertical bias

## Fixture profile exchange

Fixture profiles can be exported to and imported from JSON through the SpectraOverdrive editor menu.

## Pooling

`SpectraFixturePool` reuses inactive effect and fixture objects instead of continuously creating and destroying them.

## Stress generation

The Stress Scene Generator can create large fixture grids to benchmark:

- moving heads
- moving washes
- PARs
- lasers

## Platform stripping

`SpectraPlatformMarker` allows objects to be included or removed for:

- PC
- Quest
- iOS
