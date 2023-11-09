using BloodyMerchant.Exceptions;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BloodyMerchant.DB.Models
{
    internal class MerchantModel
    {
        public string name { get; set; } = string.Empty;
        public List<ItemModel> items { get; set; } = new();

        public List<TraderItem> GetTraderItems()
        {
            var list = new List<TraderItem>();
            foreach (var item in items)
            {
                var itemTrader = new TraderItem(
                    outputItem: new(item.OutputItem),
                    outputAmount: item.OutputAmount,
                    inputItem: new(item.InputItem),
                    inputAmount: item.InputAmount,
                    stockAmount: item.StockAmount
                    );
                list.Add(itemTrader);
            }
            return list;
        }

        public bool GetProduct(int itemPrefabID, int CurrencyfabID, out ItemModel item)
        {
            item = items.Where(x => x.InputItem == CurrencyfabID && x.InputItem == itemPrefabID).FirstOrDefault();
            if(item == null)
            {
                return false;
            }
            return true;
        }

        public bool AddProduct(int ItemPrefabID, int CurrencyfabID, int Stack, int Price, int Amount)
        {
            if(!GetProduct(ItemPrefabID, CurrencyfabID, out ItemModel item))
            {
                item = new ItemModel(ItemPrefabID, Stack, CurrencyfabID, Price, Amount);
                items.Add(item);
                Database.saveDatabase();
                return true;
            }

            throw new ProductExistException();
            
        }

        public bool RemoveProduct(int ItemPrefabID, int CurrencyfabID) 
        {
            if (GetProduct(ItemPrefabID, CurrencyfabID, out ItemModel item))
            {
                items.Remove(item);
                Database.saveDatabase();
                return true;
            }

            throw new ProductDontExistException();
        }

        public bool Spawn()
        {
            return true;
        }
        
    }
}
