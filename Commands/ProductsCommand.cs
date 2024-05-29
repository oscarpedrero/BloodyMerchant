using BloodyMerchant.DB.Models;
using BloodyMerchant.DB;
using ProjectM;
using System.Linq;
using VampireCommandFramework;
using BloodyMerchant.Exceptions;
using System;

namespace BloodyMerchant.Commands
{

    [CommandGroup("bm product")]
    internal class ProductsCommand
    {

        [Command("list", usage: "", description: "List product of merchant", adminOnly: true)]
        public void ListMerchant(ChatCommandContext ctx, string merchantName)
        {

            try
            {
                if (Database.GetMerchant(merchantName, out MerchantModel merchant))
                {
                    ctx.Reply($"{merchant.name} Products List");
                    ctx.Reply($"----------------------------");
                    ctx.Reply($"--");
                    foreach (var item in merchant.items)
                    {
                        ctx.Reply($"Currency {item.InputItem}");
                        ctx.Reply($"Item {item.OutputItem}");
                        ctx.Reply($"Price {item.InputAmount}");
                        ctx.Reply($"Stack {item.OutputAmount}");
                        ctx.Reply($"Stock {item.StockAmount}");
                        ctx.Reply($"--");
                    }
                    ctx.Reply($"----------------------------");
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
            catch (ProductExistException)
            {
                throw ctx.Error($"This product configuration already exists at merchant '{merchantName}'");
            }
            catch (Exception e)
            {
                throw ctx.Error($"Error: {e.Message}");
            }

            
            
        }

        // .merchant product add Test1 736318803 -257494203 20 1 50
        [Command("clean", usage: "<NameOfMerchant>", description: "Clean all products to a merchant", adminOnly: true)]
        public void CleanProducts(ChatCommandContext ctx, string merchantName)
        {
            try
            {
                if(Database.GetMerchant(merchantName, out MerchantModel merchant))
                {
                    merchant.Clean();
                    ctx.Reply($"Product successfully added to merchant '{merchantName}'");
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
            catch (ProductExistException)
            {
                throw ctx.Error($"This product configuration already exists at merchant '{merchantName}'");
            } 
            catch (Exception e)
            {
                throw ctx.Error($"Error: {e.Message}");
            }

            
        }

        // .merchant product add Test1 736318803 -257494203 20 1 50
        [Command("add", usage: "<NameOfMerchant> <ItemPrefabID> <CurrencyfabID> <Stack> <Price> <Stock> [Autorefill true/false]", description: "Add a product to a merchant", adminOnly: true)]
        public void CreateProduct(ChatCommandContext ctx, string merchantName, int ItemPrefabID, int CurrencyfabID, int Stack, int Price, int Stock, bool Autorefill = false)
        {
            try
            {
                if(Database.GetMerchant(merchantName, out MerchantModel merchant))
                {
                    merchant.AddProduct(ItemPrefabID, CurrencyfabID, Stack, Price, Stock, Autorefill);
                    if (merchant.config.IsEnabled)
                    {
                        merchant.ModifyMerchant(ctx.Event.SenderUserEntity, merchant.merchantEntity);
                    }
                    ctx.Reply($"Product successfully added to merchant '{merchantName}'");
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
            catch (ProductExistException)
            {
                throw ctx.Error($"This product configuration already exists at merchant '{merchantName}'");
            } 
            catch (Exception e)
            {
                throw ctx.Error($"Error: {e.Message}");
            }

            
        }

        // .merchant product remove Test1 736318803 -257494203
        [Command("remove", usage: "<NameOfMerchant> <ItemPrefabID>", description: "Remove a product to a merchant", adminOnly: true)]
        public void RemoveProduct(ChatCommandContext ctx, string merchantName, int ItemPrefabID)
        {
            try
            {
                if (Database.GetMerchant(merchantName, out MerchantModel merchant))
                {
                    merchant.RemoveProduct(ItemPrefabID);
                    if (merchant.config.IsEnabled)
                    {
                        merchant.ModifyMerchant(ctx.Event.SenderUserEntity, merchant.merchantEntity);
                    }
                    ctx.Reply($"Merchant '{merchantName}'\'s product has been successfully removed");
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
