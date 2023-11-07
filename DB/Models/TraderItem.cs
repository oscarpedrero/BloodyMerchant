using ProjectM;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BloodyMerchant.DB.Models
{
    public struct TraderItem
    {
        public PrefabGUID OutputItem { get; set; }
        public int OutputAmount { get; set; }
        public PrefabGUID InputItem { get; set; }
        public int InputAmount { get; set; }
        public int StockAmount { get; set; }

        public TraderItem(PrefabGUID outputItem, int outputAmount, PrefabGUID inputItem, int inputAmount, int stockAmount)
        {
            OutputItem = outputItem;
            OutputAmount = outputAmount;
            InputItem = inputItem;
            InputAmount = inputAmount;
            StockAmount = stockAmount;
        }
    }
}
