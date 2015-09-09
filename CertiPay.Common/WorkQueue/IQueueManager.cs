using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace CertiPay.Common.WorkQueue
{
    /// <summary>
    /// Provides an interface for interacting with the queuing system
    /// </summary>
    public interface IQueueManager
    {
        // TODO Do we want to require that messages implement a certain interface for things like priority or just function as FIFO queue?

        /// <summary>
        /// Retrieves a list of queue names in use and their current sizes
        /// </summary>
        Task<IEnumerable<Queue>> GetQueues();

        /// <summary>
        /// Get all items in the given queue, but do not remove them
        /// </summary>
        Task<IEnumerable<T>> GetAll<T>(String queueName, int page = 1, int itemsPerPage = 20) where T : class;

        /// <summary>
        /// Poll for Work for the given queue, removing the item from the queue
        /// </summary>
        Task<T> GetWork<T>(String queueName) where T : class;

        /// <summary>
        /// Pops work items off the queue until empty
        /// </summary>
        IEnumerable<T> GetConsumingEnumerable<T>(String queueName) where T : class;

        /// <summary>
        /// Add an item T to the queue with the given name
        /// </summary>
        Task Enqueue<T>(String queueName, T t) where T : class;

        /// <summary>
        /// Move a completed item T to the processed queue with relevant server/version info
        /// </summary>
        Task MarkComplete<T>(String queueName, CompletedWorkItem<T> t) where T : class;

        /// <summary>
        /// Move a failed item T to the processed queue with relevant server/version info
        /// </summary>
        Task MarkFailed<T>(String queueName, FailedWorkItem<T> t) where T : class;

        // Remove Item from Queue (POP vs Selective Remove)
        // Task Remove(String queueName, int ID);

        /// <summary>
        /// Purge the queue with the given name
        /// </summary>
        Task Purge(String queueName);
    }
}