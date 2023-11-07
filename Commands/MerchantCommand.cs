using BloodyMerchant.DB;
using BloodyMerchant.DB.Models;
using VampireCommandFramework;

namespace BloodyMerchant.Commands
{
    [CommandGroup("merchant")]
    internal class MerchantCommand
    {
        [Command("create", usage: "<NameOfMerchant>", description: "Create a merchant", adminOnly: true)]
        public void CreateMerchant(ChatCommandContext ctx, string merchantName)
        {
            var merchant = new MerchantModel();
            merchant.name = merchantName;

            Database.Merchants.Add(merchant);

            Database.saveDatabase();

            ctx.Reply($"Merchant '{merchantName}' created successfully");
        }

        [Command("product add", usage: "<NameOfMerchant> <ItemPrefabID> <CurrencyfabID> <Stack> <Price> <Amount> ", description: "Add a product to a merchant", adminOnly: true)]
        public void CreateProduct(ChatCommandContext ctx, string merchantName, int ItemPrefabID, int CurrencyfabID, int Stack, int Price, int Amount)
        {
            ctx.Reply($"product successfully added to merchant '{merchantName}'");
        }
    }
}
