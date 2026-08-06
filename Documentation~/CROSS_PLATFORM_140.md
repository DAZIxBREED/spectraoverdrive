# SpectraOverdrive 1.4 Cross-Platform Policy

## Determinism

Condition and variation decisions use compiled primitive data, the show clock,
the beat map, synchronized macro values, and integer seeds. They do not use
frame count, `UnityEngine.Random`, coroutines, runtime collections, or client
wall-clock time.

Variation groups use absolute show time. Probability and Every-N conditions use
the cue-relative clock by design.

## Mobile behavior

Quest, iOS, and Android use the same logical decisions as PCVR. Their lower
update rates can display fewer intermediate frames, but they do not choose a
different cycle or option at the same show time.

The compatibility validator warns when second-based condition or variation
cycles are shorter than 80 ms. The show validator warns below 50 ms.

## Active-cue budgets

Rejected conditions, rejected variation options, disabled fallbacks, and
rhythm-gated-off cues do not consume local active-cue capacity. Rendering
fallbacks remain platform-local after deterministic selection.

## Audio condition limitation

The runtime cannot safely read shader AudioLink textures back to the CPU in an
Udon-safe cross-platform path. Audio conditions therefore consume the
`SpectraAudioLinkAdapter` CPU-side manual/published values. Worlds using direct
AudioLink texture sampling must bridge desired bands into those fields.

## Snapshot networking

Snapshot recall synchronizes four target floats, four starting floats, one
server timestamp, one duration, one snapshot index, and revisions. It does not
serialize a large snapshot object. Late joiners reconstruct the current values
from server time.
