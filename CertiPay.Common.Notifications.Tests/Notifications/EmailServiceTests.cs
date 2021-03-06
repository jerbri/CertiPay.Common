﻿using CertiPay.Common.Notifications;
using NUnit.Framework;
using System;
using System.Net.Mail;
using CertiPay.Common;

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

        [TestCase("mattgwagner@gmail.com")]
        [TestCase("jsmith@worksafepays.com")]
        [TestCase("jsmith@certipay.com")]
        [TestCase("jeremy@jeremyandchana.com")]        
        public void Should_Allow_Any_Email_When_Testing_Domains_Disabled(string email)
        {

            EmailService.AllowedTestingDomainsEnabled = true;

            IEmailService emailer = new EmailService(new SmtpClient());

            using (var msg = new MailMessage { From = new MailAddress("test@test.com") })
            {
                msg.To.Add(email);

                emailer.Send(msg);
            }

        }

        [TestCase("mattgwagner@gmail.com")]
        [TestCase("jsmith@worksafepays.com")]
        [TestCase("jsmith@certipay.com")]
        [TestCase("jeremy@jeremyandchana.com")]
        [ExpectedException(ExpectedException = typeof(InvalidOperationException), ExpectedMessage = "A recipient must be specified.")]
        public void Should_Not_Allow_Any_Email_When_Testing_Domains_Enabled(string email)
        {

            EmailService.AllowedTestingDomainsEnabled = false;

            IEmailService emailer = new EmailService(new SmtpClient());

            using (var msg = new MailMessage { From = new MailAddress("test@test.com") })
            {
                msg.To.Add(email);

                emailer.Send(msg);
            }

        }

    }
}