using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BloodyMerchant.DB.Models
{
    internal class ConfigMerchantModel
    {
        public bool IsEnabled { get; set; } = false;
        public float x { get; set; } = 0;
        public float z { get; set; } = 0;
        public bool Immortal { get; set; } = false;
        public bool CanMove { get; set; } = false;
        public bool Autorepawn { get; set; } = false;

    }
}
