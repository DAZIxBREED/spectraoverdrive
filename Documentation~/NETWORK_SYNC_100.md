# SpectraOverdrive 1.0 Network Synchronization

## Authoritative state

`SpectraShowNetworkController` synchronizes:

- revision
- active show index and content signature
- playback state
- server-time start point
- paused/seek offset
- playback speed
- active loop
- emergency blackout
- global strobe and laser permission
- active operator display name

This deliberately excludes cue arrays and per-frame fixture values.

## Clock model

While playing:

`showTime = (serverTime - playStartedServerTime) * playbackSpeed`

The active runtime player then applies the selected loop and evaluates its
local baked cue arrays. Quest, iOS, Android, and PCVR therefore share musical
time without transmitting fixture changes every frame.

## Late joiners

VRChat gives the joining client the latest manual-sync state. The client checks
the active baked content signature, reconstructs current show time from server
time, selects the active loop and safety state, then evaluates the show
directly at that position.

## Ownership recovery

When ownership transfers, the new owner preserves the currently calculated
show offset, rebases the server-time origin, increments the revision, and
serializes a fresh state. Owner heartbeats refresh the authoritative state
without carrying high-frequency fixture data.

## Mismatch safety

If a client’s baked show signature differs from the synchronized signature,
the player enters `Invalid`, activates local emergency blackout, and reports
`ShowMismatch`. This prevents differently baked clients from improvising
incompatible fixture output.
