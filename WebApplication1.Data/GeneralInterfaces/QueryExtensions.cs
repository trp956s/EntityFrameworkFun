using ExecutionStrategyCore;
using System.Linq;
using System.Threading.Tasks;

namespace WebApplication1.Data.GeneralInterfaces
{
    public static class QueryExtensions
    {
        public static async Task<ReturnType> QuerySingleAsync<ReturnType>(this ITaskRunner runner, IAsyncQuerySingleFactory<ReturnType> mapWrapper, IRunner<IQueryable<ReturnType>> parameterWrapper)
        {
            return await new QuerySingleAsync<ReturnType>(mapWrapper, parameterWrapper).Run(runner);
        }
    }

    public struct QuerySingleAsync<ReturnType> : 
        IRunner<QuerySingleAsync<ReturnType>>,
        IMapper<ITaskRunner, Task<ReturnType>>
    {
        private IAsyncQuerySingleFactory<ReturnType> mapWrapper;
        private IRunner<IQueryable<ReturnType>> parameterWrapper;

        public QuerySingleAsync(IAsyncQuerySingleFactory<ReturnType> mapWrapper, IRunner<IQueryable<ReturnType>> parameterWrapper)
        {
            this.mapWrapper = mapWrapper;
            this.parameterWrapper = parameterWrapper;
        }

        public QuerySingleAsync<ReturnType> Run()
        {
            return this;
        }

        public async Task<ReturnType> Run(ITaskRunner runner)
        {
            return await runner.XAsync2<IAsyncQuerySingleFactory<ReturnType>, IQueryable<ReturnType>, ReturnType>(mapWrapper, parameterWrapper);
        }
    }
}
