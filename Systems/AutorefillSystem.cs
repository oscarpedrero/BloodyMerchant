using Bloody.Core.Helper;
using BloodyMerchant.DB;
using ProjectM;
using ProjectM.Network;
using System.Linq;
using Unity.Collections;
using Unity.Entities;

namespace BloodyMerchant.Systems
{
    internal class AutorefillSystem
    {
        public static void OnTraderPurchase(NativeArray<Entity> entities)
        {
            var networkIdLookupEntity = QueryComponents.GetEntitiesByComponentTypes<NetworkIdSystem.Singleton>(EntityQueryOptions.IncludeSystems)[0];
            var singleton = networkIdLookupEntity.Read<NetworkIdSystem.Singleton>();
            Entity _trader;
            foreach (var _entity in entities)
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
