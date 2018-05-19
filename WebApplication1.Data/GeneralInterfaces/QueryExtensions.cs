using ExecutionStrategyCore;
using Microsoft.EntityFrameworkCore;
using System;
using System.Linq;
using System.Threading.Tasks;

namespace WebApplication1.Data.GeneralInterfaces
{
    public static class QueryExtensions
    {
        public static ISingleAsyncQuery<ReturnType> CreateSingleAsyncQuery<ReturnType>(this ITaskRunner runner, IAsyncQuerySingleFactory<ReturnType> mapWrapper, IRunner<IQueryable<ReturnType>> parameterWrapper)
        {
            var querySingleAsyncFactory = new SingleAsyncQuery<ReturnType>(mapWrapper, parameterWrapper);
            return runner.Run(querySingleAsyncFactory);
        }

        public static IDeleteSingleAsync CreateDeleteSingleAsync<T, DbContextType>(this ITaskRunner runner, T mapWrapper, IRunner<DbContextType> parameterWrapper)
            where T : IRunner<InternalValueCache<IMapper<DbContextType, Task<int>>>>
            where DbContextType : DbContext
        {
            var deleteSingleAsyncFactory = new DeleteSingleAsync<T, DbContextType>(mapWrapper, parameterWrapper);
            return runner.Run(deleteSingleAsyncFactory);
        }

        public static IMapper<ITaskRunner, Task<ReturnType>> AsCommand<ReturnType>(this IMapper<ITaskRunner, Task<ReturnType>> mapper)
        {
            return mapper;
        }
    }

    public interface ISingleAsyncQuery<ReturnType> :
        IRunner<ISingleAsyncQuery<ReturnType>>,
        //IMapper<ITaskRunner, Task<ReturnType>>,
        IAsyncMapper2<ITaskRunner, ReturnType>
    { }

    public struct SingleAsyncQuery<ReturnType> : ISingleAsyncQuery<ReturnType>
    {
        private IAsyncQuerySingleFactory<ReturnType> mapWrapper;
        private IRunner<IQueryable<ReturnType>> parameterWrapper;

        public SingleAsyncQuery(IAsyncQuerySingleFactory<ReturnType> mapWrapper, IRunner<IQueryable<ReturnType>> parameterWrapper)
        {
            this.mapWrapper = mapWrapper;
            this.parameterWrapper = parameterWrapper;
        }

        //can only be called through pipeline
        public async Task<InternalValueCache<ReturnType>> MapAsync(WrappedParameter<ITaskRunner> wrappedParameter)
        {
            var val = await Run(wrappedParameter.GetValue());
            return new InternalValueCache<ReturnType>(val);
        }

        public ISingleAsyncQuery<ReturnType> Run()
        {
            return this;
        }

        private async Task<ReturnType> Run(ITaskRunner runner)
        {
            return await runner.XAsync2<IAsyncQuerySingleFactory<ReturnType>, IQueryable<ReturnType>, ReturnType>(mapWrapper, parameterWrapper);
        }
    }

    public interface IDeleteSingleAsync :
        IRunner<IDeleteSingleAsync>,
        IMapper<ITaskRunner, Task<int>>
    { }

    public struct DeleteSingleAsync<T, DbContextType> : IDeleteSingleAsync
        where T : IRunner<InternalValueCache<IMapper<DbContextType, Task<int>>>>
    {
        private T mapWrapper;
        private IRunner<DbContextType> parameterWrapper;

        public DeleteSingleAsync(T mapWrapper, IRunner<DbContextType> parameterWrapper)
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
            return await runner.XAsync2<T, DbContextType, int>(mapWrapper, parameterWrapper);
        }
    }
}
