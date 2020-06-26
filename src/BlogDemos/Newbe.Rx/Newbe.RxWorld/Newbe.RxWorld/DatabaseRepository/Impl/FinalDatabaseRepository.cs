using System;
using System.Threading.Tasks;

namespace Newbe.RxWorld.DatabaseRepository.Impl
{
    public class FinalDatabaseRepository : IDatabaseRepository
    {
        private readonly IBatchOperator<int, int> _batchInsertOperator;

        public FinalDatabaseRepository(
            IDatabase database)
        {
            var options = new BatchOperatorOptions<int, int>
            {
                BufferTime = TimeSpan.FromMilliseconds(50),
                BufferCount = 100,
                DoManyFunc = database.InsertMany,
            };
            _batchInsertOperator = new BatchOperator<int, int>(options);
        }

        public Task<int> InsertData(int item)
        {
            return _batchInsertOperator.CreateTask(item);
        }
    }
}