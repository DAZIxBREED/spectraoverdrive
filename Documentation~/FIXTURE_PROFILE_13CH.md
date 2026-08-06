# SpectraOverdrive Generic 13-Channel Moving Head

| Offset | Function |
|---:|---|
| 0 | Master dimmer |
| 1 | Red |
| 2 | Green |
| 3 | Blue |
| 4 | Pan coarse |
| 5 | Pan fine |
| 6 | Tilt coarse |
| 7 | Tilt fine |
| 8 | Strobe |
| 9 | Zoom |
| 10 | Gobo selection |
| 11 | Gobo rotation |
| 12 | Prism |

This profile is the first implemented SpectraOverdrive moving-head control contract.

## Mobile rendering behavior

- Pan and tilt are applied in the vertex shader.
- Zoom modifies the beam-shell width.
- Gobos sample a texture atlas.
- Strobe is locally disabled when photosensitive mode is active.
- RGB defaults to the material fallback color when no DMX color is present.
- Beam visibility is dithered rather than relying on a deep transparent stack.
