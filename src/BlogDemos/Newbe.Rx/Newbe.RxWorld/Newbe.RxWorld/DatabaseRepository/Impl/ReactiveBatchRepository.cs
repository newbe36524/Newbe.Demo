using System;
using System.Threading.Tasks;

namespace Newbe.RxWorld.DatabaseRepository.Impl
{
    public class ReactiveBatchRepository : IDatabaseRepository
    {
        private readonly IBatchOperator<int, int> _batchInsertOperator;

        public ReactiveBatchRepository(
            IDatabase database)
        {
            var options = new BatchOperatorOptions<int, int>
            {
                BufferTime = TimeSpan.FromMilliseconds(10),
                BufferCount = 10000,
                DoManyFunc = database.InsertMany,
            };
            _batchInsertOperator = new ReactiveBatchOperator<int, int>(options);
        }

        public Task<int> InsertData(int item)
        {
            return _batchInsertOperator.CreateTask(item);
        }
    }
}