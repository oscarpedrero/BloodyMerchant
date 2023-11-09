using BloodyMerchant.DB.Models;
using BloodyMerchant.DB;
using ProjectM;
using System.Linq;
using VampireCommandFramework;
using BloodyMerchant.Exceptions;
using System;

namespace BloodyMerchant.Commands
{

    [CommandGroup("merchant product")]
    internal class ProductsCommand
    {
        [Command("add", usage: "<NameOfMerchant> <ItemPrefabID> <CurrencyfabID> <Stack> <Price> <Amount> ", description: "Add a product to a merchant", adminOnly: true)]
        public void CreateProduct(ChatCommandContext ctx, string merchantName, int ItemPrefabID, int CurrencyfabID, int Stack, int Price, int Amount)
        {
            try
            {
                if(Database.GetMerchant(merchantName, out MerchantModel merchant))
                {
                    merchant.AddProduct(ItemPrefabID, Stack, CurrencyfabID, Price, Amount);
                    ctx.Reply($"Product successfully added to merchant '{merchantName}'");
                }
            }
            catch (MerchantDontExistException)
            {
                throw ctx.Error($"Merchant with name '{merchantName}' does not exist.");
            } 
            catch (ProductExistException)
            {
                throw ctx.Error($"This product configuration already exists at merchant '{merchantName}'");
            } 
            catch (Exception e)
            {
                throw ctx.Error($"Error: {e.Message}");
            }

            
        }

        [Command("remove", usage: "<NameOfMerchant> <ItemPrefabID> <CurrencyfabID>", description: "Remove a product to a merchant", adminOnly: true)]
        public void RemoveProduct(ChatCommandContext ctx, string merchantName, int ItemPrefabID, int CurrencyfabID)
        {
            try
            {
                if (Database.GetMerchant(merchantName, out MerchantModel merchant))
                {
                    merchant.RemoveProduct(ItemPrefabID, CurrencyfabID);
                    ctx.Reply($"Merchant '{merchantName}'\'s product has been successfully removed");
                }
            }
            catch (MerchantDontExistException)
            {
                throw ctx.Error($"Merchant with name '{merchantName}' does not exist.");
            }
            catch (ProductDontExistException)
            {
                throw ctx.Error($"This product does not exist at merchant '{merchantName}'");
            }
            catch (Exception e)
            {
                throw ctx.Error($"Error: {e.Message}");
            }

            
        }
    }
}
