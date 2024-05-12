using BloodyMerchant.DB;
using BloodyMerchant.DB.Models;
using BloodyMerchant.Exceptions;
using ProjectM;
using Stunlock.Core;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;
using VampireCommandFramework;

namespace BloodyMerchant.Commands
{
    [CommandGroup("merchant config")]
    internal class ConfigCommand
    {
        [Command("show", usage: "<NameOfMerchant>", description: "Show config of merchant", adminOnly: true)]
        public void ShowConfigMerchant(ChatCommandContext ctx, string merchantName)
        {
            try
            {
                if (Database.GetMerchant(merchantName, out MerchantModel merchant))
                {
                    ctx.Reply($"Merchant '{merchantName}' config:");
                    ctx.Reply($"----------------------------------------------");
                    ctx.Reply($"Immortal: '{merchant.config.Immortal}'");
                    ctx.Reply($"Move: '{merchant.config.CanMove}'");
                    ctx.Reply($"Autorespawn '{merchant.config.Autorepawn}' config:");
                }
            }
            catch (MerchantExistException)
            {
                throw ctx.Error($"Merchant with name '{merchantName}' exist.");
            }
            catch (Exception e)
            {
                throw ctx.Error($"Error: {e.Message}");
            }
        }

        [Command("immortal", usage: "<NameOfMerchant> <true/false>", description: "Set config immortal of merchant", adminOnly: true)]
        public void InmortalMerchant(ChatCommandContext ctx, string merchantName, bool value)
        {
            try
            {
                if (Database.GetMerchant(merchantName, out MerchantModel merchant))
                {
                    merchant.config.Immortal = value;
                    if (value)
                    {
                        merchant.MakeNPCImmortal(ctx.Event.SenderUserEntity, merchant.merchantEntity);
                    } else
                    {
                        merchant.MakeNPCMortal(ctx.Event.SenderUserEntity, merchant.merchantEntity);
                    }

                    Database.saveDatabase();
                    ctx.Reply($"Merchant '{merchantName}' immortal {value}");
                }
                else
                {
                    throw new MerchantDontExistException();
                }
            }
            catch (MerchantDontExistException)
            {
                throw ctx.Error($"Merchant with name '{merchantName}' does not exist.");
            }
            catch (Exception e)
            {
                throw ctx.Error($"Error: {e.Message}");
            }
        }

        [Command("move", usage: "<NameOfMerchant> <true/false>", description: "Set config move of merchant", adminOnly: true)]
        public void MoveMerchant(ChatCommandContext ctx, string merchantName, bool value)
        {
            try
            {
                if (Database.GetMerchant(merchantName, out MerchantModel merchant))
                {
                    merchant.config.CanMove = value;
                    if (value)
                    {
                        merchant.merchantEntity.Add<MoveEntity>();
                        Plugin.Logger.LogDebug($"NPC Add Move");
                    }
                    else
                    {
                        merchant.merchantEntity.Remove<MoveEntity>();
                        Plugin.Logger.LogDebug($"NPC Remove Move");
                    }

                    Database.saveDatabase();

                    ctx.Reply($"Merchant '{merchantName}' move {value}");
                }
                else
                {
                    throw new MerchantDontExistException();
                }
            }
            catch (MerchantDontExistException)
            {
                throw ctx.Error($"Merchant with name '{merchantName}' does not exist.");
            }
            catch (Exception e)
            {
                throw ctx.Error($"Error: {e.Message}");
            }
        }

        [Command("enabled", usage: "<NameOfMerchant> <true/false>", description: "Enable merchant", adminOnly: true)]
        public void EnableMerchant(ChatCommandContext ctx, string merchantName, bool value)
        {
            try
            {
                if (Database.GetMerchant(merchantName, out MerchantModel merchant))
                {
                    merchant.config.IsEnabled = value;

                    Database.saveDatabase();
                    ctx.Reply($"Merchant '{merchantName}' enabled {value}");
                }
                else
                {
                    throw new MerchantDontExistException();
                }
            }
            catch (MerchantDontExistException)
            {
                throw ctx.Error($"Merchant with name '{merchantName}' does not exist.");
            }
            catch (Exception e)
            {
                throw ctx.Error($"Error: {e.Message}");
            }
        }

        [Command("autorespawn", usage: "<NameOfMerchant> <true/false>", description: "Autorespawn of merchant", adminOnly: true)]
        public void AutorespawnMerchant(ChatCommandContext ctx, string merchantName, bool value)
        {
            try
            {
                if (Database.GetMerchant(merchantName, out MerchantModel merchant))
                {
                    merchant.config.Autorepawn = value;

                    Database.saveDatabase();
                    ctx.Reply($"Merchant '{merchantName}' autorespawn {value}");
                }
                else
                {
                    throw new MerchantDontExistException();
                }
            }
            catch (MerchantDontExistException)
            {
                throw ctx.Error($"Merchant with name '{merchantName}' does not exist.");
            }
            catch (Exception e)
            {
                throw ctx.Error($"Error: {e.Message}");
            }
        }
    }
}
