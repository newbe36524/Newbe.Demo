using System;
using System.Diagnostics;
using System.Linq;
using System.Reactive.Linq;
using System.Reactive.Threading.Tasks;
using System.Threading;
using System.Threading.Tasks;
using FluentAssertions;
using Newbe.RxWorld.DatabaseRepository;
using Newbe.RxWorld.DatabaseRepository.Impl;
using Xunit;
using Xunit.Abstractions;

namespace Newbe.RxWorld
{
    public class DatabaseRepositoryTest
    {
        private readonly ITestOutputHelper _testOutputHelper;

        public DatabaseRepositoryTest(
            ITestOutputHelper testOutputHelper)
        {
            _testOutputHelper = testOutputHelper;
        }

        private static IDatabase CreateDataBase(string dbName)
            => new SQLiteDatabase($"{dbName}.db");

        [Fact]
        public async Task Normal10()
        {
            var repo = new NormalDatabaseRepository(_testOutputHelper, CreateDataBase(nameof(Normal10)));
            await RunTest(repo, 10);
        }

        [Fact]
        public async Task Batch10()
        {
            var repo = new AutoBatchDatabaseRepository(_testOutputHelper, CreateDataBase(nameof(Batch10)));
            await RunTest(repo, 10);
        }

        [Fact]
        public async Task Normal12345()
        {
            var repo = new NormalDatabaseRepository(_testOutputHelper, CreateDataBase(nameof(Normal12345)));
            await RunTest(repo, 10000, 2000, 300, 40, 5);
        }

        [Fact]
        public async Task AutoBatchDatabaseRepository12345()
        {
            var db = CreateDataBase(nameof(AutoBatchDatabaseRepository12345));
            var repo = new AutoBatchDatabaseRepository(_testOutputHelper, db);
            await RunTest(repo, 5, 100000, 2000, 300, 40, 5);
        }

        [Fact]
        public async Task ReactiveBatchRepository12345()
        {
            var db = CreateDataBase(nameof(ReactiveBatchRepository12345));
            var repo = new ReactiveBatchRepository(db);
            await RunTest(repo, 5, 100000, 2000, 300, 40, 5);
        }

        [Fact]
        public async Task ChannelDatabaseRepository()
        {
            var db = CreateDataBase(nameof(ChannelDatabaseRepository));
            var repo = new ChannelDatabaseRepository(_testOutputHelper, db);
            await RunTest(repo, 5, 100000, 2000, 300, 40, 5);
        }

        [Fact]
        public async Task ConcurrentDicDatabaseRepository12345()
        {
            var db = CreateDataBase(nameof(ConcurrentDicDatabaseRepository12345));
            var repo = new ConcurrentQueueDatabaseRepository(_testOutputHelper, db);
            await RunTest(repo, 5, 100000, 2000, 300, 40, 5);
        }
        
        [Fact]
        public async Task AutoFlushListDatabaseRepository12345()
        {
            var db = CreateDataBase(nameof(AutoFlushListDatabaseRepository12345));
            var repo = new AutoFlushListDatabaseRepository(_testOutputHelper, db);
            await RunTest(repo, 5, 100000, 2000, 300, 40, 5);
        }

        [Fact]
        public async Task DirectlyInsertMany12345()
        {
            var db = CreateDataBase(nameof(DirectlyInsertMany12345));
            await TestInsertMany(db, 5, 100000, 2000, 300, 40, 5);
        }

        private async Task RunTest(IDatabaseRepository repo, params int[] counts)
        {
            var start = 0;
            foreach (var count in counts)
            {
                try
                {
                    var counter = 0;
                    var sw = Stopwatch.StartNew();
                    var finalTcs = new TaskCompletionSource<int>();
                    Parallel.For(start, count + start, async x =>
                    {
                        try
                        {
                            await repo.InsertData(x);
                            var newValue = Interlocked.Increment(ref counter);
                            if (newValue >= count)
                            {
                                finalTcs.SetResult(0);
                            }
                        }
                        catch (Exception e)
                        {
                            finalTcs.SetException(e);
                        }
                    });
                    await finalTcs.Task;
                    _testOutputHelper.WriteLine($"count : {count} cost {sw.ElapsedMilliseconds} ms");
                    await Task.Delay(TimeSpan.FromMilliseconds(10));
                    start += count;
                }
                catch (Exception e)
                {
                    _testOutputHelper.WriteLine($"there is an error : {e}");
                }
            }
        }

        private async Task TestInsertMany(IDatabase db, params int[] counts)
        {
            var start = 0;
            foreach (var count in counts)
            {
                try
                {
                    var sw = Stopwatch.StartNew();
                    await db.InsertMany(Enumerable.Range(start, count));
                    _testOutputHelper.WriteLine($"count : {count} cost {sw.ElapsedMilliseconds} ms");
                    await Task.Delay(TimeSpan.FromMilliseconds(10));
                    start += count;
                }
                catch (Exception e)
                {
                    _testOutputHelper.WriteLine($"there is an error : {e}");
                }
            }
        }
    }
}