using ProjectM;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BloodyMerchant.DB.Models
{
    public class ItemModel
    {
        public int OutputItem { get; set; }
        public int OutputAmount { get; set; }
        public int InputItem { get; set; }
        public int InputAmount { get; set; }
        public int StockAmount { get; set; }

        public ItemModel(int outputItem, int outputAmount, int inputItem, int inputAmount, int stockAmount)
        {
            OutputItem = outputItem;
            OutputAmount = outputAmount;
            InputItem = inputItem;
            InputAmount = inputAmount;
            StockAmount = stockAmount;
        }
    }
}
