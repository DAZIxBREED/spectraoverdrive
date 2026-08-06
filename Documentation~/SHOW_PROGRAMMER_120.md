# SpectraOverdrive 1.2 Show Programmer

## Procedural modulation

Every cue can carry one deterministic procedural modulator after keyframed
automation and before its performance-macro binding.

The evaluation order is:

1. base cue value
2. flattened keyframe automation
3. procedural modulation
4. synchronized performance macro
5. cue fade/easing weight
6. cue blending
7. live override
8. accessibility and emergency safety

Supported waveforms are Sine, Triangle, Saw Up, Saw Down, Square, Pulse, and
seeded Sample-and-Hold. The time base may be seconds, musical beats, or musical
bars. Each modulator also stores cycle length, phase, pulse duty cycle, optional
step quantization, a `Vector4` offset, and a `Vector4` depth.

All waveforms produce a normalized `0..1` signal. The modifier is:

`modifier = offset + depth * signal`

Replace, Add, or Multiply then combines the modifier with the cue value. The
vector maps to the cue's compiled channels; for example, intensity uses X,
movement uses X/Y/Z for pan/tilt/speed, color uses RGBA, and zoom/focus uses XY.

Sample-and-Hold is generated from the cue seed and integer cycle. It does not
use `UnityEngine.Random`, so late joiners and devices at different frame rates
receive the same value.

## Performance macros

A show defines zero to four macro buses. Each macro has:

- name
- description
- default `0..1` value
- synchronized transition duration
- display color

A cue may bind to one macro. It defines the value vector at macro zero, the
value vector at macro one, and Replace/Add/Multiply behavior. The runtime
interpolates that vector from the synchronized macro value.

Macro transitions are networked as four start values, four targets, a server
timestamp, and one transition duration. Clients reconstruct the same smoothstep
transition locally. No per-frame macro values are serialized.

## Scene stacks

Any hot-cue marker may also be a scene. A scene stores:

- scene bank `0..7`
- stable order within the bank
- optional selection advance after triggering
- the existing hot-cue quantization and safe transition duration

`SpectraSceneStackController` sorts scene markers by order without allocating a
runtime list. Triggering a scene uses the existing server-time hot-cue schedule,
so scene jumps retain beat/bar quantization, synchronized dip transitions, and
late-join reconstruction.

## Runtime cost

Modulators and macros are fixed parallel arrays. Only active cues are evaluated.
No modulator objects, curves, delegates, reflection, LINQ, or temporary
collections exist in runtime playback. This is why the same show logic remains
usable on PCVR, Quest, iOS, and Android.
