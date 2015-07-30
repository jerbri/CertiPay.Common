using CertiPay.Common.Logging;
using NUnit.Framework;
using Serilog;
using System.Configuration;

namespace CertiPay.Common.Tests.Logging
{
    public class LogManagerTests
    {
        [Test]
        public void Ensure_Can_Write_To_Rolling_File()
        {
            LogManager.GetCurrentClassLogger().Warn("This is some basic text output!");
        }

        [Test]
        public void Ensure_Can_Write_To_Email_Sinks()
        {
            LogManager.GetCurrentClassLogger().Fatal("An error occurred while running this test!");
        }
    }
}