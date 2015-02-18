using System;
using System.Collections.Generic;

namespace CertiPay.Common.Notifications
{
    public class Notification
    {
        public String Content { get; set; }

        public ICollection<String> Recipients { get; set; }

        public Notification()
        {
            this.Recipients = new List<String>();
        }
    }
}