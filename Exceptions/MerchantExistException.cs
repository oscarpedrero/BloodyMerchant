using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.Text;
using System.Threading.Tasks;

namespace BloodyMerchant.Exceptions
{
    internal class MerchantExistException : Exception
    {
        public MerchantExistException()
        {
        }

        public MerchantExistException(string message)
            : base(message)
        {
        }

        public MerchantExistException(string message, Exception innerException)
            : base(message, innerException)
        {
        }

        protected MerchantExistException(SerializationInfo info, StreamingContext context)
            : base(info, context)
        {
        }
    }
}
