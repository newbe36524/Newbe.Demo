using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Xunit.Abstractions;

namespace Newbe.RxWorld.DatabaseRepository.Impl
{
    public class ConcurrentQueueDatabaseRepository : IDatabaseRepository
    {
        private readonly ITestOutputHelper _testOutputHelper;
        private readonly IDatabase _database;
        private readonly ConcurrentQueue<BatchItem> _queue;

        // ReSharper disable once PrivateFieldCanBeConvertedToLocalVariable
        private readonly Task _batchInsertDataTask;

        public ConcurrentQueueDatabaseRepository(
            ITestOutputHelper testOutputHelper,
            IDatabase database)
        {
            _testOutputHelper = testOutputHelper;
            _database = database;
            _queue = new ConcurrentQueue<BatchItem>();
            _batchInsertDataTask = Task.Factory.StartNew(RunBatchInsert, TaskCreationOptions.LongRunning);
            _batchInsertDataTask.ConfigureAwait(false);
        }

        public Task<int> InsertData(int item)
        {
            var taskCompletionSource = new TaskCompletionSource<int>();
            _queue.Enqueue(new BatchItem
            {
                Item = item,
                TaskCompletionSource = taskCompletionSource
            });
            return taskCompletionSource.Task;
        }

        private void RunBatchInsert()
        {
            foreach (var batchItems in GetBatches())
            {
                try
                {
                    BatchInsertData(batchItems).Wait();
                }
                catch (Exception e)
                {
                    _testOutputHelper.WriteLine($"there is an error : {e}");
                }
            }

            IEnumerable<IList<BatchItem>> GetBatches()
            {
                var sleepTime = TimeSpan.FromMilliseconds(50);
                while (true)
                {
                    const int maxCount = 100;
                    var oneBatchItems = GetWaitingItems()
                        .Take(maxCount)
                        .ToList();
                    if (oneBatchItems.Any())
                    {
                        yield return oneBatchItems;
                    }
                    else
                    {
                        Thread.Sleep(sleepTime);
                    }
                }

                IEnumerable<BatchItem> GetWaitingItems()
                {
                    while (_queue.TryDequeue(out var item))
                    {
                        yield return item;
                    }
                }
            }
        }

        private async Task BatchInsertData(IEnumerable<BatchItem> items)
        {
            var batchItems = items as BatchItem[] ?? items.ToArray();
            try
            {
                var totalCount = await _database.InsertMany(batchItems.Select(x => x.Item));
                foreach (var batchItem in batchItems)
                {
                    batchItem.TaskCompletionSource.SetResult(totalCount);
                }
            }
            catch (Exception e)
            {
                foreach (var batchItem in batchItems)
                {
                    batchItem.TaskCompletionSource.SetException(e);
                }

                throw;
            }
        }

        private struct BatchItem
        {
            public TaskCompletionSource<int> TaskCompletionSource { get; set; }
            public int Item { get; set; }
        }
    }
}