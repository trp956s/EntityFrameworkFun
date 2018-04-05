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
    public interface IQuery<DbSetType, ReturnType> : IMapper<DbSet<DbSetType>, Task<InternalRunnerWrapper<ReturnType>>> { }

    public class DbSet<T>
    { }

    public struct GetAll : IQuery<int, int>
    {
        public Task<InternalRunnerWrapper<int>> Run(DbSet<int> arg)
        {
            return Task.FromResult(
                new InternalRunnerWrapper<int>(new ValueCacheRunner<int>(0))
            );
        }
    }

    public interface IDbSetRunner<DbSetType>
    {
        IRunner<Task<InternalRunnerWrapper<T>>> Run<T>(IQuery<DbSetType, T> ga);
    }

    public struct DbBlogSetRunner : IDbSetRunner<int>, IRunner<DbSet<int>>
    {
        //inject dbset 
        private readonly DbSet<int> dbSet;

        public DbBlogSetRunner(DbSet<int> dbSet)
        {
            this.dbSet = dbSet;
        }

        public IRunner<Task<InternalRunnerWrapper<T>>> Run<T>(IQuery<int, T> query)
        {
            return new MapperRunner<DbSet<int>, T>(query, this);
        }

        DbSet<int> IRunner<DbSet<int>>.Run()
        {
            return dbSet;
        }
    }
}
