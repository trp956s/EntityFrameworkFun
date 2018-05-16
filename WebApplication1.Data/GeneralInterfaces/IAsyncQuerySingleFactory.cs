using ExecutionStrategyCore;
using System.Linq;
using System.Threading.Tasks;

namespace WebApplication1.Data.GeneralInterfaces
{
    public interface IAsyncQuerySingleFactory<T> : IRunner<InternalValueCache<IMapper<IQueryable<T>, Task<T>>>>
    {
    }
}
