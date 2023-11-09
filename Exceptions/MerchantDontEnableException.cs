using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.Text;
using System.Threading.Tasks;

namespace BloodyMerchant.Exceptions
{
    internal class MerchantDontEnableException : Exception
    {
        public MerchantDontEnableException()
        {
        }

        public MerchantDontEnableException(string message)
            : base(message)
        {
        }

        public MerchantDontEnableException(string message, Exception innerException)
            : base(message, innerException)
        {
        }

        protected MerchantDontEnableException(SerializationInfo info, StreamingContext context)
            : base(info, context)
        {
        }
    }
}
