using Bloodstone.API;
using BloodyMerchant.DB;
using HarmonyLib;
using ProjectM;
using ProjectM.Network;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Unity.Collections;
using Unity.Entities;

namespace BloodyMerchant.Systems
{
    internal class AutorefillSystem
    {
        [HarmonyPatch(typeof(TraderPurchaseSystem), nameof(TraderPurchaseSystem.OnUpdate))]
        [HarmonyPrefix]
        public static void Prefix(TraderPurchaseSystem __instance)
        {
            var _entities = __instance.__TraderPurchaseJob_entityQuery.ToEntityArray(Allocator.Temp);

            foreach (var _entity in _entities)
            {
                var _event = _entity.Read<TraderPurchaseEvent>();
                Entity _trader = VWorld.Server.GetExistingSystem<NetworkIdSystem>()._NetworkIdToEntityMap[_event.Trader];

                if (_trader.Read<Health>().MaxHealth.Value != 1111) continue;

                var _entryBuffer = _trader.ReadBuffer<TraderEntry>();
                var _inputBuffer = _trader.ReadBuffer<TradeCost>();
                var _outputBuffer = _trader.ReadBuffer<TradeOutput>();

                for (int i = 0; i < _entryBuffer.Length; i++)
                {
                    TraderEntry _newEntry = _entryBuffer[i];
                    if (_entryBuffer[i].StockAmount == 1 && _event.ItemIndex == _newEntry.OutputStartIndex)
                    {
                        int _outputItem = _outputBuffer[i].Item.GuidHash;
                        int _inputItem = _inputBuffer[i].Item.GuidHash;

                        foreach (var merchant in Database.Merchants)
                        {
                            var item = merchant.items.Where(x => x.InputItem == _inputItem && x.OutputItem == _outputItem).FirstOrDefault();
                            if (item != null && item.Autorefill)
                            {
                                _newEntry.StockAmount = item.StockAmount + 1;
                                _entryBuffer[i] = _newEntry;
                            }
                        }
                    }
                }
            }
        }
    }
}
