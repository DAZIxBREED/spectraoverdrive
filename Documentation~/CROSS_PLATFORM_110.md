# SpectraOverdrive 1.1 Cross-Platform Policy

PCVR, Quest, iOS, and Android use one compiled show signature and one musical
timeline. Platform choice changes cost, not intent.

## Shared guarantees

- identical server-time show position
- identical marker and hot-cue targets
- identical automation key data
- identical deterministic random seeds
- identical emergency and operator safety state
- local-only accessibility and quality decisions

## Platform behavior

PCVR keeps full movement and optics within its ceiling. Quest uses simplified
movement and reduced transparent beams. iOS and Android default to
emissive-only fallbacks for costly optical cues, with independent fixture,
beam, shader, audio-update, and cue ceilings.

Quest detection uses Android plus the local VR state. A non-VR Android client
selects the Android phone/tablet policy. iOS uses `UNITY_IOS`. PCVR and desktop
use the PC policy.

## Adaptive pressure

The allocator measures an exponentially smoothed frame time. It requires
multiple bad samples before lowering quality, more good samples before raising
quality, and a cooldown between changes. Quality floors and ceilings prevent a
creator or user policy from being crossed.

The following may be reduced locally:

- cue concurrency
- fixture evaluation count
- transparent beam count
- update cadence
- shader quality tier

Show time, network revision, hot-cue timing, safety state, and cue identity are
never reduced or resampled.
