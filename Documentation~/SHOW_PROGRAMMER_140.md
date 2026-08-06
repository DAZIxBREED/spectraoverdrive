# SpectraOverdrive 1.4 Show Programmer

## Cue conditions

Each cue has one optional deterministic condition.

| Mode | Selection rule |
| --- | --- |
| Probability | Seeded hash is below the authored probability each cycle |
| Every Nth Cycle | Enables one cycle out of N, with an optional offset |
| Macro Above | Synchronized macro is at or above the threshold |
| Macro Below | Synchronized macro is below the threshold |
| Audio Above | CPU-side AudioLink/manual band is at or above the threshold |
| Audio Below | CPU-side AudioLink/manual band is below the threshold |

Probability and Every-N support seconds, beats, or bars, cycle length, phase,
and inversion. Reusing the same cue seed intentionally repeats the same
probability pattern.

## Variation groups

A variation group selects one option while preserving normal cue timing. Up to
16 groups and eight options per group are supported. Every enabled cue in a
group must use the same mode, option count, clock, cycle length, phase, seed,
and macro binding.

A variation option may contain multiple cues. For example, option 0 can contain
one movement cue, one palette cue, and one gobo cue; option 1 can contain a
completely different layered look.

Cycle, Ping-Pong, and Seeded Random use absolute show time. Macro Select maps a
synchronized macro value over the option count.

## Macro snapshots

`performanceMacroSnapshots` stores up to 16 named `Vector4` values. Components
of the vector map to macro buses 1 through 4. Values are clamped to zero through
one by the compiler. Each snapshot includes one synchronized transition time.

The generated production rig contains `SpectraMacroSnapshotController` with:

- next/previous selection
- direct selection methods for snapshots 0 through 15
- direct recall methods for snapshots 0 through 15
- selected name, color, values, transition, and count for UI display

## Quick tools

The cue inspector provides:

- 50% Chance
- Every 4 Bars
- Energy > 50%
- Clear Condition
- Cycle A / Cycle B
- Seeded A
- Clear Variation

The SHOW panel can create starter palettes and four macro snapshots.

## Runtime budgeting

Conditions and variation selection run before active-cue insertion. A cue that
is currently rejected does not consume the platform's active-cue capacity.
This is particularly important on Quest, iOS, and Android.
