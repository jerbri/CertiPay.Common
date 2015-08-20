using CertiPay.Common.Logging;
using NUnit.Framework;

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

        [Test]
        public void Can_Include_Context()
        {
            LogManager
                .GetCurrentClassLogger()
                .WithContext("Test", true)
                .WithContext("OtherContext", new { id = 1, test = false }, destructureObjects: true)
                .Fatal("An error occured with context!");

            LogManager
                .GetCurrentClassLogger()
                .Fatal("An error occured without context!");
        }
    }
}