using System.Threading.Tasks;

namespace Newbe.RxWorld.DatabaseRepository.Impl
{
    public interface IBatchOperator<in TInput, TOutput> :
        IBatchOperator
    {
        Task<TOutput> CreateTask(TInput input);
    }

    public interface IBatchOperator
    {
    }
}