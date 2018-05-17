using ExecutionStrategyCore;
using System;
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

            return await runner.RunMapper(query);
        }

        //todo: replace bloggingcontent with T : DbContent out
        public static async Task<int> DeleteSingleAsync<T>(this ITaskRunner runner, T mapWrapper, IRunner<BloggingContext> parameterWrapper)
            where T : IRunner<InternalValueCache<IMapper<BloggingContext, Task<int>>>>
        {
            var deleteSingleAsyncFactory = new DeleteSingleAsync<T>(mapWrapper, parameterWrapper);
            var delete = runner.Run(deleteSingleAsyncFactory);

            return await runner.RunMapper(delete);
        }

        //todo: move
        public static async Task<ReturnType> RunMapper<ReturnType>(this ITaskRunner runner, IMapper<ITaskRunner, Task<ReturnType>> mapper)
        {
            return await new TaskMapRunner<ReturnType>(runner).Run(mapper);
        }
    }

    //todo: move
    public interface ITaskMapRunner<ReturnType> : IMapper<IMapper<ITaskRunner, Task<ReturnType>>, Task<ReturnType>>
    { }

    //todo: move
    public class TaskMapRunner<ReturnType> : ITaskMapRunner<ReturnType>, IRunner<ITaskMapRunner<ReturnType>>
    {
        private ITaskRunner runner;

        public TaskMapRunner(ITaskRunner runner)
        {
            this.runner = runner;
        }

        public async Task<ReturnType> Run(IMapper<ITaskRunner, Task<ReturnType>> mapper)
        {
            return await mapper.Run(runner);
        }

        public ITaskMapRunner<ReturnType> Run()
        {
            return this;
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

    public interface IDeleteSingleAsync :
        IRunner<IDeleteSingleAsync>,
        IMapper<ITaskRunner, Task<int>>
    { }

    public struct DeleteSingleAsync<T> : IDeleteSingleAsync
        where T : IRunner<InternalValueCache<IMapper<BloggingContext, Task<int>>>>
    {
        private T mapWrapper;
        private IRunner<BloggingContext> parameterWrapper;

        public DeleteSingleAsync(T mapWrapper, IRunner<BloggingContext> parameterWrapper)
        {
            this.mapWrapper = mapWrapper;
            this.parameterWrapper = parameterWrapper;
        }

        public IDeleteSingleAsync Run()
        {
            return this;
        }

        public async Task<int> Run(ITaskRunner runner)
        {
            return await runner.XAsync2<T, BloggingContext, int>(mapWrapper, parameterWrapper);
        }
    }
}
