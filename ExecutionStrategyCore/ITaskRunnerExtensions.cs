using System.Threading.Tasks;

namespace ExecutionStrategyCore
{
    public static class ITaskRunnerExtensions
    {
        public static T Run<T>(this ITaskRunner runner, InternalRunnerWrapper<T> internalWrapper)
        {
            return runner.Run(internalWrapper.Runner);
        }

        public static async Task<T> Run<T>(this ITaskRunner runner, IRunner<Task<InternalRunnerWrapper<T>>> internalWrapperTask)
        {
            var internalWrapper = await runner.Run(internalWrapperTask);
            return runner.Run(internalWrapper.Runner);
        }

        public static async Task<ReturnType> Run<ParameterType, ReturnType>(this ITaskRunner runner,
            IMapper<ParameterType, Task<InternalRunnerWrapper<ReturnType>>> mapper, IRunner<ParameterType> parameterWrapper)
        {
            IRunner<Task<InternalRunnerWrapper<ReturnType>>> map = new MapperRunner<ParameterType, InternalRunnerWrapper<ReturnType>>(mapper, parameterWrapper);

            var internalWrapper = await runner.Run(map);
            return runner.Run(internalWrapper.Runner);
        }

        public static MapperRunner<ParameterType, ReturnType> ToRunner<ParameterType, ReturnType>(
            this IMapper<ParameterType, Task<ReturnType>> mapper, IRunner<ParameterType> parameter
        )
        {
            return new MapperRunner<ParameterType, ReturnType>(mapper, parameter);
        }

        public static InternalRunnerWrapper<T> ToWrapper<T>(this T value)
        {
            return new InternalRunnerWrapper<T>(new ValueCacheRunner<T>(value));
        }
    }
}
