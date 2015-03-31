using CertiPay.Common.Logging;
using NUnit.Framework;
using Serilog;
using Serilog.Sinks.Email;

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
            SerilogManager.Configuration =
                SerilogManager
                .Configuration
                .WriteTo
                .Email(new EmailConnectionInfo
                {
                    FromEmail = "Errors@Certipay.com",
                    ToEmail = "Errors@CertiPay.com",
                });

            LogManager.GetCurrentClassLogger().Error("An error occurred while running this test!");
        }
    }
}