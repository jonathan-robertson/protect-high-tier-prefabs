using HarmonyLib;
using ProtectHighTierPrefabs.Utilities;
using System;
using System.Collections.Generic;

namespace ProtectHighTierPrefabs.Patches
{
    [HarmonyPatch(typeof(NetPackageSetBlock), nameof(NetPackageSetBlock.ProcessPackage))]
    internal class NetPackageSetBlock_ProcessPackage_Patches
    {
        private static readonly ModLog<NetPackageSetBlock_ProcessPackage_Patches> _log = new ModLog<NetPackageSetBlock_ProcessPackage_Patches>();

        public static bool Prefix(GameManager _callbacks, int ___localPlayerThatChanged, PlatformUserIdentifierAbs ___persistentPlayerId, ref List<BlockChangeInfo> ___blockChanges)
        {
            try
            {
                BlockPlacementManager.HandleBlockChanges(_callbacks, ___localPlayerThatChanged, ___persistentPlayerId, ___blockChanges, out ___blockChanges);

                // only allow further processing and broadcast to other players if there are any block changes not already reversed
                return ___blockChanges.Count > 0;
            }
            catch (Exception e)
            {
                _log.Error($"Failed to handle prefix for NetPackageSetBlock.ProcessPackage on behalf of ___persistentPlayerId: {___persistentPlayerId} / ___localPlayerThatChanged: {___localPlayerThatChanged}", e);
                return true;
            }
        }
    }
}
