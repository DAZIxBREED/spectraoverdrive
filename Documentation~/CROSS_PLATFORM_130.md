# SpectraOverdrive 1.3 Cross-Platform Policy

## Shared deterministic behavior

PCVR, Quest, iOS, and Android use the same compiled rhythm-gate and palette data.
The following are platform-invariant:

- rhythm pattern and step index
- variable-tempo beat/bar conversion
- Euclidean distribution
- seeded gate decisions
- custom-mask decisions
- stepped, ping-pong, and seeded palette selection
- macro-morph interpolation input
- cue fallback color and palette blend

Rendering quality may differ, but show-time decisions do not.

## Runtime limits

- maximum 16 palettes per show
- maximum 16 colors per palette
- maximum 32 rhythm-gate steps
- maximum four macro buses
- no runtime palette collections or dynamic allocation

## Mobile guidance

Quest, iOS, and Android evaluate at lower default rates than PCVR. Gate steps
shorter than the local evaluation interval can be intentionally skipped visually,
although the deterministic clock remains correct. The platform validator warns
about seconds-based steps below 80 ms on mobile and show validation warns about
steps below 50 ms or beat steps shorter than 1/16 beat.

Use emissive-friendly palette colors on iOS and Android. Rapid palette switching
must still respect the show's accessibility declaration and each user's local
rapid-color comfort settings. Strobe and laser policies remain independent of
rhythm gates.

## Upgrade behavior

Schema-v5 shows migrate with:

- no color palettes
- all rhythm gates disabled
- all palette bindings disabled
- existing cue colors preserved
- existing performance macros preserved

Re-bake every runtime player after migration.
