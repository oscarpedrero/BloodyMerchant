using BepInEx;
using BepInEx.Unity.IL2CPP;
using BepInEx.Logging;
using HarmonyLib;
using Unity.Entities;
using BloodyMerchant.Compat;
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
    // Bloodstone dependency removed - it is abandoned and its network hooks break
    // every server network event on V Rising 1.1.x. BloodyCore already provides the
    // initialization event this mod actually uses.
    [BepInProcess("VRisingServer.exe")]
    [BepInDependency("gg.deca.VampireCommandFramework")]
    [BepInDependency("trodi.Bloody.Core")]
    [BepInDependency("trodi.bloody.Wallet", BepInDependency.DependencyFlags.SoftDependency)]
    public class Plugin : BasePlugin
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
            // Bloodstone's OnGameInitialized used to set this. BloodyCore hands us the
            // world directly, so we no longer need Bloodstone for it.
            World = world ?? Compat.VWorld.Server;

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

        // OnGameInitialized() removed - that was Bloodstone's IRunOnInitialized hook and
        // its only job was setting World, which GameDataOnInitialize now handles.
    }
}
