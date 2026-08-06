# SpectraOverdrive Show Programmer 1.0

## Authoring

The 1.0 authoring model remains non-destructive. `SpectraShowAsset` is the
editable source of truth and contains stable IDs, tempo maps, fixture-group
references, tracks, cue blocks, markers, loops, platform policies, and
accessibility metadata.

New schema-v3 cue values:

- gobo index and rotation
- prism amount
- zoom and focus
- audio-band intensity modulation
- routed Udon/world events

The visual timeline displays these cues through the same draggable, resizable,
snappable blocks used by intensity, color, movement, strobe, laser, and
blackout cues.

## Assisted starter shows

**Assisted Starter Show** reads actual `AudioClip` samples. It calculates RMS
energy and high-frequency sample-difference energy in bounded buckets, detects
transients, aligns impacts to the beat grid, and generates:

- phrase intensity cues
- palette color cues
- deterministic movement cues
- detected impact cues
- musical-structure markers
- a full-length bass-energy modulation cue

Every result is a normal editable cue. Generated tracks are clearly prefixed
and can be replaced without touching hand-authored tracks.

## Live performance capture

`SpectraLiveOverrideRecorder` captures bounded, preallocated operator actions.
**Convert Live Recording** resolves action hold durations and produces
editable intensity, color, movement, and optics tracks. Existing hand-authored
tracks remain intact.

## Production compilation

The compiler validates the show, resolves musical timing, sorts cue order
deterministically, flattens all values into Udon-safe arrays, compiles
markers/loops/platform fallbacks, and calculates a deterministic runtime
content signature.

The runtime baker copies only primitive values, Unity value types, strings, and
flat arrays onto `SpectraShowRuntimePlayer`.

Global and event tracks may intentionally omit a fixture group. Global visual
cues broadcast to every compiled group, blackout remains a highest-priority
safety operation, and routed network events are emitted only by the authority
object's owner so clients never multiply-broadcast the same timeline event.
