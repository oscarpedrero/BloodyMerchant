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

namespace BloodyMerchant
{
    [BepInPlugin(MyPluginInfo.PLUGIN_GUID, MyPluginInfo.PLUGIN_NAME, MyPluginInfo.PLUGIN_VERSION)]
    [BepInDependency("gg.deca.Bloodstone")]
    [BepInDependency("gg.deca.VampireCommandFramework")]
    public class Plugin : BasePlugin, IRunOnInitialized
    {

        public static ManualLogSource Logger;
        private Harmony _harmony;

        public World world;

        public override void Load()
        {

            Logger = Log;
            _harmony = new Harmony(MyPluginInfo.PLUGIN_GUID);
            _harmony.PatchAll(System.Reflection.Assembly.GetExecutingAssembly());

            world = VWorld.Server;

            GameData.OnInitialize += GameDataOnInitialize;
            GameData.OnDestroy += GameDataOnDestroy;

            Database.Initialize();

            // Plugin startup logic
            Log.LogInfo($"Plugin {MyPluginInfo.PLUGIN_GUID} is loaded!");
        }

        private static void GameDataOnInitialize(World world)
        {
            Logger.LogInfo("GameDataOnInitialize");
        }

        private static void GameDataOnDestroy()
        {
            Logger.LogInfo("GameDataOnDestroy");
        }

        public void OnGameInitialized()
        {
            Logger.LogInfo("OnGameInitialized");
        }
    }
}
