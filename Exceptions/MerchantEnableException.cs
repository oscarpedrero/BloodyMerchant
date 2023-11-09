using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.Text;
using System.Threading.Tasks;

namespace BloodyMerchant.Exceptions
{
    internal class MerchantEnableException : Exception
    {
        public MerchantEnableException()
        {
        }

        public MerchantEnableException(string message)
            : base(message)
        {
        }

        public MerchantEnableException(string message, Exception innerException)
            : base(message, innerException)
        {
        }

        protected MerchantEnableException(SerializationInfo info, StreamingContext context)
            : base(info, context)
        {
        }
    }
}
