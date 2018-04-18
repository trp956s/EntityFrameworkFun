using System.Threading.Tasks;

namespace ExecutionStrategyCore
{
    public static class ITaskRunnerExtensions
    {
        public static T Run<T>(this ITaskRunner runner, InternalRunnerWrapper<T> internalWrapper)
        {
            return runner.Run(internalWrapper.Runner);
        }

        public static T Run<T>(this ITaskRunner runner, InternalValueCache<T> internalValueCache)
        {
            return runner.Run(new InternalValueCacheUnwrapper<T>(internalValueCache));
        }

        public static async Task<T> Run<T>(this ITaskRunner runner, IRunner<Task<InternalRunnerWrapper<T>>> internalWrapperTask)
        {
            var internalWrapper = await runner.Run(internalWrapperTask);
            return runner.Run(internalWrapper.Runner);
        }

        public static async Task<ReturnType> Run<ParameterType, ReturnType>(this ITaskRunner runner,
            IMapper<ParameterType, Task<InternalValueCache<ReturnType>>> mapper, IRunner<ParameterType> parameterWrapper)
        {
            IRunner<Task<InternalValueCache<ReturnType>>> map = new MapperRunner<ParameterType, InternalValueCache<ReturnType>>(mapper, parameterWrapper);

            var internalWrapper = await runner.Run(map);
            return runner.Run(internalWrapper);
        }

        public static MapperRunner<ParameterType, ReturnType> ToRunner<ParameterType, ReturnType>(
            this IMapper<ParameterType, Task<ReturnType>> mapper, IRunner<ParameterType> parameter
        )
        {
            return new MapperRunner<ParameterType, ReturnType>(mapper, parameter);
        }

        public static InternalValueCache<T> ToWrapper<T>(this T value)
        {
            return new InternalValueCache<T>(value);
        }
    }
}
