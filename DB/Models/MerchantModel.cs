using Bloodstone.API;
using BloodyMerchant.Exceptions;
using BloodyMerchant.Services;
using BloodyMerchant.Systems;
using BloodyMerchant.Utils;
using ProjectM;
using ProjectM.Network;
using ProjectM.Shared;
using Stunlock.Core;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Unity.Collections;
using Unity.Entities;
using Unity.Mathematics;
using UnityEngine;

namespace BloodyMerchant.DB.Models
{
    internal class MerchantModel
    {
        private static WaitForFrames _waitForFrames;
        public string name { get; set; } = string.Empty;
        public int PrefabGUID { get; set; }
        public List<ItemModel> items { get; set; } = new();

        public Entity merchantEntity { get; set; } = new();

        public ConfigMerchantModel config { get; set; } = new();

        public List<TraderItem> GetTraderItems()
        {
            var list = new List<TraderItem>();
            foreach (var item in items)
            {
                var itemTrader = new TraderItem(
                    outputItem: new PrefabGUID(item.OutputItem),
                    outputAmount: item.OutputAmount,
                    inputItem: new PrefabGUID(item.InputItem),
                    inputAmount: item.InputAmount,
                    stockAmount: item.StockAmount
                    );
                list.Add(itemTrader);
            }
            return list;
        }

        public bool GetProduct(int itemPrefabID, out ItemModel item)
        {
            item = items.Where(x => x.OutputItem == itemPrefabID).FirstOrDefault();
            if(item == null)
            {
                return false;
            }
            return true;
        }

        public bool AddProduct(int ItemPrefabID, int CurrencyfabID, int Stack, int Price, int Amount, bool Autorefill)
        {
            if(!GetProduct(ItemPrefabID, out ItemModel item))
            {
                item = new ItemModel(ItemPrefabID, Stack, CurrencyfabID, Price, Amount, Autorefill);
                items.Add(item);
                Database.saveDatabase();
                return true;
            }

            throw new ProductExistException();
            
        }

        public bool RemoveProduct(int ItemPrefabID) 
        {
            if (GetProduct(ItemPrefabID, out ItemModel item))
            {
                items.Remove(item);
                Database.saveDatabase();
                return true;
            }

            throw new ProductDontExistException();
        }

        public bool SpawnWithLocation(Entity sender, float3 pos)
        {

            _waitForFrames = new WaitForFrames();

            if (!config.IsEnabled)
            {


                UnitSpawnerService.UnitSpawner.SpawnWithCallback(sender, new PrefabGUID(PrefabGUID), new(pos.x, pos.z), -1, (Entity e) => {
                    merchantEntity = e;
                    config.z = pos.z;
                    config.x = pos.x;
                    config.IsEnabled = true;
                    _waitForFrames.Start(
                        world =>
                        {
                            ModifyMerchant(sender, e);
                            _waitForFrames.Stop();
                        },
                        input =>
                        {
                            if (input is not int secondAutoUIr)
                            {
                                Plugin.Logger.LogError("Starting timer delay function parameter is not a valid integer");
                                return TimeSpan.MaxValue;
                            }

                            var seconds = 1;
                            return TimeSpan.FromSeconds(seconds);
                    });
                });
                return true;
            }

            throw new MerchantEnableException();
        }

        public void ModifyMerchant(Entity user, Entity merchant)
        {

            var _items = GetTraderItems();

            var _tradeOutputBuffer = merchant.ReadBuffer<TradeOutput>();
            var _traderEntryBuffer = merchant.ReadBuffer<TraderEntry>();
            var _tradeCostBuffer = merchant.ReadBuffer<TradeCost>();

            _tradeOutputBuffer.Clear();
            _traderEntryBuffer.Clear();
            _tradeCostBuffer.Clear();

            var i = 0;
            foreach (var item in _items)
            {
                _tradeOutputBuffer.Add(new TradeOutput
                {
                    Amount = (ushort)item.OutputAmount,
                    Item = item.OutputItem,
                });

                _tradeCostBuffer.Add(new TradeCost
                {
                    Amount = (ushort)item.InputAmount,
                    Item = item.InputItem,
                });

                _traderEntryBuffer.Add(new TraderEntry
                {
                    RechargeInterval = -1,
                    CostCount = 1,
                    CostStartIndex = (byte)i,
                    FullRechargeTime = -1,
                    OutputCount = 1,
                    OutputStartIndex = (byte)i,
                    StockAmount = (ushort)item.StockAmount,
                });
                i++;
            }

            if (config.Immortal)
            {
                bool _immortal = MakeNPCImmortal(user, merchant);
                Plugin.Logger.LogDebug($"NPC immortal: {_immortal}");
            }

            if (!config.CanMove) 
            {
                bool _dontMove = MakeNPCDontMove(user, merchant);
                Plugin.Logger.LogDebug($"NPC Dont Move");
            }

            RenameMerchant(merchant, name);

        }

        public void Refill(Entity merchant, TraderPurchaseEvent _event)
        {
            var _entryBuffer = merchant.ReadBuffer<TraderEntry>();
            var _inputBuffer = merchant.ReadBuffer<TradeCost>();
            var _outputBuffer = merchant.ReadBuffer<TradeOutput>();

            for (int i = 0; i < _entryBuffer.Length; i++)
            {
                TraderEntry _newEntry = _entryBuffer[i];
                if (_entryBuffer[i].StockAmount == 1 && _event.ItemIndex == _newEntry.OutputStartIndex)
                {
                    PrefabGUID _outputItem = _outputBuffer[i].Item;
                    PrefabGUID _inputItem = _inputBuffer[i].Item;

                    var item = items.Where(x => new PrefabGUID(x.InputItem) == _inputItem && new PrefabGUID(x.OutputItem) == _outputItem).FirstOrDefault();
                    if (item != null && item.Autorefill)
                    {
                        _newEntry.StockAmount = (ushort)(item.StockAmount + 1);
                        _entryBuffer[i] = _newEntry;
                    }
                }
            }
        }

        private void RenameMerchant( Entity merchant, string nameMerchant)
        {
            Plugin.Logger.LogDebug($"NPC Add Name");
            merchant.Add<NameableInteractable>();
            NameableInteractable _nameableInteractable = merchant.Read<NameableInteractable>();
            _nameableInteractable.Name = new FixedString64Bytes(nameMerchant);
            merchant.Write(_nameableInteractable);
        }

        private bool GetEntity( string nameMerchant )
        {
            var entities = Helper.GetEntitiesByComponentTypes<NameableInteractable, TradeCost>(EntityQueryOptions.IncludeDisabledEntities);
            foreach (var entity in entities)
            {
                NameableInteractable _nameableInteractable = entity.Read<NameableInteractable>();
                if (_nameableInteractable.Name.Value == nameMerchant)
                {
                    merchantEntity = entity;
                    entities.Dispose();
                    return true;
                }
            }
            entities.Dispose();
            return false;
        }

        public bool MakeNPCDontMove(Entity user, Entity merchant)
        {
            var buff = Prefabs.Buff_BloodQuality_T01_OLD;
            var _des = VWorld.Server.GetExistingSystemManaged<DebugEventsSystem>();
            var _event = new ApplyBuffDebugEvent() { BuffPrefabGUID = buff };
            var _from = new FromCharacter()
            {
                User = user,
                Character = merchant
            };

            _des.ApplyBuff(_from, _event);
            if (BuffUtility.TryGetBuff(Plugin.World.EntityManager, merchant, buff, out var _buffEntity))
            {
                if (!_buffEntity.Has<BuffModificationFlagData>())
                {
                    _buffEntity.Add<BuffModificationFlagData>();
                }

                var _buffModificationFlagData = _buffEntity.Read<BuffModificationFlagData>();
                _buffModificationFlagData.ModificationTypes = (long)BuffModificationTypes.MovementImpair;
                _buffEntity.Write(_buffModificationFlagData);

                return true;
            }
            return false;
        }

        public bool MakeNPCImmortal(Entity user, Entity merchant)
        {
            var buff = Prefabs.Buff_Manticore_ImmaterialHomePos;
            var _des = VWorld.Server.GetExistingSystemManaged<DebugEventsSystem>();
            var _event = new ApplyBuffDebugEvent() { BuffPrefabGUID = buff };
            var _from = new FromCharacter()
            {
                User = user,
                Character = merchant
            };

            _des.ApplyBuff(_from, _event);
            if (BuffUtility.TryGetBuff(Plugin.World.EntityManager, merchant, buff, out var _buffEntity))
            {
                _buffEntity.Add<Buff_Persists_Through_Death>();
                if (_buffEntity.Has<LifeTime>())
                {
                    var _time = _buffEntity.Read<LifeTime>();
                    _time.Duration = -1;
                    _time.EndAction = LifeTimeEndAction.None;
                    _buffEntity.Write(_time);
                }

                if (_buffEntity.Has<RemoveBuffOnGameplayEvent>())
                    _buffEntity.Remove<RemoveBuffOnGameplayEvent>();

                if (_buffEntity.Has<RemoveBuffOnGameplayEventEntry>())
                    _buffEntity.Remove<RemoveBuffOnGameplayEventEntry>();

                return true;
            }
            return false;
        }

        public bool MakeNPCMortal(Entity user, Entity merchant)
        {
            var buff = Prefabs.Buff_Manticore_ImmaterialHomePos;
            if (BuffUtility.TryGetBuff(VWorld.Server.EntityManager, merchant, buff, out var buffEntity))
            {
                DestroyUtility.Destroy(VWorld.Server.EntityManager, buffEntity, DestroyDebugReason.TryRemoveBuff);
                return true;
            }
            return false;
        }

        public bool KillMerchant(Entity user) {


            if (GetEntity(name))
            {
                config.IsEnabled = false;
                StatChangeUtility.KillEntity(Plugin.World.EntityManager, merchantEntity, user, 0, StatChangeReason.BloodConsumeBuffDestroySystem_0, true);
                return true;
            } else
            {
                Plugin.Logger.LogDebug($"Entity of merchant not found");
                throw new MerchantDontExistException();
            }

        }


    }
}
