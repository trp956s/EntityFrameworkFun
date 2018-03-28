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

    public class ExecutionStrategy<T>
    {
        public ExecutionStrategy(Func<Task<T>> run, object source)
        {
            Run = run;
            Source = source;
        }

        internal Func<Task<T>> Run { get; }
        public object Source { get; }
    }
}
