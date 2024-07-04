using System.Collections.Generic;
using UnityEngine;

namespace ProtectHighTierPrefabs.Utilities
{
    internal class ZoneManager
    {
        private const string _buffInLandClaimPreventionZoneName = "protectHighTierPrefabsInLandClaimPreventionZone";
        private const string _buffInBedPreventionZoneName = "protectHighTierPrefabsInBedPreventionZone";

        private static readonly ModLog<ZoneManager> _log = new ModLog<ZoneManager>();
        private static readonly Dictionary<int, BoundsInt> _landClaimPreventionZone = new Dictionary<int, BoundsInt>();
        private static readonly Dictionary<int, BoundsInt> _bedPreventionZone = new Dictionary<int, BoundsInt>();

        public static void OnGameStartDone()
        {
            var landClaimSize = GameStats.GetInt(EnumGameStats.LandClaimSize); // default: 41
            var landClaimRadius = landClaimSize % 2 == 1 ? (landClaimSize - 1) / 2 : landClaimSize / 2;
            var bedrollRadius = GamePrefs.GetInt(EnumGamePrefs.BedrollDeadZoneSize);

            var landclaimPadding = new Vector3i(landClaimRadius, 0, landClaimRadius);
            var bedrollPadding = new Vector3i(bedrollRadius, 0, bedrollRadius);

            var pois = GameManager.Instance.GetDynamicPrefabDecorator().GetPOIPrefabs();
            for (var i = 0; i < pois.Count; i++)
            {
                var pi = pois[i];

                switch (pi.prefab.DifficultyTier)
                {
                    case 4:
                    case 5:
                        if (_landClaimPreventionZone.ContainsKey(pi.id)
                            || _bedPreventionZone.ContainsKey(pi.id))
                        {
                            _log.Error($"Prefab {pi.id} appears to have already been recorded but is showing up again in this hook => id: {pi.id} | tier: {pi.prefab.DifficultyTier} | filename: {pi.prefab.PrefabName} | tags: {pi.prefab.GetQuestTags()}");
                            return;
                        }
                        _landClaimPreventionZone.Add(pi.id, new BoundsInt(pi.boundingBoxPosition - landclaimPadding, pi.boundingBoxSize + (landclaimPadding * 2)));
                        _bedPreventionZone.Add(pi.id, new BoundsInt(pi.boundingBoxPosition - bedrollPadding, pi.boundingBoxSize + (bedrollPadding * 2)));
                        break;
                }
            }
        }

        public static void OnGameShutdown()
        {
            _landClaimPreventionZone.Clear();
            _bedPreventionZone.Clear();
        }

        public static bool InLandClaimPreventionZone(Vector3i pos)
        {
            return Contains(pos, _landClaimPreventionZone);
        }

        public static bool InBedrollPreventionZone(Vector3i pos)
        {
            return Contains(pos, _bedPreventionZone);
        }

        public static void HandleInLandClaimPreventionZone(EntityPlayer player, Vector3i blockPos)
        {
            BuffIfInZone(player, blockPos, _landClaimPreventionZone, _buffInLandClaimPreventionZoneName);
        }

        public static void HandleInBedrollPreventionZone(EntityPlayer player, Vector3i blockPos)
        {
            BuffIfInZone(player, blockPos, _bedPreventionZone, _buffInBedPreventionZoneName);
        }

        private static void BuffIfInZone(EntityPlayer player, Vector3i blockPos, Dictionary<int, BoundsInt> zone, string buffName)
        {
            var withinZone = Contains(blockPos, zone);
            if (player.Buffs.HasBuff(buffName))
            {
                if (!withinZone)
                {
                    player.Buffs.RemoveBuff(buffName);
                }
            }
            else
            {
                if (withinZone)
                {
                    _ = player.Buffs.AddBuff(buffName);
                }
            }
        }

        private static bool Contains(Vector3i pos, Dictionary<int, BoundsInt> dict)
        {
            foreach (var kvp in dict)
            {
                if (kvp.Value.Contains(pos))
                {
                    return true;
                }
            }
            return false;
        }
    }
}
