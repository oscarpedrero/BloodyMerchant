using BepInEx;
using BloodyMerchant.DB.Models;
using Steamworks;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Text.Json;
using System.Threading.Tasks;
using VRising.GameData;

namespace BloodyMerchant.DB
{

    

    internal class Database
    {

        public static readonly string ConfigPath = Path.Combine(Paths.ConfigPath, "BloodyShop");
        public static string MerchantListFile = Path.Combine(ConfigPath, "merchants.json");

        public static List<MerchantModel> Merchants { get; set; } = new();

        public static void Initialize()
        {
            createDatabaseFiles();
        }

        public static bool saveDatabase()
        {
            try
            {
                var jsonOutPut = JsonSerializer.Serialize(Merchants);
                File.WriteAllText(MerchantListFile, jsonOutPut);
                Plugin.Logger.LogInfo($"Save Database: OK");
                return true;
            }
            catch (Exception error)
            {
                Plugin.Logger.LogError($"Error SaveDatabase: {error.Message}");
                return false;
            }
        }

        public static bool loadDatabase()
        {
            try
            {
                string json = File.ReadAllText(MerchantListFile);
                Merchants = JsonSerializer.Deserialize<List<MerchantModel>>(json);
                Plugin.Logger.LogInfo($"Load Database: OK");
                return true;
            } catch (Exception error)
            {
                Plugin.Logger.LogError($"Error LoadDatabase: {error.Message}");
                return false;
            }
        }

        public static bool createDatabaseFiles()
        {
            if (!Directory.Exists(ConfigPath)) Directory.CreateDirectory(ConfigPath);
            if (!File.Exists(MerchantListFile)) File.WriteAllText(MerchantListFile, "[]");
            Plugin.Logger.LogInfo($"Create Database: OK");
            return true;
        }
    }
}
