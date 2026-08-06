# Cue Layers and Deterministic Arbitration

SpectraOverdrive 1.5 introduces two routing stages that execute before the
platform active-cue budget: cue layers and arbitration groups.

## Cue layers

A show can define zero through sixteen `SpectraCueLayer` entries. A cue with
`layerIndex = -1` is unlayered and remains available regardless of operator
layer masks. A non-negative index binds the cue to the corresponding compiled
layer.

Each layer compiles to primitive arrays containing:

- name and display color
- default-enabled state
- PC, Quest, iOS, and Android availability flags
- priority bias from -100 through 100
- active-cue ceiling from 0 through 32; zero means no layer-specific ceiling

The runtime stores the operator state as two integers:

- `cueLayerEnabledMask`
- `cueLayerSoloMask`

The network controller synchronizes those masks with a revision counter. A zero
solo mask means no solo filtering. When a solo mask is nonzero, a cue must be in
both the enabled and solo masks. Platform availability is always enforced
locally after the synchronized mask is applied.

## Arbitration groups

A cue may join one of sixteen groups. All enabled cues in a group must use the
same arbitration mode, clock, cycle length, phase, and seed.

- `HighestPriority` compares authored cue priority plus layer priority bias.
- `LatestStart` chooses the most recently started active cue, then uses priority
  and deterministic compiled order as tie breakers.
- `EarliestStart` chooses the oldest active cue, then uses the same tie breakers.
- `DeterministicCycle` enumerates active candidates in compiled order and
  selects one ordinal from an absolute seconds, beat, or bar clock.

The compiler sorts equal-time cues by priority and stable ID. No arbitration
mode uses `UnityEngine.Random`, frame count, local time, or allocation-heavy
collections.

## Evaluation sequence

1. Time-window test
2. Cue condition
3. Variation selection
4. Rhythm-gate activity
5. Layer enabled/solo/platform test
6. Arbitration winner test
7. Priority plus layer-bias ordering
8. Per-layer admission ceiling
9. Global platform active-cue ceiling
10. Cue evaluation and group publication

This sequence is important: inactive alternatives never consume the bounded
mobile admission budget.

## Live control

`SpectraCueLayerController` exposes selection, enable, disable, toggle, solo,
clear-solo, and reset operations. It also exposes direct layer selection methods
for layers 0 through 15 so Unity UI and VRChat interactions do not need integer
method arguments.

Layer state is show-specific. Selecting a different show restores that show's
compiled defaults and synchronizes the new masks.
