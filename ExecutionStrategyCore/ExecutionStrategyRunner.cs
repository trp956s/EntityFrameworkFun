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
    internal interface IQuery<DbSetType, ReturnType> : IMapper<FakeDbSet<DbSetType>, Task<InternalRunnerWrapper<ReturnType>>> { }

    internal class FakeDbSet<T>
    { }

    internal struct GetAll : IQuery<int, int>
    {
        public Task<InternalRunnerWrapper<int>> Run(FakeDbSet<int> arg)
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

    internal struct DbBlogSetRunner : IDbSetRunner<int>, IRunner<FakeDbSet<int>>
    {
        //inject dbset 
        private readonly FakeDbSet<int> dbSet;

        public DbBlogSetRunner(FakeDbSet<int> dbSet)
        {
            this.dbSet = dbSet;
        }

        public IRunner<Task<InternalRunnerWrapper<T>>> Run<T>(IQuery<int, T> query)
        {
            return query.ToRunner(this);
        }

        FakeDbSet<int> IRunner<FakeDbSet<int>>.Run()
        {
            return dbSet;
        }
    }
}
