using System;

namespace CertiPay.Common.WorkQueue
{
    public class CompletedWorkItem<T>
    {
        public T WorkItem { get; set; }

        public String Server { get; set; }

        public DateTime CompletedAt { get; set; }

        // TODO Version?

        public CompletedWorkItem()
        {
            this.Server = System.Environment.MachineName;
            this.CompletedAt = DateTime.UtcNow;
        }
    }
}