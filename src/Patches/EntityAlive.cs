using HarmonyLib;
using ProtectHighTierPrefabs.Utilities;
using System;

namespace ProtectHighTierPrefabs.Patches
{
    [HarmonyPatch(typeof(EntityAlive), "updateCurrentBlockPosAndValue")]
    internal class EntityAlive_updateCurrentBlockPosAndValue_Patches
    {
        private static readonly ModLog<EntityAlive_updateCurrentBlockPosAndValue_Patches> _log = new ModLog<EntityAlive_updateCurrentBlockPosAndValue_Patches>();

        /// <summary>
        /// Patch responsible for identifying when the player's block position changes.
        /// </summary>
        /// <param name="__instance">EntityAlive instance to check from.</param>
        /// <param name="___blockPosStandingOn">Block Position this entity is standing on.</param>
        public static void Postfix(EntityAlive __instance, Vector3i ___blockPosStandingOn)
        {
            try
            {
                if (ConnectionManager.Instance.IsServer
                    && __instance is EntityPlayer player)
                {
                    ZoneManager.HandleInBedrollPreventionZone(player, ___blockPosStandingOn);
                    ZoneManager.HandleInLandClaimPreventionZone(player, ___blockPosStandingOn);
                }
            }
            catch (Exception e)
            {
                _log.Error($"Postfix failed: handle block pos change for {__instance}.", e);
            }
        }
    }
}
