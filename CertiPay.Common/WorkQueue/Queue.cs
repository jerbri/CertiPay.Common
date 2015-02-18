using System;

namespace CertiPay.Common.WorkQueue
{
    public class Queue
    {
        public const String QueuedNamespace = "Queued|";

        public const String ProcessedNamespace = "Processed|";

        public const String FailedNamespace = "Failed|";

        public String Name { get; set; }

        public long Queued { get; set; }

        public long Processed { get; set; }

        public long Failed { get; set; }

        // ------- Names of work queues will go here for strongly-typed references -------

        public const String SMSNotifications = "SMSNotifications";

        public const String EmailNotifications = "EmailNotifications";
    }
}