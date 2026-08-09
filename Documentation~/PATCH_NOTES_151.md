# SpectraOverdrive 1.5.1 Patch Notes

SpectraOverdrive 1.5.1 is a schema-v8 stability release focused on live operator state, late-join reconstruction, and defensive runtime arbitration.

## Cue-layer state invariants

The runtime now guarantees that the solo mask is always a subset of the enabled mask. A layer cannot remain soloed after it is disabled. Soloing a disabled layer enables and solos it in one authoritative update, while disabling or toggling off a soloed layer clears the corresponding solo bit immediately.

This removes an operator failure mode where a stale solo bit could suppress every other layered cue.

## Late-join initialization

Before a client receives the first synchronized performance-macro or cue-layer revision, the baked player's own compiled defaults remain active. The network controller no longer overwrites those local defaults with its field initializers while waiting for deserialization.

After the first authoritative revision arrives, normal server-time reconstruction takes over.

## Arbitration hardening

Authored shows already validate that all cues in an arbitration group share the same mode, time base, cycle length, phase, and seed. 1.5.1 adds a runtime defense as well: malformed candidates that disagree with the group's prepared configuration are rejected rather than participating with mixed rules.

`SpectraShowRuntimePlayer.arbitrationConfigurationMismatchCount` reports how many active malformed candidates were rejected during the current evaluation.

## Migration

No schema migration is required. 1.5.0 and 1.5.1 both use schema v8. Re-bake production runtime players after replacing the package so the corrected runtime code and diagnostics are present.
