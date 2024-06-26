﻿using BepInEx;
using BloodyMerchant.DB.Models;
using BloodyMerchant.Exceptions;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text.Json;
using Bloody.Core;

namespace BloodyMerchant.DB
{
    internal class Database
    {

        public static readonly string ConfigPath = Path.Combine(Paths.ConfigPath, "BloodyMerchant");
        public static string MerchantListFile = Path.Combine(ConfigPath, "merchants.json");

        public static List<MerchantModel> Merchants { get; set; } = new();
        public static List<int> ValidMerchantIds { get; set; } = new List<int> {
            194933933,
            233171451,
            281572043,
            -1594911649,
            -1168705805,
            -375258845,
            -208499374,
            -1810631919,
            -1292194494,
            1631713257,
            345283594,
            -1990875761,
            1687896942,
            -915182578,
            739223277
            }
        ;

        public static void Initialize()
        {
            createDatabaseFiles();
            loadDatabase();
        }

        public static bool saveDatabase()
        {
            try
            {
                var jsonOutPut = JsonSerializer.Serialize(Merchants, new JsonSerializerOptions { WriteIndented = true });
                File.WriteAllText(MerchantListFile, jsonOutPut);
                Plugin.Logger.LogDebug($"Save Database: OK");
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
                Plugin.Logger.LogDebug($"Load Database: OK");
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
            Plugin.Logger.LogDebug($"Create Database: OK");
            return true;
        }

        public static bool GetMerchant(string MerchantName, out MerchantModel merchant)
        {
            merchant = Merchants.Where(x => x.name == MerchantName).FirstOrDefault();
            if (merchant == null)
            {
                return false;
            }
            return true;
        }

        public static bool AddMerchant(string MerchantName, int prefabGUIDOfMerchant, bool immortal, bool canMove, bool autoRespawn)
        {
            if (GetMerchant(MerchantName, out MerchantModel merchant))
            {
                throw new MerchantExistException();
            }

            merchant = new MerchantModel();
            merchant.name = MerchantName;
            merchant.PrefabGUID = prefabGUIDOfMerchant;
            merchant.config.Immortal = immortal;
            merchant.config.CanMove = canMove;
            merchant.config.Autorepawn = autoRespawn;

            Merchants.Add(merchant);
            saveDatabase();
            return true;

        }

        public static bool RemoveMerchant(string MerchantName)
        {
            if (GetMerchant(MerchantName, out MerchantModel merchant))
            {
                if(merchant.config.IsEnabled)
                {
                    throw new MerchantEnableException();
                }

                Merchants.Remove(merchant);
                saveDatabase();
                return true;
            }

            throw new MerchantDontExistException();
        }
    }
}
