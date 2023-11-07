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
        public List<TraderItem> items { get; set; } = new();

        public bool Spawn()
        {
            return true;
        }
        
    }
}
