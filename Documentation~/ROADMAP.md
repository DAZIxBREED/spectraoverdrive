# SpectraOverdrive Roadmap

## 0.3 — First controlled fixture

- Complete DMX packing modes
- Moving-head 13-channel profile
- Shader pan/tilt
- RGB, dimmer, strobe, zoom, and gobo
- AudioLink sampling
- fixture-group cue output

## 0.4 — Quest show renderer

- Beam shell family
- wash and spot projections
- gobo atlas
- lens and halo meshes
- receiver masks
- laser ribbons
- mobile effect budgeting

## 0.5 — Authoring tools

- fixture-profile editor
- patch table
- group editor
- cue/chase editor
- receiver-zone authoring
- scene validator
- performance heat map

## 0.6 — VRSL compatibility

- scene scanner
- address and profile migration
- AudioLink setting migration
- DMX texture compatibility
- conversion report
- side-by-side PC/Quest preview

## 0.7 — iOS renderer

- iOS-specific shaders
- touch operator UI
- thermal-friendly quality shifts
- dedicated iOS build validation

## 0.8 — Runtime show data

- versioned show assets and portable IDs
- beat grid and variable tempo conversion
- deterministic compiler
- flat runtime cue arrays
- runtime cue blending and safety layers
- portable JSON

## 0.9 — Visual Show Programmer

- advanced beat-aware timeline
- waveform reference and tap tempo
- draggable cues, markers, and loops
- deterministic movement generators
- cue, movement, palette, and section presets
- PCVR, Quest, iOS, and Android fallback compilation
- mobile cue and update-rate budgets
- cross-platform compatibility reports
- schema-v1 migration
- Neon Drop demo generator

## 0.10 — Synchronized operator release — delivered in 1.0

- production operator console prefab
- runtime show selection
- synchronized pause and seeking
- fixture solo, isolate, and manual override layers
- ownership recovery hardening
- late-joiner state reconstruction

## 0.11 — Sharing and live capture — delivered in 1.0

- expanded portable import/export validation
- live override recording
- audio-reactive timeline modulation
- receiver-zone timeline automation
- per-cue accessibility alternatives

## 0.12 — Assisted programming and optimization — delivered in 1.0

- editable starter-show generation
- VRSL fixture-group conversion
- device profiling and stress certification
- build-size reports and automated stripping
- full Quest, iOS, and Android optimization passes

## 1.0 — Integrated production foundation

- server-time synchronized show banks
- late-join and ownership recovery
- signed runtime content and SHA-256 portable-show integrity
- allocation-free snapshots
- live synchronized overrides, recording, and timeline conversion
- gobo, prism, zoom/focus, AudioLink-modulated, and event cues
- waveform-assisted editable show generation
- production rig generator
- schema-v3 migration

## 1.1 — Performance authoring and capability intelligence

- schema-v4 flattened cue automation
- variable-tempo runtime beat conversion
- synchronized quantized hot cues and transitions
- fixture-group and per-fixture capability contracts
- deterministic emissive capability fallbacks
- smoothed device-pressure quality adaptation
- PCVR, Quest, iOS, and Android platform simulator
- expanded migration, validation, reporting, and device tests
- release-readiness and device-test tooling

## 1.2 — Procedural performance and scene operation

- schema-v5 deterministic procedural modulation
- seconds-, beat-, and bar-based waveform clocks
- seeded Sample-and-Hold and quantized modulation
- four synchronized performance macro buses
- server-time macro transitions independent of local frame rate
- per-cue vector macro ranges and application modes
- eight-bank ordered scene stacks
- quantized scene triggering through the hot-cue clock
- desktop/VR macro and scene controllers
- procedural timeline presets and visualization
- schema-v1 through schema-v4 migration
- expanded four-platform validation and device tests

## 1.3 — Rhythmic cue intelligence and dynamic color language

- schema-v6 deterministic rhythm gates
- Pulse, Alternating, Euclidean, Seeded Random, and Custom Mask patterns
- seconds-, beat-, and bar-relative gate clocks
- allocation-free gate evaluation before cue-budget selection
- show-defined dynamic color palettes
- Fixed, Step, Ping-Pong, Seeded Random, and Macro Morph palette modes
- flattened Udon-safe palette arrays
- timeline quick-authoring tools and visual badges
- expanded release reports, platform analysis, self-tests, and device matrix
- schema-v1 through schema-v5 migration

## 1.4 — Deterministic generative routing and snapshot operation

- schema-v7 cue conditions with probability, every-N, macro, and audio modes
- synchronized variation groups with cycle, ping-pong, seeded, and macro modes
- absolute show-clock option selection across overlapping cues and late joiners
- condition and variation rejection before active-cue budget insertion
- sixteen four-macro performance snapshots with server-time transitions
- desktop/VR macro snapshot selection and recall controller
- timeline condition/variation badges and quick-authoring helpers
- expanded compiler, signatures, JSON migration, reporting, self-tests, and
  four-platform device matrix
- schema-v1 through schema-v6 migration

## 1.5 — Operator cue layers and deterministic arbitration

Delivered in 1.5.0: synchronized layer masks, platform-aware layer policy,
priority bias, per-layer admission limits, deterministic arbitration groups,
a layer controller, timeline authoring, migration, validation, and demo content.
