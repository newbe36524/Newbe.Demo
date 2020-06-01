using System;
using System.Diagnostics;
using System.Linq;
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

        [Fact]
        public async Task Normal10()
        {
            var repo = new NormalDatabaseRepository(_testOutputHelper, new Database());
            await RunTest(repo, 10);
        }

        [Fact]
        public async Task Batch10()
        {
            var repo = new AutoBatchDatabaseRepository(_testOutputHelper, new Database());
            await RunTest(repo, 10);
        }

        [Fact]
        public async Task Normal12345()
        {
            var repo = new NormalDatabaseRepository(_testOutputHelper, new Database());
            await RunTest(repo, 10000, 2000, 300, 40, 5);
        }

        [Fact]
        public async Task AutoBatchDatabaseRepository12345()
        {
            var repo = new AutoBatchDatabaseRepository(_testOutputHelper, new Database());
            await RunTest(repo, 10000, 2000, 300, 40, 5);
        }

        [Fact]
        public async Task FinalDatabaseRepository12345()
        {
            var repo = new FinalDatabaseRepository(new Database());
            await RunTest(repo, 10000, 2000, 300, 40, 5);
        }

        [Fact]
        public async Task ConcurrentDicDatabaseRepository12345()
        {
            var repo = new ConcurrentQueueDatabaseRepository(_testOutputHelper, new Database());
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
                    var allCount = await Task.WhenAll(Enumerable.Range(start, count).Select(repo.InsertData));
                    _testOutputHelper.WriteLine($"time : {sw.ElapsedMilliseconds}");
                    var currentCount = allCount.Max();
                    _testOutputHelper.WriteLine($"current total count : {currentCount}");
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