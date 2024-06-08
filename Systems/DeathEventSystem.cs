using BloodyMerchant.DB;
using ProjectM;
using System.Linq;
using Unity.Collections;


namespace BloodyMerchant.Systems;

public class DeathEventSystem
{
    public static void OnDeath(DeathEventListenerSystem sender, NativeArray<DeathEvent> deathEvents)
    {
        foreach (DeathEvent ev in deathEvents)
        {
            //-- Just track whatever died...
            if (!ev.Died.Has<TradeCost>())
            {
                return;
            }

            if (ev.Died.Has<NameableInteractable>())
            {
                var name = ev.Died.Read<NameableInteractable>().Name.Value;

                foreach (var merchant in Database.Merchants.Where(x => x.name == name).ToList())
                {
                    if(merchant.merchantEntity.Exists())
                    {
                        merchant.CleanIconMerchant(ev.Killer);
                    }
                }
            }
         }

        }
    }
