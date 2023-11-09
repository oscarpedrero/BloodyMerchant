using BloodyMerchant.DB;
using BloodyMerchant.DB.Models;
using BloodyMerchant.Exceptions;
using ProjectM;
using System;
using System.Linq;
using VampireCommandFramework;

namespace BloodyMerchant.Commands
{
    [CommandGroup("merchant")]
    internal class MerchantCommand
    {
        [Command("create", usage: "<NameOfMerchant>", description: "Create a merchant", adminOnly: true)]
        public void CreateMerchant(ChatCommandContext ctx, string merchantName)
        {

            try
            {
                if (Database.AddMerchant(merchantName))
                {
                    ctx.Reply($"Merchant '{merchantName}' created successfully");
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

        [Command("remove", usage: "<NameOfMerchant>", description: "Remove a merchant", adminOnly: true)]
        public void RemoveMerchant(ChatCommandContext ctx, string merchantName)
        {

            try
            {
                if (Database.RemoveMerchant(merchantName))
                {
                    ctx.Reply($"Merchant '{merchantName}' remove successfully");
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

        [Command("spawn", usage: "<NameOfMerchant>", description: "Spawn a merchant in your location", adminOnly: true)]
        public void Spawn(ChatCommandContext ctx, string merchantName, int ItemPrefabID, int CurrencyfabID, int Stack, int Price, int Amount)
        {
            ctx.Reply($"product successfully added to merchant '{merchantName}'");
        }

        [Command("kill", usage: "<NameOfMerchant>", description: "Kill a merchant", adminOnly: true)]
        public void Kill(ChatCommandContext ctx, string merchantName, int ItemPrefabID, int CurrencyfabID, int Stack, int Price, int Amount)
        {
            ctx.Reply($"product successfully added to merchant '{merchantName}'");
        }
    }
}
