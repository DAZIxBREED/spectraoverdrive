# SpectraOverdrive 1.1 Device Test Matrix

Record Unity version, SDK version, package version, device model, OS version,
world build ID, fixture count, and show signature for every row.

| Test | PCVR | Quest | iOS | Android |
| --- | --- | --- | --- | --- |
| World loads without shader errors | ☐ | ☐ | ☐ | ☐ |
| Correct local platform selected | ☐ | ☐ | ☐ | ☐ |
| Signed show matches owner | ☐ | ☐ | ☐ | ☐ |
| Play/pause/seek/loop synchronized | ☐ | ☐ | ☐ | ☐ |
| Late join reconstructs current state | ☐ | ☐ | ☐ | ☐ |
| Ownership transfer preserves time | ☐ | ☐ | ☐ | ☐ |
| Variable-tempo beat position matches | ☐ | ☐ | ☐ | ☐ |
| Automation matches reference values | ☐ | ☐ | ☐ | ☐ |
| Beat-quantized hot cue executes together | ☐ | ☐ | ☐ | ☐ |
| Hot-cue dip transition is synchronized | ☐ | ☐ | ☐ | ☐ |
| Capability fallbacks match report | ☐ | ☐ | ☐ | ☐ |
| Emergency blackout overrides all layers | ☐ | ☐ | ☐ | ☐ |
| Local strobe/laser disable remains local | ☐ | ☐ | ☐ | ☐ |
| Reduced motion and color comfort work | ☐ | ☐ | ☐ | ☐ |
| Adaptive quality changes without time drift | ☐ | ☐ | ☐ | ☐ |
| No sustained budget overflow | ☐ | ☐ | ☐ | ☐ |

Test hot cues with the owner and at least one late joiner. Test a capability
fallback on a fixture that genuinely lacks the requested feature. On iOS and
Android, run long enough to expose sustained device pressure rather than only
checking the first minute.
