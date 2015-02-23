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
    }
}