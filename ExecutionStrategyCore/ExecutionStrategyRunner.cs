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

        public async Task<T> Run<T>(InternalRunnerWrapper<Task<T>> wrapper)
        {
            return await wrapper.Runner.Run();
        }
    }

    public interface ITaskRunner //will replace IExecutionStrategyRunner
    {
        Task<T> Run<T>(InternalRunnerWrapper<Task<T>> wrapper);
    }

    public interface IMapper<T1, T2>
    {
        T2 Run(T1 arg);
    }

    public interface IQuerySingle<DbSetType> : IMapper<DbSet<DbSetType>, InternalRunnerWrapper<Task<DbSetType>>> { }

    public class DbSet<T>
    { }

    public struct GetAll : IQuerySingle<int>
    {
        public InternalRunnerWrapper<Task<int>> Run(DbSet<int> arg)
        {
            return new InternalRunnerWrapper<Task<int>>(new ValueCacheRunner<Task<int>>(Task.FromResult(0)));
        }
    }

    public struct ValueCacheRunner<T> : IRunner<T>
    {
        private readonly T value;

        public ValueCacheRunner(T value) {
            this.value = value;
        }

        public T Run()
        {
            return value;
        }
    }

    public interface IDbSetRunner<DbSetType>
    {
        Task<T> Run<T>(IMapper<DbSet<DbSetType>, InternalRunnerWrapper<Task<T>>> ga);
    }

    public static class DbSetRunner<DbSetType>
    {
        public static async Task<T> Run<T>(ITaskRunner runner, DbSet<DbSetType> dbSet, IMapper<DbSet<DbSetType>, InternalRunnerWrapper<Task<T>>> ga)
        {
            return await runner.Run(ga.Run(dbSet));
        }
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

        public async Task<T> Run<T>(IMapper<DbSet<int>, InternalRunnerWrapper<Task<T>>> ga)
        {
            return await DbSetRunner<int>.Run(runner, dbSet, ga);
        }
    }
}
