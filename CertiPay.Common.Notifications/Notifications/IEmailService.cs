﻿using CertiPay.Common.Logging;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net.Http;
using System.Net.Mail;
using System.Threading.Tasks;

namespace CertiPay.Common.Notifications
{
    /// <summary>
    /// Provide a simple interface for sending email messages, including
    /// filtering blocked emails or preventing test emails from being sent
    /// outside of the company
    /// </summary>
    /// <remarks>
    /// Implementation may be sent into background processing.
    /// </remarks>
    public interface IEmailService : INotificationSender<EmailNotification>
    {
        void Send(MailMessage message);

        Task SendAsync(MailMessage message);
    }

    public class EmailService : IEmailService
    {
        private static readonly ILog Log = LogManager.GetLogger<IEmailService>();

        private readonly SmtpClient _smtp;

        private static TimeSpan _downloadTimeout = TimeSpan.FromMinutes(1);

        private static IEnumerable<String> _testingDomains = new[] { "certipay.com", "certigy.com" };
        private static bool _allowedTestingDomainsEnabled = !EnvUtil.IsProd;

        /// <summary>
        /// The length of time we'll allow for downloading attachments for email notifications
        /// </summary>
        public static TimeSpan DownloadTimeout
        {
            get { return _downloadTimeout; }
            set { _downloadTimeout = value; }
        }

        /// <summary>
        /// A list of domains that we will allow emails to go to from outside of the production environment
        /// </summary>
        public static IEnumerable<String> AllowedTestingDomains
        {
            get { return _testingDomains; }
            set { _testingDomains = value; }
        }

        /// <summary>
        /// Determines if AllowedTestingDomains will be evaluated when sending emails.
        /// </summary>
        /// <remarks>Enabled by default in all environments except production.</remarks>
        public static bool AllowedTestingDomainsEnabled
        {
            get { return _allowedTestingDomainsEnabled; }
            set { _allowedTestingDomainsEnabled = value; }
        }

        public EmailService(SmtpClient smtp)
        {
            this._smtp = smtp;
        }

        public async Task SendAsync(EmailNotification notification)
        {
            using (Log.Timer("EmailService.SendAsync", context: notification))
            using (var msg = new MailMessage { })
            {
                // If no address is provided, it will use the default one from the Smtp config

                if (!String.IsNullOrWhiteSpace(notification.FromAddress))
                {
                    msg.From = new MailAddress(notification.FromAddress);
                }

                foreach (var recipient in notification.Recipients)
                {
                    msg.To.Add(recipient);
                }

                foreach (var recipient in notification.CC)
                {
                    msg.CC.Add(recipient);
                }

                foreach (var recipient in notification.BCC)
                {
                    msg.Bcc.Add(recipient);
                }

                msg.Subject = notification.Subject;
                msg.Body = notification.Content;
                msg.IsBodyHtml = true;

                foreach (var attachment in notification.Attachments)
                {
                    await AttachUrl(msg, attachment);
                }

                await SendAsync(msg);
            }
        }

        public void Send(MailMessage message)
        {
            using (Log.Timer("EmailService.Send", context: ForLog(message)))
            {
                FilterRecipients(message.To);
                FilterRecipients(message.CC);
                FilterRecipients(message.Bcc);

                _smtp.Send(message);

                Log.Info("Sent email {@message}", message);
            }
        }

        public async Task SendAsync(MailMessage message)
        {
            FilterRecipients(message.To);
            FilterRecipients(message.CC);
            FilterRecipients(message.Bcc);

            await _smtp.SendMailAsync(message).ContinueWith(x =>
            {
                Log.Info("Sent email {@message}", ForLog(message));
            }, 
            TaskContinuationOptions.OnlyOnRanToCompletion);
        }

        public virtual void FilterRecipients(MailAddressCollection addresses)
        {
            if (_allowedTestingDomainsEnabled)
            {
                // This is so we don't accidentally send customers emails from non-prod environments

                var to_remove = from email in addresses
                                where !AllowedTestingDomains.Contains(email.Host.ToLower())
                                select email;

                foreach (var address in to_remove.ToList())
                {
                    Log.Info("Filtering address {0} from email from test environment", address);
                    addresses.Remove(address);
                }
            }

            // TODO Check blacklisted email addresses?
        }

        public async Task AttachUrl(MailMessage msg, EmailNotification.Attachment attachment)
        {
            using (HttpClient client = new HttpClient(new HttpClientHandler { AllowAutoRedirect = true }) { Timeout = DownloadTimeout })
            {
                Log.Info("Attaching {@attachment} for {@msg}", attachment, ForLog(msg));

                if (String.IsNullOrWhiteSpace(attachment.Uri))
                {
                    Log.Warn("No Url provided for attachment, skipping");
                    return;
                }

                if (String.IsNullOrWhiteSpace(attachment.Filename))
                {
                    Log.Warn("No filename provided for attachment, using default");
                    attachment.Filename = "Attachment.pdf";
                }

                byte[] data = await client.GetByteArrayAsync(attachment.Uri);

                // MailMessage disposes of the attachment stream when it disposes

                msg.Attachments.Add(new Attachment(new MemoryStream(data), attachment.Filename));

                Log.Info("Completed attachment for {@msg}", ForLog(msg));
            }
        }

        private static object ForLog(MailMessage msg)
        {
            return new
            {
                To = String.Join(",",  msg.To.Select(_ => _.Address)),
                CC = String.Join(",", msg.CC.Select(_ => _.Address)),
                Bcc = String.Join(",", msg.Bcc.Select(_ => _.Address)),
                msg.From,
                msg.Subject
            };
        }
    }
}