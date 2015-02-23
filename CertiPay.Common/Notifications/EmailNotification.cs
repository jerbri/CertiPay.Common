using System;
using System.Collections.Generic;

namespace CertiPay.Common.Notifications
{
    /// <summary>
    /// Represents an email notification sent a user, employee, or administrator
    /// </summary>
    public class EmailNotification : Notification
    {
        public static String QueueName { get { return "EmailNotifications"; } }

        public String FromAddress { get; set; }

        public ICollection<String> CC { get; set; }

        public ICollection<String> BCC { get; set; }

        public String Subject { get; set; }

        public ICollection<Attachment> Attachments { get; set; }

        public EmailNotification()
        {
            this.Recipients = new List<String>();
            this.Attachments = new List<Attachment>();
            this.CC = new List<String>();
            this.BCC = new List<String>();
        }

        public class Attachment
        {
            public String Filename { get; set; }

            public String Uri { get; set; }
        }
    }
}