# Protect High Tier Prefabs

[![🧪 Tested with 7DTD 1.2 (b27)](https://img.shields.io/badge/🧪%20Tested%20with-7DTD%201.2%20(b27)-blue.svg)](https://7daystodie.com/)
[![✅ Dedicated Servers Supported ServerSide](https://img.shields.io/badge/✅%20Dedicated%20Servers-Supported%20Serverside-blue.svg)](https://7daystodie.com/)
[![❌ Single Player and P2P Unsupported](https://img.shields.io/badge/❌%20Single%20Player%20and%20P2P-Unsupported-red.svg)](https://7daystodie.com/)
[![📦 Automated Release](https://github.com/jonathan-robertson/protect-high-tier-prefabs/actions/workflows/release.yml/badge.svg)](https://github.com/jonathan-robertson/protect-high-tier-prefabs/actions/workflows/release.yml)

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

| Environment          | Compatible | Does EAC Need to be Disabled? | Who needs to install? |
| -------------------- | ---------- | ----------------------------- | --------------------- |
| Dedicated Server     | Yes        | no                            | only server           |
| Peer-to-Peer Hosting | Untested   | N/A                           | N/A                   |
| Single Player Game   | Untested   | N/A                           | N/A                   |
