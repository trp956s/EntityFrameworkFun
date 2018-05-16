using ExecutionStrategyCore;
using System.Linq;
using System.Threading.Tasks;

namespace WebApplication1.Data.GeneralInterfaces
{
    public static class QueryExtensions
    {
        public static async Task<ReturnType> QuerySingleAsync<T, ReturnType>(this ITaskRunner runner, T mapWrapper, IRunner<IQueryable<ReturnType>> parameterWrapper)
        where T : IAsyncQuerySingleFactory<ReturnType>
        {
            return await runner.XAsync2<T, IQueryable<ReturnType>, ReturnType>(mapWrapper, parameterWrapper);
        }
    }
}
