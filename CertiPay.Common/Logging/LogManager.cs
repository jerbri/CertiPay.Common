using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CertiPay.Common.Logging
{
    public static class LogManager
    {
        public static ILog GetLogger<T>()
        {
            throw new NotImplementedException();
        }

        public static ILog GetLogger(Type type)
        {
            throw new NotImplementedException();
        }

        public static ILog GetLogger(String key)
        {
            throw new NotImplementedException();
        }
    }
}
