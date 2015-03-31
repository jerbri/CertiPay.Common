using CertiPay.Common.Logging;
using NUnit.Framework;

namespace CertiPay.Common.Tests.Logging
{
    public class LogManagerTests
    {
        [Test]
        public void Ensure_Can_Write_To_Rolling_File()
        {
            LogManager.GetCurrentClassLogger().Info("This is some basic text output!");
        }
    }
}