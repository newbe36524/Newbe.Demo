using System;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.Extensions.Logging.Abstractions;
using Xunit.Abstractions;

namespace Newbe.RxWorld.DatabaseRepository.Impl
{
    public class AutoFlushListDatabaseRepository : IDatabaseRepository
    {
        private readonly IDatabase _database;
        private readonly AutoFlushList<BatchItem> _autoFlushList;

        public AutoFlushListDatabaseRepository(
            ITestOutputHelper testOutputHelper,
            IDatabase database)
        {
            _database = database;
            _autoFlushList = new AutoFlushList<BatchItem>(10000,
                TimeSpan.FromMilliseconds(10),
                DoMany,
                new NullLogger<AutoFlushList<BatchItem>>());
        }

        private void DoMany(ConcurrentList<BatchItem> list)
        {
            Task.Run(async () =>
            {
                using (list)
                {
                    var items = list.Buffer[..list.Count];
                    try
                    {
                        var result = await _database.InsertMany(items.Select(x => x.Item));
                        Parallel.ForEach(items, item => { item.Tcs.SetResult(result); });
                    }
                    catch (Exception e)
                    {
                        Parallel.ForEach(items, item => { item.Tcs.SetException(e); });
                    }
                }
            });
        }

        public async Task<int> InsertData(int item)
        {
            var taskCompletionSource = new TaskCompletionSource<int>();
            var valueTask = _autoFlushList.Push(new BatchItem
            {
                Item = item,
                Tcs = taskCompletionSource
            });
            if (!valueTask.IsCompleted)
            {
                await valueTask;
            }

            await taskCompletionSource.Task;
            return taskCompletionSource.Task.Result;
        }

        private record BatchItem
        {
            public TaskCompletionSource<int> Tcs { get; init; }
            public int Item { get; init; }
        }
    }
}