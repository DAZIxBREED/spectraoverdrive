# Upgrade from SpectraOverdrive 0.9.0 to 1.0.0

## Show assets

Opening a schema-v2 show upgrades it to schema v3. Existing cue timing,
fallbacks, stable IDs, and platform policies remain intact. New cue fields
receive conservative defaults:

- zoom/focus: `0.5`
- audio amount: `0.5`
- event-once: enabled
- platform snapshot/fixture/optical budgets: platform defaults

Portable schema-v1 JSON migrates through v2 and then v3.

## Runtime players

Rebake every 0.9 runtime player. Schema-v3 adds content signatures and flat
audio/event arrays; old baked players intentionally fail the 1.0 consistency
check rather than evaluating partially.

Use **Create Production Runtime Rig** for the recommended wiring. Existing
custom rigs can instead add and connect:

- `SpectraShowNetworkController`
- `SpectraLiveOverrideLayer`
- `SpectraLiveOverrideRecorder`
- `SpectraShowSnapshotCache`
- `SpectraShowEventRouter`

## Portable sharing

1.0 exports include a SHA-256 integrity field. Existing unsigned files still
import and migrate. Signed files whose content no longer matches the hash are
rejected.

## Legacy systems

`SpectraCueSequence`, the original `SpectraShowBank`, legacy operator methods,
DMX, VRSL conversion, receivers, fixture profiles, and shaders remain
available. The new `SpectraProgrammedShowBank` is the preferred timeline-show
bank.
