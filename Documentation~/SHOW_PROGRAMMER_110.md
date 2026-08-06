# SpectraOverdrive Show Programmer 1.1

## Automation

Every cue can carry zero to sixteen normalized `SpectraAutomationKey` values.
The compiler flattens keys into shared time, value, and interpolation arrays.
Each cue stores only its offset, count, and mode.

Modes:

- Disabled: use the cue's authored value.
- Replace: the envelope is the complete value.
- Add: add the envelope to the authored value.
- Multiply: multiply each value channel by the envelope.

Interpolation is Step, Linear, or Smooth. Color automation uses RGBA. Movement
automation uses pan, tilt, speed, and the fourth general parameter. Intensity,
gobo, prism, zoom/focus, laser, strobe, and blackout consume the channels
appropriate to their cue type.

The timeline inspector includes working Pulse, Riser, Four-Beat Gate, and Clear
actions. Keys remain directly editable through the serialized cue inspector.

## Live hot cues

A marker becomes a live hot cue when `hotCue` is enabled. It stores:

- target show time
- quantization
- dip-transition duration
- name and marker kind

`SpectraHotCueBankController` exposes next, previous, trigger, and cancel calls
for desktop or VR UI buttons. The network controller schedules the jump against
VRChat server time. Before and after the scheduled instant all clients derive
the same show offset. The transition is a synchronized dip of master intensity;
it does not require evaluating two complete show states on mobile.

## Capability-aware authoring

Each show fixture group declares a `SpectraFixtureCapability` mask. The compiler
infers a required mask for every cue and validates the relationship. A cue can
be disabled, attempted as-authored, or converted to an emissive/intensity
approximation if the group lacks the required capability.

Each `SpectraFixtureRuntime` also has its own mask. Group output neutralizes
unsupported color, movement, optics, strobe, laser, and AudioLink values before
publishing material properties.
