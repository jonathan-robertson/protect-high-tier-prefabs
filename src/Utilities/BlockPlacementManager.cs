using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace ProtectHighTierPrefabs.Utilities
{
    internal class BlockPlacementManager
    {
        private const string _landclaimClass = "LandClaim";
        private const string _bedClass = "SleepingBag";

        private const string _buffInterruptLandClaimPlacementName = "protectHighTierPrefabsInterruptLandClaimPlacement";
        private const string _buffInterruptBedPlacementName = "protectHighTierPrefabsInterruptBedPlacement";

        private static readonly WaitForSeconds WaitBriefly = new WaitForSeconds(0.1f);
        private static readonly Dictionary<string, ItemStack> _itemStacks = new Dictionary<string, ItemStack>();
        private static readonly List<BlockChangeInfo> _allPreventedBlocks = new List<BlockChangeInfo>();
        private static readonly Queue<BlockChangeInfo> _landClaimsToReturn = new Queue<BlockChangeInfo>();
        private static readonly Queue<BlockChangeInfo> _bedsToReturn = new Queue<BlockChangeInfo>();

        private static readonly ModLog<BlockPlacementManager> _log = new ModLog<BlockPlacementManager>();

        public static void HandleBlockChanges(GameManager gameManager, int playerEntityId, PlatformUserIdentifierAbs userIdentifier, List<BlockChangeInfo> blockChangeInfo, out List<BlockChangeInfo> allowed)
        {
            ProcessBlockChanges(blockChangeInfo, out allowed);
            if (_allPreventedBlocks.Count == 0) { return; }

            try
            {
                if (!gameManager.persistentPlayers.Players.TryGetValue(userIdentifier, out var persistentPlayerData))
                {
                    _log.Error($"failed to retrieve presistent player data for player {playerEntityId}");
                    return;
                }
                if (!PlayerHelper.TryGetClientInfo(playerEntityId, out var clientInfo))
                {
                    _log.Error($"failed to retrieve client for player {playerEntityId}");
                    return;
                }
                if (!gameManager.World.Players.dict.TryGetValue(clientInfo.entityId, out var player))
                {
                    _log.Error($"failed to retrieve player entity for player {playerEntityId}");
                    return;
                }

                EraseDeniedBlocks(clientInfo, persistentPlayerData);
                while (_landClaimsToReturn.Count > 0)
                {
                    var deniedLandClaimChange = _landClaimsToReturn.Dequeue();
                    _ = ThreadManager.StartCoroutine(ReturnDeniedLandClaimBlock(clientInfo, player, deniedLandClaimChange.pos, deniedLandClaimChange.blockValue));
                }
                while (_bedsToReturn.Count > 0)
                {
                    var deniedSleepingBagChange = _bedsToReturn.Dequeue();
                    _ = ThreadManager.StartCoroutine(ReturnDeniedSleepingBagBlock(clientInfo, persistentPlayerData, player, deniedSleepingBagChange.pos, deniedSleepingBagChange.blockValue));
                }
            }
            finally
            {
                _allPreventedBlocks.Clear();
                _landClaimsToReturn.Clear();
                _bedsToReturn.Clear();
            }
        }

        /// <summary>
        /// Verify and label all BlockChangeInfo objects within the given list as being allowed or denied.
        /// </summary>
        /// <param name="blockChangeInfo">BlockChangeInfo list of changes to process.</param>
        /// <param name="allowed">BlockChangeInfo list containing allowed block changes.</param>
        private static void ProcessBlockChanges(List<BlockChangeInfo> blockChangeInfo, out List<BlockChangeInfo> allowed)
        {
            allowed = new List<BlockChangeInfo>();
            for (var i = 0; i < blockChangeInfo.Count; i++)
            {
                var blockHasChanged = blockChangeInfo[i].bChangeBlockValue;
                var blockPlacementAllowed = CanPlaceBlock(blockChangeInfo[i].pos, blockChangeInfo[i].blockValue, out var isLandClaim);

                if (!blockHasChanged // monitor only instances where the block was replaced or removed
                    || blockPlacementAllowed)
                {
                    allowed.Add(blockChangeInfo[i]);
                    continue;
                }

                if (isLandClaim)
                {
                    _allPreventedBlocks.Add(blockChangeInfo[i]);
                    _landClaimsToReturn.Enqueue(blockChangeInfo[i]);
                }
                else
                {
                    _allPreventedBlocks.Add(blockChangeInfo[i]);
                    _bedsToReturn.Enqueue(blockChangeInfo[i]);
                }
            }
        }

        /// <summary>
        /// Identify whether the given BlockValue can be placed at the provided Position.
        /// </summary>
        /// <param name="pos">Vector3i represents map coordinates a client is attempting to place the given BlockValue at.</param>
        /// <param name="blockValue">BlockValue a client if attempting to place at the given Position.</param>
        /// <returns>Whether the given BlockValue can be placed at the given Position.</returns>
        private static bool CanPlaceBlock(Vector3i pos, BlockValue blockValue, out bool isLandClaim)
        {
            if (blockValue.Block.Properties.Values.TryGetString("Class", out var className))
            {
                switch (className)
                {
                    case _landclaimClass:
                        isLandClaim = true;
                        return !ZoneManager.InLandClaimPreventionZone(pos);
                    case _bedClass:
                        isLandClaim = false;
                        return !ZoneManager.InBedrollPreventionZone(pos);
                }
            }
            isLandClaim = false;
            return true;
        }

        /// <summary>
        /// Send NetPackage to erase blocks in the given lists by returning them to air.
        /// </summary>
        /// <param name="clientInfo">ClientInfo for the client we'll send this package to.</param>
        /// <param name="persistentPlayerData">PersistentPlayerData for the placer.</param>
        private static void EraseDeniedBlocks(ClientInfo clientInfo, PersistentPlayerData persistentPlayerData)
        {
            // Send netpackage to erase blocks on placer's client
            var eraseBlockChanges = new List<BlockChangeInfo>();
            for (var i = 0; i < _allPreventedBlocks.Count; i++)
            {
                eraseBlockChanges.Add(new BlockChangeInfo(_allPreventedBlocks[i].pos, BlockValue.Air, true));
            }
            clientInfo.SendPackage(NetPackageManager.GetPackage<NetPackageSetBlock>().Setup(persistentPlayerData, eraseBlockChanges, clientInfo.entityId));
        }

        private static IEnumerator ReturnDeniedLandClaimBlock(ClientInfo clientInfo, EntityPlayer player, Vector3i pos, BlockValue blockValue)
        {
            // interrupt claim sound and replace with rejection sound along with notice
            _ = player.Buffs.AddBuff(_buffInterruptLandClaimPlacementName);
            yield return WaitBriefly;
            try
            {
                RemoveMarker(clientInfo, pos, "land_claim", EnumMapObjectType.LandClaim);
                GiveOneBlock(clientInfo, pos, blockValue);
            }
            catch (Exception e)
            {
                _log.Error($"failed to return land claim block {blockValue} to player {clientInfo.entityId}", e);
            }
        }

        private static IEnumerator ReturnDeniedSleepingBagBlock(ClientInfo clientInfo, PersistentPlayerData persistentPlayerData, EntityPlayer player, Vector3i pos, BlockValue blockValue)
        {
            // interrupt claim sound and replace with rejection sound along with notice
            _ = player.Buffs.AddBuff(_buffInterruptBedPlacementName);
            yield return WaitBriefly;
            try
            {
                RemoveMarker(clientInfo, pos, "sleeping_bag", EnumMapObjectType.SleepingBag);
                GiveOneBlock(clientInfo, pos, blockValue);

                // trick client into re-registering previous bedroll (if one exists)
                if (persistentPlayerData.HasBedrollPos)
                {
                    var serverSideBedroll = GameManager.Instance.World.GetBlock(persistentPlayerData.BedrollPos);
                    clientInfo.SendPackage(NetPackageManager.GetPackage<NetPackageSetBlock>().Setup(persistentPlayerData, new List<BlockChangeInfo> {
                        new BlockChangeInfo(persistentPlayerData.BedrollPos, BlockValue.Air, false, false), // delete current bedroll from local
                        new BlockChangeInfo(persistentPlayerData.BedrollPos, serverSideBedroll, false, false) // add back same bedroll to local
                    }, clientInfo.entityId));
                }
            }
            catch (Exception e)
            {
                _log.Error($"failed to return sleeping bag block {blockValue} to player {clientInfo.entityId}", e);
            }
        }
        private static void RemoveMarker(ClientInfo clientInfo, Vector3i pos, string name, EnumMapObjectType type)
        {
            NavObjectManager.Instance.UnRegisterNavObjectByPosition(pos.ToVector3(), name);
            clientInfo.SendPackage(NetPackageManager.GetPackage<NetPackageEntityMapMarkerRemove>().Setup(type, pos.ToVector3()));
        }

        private static void GiveOneBlock(ClientInfo clientInfo, Vector3i pos, BlockValue blockValue)
        {
            var blockName = blockValue.Block.GetBlockName();
            if (StringParsers.TryParseBool(blockValue.Block.Properties.Values[Block.PropCanPickup], out var canPickup, 0, -1, true)
                && canPickup
                && blockValue.Block.Properties.Params1.ContainsKey(Block.PropCanPickup))
            {
                blockName = blockValue.Block.Properties.Params1[Block.PropCanPickup];
            }
            GiveOneItem(clientInfo, pos, blockName);
        }

        private static void GiveOneItem(ClientInfo clientInfo, Vector3i pos, string name)
        {
            if (!_itemStacks.TryGetValue(name, out var itemStack))
            {
                itemStack = new ItemStack(ItemClass.GetItem(name, true), 1);
                _itemStacks.Add(name, itemStack);
            }
            GiveItemStack(clientInfo, pos, itemStack); // target bag to drop at block placement position in case player inventory is full
        }

        private static void GiveItemStack(ClientInfo clientInfo, Vector3i pos, ItemStack itemStack)
        {
            var entityId = EntityFactory.nextEntityID++;
            GameManager.Instance.World.SpawnEntityInWorld((EntityItem)EntityFactory.CreateEntity(new EntityCreationData
            {
                entityClass = EntityClass.FromString("item"),
                id = entityId,
                itemStack = itemStack,
                pos = pos,
                rot = new Vector3(20f, 0f, 20f),
                lifetime = 60f,
                belongsPlayerId = clientInfo.entityId
            }));
            clientInfo.SendPackage(NetPackageManager.GetPackage<NetPackageEntityCollect>().Setup(entityId, clientInfo.entityId));
            _ = GameManager.Instance.World.RemoveEntity(entityId, EnumRemoveEntityReason.Despawned);
        }
    }
}
