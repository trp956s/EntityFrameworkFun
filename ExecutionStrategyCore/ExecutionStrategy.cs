using System;
using System.Threading.Tasks;

namespace ExecutionStrategyCore
{
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
            return func();
        }
    }
}
