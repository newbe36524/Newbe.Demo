using System.Threading.Tasks;

namespace Newbe.RxWorld.DatabaseRepository
{
    public interface IDatabase
    {
        Task InsertOne();
        Task InsertMany(int count);
    }
}