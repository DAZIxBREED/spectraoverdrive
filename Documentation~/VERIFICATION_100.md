# SpectraOverdrive 1.0 Verification

## One-click editor test

Run **SpectraOverdrive > Show Programmer > Run Runtime Self-Test**.

The test covers:

- fixed and variable-tempo conversion
- beat snapping, duplication, splitting, and cue clipboard
- waveform min/max generation
- real audio energy/transient assisted generation
- schema-v3 signed JSON round trip and corruption rejection
- deterministic compiled content signatures
- flattened cue, marker, loop, audio, event, and fallback arrays
- group-free global cue execution and baked device budgets
- intensity, gobo, and zoom runtime evaluation
- live override application
- snapshot capture and restore
- offline authoritative network-state reconstruction
- emergency blackout
- release-readiness compilation and platform budgets

## Release check

Run **Run 1.0 Release Readiness Check** on each show. A ready report confirms:

- no show validation errors
- explicit PC, Quest, iOS, and Android policies
- successful compile and signed JSON round trip
- identical runtime signature after import
- cue concurrency inside every platform budget
- safety metadata review

## Source-package checks

The release archive should additionally pass:

- all C# syntax trees are error-free
- flattened compiler fields map to runtime fields
- no intentional stub markers or placeholder method bodies
- package metadata version is `1.0.0`
- ZIP CRC/integrity test

Unity/Udon and the physical-device matrix remain mandatory downstream checks.
