# SpectraOverdrive 1.0 Device Test Matrix

Static source checks are not a substitute for Unity, Udon, and physical-device
tests. Complete this matrix before public distribution.

| Test | PCVR | Quest | iOS | Android |
| --- | --- | --- | --- | --- |
| Package imports without compiler errors | Required | Same project | Same project | Same project |
| Runtime self-test passes | Required | N/A editor | N/A editor | N/A editor |
| Release-readiness report is ready | Required | Shared asset | Shared asset | Shared asset |
| Join running show at intro/drop/loop | Required | Required | Required | Required |
| Owner leaves during playback | Required | Required | Required | Required |
| Pause, seek, speed, loop, resume | Required | Required | Required | Required |
| Emergency blackout during all effects | Required | Required | Required | Required |
| Local strobe/laser disable stays local | Required | Required | Required | Required |
| Live override and clear | Required | Required | Required | Required |
| Content mismatch activates blackout | Required | Required | Required | Required |
| Gobo/prism/zoom fallback is correct | Full | Simplified | Emissive/disabled | Emissive/disabled |
| 20-minute thermal/performance run | Recommended | Required | Required | Required |
| No steady-state GC spikes from show player | Required | Required | Required | Required |

Record SDK version, device/OS, world build ID, show content signature, average
FPS, worst frame time, active fixture count, active cue peak, and any dropped
cue count for every run.
