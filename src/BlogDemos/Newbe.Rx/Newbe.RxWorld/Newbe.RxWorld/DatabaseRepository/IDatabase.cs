using System.Collections.Generic;
using System.Threading.Tasks;

namespace Newbe.RxWorld.DatabaseRepository
{
    public interface IDatabase
    {
        Task<int> InsertOne(int item);
        Task<int> InsertMany(IEnumerable<int> items);
    }
}