using System;

namespace CertiPay.Common.WorkQueue
{
    public class CompletedWorkItem<T>
    {
        public T WorkItem { get; set; }

        public String Server { get; set; }

        public DateTime CompletedAt { get; set; }

        public String Version { get; set; }

        public EnvUtil.Environment Environment { get; set; }

        public CompletedWorkItem()
        {
            this.Server = System.Environment.MachineName;
            this.CompletedAt = DateTime.UtcNow;
            this.Version = Utilities.Version;
            this.Environment = EnvUtil.Current;
        }
    }
}