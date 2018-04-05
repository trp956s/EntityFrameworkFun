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

        public T Run<T>(IRunner<T> executionWrapper)
        {
            return executionWrapper.Run();
        }
    }

    /*
     * consider moving all this into the Data Layer library
     */
    internal interface IQuery<DbSetType, ReturnType> : IMapper<DbSet<DbSetType>, Task<InternalRunnerWrapper<ReturnType>>> { }

    internal class DbSet<T>
    { }

    internal struct GetAll : IQuery<int, int>
    {
        public Task<InternalRunnerWrapper<int>> Run(DbSet<int> arg)
        {
            return Task.FromResult(
                new InternalRunnerWrapper<int>(new ValueCacheRunner<int>(0))
            );
        }
    }

    internal interface IDbSetRunner<DbSetType>
    {
        IRunner<Task<InternalRunnerWrapper<T>>> Run<T>(IQuery<DbSetType, T> ga);
    }

    internal struct DbBlogSetRunner : IDbSetRunner<int>, IRunner<DbSet<int>>
    {
        //inject dbset 
        private readonly DbSet<int> dbSet;

        public DbBlogSetRunner(DbSet<int> dbSet)
        {
            this.dbSet = dbSet;
        }

        public IRunner<Task<InternalRunnerWrapper<T>>> Run<T>(IQuery<int, T> query)
        {
            return query.ToRunner(this);
        }

        DbSet<int> IRunner<DbSet<int>>.Run()
        {
            return dbSet;
        }
    }
}
