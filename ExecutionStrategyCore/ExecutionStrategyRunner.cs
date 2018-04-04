using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace ExecutionStrategyCore
{
    public class ExecutionStrategyRunner : IExecutionStrategyRunner, ITaskRunner
    {
        public async Task<T> Run<T>(ExecutionStrategy<T> executionStrategy)
        {
            return await executionStrategy.Run();
        }

        public async Task<T> Run<T>(IRunner<Task<T>> executionWrapper)
        {
            return await executionWrapper.Run();
        }
    }

    /*
     * consider moving all this into the Data Layer library
     */
    public interface IQuery<DbSetType, ReturnType> : IMapper<IRunner<DbSet<DbSetType>>, InternalRunnerWrapper<Task<ReturnType>>> { }

    public class DbSet<T>
    { }

    public struct GetAll : IQuery<int, int>
    {
        public InternalRunnerWrapper<Task<int>> Run(IRunner<DbSet<int>> arg)
        {
            return new InternalRunnerWrapper<Task<int>>(new ValueCacheRunner<Task<int>>(Task.FromResult(0)));
        }
    }

    public interface IDbSetRunner<DbSetType>
    {
        IRunner<InternalRunnerWrapper<Task<T>>> Run<T>(IQuery<DbSetType, T> ga);
    }

    public struct DbBlogSetRunner : IDbSetRunner<int>, IRunner<DbSet<int>>
    {
        //inject dbset 
        private readonly DbSet<int> dbSet;

        public DbBlogSetRunner(DbSet<int> dbSet)
        {
            this.dbSet = dbSet;
        }

        public IRunner<InternalRunnerWrapper<Task<T>>> Run<T>(IQuery<int, T> query)
        {
            return new MapperRunner<IRunner<DbSet<int>>, T>(query, this);
        }

        DbSet<int> IRunner<DbSet<int>>.Run()
        {
            return dbSet;
        }
    }
}
