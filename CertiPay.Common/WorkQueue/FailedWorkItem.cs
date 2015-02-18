using System;

namespace CertiPay.Common.WorkQueue
{
    public class FailedWorkItem<T> : CompletedWorkItem<T>
    {
        public String ErrorMessage { get; set; }

        // public int RetryCount { get; set; }

        // public int RetryLimit { get; set; }
    }
}