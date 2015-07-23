using CertiPay.Common.WorkQueue;
using NUnit.Framework;
using System;
using System.Linq;
using System.Threading.Tasks;

namespace CertiPay.Common.Tests.WorkQueue
{
    public class InMemoryQueueManagerTests
    {
        private const String QueueName = "InMemoryQueueManagerTests";

        private readonly IQueueManager Manager = new InMemoryQueueManager { };

        [SetUp, TearDown]
        public void Purge_Queues()
        {
            Manager.Purge(Queue.ProcessedNamespace + QueueName);
            Manager.Purge(Queue.QueuedNamespace + QueueName);
            Manager.Purge(Queue.FailedNamespace + QueueName);
        }

        [Test]
        public async Task Should_Enqueue_One_Item()
        {
            await Manager.Enqueue(QueueName, new TestItem { });

            var contents = await Manager.GetAll<TestItem>(QueueName);

            Assert.AreEqual(1, contents.Count());
        }

        [Test]
        public async Task Should_Enqueue_Multiple_Item()
        {
            const int count = 100;

            foreach (var i in Enumerable.Range(1, count))
            {
                await Manager.Enqueue(QueueName, new TestItem { });
            }

            var contents = await Manager.GetAll<TestItem>(QueueName);

            Assert.AreEqual(count, contents.Count());
        }

        // Test Cases to Add:

        [Test]
        public async Task Should_Consume_All_Items()
        {
            const int count = 100;

            foreach (var i in Enumerable.Range(1, count))
            {
                await Manager.Enqueue(QueueName, new TestItem { });
            }

            foreach (var item in Manager.GetConsumingEnumerable<TestItem>(QueueName))
            {
                // Nothing to do here
            }

            Assert.AreEqual(0, (await Manager.GetAll<TestItem>(QueueName)).Count());
        }

        [Test]
        public async Task Consuming_Should_Be_Pagable()
        {
            const int count = 100;

            foreach (var i in Enumerable.Range(1, count))
            {
                await Manager.Enqueue(QueueName, new TestItem { });
            }

            foreach (var item in Manager.GetConsumingEnumerable<TestItem>(QueueName).Take(15))
            {
                // Nothing to do here
            }

            Assert.AreEqual(count - 15, (await Manager.GetAll<TestItem>(QueueName)).Count());
        }

        [Test]
        public async Task Should_Mark_Complete()
        {
            await Manager.MarkComplete(QueueName, new CompletedWorkItem<TestItem>()
            {
                WorkItem = new TestItem { }
            });

            Assert.AreEqual(1, (await Manager.GetAll<TestItem>(Queue.ProcessedNamespace + QueueName)).Count());
        }

        [Test]
        public async Task Should_Mark_Failed()
        {
            await Manager.MarkFailed(QueueName, new FailedWorkItem<TestItem>()
            {
                WorkItem = new TestItem { }
            });

            Assert.AreEqual(1, (await Manager.GetAll<TestItem>(Queue.FailedNamespace + QueueName)).Count());
        }

        // Test Cases to Add:

        // Check stats in GetQueues

        internal class TestItem
        {
            public Guid ID = Guid.NewGuid();

            public DateTime TimeStamp = DateTime.UtcNow;
        }
    }
}