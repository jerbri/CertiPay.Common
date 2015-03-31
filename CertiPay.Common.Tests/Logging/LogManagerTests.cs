using CertiPay.Common.Logging;
using NUnit.Framework;
using Serilog;
using Serilog.Events;
using Serilog.Sinks.Email;

namespace CertiPay.Common.Tests.Logging
{
    public class LogManagerTests
    {
        [Test]
        public void Ensure_Can_Write_To_Rolling_File()
        {
            LogManager.GetCurrentClassLogger().Info("This is some basic text output!");
        }

        [Test]
        public void Ensure_Can_Write_To_Alternate_Sinks()
        {
            SerilogManager.Configuration =
                SerilogManager
                .Configuration
                .WriteTo
                .Email(new EmailConnectionInfo
                {
                    MailServer = "localhost",
                    FromEmail = "Errors@Certipay.com",
                    ToEmail = "Errors@CertiPay.com",
                },
                restrictedToMinimumLevel: LogEventLevel.Error);

            LogManager.GetCurrentClassLogger().Error("An error occurred while running this test!");
        }
    }
}