using System.Diagnostics;
using System.Linq;
using System.Threading.Tasks;
using Newbe.RxWorld.DatabaseRepository;
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
            await RunTest(10, repo);
        }

        [Fact]
        public async Task Batch10()
        {
            var repo = new AutoBatchDatabaseRepository(_testOutputHelper, new Database());
            await RunTest(10, repo);
        }

        [Fact]
        public async Task Normal10001()
        {
            var repo = new NormalDatabaseRepository(_testOutputHelper, new Database());
            await RunTest(10001, repo);
        }

        [Fact]
        public async Task Batch10001()
        {
            var repo = new AutoBatchDatabaseRepository(_testOutputHelper, new Database());
            await RunTest(10001, repo);
        }

        private async Task RunTest(int count, IDatabaseRepository repo)
        {
            var sw = Stopwatch.StartNew();
            var allCount = await Task.WhenAll(Enumerable.Range(0, count).Select(repo.InsertData));
            _testOutputHelper.WriteLine($"time : {sw.ElapsedMilliseconds}");
            var currentCount = allCount.Max();
            _testOutputHelper.WriteLine($"current total count : {currentCount}");
        }
    }
}