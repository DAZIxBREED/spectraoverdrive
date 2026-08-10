# SpectraOverdrive 1.5.2 Patch Notes

SpectraOverdrive 1.5.2 is a schema-v8 maintenance release focused on network discipline and loop-state robustness.

## Synchronized loop hardening

Loop selections are now normalized against the active compiled show before serialization. Invalid selections resolve to `-1` (no loop). During authoritative reconstruction the runtime also applies the synchronized value through `SetLoop`, so malformed or stale remote values cannot become an active local loop. `invalidLoopSelectionCount` records rejected non-negative values.

## No-op network-write suppression

Operator calls that request a state already in effect no longer increment revisions or request another serialization for loop selection, blackout, strobe permission, laser permission, master intensity, cue-layer enable state, cue-layer solo clearing, or cue-layer reset. `suppressedNoOpNetworkWriteCount` exposes how many redundant writes were avoided.

## Cue-layer reset canonicalization

The runtime now exposes `GetDefaultCueLayerEnabledMask()`, and reset logic uses the same compiled default mask for both local state and network no-op comparison.

## Compatibility

Schema remains v8. Existing 1.5.0 and 1.5.1 show assets require no migration. Re-bake runtime players after package replacement so the updated code is present.
