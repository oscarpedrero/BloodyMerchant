using Bloodstone.API;
using BloodyMerchant.DB;
using BloodyMerchant.Utils;
using HarmonyLib;
using ProjectM;
using ProjectM.Network;
using RootMotion.FinalIK;
using Stunlock.Core;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Unity.Collections;
using Unity.Entities;
using static ProjectM.Metrics;

namespace BloodyMerchant.Systems
{
    internal class AutorefillSystem
    {
        [HarmonyPatch(typeof(TraderPurchaseSystem), nameof(TraderPurchaseSystem.OnUpdate))]
        [HarmonyPrefix]
        public static void Prefix(TraderPurchaseSystem __instance)
        {
            var networkIdLookupEntity = Helper.GetEntitiesByOneComponentTypes<NetworkIdSystem.Singleton>(EntityQueryOptions.IncludeSystems)[0];
            var singleton = networkIdLookupEntity.Read<NetworkIdSystem.Singleton>();
            Entity _trader;
            var _entities = __instance._TraderPurchaseEventQuery.ToEntityArray(Allocator.Temp);
            foreach (var _entity in _entities)
            {

                var _event = _entity.Read<TraderPurchaseEvent>();
                singleton.GetNetworkIdLookupRW().TryGetValue(_event.Trader, out _trader);

                if (_trader.Has<NameableInteractable>())
                {

                    var name = _trader.Read<NameableInteractable>().Name.Value;

                    foreach (var merchant in Database.Merchants.Where(x => x.name == name).ToList())
                    {
                        merchant.Refill(_trader, _event);

                        
                    }
                    
                    
                }
            }
        }
    }
}
