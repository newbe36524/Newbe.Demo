using System;
using System.Collections.Generic;
using System.Linq;
using System.Reactive.Linq;
using System.Reactive.Subjects;
using System.Threading;
using System.Threading.Tasks;
using Xunit.Abstractions;

namespace Newbe.RxWorld.DatabaseRepository
{
    public class AutoBatchDatabaseRepository : IDatabaseRepository
    {
        private readonly ITestOutputHelper _testOutputHelper;
        private readonly IDatabase _database;
        private readonly Subject<BatchItem> _subject;

        public AutoBatchDatabaseRepository(
            ITestOutputHelper testOutputHelper,
            IDatabase database)
        {
            _testOutputHelper = testOutputHelper;
            _database = database;
            _subject = new Subject<BatchItem>();
            _subject.Buffer(TimeSpan.FromMilliseconds(10))
                .Select(list => Observable.FromAsync(async () => await BatchInsertData(list)))
                .Merge()
                .Subscribe();
        }

        public Task InsertData(int item)
        {
            var taskCompletionSource = new TaskCompletionSource<int>();
            _subject.OnNext(new BatchItem
            {
                Item = item,
                TaskCompletionSource = taskCompletionSource
            });
            return taskCompletionSource.Task;
        }

        private async Task BatchInsertData(IEnumerable<BatchItem> items)
        {
            var batchItems = items as BatchItem[] ?? items.ToArray();
            var count = batchItems.Length;
            try
            {
                await _database.InsertMany(count);
                foreach (var batchItem in batchItems)
                {
                    batchItem.TaskCompletionSource.SetResult(0);
                }
            }
            catch (Exception e)
            {
                foreach (var batchItem in batchItems)
                {
                    batchItem.TaskCompletionSource.SetException(e);
                    Console.WriteLine(e);
                }

                throw;
            }

            if (count > 0)
            {
                _testOutputHelper.WriteLine($"{count} items data inserted");
            }
        }

        private class BatchItem
        {
            public TaskCompletionSource<int> TaskCompletionSource { get; set; }
            public int Item { get; set; }
        }
    }
}