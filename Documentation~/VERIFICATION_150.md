# SpectraOverdrive 1.5 Verification

## Unity editor checks

1. Run **Run Runtime Self-Test**.
2. Run **Run 1.5 Release Readiness Check** on each production show.
3. Run **Validate Cross-Platform Budgets**.
4. Use the platform simulator for PC, Quest, iOS, and Android.
5. Re-bake the runtime player and confirm its content signature matches the
   selected show in the network controller.

## Layer checks

- Defaults match the authored enabled states.
- Toggle, enable, disable, solo, clear solo, and reset operate while stopped,
  paused, and playing.
- Late joiners reconstruct current masks.
- Changing shows restores the new show's defaults.
- Platform-disabled layers never appear on the prohibited target.
- Unlayered safety cues remain available.

## Arbitration checks

- Highest Priority respects layer bias.
- Latest Start and Earliest Start use deterministic tie breakers.
- Deterministic Cycle matches at the same show time on every client.
- Muting one candidate's layer immediately removes it from arbitration.
- Arbitration losers do not increase `activeCueCount`.
- `arbitrationSuppressedCueCount` and `layerSuppressedCueCount` reflect the
  expected rejected candidates.

## Device boundary

Static validation and editor self-tests do not replace UdonSharp compilation or
physical device tests. Verify Quest, iOS, Android, and PCVR in the complete world
project before deployment.
