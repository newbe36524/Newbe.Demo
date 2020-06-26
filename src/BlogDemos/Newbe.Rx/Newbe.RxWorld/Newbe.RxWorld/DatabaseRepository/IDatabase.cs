using System.Collections.Generic;
using System.Threading.Tasks;

namespace Newbe.RxWorld.DatabaseRepository
{
    public interface IDatabase
    {
        Task<int> InsertOne(int item);
        Task<int> InsertMany(IEnumerable<int> items);
        Task<int> UpsertOne(int key, int value);
        Task<int> UpsertMany(Dictionary<int, int> values);
    }
}