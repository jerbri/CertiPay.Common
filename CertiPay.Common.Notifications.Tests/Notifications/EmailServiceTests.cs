using CertiPay.Common.Notifications;
using NUnit.Framework;
using System;
using System.Net.Mail;

namespace CertiPay.Services.Notifications
{
    public class EmailServiceTests
    {
        [Test]
        [TestCase("mattgwagner@gmail.com")]
        [TestCase("mwagner@certipayyyy.com")]
        [TestCase("IAmACustomer@Hotmail.com")]
        [ExpectedException(ExpectedException = typeof(InvalidOperationException), ExpectedMessage = "A recipient must be specified.")]
        public void Should_Not_Email_Outsiders_NonProd(String email)
        {
            IEmailService emailer = new EmailService(new System.Net.Mail.SmtpClient());

            using (var msg = new MailMessage { From = new MailAddress("test@test.com") })
            {
                msg.To.Add(email);

                emailer.Send(msg);
            }
        }
    }
}