using System;
using System.Threading.Tasks;

namespace ExecutionStrategyCore
{
    public class ExecutionStrategy<T>
    {
        public ExecutionStrategy(Func<T> run)
        {
            Run = () => Task.FromResult(run());
        }

        public ExecutionStrategy(Func<Task<T>> run)
        {
            Run = run;
        }

        internal Func<Task<T>> Run { get; }
    }
}
