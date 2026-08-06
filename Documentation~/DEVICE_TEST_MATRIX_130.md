# SpectraOverdrive 1.3 Device Test Matrix

Run every row on a development world containing the 1.3 Neon Drop demo and a
second show migrated directly from 1.2. Record hardware, OS, client version,
world build, frame rate, thermal state, and result.

| Test | PCVR | Quest | iOS | Android |
| --- | --- | --- | --- | --- |
| Import and compile schema-v6 show | ☐ | N/A | N/A | N/A |
| Migrate untouched schema-v5 show | ☐ | N/A | N/A | N/A |
| Re-baked player passes content signature | ☐ | ☐ | ☐ | ☐ |
| Pulse gate remains beat-aligned | ☐ | ☐ | ☐ | ☐ |
| Alternating gate survives seek backward | ☐ | ☐ | ☐ | ☐ |
| Euclidean 5/8 pattern repeats correctly | ☐ | ☐ | ☐ | ☐ |
| Seeded gate matches a second client | ☐ | ☐ | ☐ | ☐ |
| Custom mask matches authored 16-step mask | ☐ | ☐ | ☐ | ☐ |
| Gate attack/release has no local drift | ☐ | ☐ | ☐ | ☐ |
| Gated-off cues reduce active-cue count | ☐ | ☐ | ☐ | ☐ |
| Fixed palette resolves primary color | ☐ | ☐ | ☐ | ☐ |
| Step palette remains beat-aligned | ☐ | ☐ | ☐ | ☐ |
| Ping-Pong palette reverses endpoints cleanly | ☐ | ☐ | ☐ | ☐ |
| Seeded palette matches a second client | ☐ | ☐ | ☐ | ☐ |
| Macro Morph matches synchronized macro | ☐ | ☐ | ☐ | ☐ |
| Palette fallback color works when disabled | ☐ | ☐ | ☐ | ☐ |
| Pause/resume preserves gate and palette phase | ☐ | ☐ | ☐ | ☐ |
| Late join reconstructs current visual step | ☐ | ☐ | ☐ | ☐ |
| Ownership transfer preserves visual step | ☐ | ☐ | ☐ | ☐ |
| Variable-tempo boundary remains aligned | ☐ | ☐ | ☐ | ☐ |
| Hot cue lands on correct gated/palette phase | ☐ | ☐ | ☐ | ☐ |
| Rapid-color local comfort smoothing applies | ☐ | ☐ | ☐ | ☐ |
| Emergency blackout overrides all cues | ☐ | ☐ | ☐ | ☐ |
| 20-minute thermal/quality soak | ☐ | ☐ | ☐ | ☐ |

## Acceptance

A target passes only when deterministic selections match the PC reference at the
same show time, safety controls remain authoritative, no content-signature
mismatch occurs, and no sustained thermal or frame-time failure exceeds the
project's target policy.
