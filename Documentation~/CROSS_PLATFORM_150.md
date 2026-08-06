# SpectraOverdrive 1.5 Cross-Platform Policy

Cue-layer and arbitration decisions are deterministic show decisions. Visual
quality remains a local platform decision.

## Shared across clients

- show clock and playback state
- cue layer enabled and solo masks
- cue conditions and variation choices
- arbitration winners
- macro targets and snapshots
- scene, hot-cue, loop, safety, and blackout state

## Local platform filtering

Each cue layer may be independently permitted on PC, Quest, iOS, and Android.
A layer disabled for a platform is rejected before arbitration and cue-budget
admission. This lets one show carry richer PC layers without allocating them on
mobile devices.

Platform filtering must not be used to hide safety cues. Safety and blackout
layers should normally be permitted on every target platform and given an
appropriate priority bias.

## Budget behavior

Per-layer ceilings are enforced before the global platform ceiling. A layer cap
of zero means the layer uses only the global ceiling. Positive caps are useful
for optional atmosphere, accent, and optics layers that must never crowd out
base intensity or safety programming.

The validator reports:

- layer bindings
- arbitrated cues
- cues disabled by layer platform policy
- sub-80 ms deterministic-cycle clocks on mobile targets
- peak remaining cue concurrency
