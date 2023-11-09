using BloodyMerchant.DB;
using HarmonyLib;
using ProjectM;
using ProjectM.Network;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Unity.Entities;

namespace BloodyMerchant.Systems
{
    internal class ServerEvents
    {
        [HarmonyPatch(typeof(GameBootstrap), nameof(GameBootstrap.OnApplicationQuit))]
        [HarmonyPrefix]
        public static void OnApplicationQuit(GameBootstrap __instance)
        {
            Plugin.Logger.LogInfo("Kill Merchants");
            Database.saveDatabase();
            foreach (var merchant in Database.Merchants.Where(x => x.config.IsEnabled == true).ToList())
            {
                var user = GetAnyUser();
                Plugin.Logger.LogInfo($"Kill Merchant {merchant.name}");
                merchant.KillMerchant(user);
            }
            Database.saveDatabase();
        }

        public static Entity GetAnyUser()
        {
            var _entities = Il2cppService.GetEntitiesByComponentTypes<User>();
            foreach (var _entity in _entities)
            {
                return _entity;
            }
            return Entity.Null;
        }
    }
}
