using System;
using System.Threading.Tasks;

namespace ExecutionStrategyCore
{
    public static class ExecutionStrategy
    {
        public static ExecutionStrategy<T> Create<T>(Func<T> run, object source = null)
        {
            return new ExecutionStrategy<T>(()=>Task.FromResult(run()), source);
        }
    }

    public class ExecutionStrategy<T> : IRunner<Task<T>>
    {
        private readonly Func<Task<T>> func;

        public ExecutionStrategy(Func<Task<T>> func, object source)
        {
            this.func = func;
            Source = source;
        }

        public object Source { get; }

        public Task<T> Run()
        {
            return this.func();
        }
    }
}
