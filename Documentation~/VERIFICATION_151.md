# SpectraOverdrive 1.5.1 Verification

## Automated source checks

Run from the package root:

```bash
python3 Tools/validate_package.py
```

The validator checks JSON/assembly files, C# delimiter integrity, duplicate declared types, local shader includes, compiled-array/runtime mappings, compiler initialization and baking, executable stub markers, and release-version consistency.

## Unity regression checks

Run **SpectraOverdrive > Show Programmer > Run Runtime Self-Test** and verify the 1.5.1 success dialog. The regression suite includes:

- solo mask canonicalization
- disabling the currently soloed layer
- soloing a disabled layer
- pre-deserialization macro/layer default preservation
- malformed mixed-configuration arbitration rejection
- all previous schema-v8, synchronization, mobile-policy, safety, automation, palette, gate, variation, snapshot, and scene tests

## Device checks

Re-run the 1.5 device matrix on PCVR, Quest, iOS, and Android. Pay special attention to late joining during an active show, layer solo/toggle operations, show switching, and owner transfer.
