# SpectraOverdrive 1.4 Device Test Matrix

Record device, OS, Unity/SDK version, world build ID, result, frame time, and
notes for every row.

| Test | PCVR | Quest | iOS | Android |
| --- | --- | --- | --- | --- |
| Import and bake schema-v7 show | Required | N/A | N/A | N/A |
| Basic play/pause/seek | Required | Required | Required | Required |
| Late join during variation cycle | Required | Required | Required | Required |
| Ownership transfer during snapshot transition | Required | Required | Required | Required |
| Cycle and Ping-Pong variation agreement | Required | Required | Required | Required |
| Seeded Random variation agreement | Required | Required | Required | Required |
| Probability condition agreement after seek | Required | Required | Required | Required |
| Every-N condition agreement after loop | Required | Required | Required | Required |
| Macro Above/Below threshold | Required | Required | Required | Required |
| Audio condition bridge | Required | Required | Required | Required |
| Recall snapshots 0-15 from UI | Required | Required | Required | Required |
| Manual macro edit clears snapshot index | Required | Required | Required | Required |
| Cue budget excludes rejected choices | Required | Required | Required | Required |
| Rhythm gates and palettes unchanged | Required | Required | Required | Required |
| Local strobe/laser comfort settings | Required | Required | Required | Required |
| Emergency blackout | Required | Required | Required | Required |

## Stress cases

- 16 variation groups active at once
- eight options in one group
- 16 macro snapshots
- rapid seeks across condition boundaries
- looping across an Every-N boundary
- owner leaves during a two-second snapshot transition
- content-signature mismatch between baked players
