using HarmonyLib;
using ProtectHighTierPrefabs.Utilities;
using System;
using UnityEngine;

namespace ProtectHighTierPrefabs.Patches
{
    [HarmonyPatch(typeof(BlockToolSelection), "ExecuteUseAction")]
    internal class BlockToolSelection_ExecuteUseAction_Patches
    {
        private static readonly ModLog<BlockToolSelection_ExecuteUseAction_Patches> _log = new ModLog<BlockToolSelection_ExecuteUseAction_Patches>();

        public static bool Prefix(ItemInventoryData _data, bool _bReleased, PlayerActionsLocal playerActions, ref float ___lastBuildTime)
        {
            try
            {
                _log.Warn("prefix");
                if (!ConnectionManager.Instance.IsServer) { return true; }
                _log.Warn("0");
                if (GameManager.Instance.IsEditMode()) { return true; }
                _log.Warn("1");
                if (GameStats.GetInt(EnumGameStats.GameModeId) == 2 && playerActions.Drop.IsPressed) { return true; }
                _log.Warn("2");
                if (_bReleased) { return true; }
                _log.Warn("3");
                if (Time.time - ___lastBuildTime < Constants.cBuildIntervall) { return true; }

                _log.Warn("4");
                if (_data is ItemClassBlock.ItemBlockInventoryData itemBlockInventoryData)
                {
                    _log.Warn("5");
                    var blockValue = itemBlockInventoryData.itemValue.ToBlockValue();
                    if (!BlockPlacementManager.CanPlaceBlock(itemBlockInventoryData.hitInfo.lastBlockPos, blockValue, out var isLandClaim))
                    {
                        _log.Warn("6");
                        //itemBlockInventoryData.holdingEntity.PlayOneShot("keystone_build_warning", false, false);
                        _ = isLandClaim
                            ? GameManager.Instance.myEntityPlayerLocal.Buffs.AddBuff(BlockPlacementManager.BUFF_INTERRUPT_LAND_CLAIM_PLACEMENT_NAME)
                            : GameManager.Instance.myEntityPlayerLocal.Buffs.AddBuff(BlockPlacementManager.BUFF_INTERRUPT_BED_PLACEMENT_NAME);
                        ___lastBuildTime = Time.time;
                        return false;
                    }
                }
            }
            catch (Exception e)
            {
                _log.Error("Prefix", e);
            }
            _log.Warn("7");
            return true;
        }
    }
}
