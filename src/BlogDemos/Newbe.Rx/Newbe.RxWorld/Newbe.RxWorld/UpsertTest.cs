using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Reactive.Linq;
using System.Reactive.Threading.Tasks;
using System.Threading.Tasks;
using Newbe.RxWorld.DatabaseRepository;
using Newbe.RxWorld.DatabaseRepository.Impl;
using Xunit;
using Xunit.Abstractions;

namespace Newbe.RxWorld
{
    public class UpsertTest
    {
        private readonly ITestOutputHelper _testOutputHelper;

        public UpsertTest(
            ITestOutputHelper testOutputHelper)
        {
            _testOutputHelper = testOutputHelper;
        }

        [Fact]
        public async Task NormalUpsertTest()
        {
            var db = new SQLiteDatabase(nameof(NormalUpsertTest));
            var repo = new NormalUpsert(db);
            await RunTest(repo, 10000, 2000, 300, 40, 5);
            // run twice for updating test
            await RunTest(repo, 10000, 2000, 300, 40, 5);
        }

        [Fact]
        public async Task BatchUpsertTest()
        {
            var db = new SQLiteDatabase(nameof(BatchUpsertTest));
            var repo = new BatchUpsert(db);
            await RunTest(repo, 10000, 2000, 300, 40, 5);
            // run twice for updating test
            await RunTest(repo, 10000, 2000, 300, 40, 5);
        }

        private async Task RunTest(IUpsertRepository repo, params int[] counts)
        {
            var start = 0;
            foreach (var count in counts)
            {
                try
                {
                    var sw = Stopwatch.StartNew();
                    var task = Enumerable.Range(start, count)
                        .ToObservable()
                        .Select(i => Observable.FromAsync(() => repo.UpsertAsync(i, i * 100)))
                        .Merge(1000)
                        .ToTask();
                    await task;
                    _testOutputHelper.WriteLine($"time : {sw.ElapsedMilliseconds}");
                    await Task.Delay(TimeSpan.FromMilliseconds(10));
                    start += count;
                }
                catch (Exception e)
                {
                    _testOutputHelper.WriteLine($"there is an error : {e}");
                }
            }
        }

        private interface IUpsertRepository
        {
            Task UpsertAsync(int key, int value);
        }

        private class NormalUpsert : IUpsertRepository
        {
            private readonly IDatabase _database;

            public NormalUpsert(
                IDatabase database)
            {
                _database = database;
            }

            public Task UpsertAsync(int key, int value)
            {
                return _database.UpsertOne(key, value);
            }
        }

        private class BatchUpsert : IUpsertRepository
        {
            private readonly IDatabase _database;
            private readonly IBatchOperator<(int, int), int> _batchOperator;

            public BatchUpsert(IDatabase database)
            {
                _database = database;
                var options = new BatchOperatorOptions<(int, int), int>
                {
                    BufferCount = 100,
                    BufferTime = TimeSpan.FromMilliseconds(50),
                    DoManyFunc = DoManyFunc
                };
                _batchOperator = new BatchOperator<(int, int), int>(options);
            }

            private Task<int> DoManyFunc(IEnumerable<(int, int)> arg)
            {
                return _database.UpsertMany(arg.ToDictionary(x => x.Item1, x => x.Item2));
            }

            public Task UpsertAsync(int key, int value)
            {
                return _batchOperator.CreateTask((key, value));
            }
        }
    }
}