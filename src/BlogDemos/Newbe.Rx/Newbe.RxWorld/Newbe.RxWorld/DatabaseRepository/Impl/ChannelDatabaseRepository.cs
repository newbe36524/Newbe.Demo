using System;
using System.Collections.Generic;
using System.Linq;
using System.Reactive.Linq;
using System.Threading.Channels;
using System.Threading.Tasks;
using Xunit.Abstractions;

namespace Newbe.RxWorld.DatabaseRepository.Impl
{
    public class ChannelDatabaseRepository : IDatabaseRepository
    {
        private readonly ITestOutputHelper _testOutputHelper;
        private readonly IDatabase _database;
        private readonly Channel<BatchItem> _channel;

        private readonly Task _task;
        private static readonly TimeSpan Waiting = TimeSpan.FromMilliseconds(50);

        public ChannelDatabaseRepository(
            ITestOutputHelper testOutputHelper,
            IDatabase database)
        {
            _testOutputHelper = testOutputHelper;
            _database = database;
            _channel = Channel.CreateUnbounded<BatchItem>();
            _task = Task.Run(async () =>
            {
                while (true)
                {
                    var readAsync = _channel.Reader.ReadAsync();
                    BatchItem item;
                    if (readAsync.IsCompleted)
                    {
                        item = readAsync.Result;
                    }
                    else
                    {
                        item = await readAsync;
                    }

                    var list = new List<BatchItem>(100) {item};
                    var time = DateTimeOffset.Now;
                    while (list.Count < 100 && DateTimeOffset.Now - time < Waiting)
                    {
                        if (_channel.Reader.TryRead(out var newItem))
                        {
                            list.Add(newItem);
                        }
                        else
                        {
                            break;
                        }
                    }

                    if (list.Any())
                    {
                        await BatchInsertData(list);
                    }
                    else
                    {
                        Task.Yield();
                    }
                }
            });
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


        public Task<int> InsertData(int item)
        {
            var tcs = new TaskCompletionSource<int>();
            var task = new BatchItem
            {
                Item = item,
                TaskCompletionSource = tcs
            };
            _channel.Writer.TryWrite(task);
            return tcs.Task;
        }

        private struct BatchItem
        {
            public TaskCompletionSource<int> TaskCompletionSource { get; set; }
            public int Item { get; set; }
        }
    }
}