using BepInEx;
using BepInEx.Unity.IL2CPP;
using BepInEx.Logging;
using HarmonyLib;
using Unity.Entities;
using Bloodstone.API;
using VampireCommandFramework;
using BloodyMerchant.DB;
using System.Linq;
using BloodyMerchant.Systems;
using Bloody.Core.API.v1;
using Bloody.Core;
using BepInEx.Configuration;
using UnityEngine;
using ProjectM.Physics;

namespace BloodyMerchant
{
    [BepInPlugin(MyPluginInfo.PLUGIN_GUID, MyPluginInfo.PLUGIN_NAME, MyPluginInfo.PLUGIN_VERSION)]
    [BepInDependency("gg.deca.Bloodstone")]
    [BepInDependency("gg.deca.VampireCommandFramework")]
    [BepInDependency("trodi.Bloody.Core")]
    [BepInDependency("trodi.bloody.Wallet", BepInDependency.DependencyFlags.SoftDependency)]
    public class Plugin : BasePlugin, IRunOnInitialized
    {

        public static ManualLogSource Logger;
        private Harmony _harmony;

        public static World World;

        public static SystemsCore SystemsCore;

        public static ConfigEntry<bool> WalletSystem;

        public override void Load()
        {

            Logger = Log;
            _harmony = new Harmony(MyPluginInfo.PLUGIN_GUID);
            _harmony.PatchAll(System.Reflection.Assembly.GetExecutingAssembly());

            EventsHandlerSystem.OnInitialize += GameDataOnInitialize;
            EventsHandlerSystem.OnDestroy += GameDataOnDestroy;

            CommandRegistry.RegisterAll();

            InitConfigServer();

            Database.Initialize();

            // Plugin startup logic
            Log.LogInfo($"Plugin {MyPluginInfo.PLUGIN_GUID} is loaded!");
        }
        private void InitConfigServer()
        {
            WalletSystem = Config.Bind("Wallet", "enabled", false, "Activate system for buy with virtual currency through BloodyWallet ( https://thunderstore.io/c/v-rising/p/Trodi/BloodyWallet/ )");
        }
        public override bool Unload()
        {
            Config.Clear();
            CommandRegistry.UnregisterAssembly();

            _harmony.UnpatchSelf();

            EventsHandlerSystem.OnDestroy -= GameDataOnDestroy;
            EventsHandlerSystem.OnInitialize -= GameDataOnInitialize;
            EventsHandlerSystem.OnDeath -= DeathEventSystem.OnDeath;

            return true;
        }

        private static void GameDataOnInitialize(World world)
        {

            SystemsCore = Core.SystemsCore;
            EventsHandlerSystem.OnTraderPurchase += AutorefillSystem.OnTraderPurchase;
            EventsHandlerSystem.OnDeath += DeathEventSystem.OnDeath;
            if (WalletSystem.Value)
            {
                VirtualBuySystem.MakeSpecialCurrenciesSoulbound();
                EventsHandlerSystem.OnPlayerBuffed += VirtualBuySystem.HandleOnPlayerBuffed;
                EventsHandlerSystem.OnPlayerBuffRemoved += VirtualBuySystem.HandleOnPlayerBuffRemoved;
            }

            /*foreach (var merchant in Database.Merchants.Where(x => x.config.Autorepawn == false).ToList())
            {
                Logger.LogDebug($"kill Autorespawn Merchant {merchant.name} off");
                merchant.KillMerchant(UserSystem.GetAnyUser());
            }*/

            Logger.LogInfo("GameDataOnInitialize BloodyMerchant");

        }

        private static void GameDataOnDestroy()
        {
            Logger.LogDebug("GameDataOnDestroy");
        }

        public void OnGameInitialized()
        {
            World = VWorld.Server;
            Logger.LogDebug("OnGameInitialized");
        }
    }
}
