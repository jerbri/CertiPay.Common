using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace CertiPay.Common.WorkQueue
{
    public sealed class InMemoryQueueManager : IQueueManager
    {
        public static readonly IDictionary<String, System.Collections.Queue> MemoryQueue = new ConcurrentDictionary<String, System.Collections.Queue>();

        public static readonly ISet<string> QueueNames = new System.Collections.Generic.HashSet<String>();

        private String GetQueueName(String queueName, String queueNamespace = Queue.QueuedNamespace)
        {
            // Check if it already has the namespace added, otherwise add it

            var namespaces = new[] { Queue.QueuedNamespace, Queue.ProcessedNamespace, Queue.FailedNamespace };

            if (namespaces.Any(ns => queueName.StartsWith(ns))) return queueName;

            return String.Format("{0}{1}", queueNamespace, queueName);
        }

        private System.Collections.Queue GetQueue(String name, String queueNamespace = Queue.QueuedNamespace)
        {
            String queueName = GetQueueName(name, queueNamespace);

            var q = default(System.Collections.Queue);

            if (!MemoryQueue.TryGetValue(queueName, out q))
            {
                q = MemoryQueue[queueName] = new System.Collections.Queue();
            }

            return q;
        }

        public Task Enqueue<T>(string queueName, T t) where T : class
        {
            QueueNames.Add(queueName);
            GetQueue(queueName).Enqueue(t);
            return Task.FromResult(0);
        }

        public async Task<IEnumerable<T>> GetAll<T>(string queueName, int page = 1, int itemsPerPage = 20) where T : class
        {
            return GetQueue(queueName).ToArray().Select(_ => _ as T);
        }

        public IEnumerable<T> GetConsumingEnumerable<T>(string queueName) where T : class
        {
            T item = default(T);

            while ((item = GetWork<T>(queueName).Result) != null)
            {
                yield return item;
            }
        }

        public async Task<IEnumerable<Queue>> GetQueues()
        {
            return from key in QueueNames
                   select new Queue
                   {
                       Name = key,
                       Queued = GetQueue(GetQueueName(key, Queue.QueuedNamespace)).Count,
                       Processed = GetQueue(GetQueueName(key, Queue.ProcessedNamespace)).Count,
                       Failed = GetQueue(GetQueueName(key, Queue.FailedNamespace)).Count,
                   };
        }

        public async Task<T> GetWork<T>(string queueName) where T : class
        {
            try
            {
                return GetQueue(queueName).Dequeue() as T;
            }
            catch
            {
                // Let it ride...
            }

            return null;
        }

        public async Task MarkComplete<T>(string queueName, CompletedWorkItem<T> t) where T : class
        {
            GetQueue(queueName, Queue.ProcessedNamespace).Enqueue(t);
        }

        public async Task MarkFailed<T>(string queueName, FailedWorkItem<T> t) where T : class
        {
            GetQueue(queueName, Queue.FailedNamespace).Enqueue(t);
        }

        public async Task Purge(string queueName)
        {
            GetQueue(queueName).Clear();
        }
    }
}