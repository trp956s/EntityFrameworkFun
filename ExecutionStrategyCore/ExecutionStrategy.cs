using System;
using System.Threading.Tasks;

namespace ExecutionStrategyCore
{
    public static class ExecutionStrategy
    {
        public static ExecutionStrategy<T> Create<T>(Func<T> run)
        {
            return new ExecutionStrategy<T>(()=>Task.FromResult(run()));
        }
    }

    public class ExecutionStrategy<T>
    {
        public ExecutionStrategy(Func<Task<T>> run)
        {
            Run = run;
        }

        internal Func<Task<T>> Run { get; }
    }
}
