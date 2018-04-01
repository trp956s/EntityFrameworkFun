using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace ExecutionStrategyCore
{
    public class ExecutionStrategyRunner : IExecutionStrategyRunner
    {
        public async Task<T> Run<T>(ExecutionStrategy<T> executionStrategy)
        {
            return await executionStrategy.Run();
        }

        public interface IAnyRunner
        {
            T Run<T>(InternalRunnerWrapper<T> wrapper);
        }

        public interface IMapper<T1, T2>
        {
            T2 Run(T1 arg);
        }

        public class DbSet<T>
        { }

        public struct GetAll : IMapper<DbSet<int>, InternalRunnerWrapper<int>>
        {
            public InternalRunnerWrapper<int> Run(DbSet<int> arg)
            {
                return new InternalRunnerWrapper<int>(new ValueCacheRunner<int>(0));
            }
        }

        private struct ValueCacheRunner<T> : IRunner<T>
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
            T Run<T>(IMapper<DbSet<DbSetType>, InternalRunnerWrapper<T>> ga);
        }

        public class DbSetRunner<DbSetType> : IDbSetRunner<DbSetType>
        {
            private readonly DbSet<DbSetType> dbSet;
            private readonly IAnyRunner runner;

            public DbSetRunner(DbSet<DbSetType> dbSet, IAnyRunner runner)
            {
                this.dbSet = dbSet;
                this.runner = runner;
            }

            public T Run<T>(IMapper<DbSet<DbSetType>, InternalRunnerWrapper<T>> ga)
            {
                return runner.Run(ga.Run(dbSet));
            }
        }

        public class DbBlogSetRunner : IDbSetRunner<int>
        {
            private readonly DbSet<int> dbSet;
            private readonly IRunner<IDbSetRunner<int>> dbSetRunnerFactory;

            //inject dbset
            public DbBlogSetRunner(DbSet<int> dbSet, IRunner<IDbSetRunner<int>> dbSetRunnerFactory)
            {
                this.dbSet = dbSet;
                this.dbSetRunnerFactory = dbSetRunnerFactory;
            }

            public T Run<T>(IMapper<DbSet<int>, InternalRunnerWrapper<T>> ga)
            {
                return dbSetRunnerFactory.Run().Run(ga);
            }
        }
    }
}
