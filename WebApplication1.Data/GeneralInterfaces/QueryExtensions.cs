using ExecutionStrategyCore;
using System.Linq;
using System.Threading.Tasks;

namespace WebApplication1.Data.GeneralInterfaces
{
    public static class QueryExtensions
    {
        public static async Task<ReturnType> QuerySingleAsync<ReturnType>(this ITaskRunner runner, IAsyncQuerySingleFactory<ReturnType> mapWrapper, IRunner<IQueryable<ReturnType>> parameterWrapper)
        {
            var querySingleAsyncFactory = new QuerySingleAsync<ReturnType>(mapWrapper, parameterWrapper);
            var query = runner.Run(querySingleAsyncFactory);
            
            return await query.Run(runner);
        }
    }

    public interface IQuerySingleAsync<ReturnType> :
        IRunner<IQuerySingleAsync<ReturnType>>,
        IMapper<ITaskRunner, Task<ReturnType>>
    { }

    public struct QuerySingleAsync<ReturnType> : IQuerySingleAsync<ReturnType>
    {
        private IAsyncQuerySingleFactory<ReturnType> mapWrapper;
        private IRunner<IQueryable<ReturnType>> parameterWrapper;

        public QuerySingleAsync(IAsyncQuerySingleFactory<ReturnType> mapWrapper, IRunner<IQueryable<ReturnType>> parameterWrapper)
        {
            this.mapWrapper = mapWrapper;
            this.parameterWrapper = parameterWrapper;
        }

        public IQuerySingleAsync<ReturnType> Run()
        {
            return this;
        }

        public async Task<ReturnType> Run(ITaskRunner runner)
        {
            return await runner.XAsync2<IAsyncQuerySingleFactory<ReturnType>, IQueryable<ReturnType>, ReturnType>(mapWrapper, parameterWrapper);
        }
    }
}
