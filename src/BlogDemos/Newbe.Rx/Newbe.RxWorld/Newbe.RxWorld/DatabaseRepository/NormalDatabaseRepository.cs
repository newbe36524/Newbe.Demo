using System.Threading;
using System.Threading.Tasks;
using Xunit.Abstractions;

namespace Newbe.RxWorld.DatabaseRepository
{
    public class NormalDatabaseRepository : IDatabaseRepository
    {
        private readonly ITestOutputHelper _testOutputHelper;
        private readonly IDatabase _database;

        public NormalDatabaseRepository(
            ITestOutputHelper testOutputHelper,
            IDatabase database)
        {
            _testOutputHelper = testOutputHelper;
            _database = database;
        }

        public Task<int> InsertData(int item)
        {
            return _database.InsertOne(item);
        }
    }
}