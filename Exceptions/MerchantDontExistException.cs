using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.Text;
using System.Threading.Tasks;

namespace BloodyMerchant.Exceptions
{
    internal class MerchantDontExistException : Exception
    {
        public MerchantDontExistException()
        {
        }

        public MerchantDontExistException(string message)
            : base(message)
        {
        }

        public MerchantDontExistException(string message, Exception innerException)
            : base(message, innerException)
        {
        }

        protected MerchantDontExistException(SerializationInfo info, StreamingContext context)
            : base(info, context)
        {
        }
    }
}
