using System;
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
            await RunTest(repo, 10000, 2000, 300, 40, 5);
        }

        [Fact]
        public async Task FinalDatabaseRepository12345()
        {
            var db = CreateDataBase(nameof(FinalDatabaseRepository12345));
            var repo = new FinalDatabaseRepository(db);
            await RunTest(repo, 10000, 2000, 300, 40, 5);
        }

        [Fact]
        public async Task ConcurrentDicDatabaseRepository12345()
        {
            var db = CreateDataBase(nameof(ConcurrentDicDatabaseRepository12345));
            var repo = new ConcurrentQueueDatabaseRepository(_testOutputHelper, db);
            await RunTest(repo, 10000, 2000, 300, 40, 5);
        }

        private async Task RunTest(IDatabaseRepository repo, params int[] counts)
        {
            var start = 0;
            foreach (var count in counts)
            {
                try
                {
                    var sw = Stopwatch.StartNew();
                    var task = Enumerable.Range(start, count)
                        .ToObservable()
                        .Select(i => Observable.FromAsync(() => repo.InsertData(i)))
                        .Merge(100)
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
    }
}