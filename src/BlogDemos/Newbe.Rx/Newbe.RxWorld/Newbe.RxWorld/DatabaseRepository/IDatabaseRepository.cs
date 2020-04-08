using System.Threading.Tasks;

namespace Newbe.RxWorld.DatabaseRepository
{
    public interface IDatabaseRepository
    {
        Task<int> InsertData(int item);
    }
}