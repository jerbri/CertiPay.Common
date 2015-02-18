using CertiPay.Common.Logging;
using System;
using System.Collections.Generic;
using System.Linq;
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

        /// <summary>
        /// A list of domains that we will allow emails to go to from outside of the production environment
        /// </summary>
        public IEnumerable<String> AllowedTestingDomains { get; set; }

        private readonly SmtpClient _smtp;

        public EmailService()
            : this(new SmtpClient())
        {
        }

        public EmailService(SmtpClient smtp)
        {
            this._smtp = smtp;

            this.AllowedTestingDomains = new[] { "certipay.com", "certigy.com" };
        }

        public async Task SendAsync(EmailNotification notification)
        {
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
                    // TODO Download any requested attachments
                }

                await SendAsync(msg);
            }
        }

        public void Send(MailMessage message)
        {
            // TODO More logging!

            this.FilterRecipients(message.To);
            this.FilterRecipients(message.CC);
            this.FilterRecipients(message.Bcc);

            Log.Debug("Email Message: From {0}", message.From);
            Log.Debug("Email Message: TO {0}", message.To);
            Log.Debug("Email Message: CC {0}", message.CC);
            Log.Debug("Email Message: BCC {0}", message.Bcc);
            Log.Debug("Email Message: Subject {0}", message.Subject);
            Log.Debug("Email Message: Body {0}", message.Body);

            _smtp.Send(message);

            // TODO Catch/Handle exceptions or not?
        }

        public async Task SendAsync(MailMessage message)
        {
            // TODO More logging!

            this.FilterRecipients(message.To);
            this.FilterRecipients(message.CC);
            this.FilterRecipients(message.Bcc);

            Log.Debug("Email Message: From {0}", message.From);
            Log.Debug("Email Message: TO {0}", message.To);
            Log.Debug("Email Message: CC {0}", message.CC);
            Log.Debug("Email Message: BCC {0}", message.Bcc);
            Log.Debug("Email Message: Subject {0}", message.Subject);
            Log.Debug("Email Message: Body {0}", message.Body);

            await _smtp.SendMailAsync(message);

            // TODO Catch/Handle exceptions or not?
        }

        public virtual void FilterRecipients(MailAddressCollection addresses)
        {
            if (EnvUtil.IsProd)
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

        public void AttachUrl(MailMessage msg, EmailNotification.Attachment attachment)
        {
            // TODO Download and attach the file at the Uri
        }
    }
}