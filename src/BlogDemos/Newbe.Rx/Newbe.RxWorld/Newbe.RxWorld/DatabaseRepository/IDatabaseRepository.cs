using System.Reactive.Linq;
using System.Reactive.Subjects;
using System.Threading.Tasks;

namespace Newbe.RxWorld.DatabaseRepository
{
    public interface IDatabaseRepository
    {
        Task<int> InsertData(int item);
    }
}