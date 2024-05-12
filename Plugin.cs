using BepInEx;
using BepInEx.Unity.IL2CPP;
using BepInEx.Configuration;
using BepInEx.Logging;
using VRising.GameData;
using HarmonyLib;
using Unity.Entities;
using UnityEngine;
using Bloodstone.API;
using System;
using VampireCommandFramework;
using ProjectM;
using Unity.Collections;
using BloodyMerchant.DB;
using System.Linq;
using BloodyMerchant.Systems;
using BloodyMerchant.Services;
using BloodyMerchant.Utils;

namespace BloodyMerchant
{
    [BepInPlugin(MyPluginInfo.PLUGIN_GUID, MyPluginInfo.PLUGIN_NAME, MyPluginInfo.PLUGIN_VERSION)]
    [BepInDependency("gg.deca.Bloodstone")]
    [BepInDependency("gg.deca.VampireCommandFramework")]
    public class Plugin : BasePlugin, IRunOnInitialized
    {

        public static ManualLogSource Logger;
        private Harmony _harmony;

        public static World World;

        public override void Load()
        {

            Logger = Log;
            _harmony = new Harmony(MyPluginInfo.PLUGIN_GUID);
            _harmony.PatchAll(System.Reflection.Assembly.GetExecutingAssembly());
            _harmony.PatchAll(typeof(AutorefillSystem));
            _harmony.PatchAll(typeof(UnitSpawnerService));

            GameData.OnInitialize += GameDataOnInitialize;
            GameData.OnDestroy += GameDataOnDestroy;

            CommandRegistry.RegisterAll();

            Database.Initialize();

            // Plugin startup logic
            Log.LogInfo($"Plugin {MyPluginInfo.PLUGIN_GUID} is loaded!");
        }

        public override bool Unload()
        {
            Config.Clear();
            CommandRegistry.UnregisterAssembly();

            _harmony.UnpatchSelf();

            GameData.OnDestroy -= GameDataOnDestroy;
            GameData.OnInitialize -= GameDataOnInitialize;

            return true;
        }

        private static void GameDataOnInitialize(World world)
        {
            Logger.LogDebug("GameDataOnInitialize");
            foreach (var merchant in Database.Merchants.Where(x => x.config.Autorepawn == false).ToList())
            {
                Plugin.Logger.LogDebug($"kill Autorespawn Merchant {merchant.name} off");

                merchant.KillMerchant(Helper.GetAnyUser());

            }
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
