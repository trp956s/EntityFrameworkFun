using System.Threading.Tasks;

namespace ExecutionStrategyCore
{
    public static class ITaskRunnerExtensions
    {
        public static async Task<T> Run<T>(this ITaskRunner runner, InternalRunnerWrapper<Task<T>> internalWrapper)
        {
            return await runner.Run<T>(internalWrapper.Runner);
        }
    }
}
