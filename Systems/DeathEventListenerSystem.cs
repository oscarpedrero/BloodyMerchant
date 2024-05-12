using BloodyMerchant.DB;
using BloodyMerchant.Utils;
using HarmonyLib;
using Internal.Cryptography;
using ProjectM;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Unity.Collections;
using Unity.Entities;

namespace BloodyMerchant.Systems;

[HarmonyPatch]
public class DeathEventListenerSystem_Patch
{
    [HarmonyPatch(typeof(DeathEventListenerSystem), "OnUpdate")]
    [HarmonyPostfix]
    public static void Postfix(DeathEventListenerSystem __instance)
    {
        NativeArray<DeathEvent> deathEvents = __instance._DeathEventQuery.ToComponentDataArray<DeathEvent>(Allocator.Temp);
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
                    merchant.KillMerchant(ev.Killer);
                }
            }
         }

        }
    }
