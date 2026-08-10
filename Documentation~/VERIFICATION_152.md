# SpectraOverdrive 1.5.2 Verification

## Automated

Run `python3 Tools/validate_package.py`, then run **SpectraOverdrive > Show Programmer > Run Runtime Self-Test** in Unity.

The 1.5.2 regression suite verifies:

- valid synchronized loop selection
- repeated loop selection produces a no-op instead of another serialization
- invalid requested loop selection normalizes to no loop
- malformed deserialized loop state is rejected by the runtime
- unchanged master intensity is a suppressed network no-op
- already-default cue-layer reset is a suppressed network no-op
- all 1.5.1 cue-layer, arbitration, macro, sync, safety, and platform tests remain intact

## Device boundary

Complete Unity/UdonSharp compilation and PCVR, Quest, iOS, and Android device tests before production deployment.
