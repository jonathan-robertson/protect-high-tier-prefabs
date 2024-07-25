# Protect High Tier Prefabs

[![🧪 Tested On](https://img.shields.io/badge/🧪%20Tested%20On-1.0%20b333-blue.svg)](https://7daystodie.com/) [![📦 Automated Release](https://github.com/jonathan-robertson/protect-high-tier-prefabs/actions/workflows/release.yml/badge.svg)](https://github.com/jonathan-robertson/protect-high-tier-prefabs/actions/workflows/release.yml)

- [Protect High Tier Prefabs](#protect-high-tier-prefabs)
  - [Summary](#summary)
    - [Support](#support)
  - [Features](#features)
    - [Positional Awareness](#positional-awareness)
    - [Placement Prevention](#placement-prevention)
  - [Compatibility](#compatibility)

## Summary

7 Days to Die mod: Prevent players from taking over Tier 4 or 5 prefabs.

### Support

🗪 If you would like support for this mod, please feel free to reach out via [Discord](https://discord.gg/hYa2sNHXya).

## Features

Prevent players from taking over Tier 4 or 5 prefabs.

### Positional Awareness

As players enter range of a tier 4 or 5 POI (either for bed or land claim), a buff notifying them of this will appear with an orange color indicating "[Blocked]" and will not disappear until they leave that prevention zone.

When a players exit a zone, the corresponding zone icon in the buff they exit will refresh with a green color indicating "[Allowed]" which will disappear within about 3 seconds.

This feature is merely for setting the right expectation and serves no other purpose beyond conveying information.

### Placement Prevention

Placing any block will trigger a check on block class and position against the existing Tier 4 and 5 POIs on the map.

If the check indicates that this block is not permitted to be placed here, that block will be removed and placed back into the player's inventory.

A notification will also trigger on the player's toolbelt to explain what just happened.

## Compatibility

Supported Languages: `English` (for now)

Environment | Compatible | Does EAC Need to be Disabled? | Who needs to install?
--- | --- | --- | ---
Dedicated Server | Yes | no | only server
Peer-to-Peer Hosting | Untested | N/A | N/A
Single Player Game | Untested | N/A | N/A
