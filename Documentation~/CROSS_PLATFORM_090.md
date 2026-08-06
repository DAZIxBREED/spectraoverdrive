# Cross-Platform Runtime Policy 0.9.0

## Shared truth

PCVR, Quest, iOS, and Android clients share:

- show ID and duration
- resolved cue start and duration
- beat and bar conversion
- marker and loop positions
- cue priority and blend intent
- colors and intensity intent
- deterministic movement seed
- safety state

They do not need to render the same number of beam meshes, transparent layers,
laser segments, or receiver effects.

## Default budgets

| Target | Active cues | Evaluation rate | Intended renderer |
| --- | ---: | ---: | --- |
| PCVR | 128 | 60 Hz | Full beam, gobo, receiver, and laser path |
| Quest | 48 | 36 Hz | Mesh beams, bounded movement, reduced transparency |
| iOS | 32 | 30 Hz | Emissive and projected-color emphasis |
| Android | 32 | 30 Hz | Emissive and projected-color emphasis |

The values are defaults, not hardware guarantees. Use the compatibility
validator and test the actual world on each target.

## Fallback meanings

- **Full** evaluates the original cue.
- **Simplified** preserves activation, timing, and general direction while
  bounding movement complexity and high-frequency strobe behavior.
- **Emissive Only** evaluates color, intensity, and blackout intent and omits
  movement, strobe, and laser work.
- **Disabled** omits the cue locally.

Fallbacks are local. They never change the authoritative show asset for another
client.

## Android and Quest

Both targets use Unity's Android build path. At runtime SpectraOverdrive checks
the local VR state: Android VR clients use the Quest policy; non-VR Android
clients use the phone/tablet policy. Platform-marker objects remain in the
Android build when either mobile variant needs them, then activate locally.

## Mobile authoring checklist

1. Give every expensive movement, strobe, and laser cue a deliberate fallback.
2. Keep peak active cues under the target budget.
3. Prefer shared materials and opaque or cutout rendering.
4. Do not ship waveform audio solely for editor reference.
5. Test thermal behavior during a full-length show, not a 30-second preview.
6. Verify local photosensitive, strobe, laser, and brightness controls.
7. Confirm emergency blackout with malformed or missing show data.

VRChat's Android optimization guidance emphasizes baked lighting, reduced
geometry, reduced texture resolution, and avoiding transparency:
https://creators.vrchat.com/platforms/android/quest-content-optimization/
