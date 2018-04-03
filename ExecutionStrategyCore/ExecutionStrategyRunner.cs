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
    public interface IQuery<DbSetType, ReturnType> : IMapper<DbSet<DbSetType>, InternalRunnerWrapper<Task<ReturnType>>> { }

    public class DbSet<T>
    { }

    public struct GetAll : IQuery<int, int>
    {
        public InternalRunnerWrapper<Task<int>> Run(DbSet<int> arg)
        {
            return new InternalRunnerWrapper<Task<int>>(new ValueCacheRunner<Task<int>>(Task.FromResult(0)));
        }
    }

    public interface IDbSetRunner<DbSetType>
    {
        Task<T> Run<T>(IQuery<DbSetType, T> ga);
    }

    public struct DbBlogSetRunner : IDbSetRunner<int>
    {
        //inject dbset and runner
        private readonly DbSet<int> dbSet;
        private readonly ITaskRunner runner;

        public DbBlogSetRunner(DbSet<int> dbSet, ITaskRunner runner)
        {
            this.dbSet = dbSet;
            this.runner = runner;
        }

        public async Task<T> Run<T>(IQuery<int, T> query)
        {
            return await query.Run(runner, dbSet);
        }
    }
}
