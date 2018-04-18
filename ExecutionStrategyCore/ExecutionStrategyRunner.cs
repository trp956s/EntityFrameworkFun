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
}
