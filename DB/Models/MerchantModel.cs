using Bloodstone.API;
using BloodyMerchant.Exceptions;
using BloodyMerchant.Services;
using BloodyMerchant.Systems;
using ProjectM;
using ProjectM.Network;
using ProjectM.Shared;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Unity.Entities;
using Unity.Mathematics;
using Unity.Transforms;
using UnityEngine.Analytics;
using UnityEngine.TextCore;
using static ProjectM.Network.ReceivePacketSystem;

namespace BloodyMerchant.DB.Models
{
    internal class MerchantModel
    {
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
                    outputItem: new(item.OutputItem),
                    outputAmount: item.OutputAmount,
                    inputItem: new(item.InputItem),
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

        public bool AutorespawnMerchant()
        {
            var sender = ServerEvents.GetAnyUser();
            if (config.Autorepawn)
            {
                UnitSpawnerService.UnitSpawner.SpawnWithCallback(sender, new PrefabGUID(PrefabGUID), new(config.x, config.z), -1, (Entity e) => {
                    merchantEntity = e;
                    config.IsEnabled = true;
                    ModifyMerchant(sender, e);
                });
                return true;
            }

            throw new MerchantEnableException();
        }

        public bool SpawnWithLocation(Entity sender, float3 pos)
        {
            if(!config.IsEnabled)
            {
                UnitSpawnerService.UnitSpawner.SpawnWithCallback(sender, new PrefabGUID(PrefabGUID), new(pos.x, pos.z), -1, (Entity e) => {
                    merchantEntity = e;
                    config.z = pos.z;
                    config.x = pos.x;
                    config.IsEnabled = true;
                    ModifyMerchant(sender, e);
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
            foreach ( var item in _items )
            {
                _tradeOutputBuffer.Add(new TradeOutput
                {
                    Amount = item.OutputAmount,
                    Item = item.OutputItem,
                });

                _tradeCostBuffer.Add(new TradeCost
                {
                    Amount = item.InputAmount,
                    Item = item.InputItem,
                });

                _traderEntryBuffer.Add(new TraderEntry
                {
                    RechargeInterval = -1,
                    CostCount = 1,
                    CostStartIndex = i,
                    FullRechargeTime = -1,
                    OutputCount = 1,
                    OutputStartIndex = i,
                    StockAmount = item.StockAmount,
                });
            }
            var _health = merchant.Read<Health>();
            _health.MaxHealth.Value = 1;
            merchant.Write(_health);

            if (config.Immortal)
            {
                bool _immortal = MakeNPCImmortal(user, merchant, new PrefabGUID(-61473528));
                Plugin.Logger.LogInfo($"NPC immortal: {_immortal}");
            }

            if (!config.CanMove) 
            { 
                merchant.Remove<MoveEntity>();
                Plugin.Logger.LogInfo($"NPC Remove Move");
            }

            // TODO: Under Investigate
            RenameMerchant(merchant, name);

        }

        private static void RenameMerchant( Entity merchant, string nameMerchant)
        {
            /*var nameable = new NameableInteractableComponent();
            nameable.name = nameMerchant;
            Plugin.World.EntityManager.SetComponentData(merchant, nameable);
            merchant.WithComponentData((ref NameableInteractable nameable) =>
            {
                nameable.Name = nameMerchant;
                return;
            });*/

            //var nameable = merchant.Read<Name>;
        }

        public bool MakeNPCImmortal(Entity user, Entity merchant, PrefabGUID buff)
        {
            var _des = VWorld.Server.GetExistingSystem<DebugEventsSystem>();
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

        public bool MakeNPCMortal(Entity user, Entity merchant, PrefabGUID buff)
        {
            if (BuffUtility.TryGetBuff(VWorld.Server.EntityManager, merchant, buff, out var buffEntity))
            {
                DestroyUtility.Destroy(VWorld.Server.EntityManager, buffEntity, DestroyDebugReason.TryRemoveBuff);
                return true;
            }
            return false;
        }

        public bool KillMerchant(Entity user) {

            if(config.IsEnabled)
            {
                config.IsEnabled = false;
                StatChangeUtility.KillEntity(Plugin.World.EntityManager, merchantEntity, user, 0, true);
                return true;
            }

            throw new MerchantDontEnableException();
        }


    }
}
