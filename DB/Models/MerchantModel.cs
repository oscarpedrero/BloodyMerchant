using Bloodstone.API;
using Bloody.Core.API.v1;
using Bloody.Core.Helper.v1;
using Bloody.Core.Patch.Server;
using BloodyMerchant.Exceptions;
using BloodyMerchant.Systems;
using ProjectM;
using ProjectM.Network;
using ProjectM.Shared;
using Stunlock.Core;
using System.Collections.Generic;
using System.Linq;
using Unity.Collections;
using Unity.Entities;
using Unity.Mathematics;

namespace BloodyMerchant.DB.Models
{
    internal class MerchantModel
    {
        private Entity icontEntity;

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

        public bool Clean()
        {

            items.Clear();
            Database.saveDatabase();
            if (GetEntity(name))
            {
                Addinventory(merchantEntity);
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
                AddRealtimeObjet(item);
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
                RemoveRealtimeObjet(item);
                return true;
            }

            throw new ProductDontExistException();
        }

        public bool SpawnWithLocation(Entity sender, float3 pos)
        {

            if (!config.IsEnabled)
            {

                SpawnSystem.SpawnUnitWithCallback(sender, new PrefabGUID(PrefabGUID), new(pos.x, pos.z), -1, (Entity e) => {
                    merchantEntity = e;
                    config.z = pos.z;
                    config.x = pos.x;
                    config.IsEnabled = true;
                    ModifyMerchant(sender, e);
                    var action = () =>
                    {
                        Addinventory(e);
                    };
                    CoroutineHandler.StartFrameCoroutine(action, 3, 1);
                });

                var entityManager = Plugin.SystemsCore.EntityManager;
                var actionIcon = () =>
                {
                    SpawnSystem.SpawnUnitWithCallback(sender, Prefabs.MapIcon_POI_Discover_Merchant, new float2(config.x, config.z), -1, (Entity e) =>
                    {
                        icontEntity = e;
                        e.Add<MapIconData>();
                        e.Add<MapIconTargetEntity>();
                        var mapIconTargetEntity = e.Read<MapIconTargetEntity>();
                        mapIconTargetEntity.TargetEntity = NetworkedEntity.ServerEntity(merchantEntity);
                        mapIconTargetEntity.TargetNetworkId = merchantEntity.Read<NetworkId>();
                        e.Write(mapIconTargetEntity);
                        e.Add<NameableInteractable>();
                        NameableInteractable _nameableInteractable = e.Read<NameableInteractable>();
                        _nameableInteractable.Name = new FixedString64Bytes(name + "_icon");
                        e.Write(_nameableInteractable);
                    });
                };
                ActionScheduler.RunActionOnceAfterDelay(actionIcon, 3);


                return true;
            }

            throw new MerchantEnableException();
        }

        public void ModifyMerchant(Entity user, Entity merchant)
        {

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

        public void Addinventory(Entity merchant)
        {
            var entityManager = Plugin.SystemsCore.EntityManager;
            var unitSpawnerUpdateSystem = Plugin.SystemsCore.UnitSpawnerUpdateSystem;

            var _items = GetTraderItems();

            var _tradeOutputBuffer = entityManager.GetBuffer<TradeOutput>(merchant);
            var _traderEntryBuffer = entityManager.GetBuffer<TraderEntry>(merchant);
            var _tradeCostBuffer = entityManager.GetBuffer<TradeCost>(merchant);

            _tradeOutputBuffer.Clear();
            _traderEntryBuffer.Clear();
            _tradeCostBuffer.Clear();

            var i = 0;
            foreach (var item in _items)
            {
                if (i > 32) break;
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
                    RechargeInterval = 10,
                    CostCount = 1,
                    CostStartIndex = (byte)i,
                    FullRechargeTime = 60,
                    OutputCount = 1,
                    OutputStartIndex = (byte)i,
                    StockAmount = (ushort)item.StockAmount,
                });
                i++;
            }
        }

        public void CreateIcon(Entity sender)
        {

            var entityManager = Plugin.SystemsCore.EntityManager;
            var unitSpawnerUpdateSystem = Plugin.SystemsCore.UnitSpawnerUpdateSystem;

            icontEntity.Add<MapIconData>();
            icontEntity.Add<MapIconTargetEntity>();
            var mapIconTargetEntity = icontEntity.Read<MapIconTargetEntity>();
            mapIconTargetEntity.TargetEntity = NetworkedEntity.ServerEntity(merchantEntity);
            mapIconTargetEntity.TargetNetworkId = merchantEntity.Read<NetworkId>();
            icontEntity.Write(mapIconTargetEntity);
            icontEntity.Add<NameableInteractable>();
            NameableInteractable _nameableInteractable = icontEntity.Read<NameableInteractable>();
            _nameableInteractable.Name = new FixedString64Bytes(name + "_icon");
            icontEntity.Write(_nameableInteractable);
        }

        internal bool AddRealtimeObjet(ItemModel item)
        {
            if (GetEntity(name))
            {

                var _tradeOutputBuffer = merchantEntity.ReadBuffer<TradeOutput>();
                var _traderEntryBuffer = merchantEntity.ReadBuffer<TraderEntry>();
                var _tradeCostBuffer = merchantEntity.ReadBuffer<TradeCost>();

                var i = _tradeCostBuffer.Length;

                _tradeOutputBuffer.Add(new TradeOutput
                {
                    Amount = (ushort)item.OutputAmount,
                    Item = new PrefabGUID(item.OutputItem),
                });

                _tradeCostBuffer.Add(new TradeCost
                {
                    Amount = (ushort)item.InputAmount,
                    Item = new PrefabGUID(item.InputItem),
                });

                _traderEntryBuffer.Add(new TraderEntry
                {
                    RechargeInterval = 10,
                    CostCount = 1,
                    CostStartIndex = (byte)i,
                    FullRechargeTime = 60,
                    OutputCount = 1,
                    OutputStartIndex = (byte)i,
                    StockAmount = (ushort)item.StockAmount,
                });

                return true;
            }
            else
            {
                return false;
            }
            
        }

        internal bool RemoveRealtimeObjet(ItemModel item)
        {
            if (GetEntity(name))
            {
                Addinventory(merchantEntity);
                return true;
            }
            else
            {
                return false;
            }

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
            if (!merchant.Has<NameableInteractable>())
            {
                merchant.Add<NameableInteractable>();
            }
            NameableInteractable _nameableInteractable = merchant.Read<NameableInteractable>();
            _nameableInteractable.Name = new FixedString64Bytes(nameMerchant);
            merchant.Write(_nameableInteractable);
        }

        public bool GetEntity( string nameMerchant )
        {
            var entities = QueryComponents.GetEntitiesByComponentTypes<NameableInteractable, TradeCost>(EntityQueryOptions.IncludeDisabledEntities);
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

        private bool GetIcon( string nameMerchant )
        {
            var entities = QueryComponents.GetEntitiesByComponentTypes<NameableInteractable, MapIconData>(EntityQueryOptions.Default,true);
            Plugin.Logger.LogInfo($"Encontrados {entities.Length}");
            foreach (var entity in entities)
            {
                NameableInteractable _nameableInteractable = entity.Read<NameableInteractable>();
                if (_nameableInteractable.Name.Value == nameMerchant + "_icon")
                {
                    Plugin.Logger.LogInfo($"Icono Encontrado");
                    icontEntity = entity;
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

        public bool KillMerchant(Entity user)
        {

            if (GetEntity(name))
            {
                CleanIconMerchant(user);
                config.IsEnabled = false;
                StatChangeUtility.KillOrDestroyEntity(Plugin.World.EntityManager, merchantEntity, user, user, 0, StatChangeReason.Any, true);
                return true;
            }
            else
            {
                Plugin.Logger.LogDebug($"Entity of merchant not found");
                throw new MerchantDontExistException();
            }


        }

        public bool CleanIconMerchant(Entity user)
        {
            if (!icontEntity.Exists())
            {
                if (!GetIcon(name))
                {
                    Plugin.Logger.LogInfo($"Entity icon not found");
                    return true;
                }
            }
            icontEntity.Remove<MapIconData>();
            icontEntity.Remove<MapIconTargetEntity>();
            icontEntity.Remove<NameableInteractable>();
            StatChangeUtility.KillOrDestroyEntity(Plugin.World.EntityManager, icontEntity, user, user, 0, StatChangeReason.Any, true);
            Plugin.Logger.LogInfo($"Entity icon remove");
            return true;
        }

    }
}
