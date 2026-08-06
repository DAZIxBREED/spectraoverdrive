# SpectraOverdrive 1.0 Cross-Platform Policy

## Shared on every platform

- show ID and content signature
- playback time and state
- beat, bar, marker, and loop positions
- cue priority and deterministic ordering
- primary colors and musical intent
- emergency blackout, laser, and strobe permission
- deterministic movement seeds

## Local rendering

PCVR can retain the full optical cue path. Quest defaults to simplified
movement, bounded transparent beams, shared materials, reduced concurrency,
and lower evaluation frequency. iOS and non-VR Android default to emissive
color/intensity preservation with expensive optics or transparency omitted.

Gobo, prism, zoom/focus, movement, event, strobe, and laser cues use explicit
per-cue fallbacks. Audio-reactive intensity remains available in emissive-only
mode.

## Adaptive quality

`SpectraAdaptiveBudgetAllocator` measures local frame time and changes
`SpectraLocalQualityController` only after multiple bad or good samples.
`SpectraShowRuntimePlayer` scales its active-cue ceiling and evaluation
frequency from that local tier. The network clock is unaffected.

## Allocation policy

The steady runtime path reuses cue-selection buffers, preallocates live
recording arrays, preallocates synchronized override arrays, and stores
snapshot data in a circular flattened cache. Editor waveform, analysis, JSON,
and report objects never ship as per-frame runtime work.
