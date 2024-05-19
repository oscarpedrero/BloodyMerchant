using Bloodstone.API;
using BloodyMerchant.DB;
using BloodyMerchant.DB.Models;
using BloodyMerchant.Exceptions;
using ProjectM;
using System;
using System.Linq;
using Unity.Entities;
using Unity.Transforms;
using VampireCommandFramework;

namespace BloodyMerchant.Commands
{
    [CommandGroup("bm")]
    internal class MerchantCommand
    {
        [Command("list", usage: "", description: "List of merchant", adminOnly: true)]
        public void ListMerchant(ChatCommandContext ctx)
        {

            var merchants = Database.Merchants.ToList();

            if(merchants.Count == 0)
            {
                throw ctx.Error($"There are no merchants created");
            }
            ctx.Reply($"Merchant List");
            ctx.Reply($"----------------------------");
            ctx.Reply($"--");
            foreach ( var merchant in merchants)
            {
                ctx.Reply($"Merchant {merchant.name}");
                ctx.Reply($"--");
            }
            ctx.Reply($"----------------------------");
        }

        [Command("create", usage: "<NameOfMerchant> [PrefabGUIDOfMerchant] [Immortal] [Move] [Autorespawn]", description: "Create a merchant", adminOnly: true)]
        public void CreateMerchant(ChatCommandContext ctx, string merchantName, int prefabGUIDOfMerchant = -1810631919, bool immortal = false, bool canMove = true, bool autorespawn = true)
        {
            try
            {
                var merchatValid = Database.ValidMerchantIds.Where(x=> x == prefabGUIDOfMerchant).ToList();

                if(merchatValid.Count <= 0)
                {
                    throw ctx.Error($"PrefabGUIDOfMerchant not valid");
                }

                if (Database.AddMerchant(merchantName, prefabGUIDOfMerchant, immortal, canMove, autorespawn))
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
            catch (MerchantEnableException)
            {
                throw ctx.Error($"The merchant is in the world, you must kill the merchant with the command .merchant kill {merchantName}");
            }
            catch (Exception e)
            {
                throw ctx.Error($"Error: {e.Message}");
            }

        }

        // .merchant spawn Test1
        [Command("spawn", usage: "<NameOfMerchant>", description: "Spawn a merchant in your location", adminOnly: true)]
        public void Spawn(ChatCommandContext ctx, string merchantName)
        {
            try
            {
                if (Database.GetMerchant(merchantName, out MerchantModel merchant))
                {
                    Entity user = ctx.Event.SenderUserEntity;
                    var pos = VWorld.Server.EntityManager.GetComponentData<LocalToWorld>(user).Position;
                    merchant.SpawnWithLocation(user, pos);
                    ctx.Reply($"Merchant '{merchantName}' has spawned correctly");
                } else
                {
                    throw new MerchantDontExistException();
                }
            }
            catch (MerchantDontExistException)
            {
                throw ctx.Error($"Merchant with name '{merchantName}' does not exist.");
            }
            catch (MerchantEnableException)
            {
                throw ctx.Error($"Merchant with name '{merchantName}' already in world.");
            }
            catch (Exception e)
            {
                throw ctx.Error($"Error: {e.Message}");
            }
        }

        // .merchant kill Test1
        [Command("kill", usage: "<NameOfMerchant>", description: "Kill a merchant", adminOnly: true)]
        public void Kill(ChatCommandContext ctx, string merchantName)
        {
            try
            {
                if (Database.GetMerchant(merchantName, out MerchantModel merchant))
                {
                    Entity user = ctx.Event.SenderUserEntity;
                    merchant.KillMerchant(user);
                    ctx.Reply($"Merchant '{merchantName}' has killed correctly");
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
            catch (MerchantDontEnableException)
            {
                throw ctx.Error($"Merchant with name '{merchantName}' does not exist in world.");
            }
            catch (Exception e)
            {
                throw ctx.Error($"Error: {e.Message}");
            }

        }
    }
}
