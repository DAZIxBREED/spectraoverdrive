# SpectraOverdrive 1.2 Device Test Matrix

Run every row on a development world containing the 1.2 Neon Drop demo and a
production rig. Record device, OS, VRChat build, Unity build target, result,
average frame rate, thermal behavior, and notes.

| Test | PCVR | Quest | iOS | Android |
| --- | --- | --- | --- | --- |
| World load and baked signature accepted | ☐ | ☐ | ☐ | ☐ |
| Play, pause, stop, seek | ☐ | ☐ | ☐ | ☐ |
| Late join reconstructs current time | ☐ | ☐ | ☐ | ☐ |
| Ownership transfer preserves show | ☐ | ☐ | ☐ | ☐ |
| Sine and pulse modulation align | ☐ | ☐ | ☐ | ☐ |
| Seeded Sample-and-Hold matches clients | ☐ | ☐ | ☐ | ☐ |
| Macro transition aligns while playing | ☐ | ☐ | ☐ | ☐ |
| Macro transition aligns while paused | ☐ | ☐ | ☐ | ☐ |
| Scene stack order and banks | ☐ | ☐ | ☐ | ☐ |
| Beat/bar-quantized scene jump | ☐ | ☐ | ☐ | ☐ |
| Local strobe and laser disable | ☐ | ☐ | ☐ | ☐ |
| Local brightness and motion limits | ☐ | ☐ | ☐ | ☐ |
| Emergency blackout overrides everything | ☐ | ☐ | ☐ | ☐ |
| Adaptive quality degrades locally only | ☐ | ☐ | ☐ | ☐ |
| AudioLink and manual fallback | ☐ | ☐ | ☐ | ☐ |
| Optics use declared fallback | ☐ | ☐ | ☐ | ☐ |
| Thirty-minute thermal soak | N/A | ☐ | ☐ | ☐ |

Release acceptance requires no content-signature mismatch, no divergent
Sample-and-Hold cycle, no macro transition drift visible across clients, no
scene target mismatch, and a functioning emergency blackout on every target.
