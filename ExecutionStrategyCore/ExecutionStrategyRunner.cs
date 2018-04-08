using System.Threading.Tasks;

namespace ExecutionStrategyCore
{
    public class ExecutionStrategyRunner : ITaskRunner
    {
        public T Run<T>(IRunner<T> executionWrapper)
        {
            return executionWrapper.Run();
        }
    }

    /*
     * example of DbSetRunner
     */
    internal interface IQuery<DbSetType, ReturnType> : IMapper<FakeDbSet<DbSetType>, Task<InternalRunnerWrapper<ReturnType>>> { }

    internal class FakeDbSet<T>
    { }

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
