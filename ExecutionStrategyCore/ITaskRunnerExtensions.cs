using System;
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
            var map = new MapperRunner<ParameterType, ReturnType>(mapper, parameterWrapper);

            return await runner.Run(map);
        }

        public static MapperRunner<ParameterType, ReturnType> ToRunner<ParameterType, ReturnType>(
            this IMapper<ParameterType, Task<InternalValueCache<ReturnType>>> mapper, IRunner<ParameterType> parameter
        )
        {
            return new MapperRunner<ParameterType, ReturnType>(mapper, parameter);
        }

        public static InternalValueCache<T> ToWrapper<T>(this T value)
        {
            return new InternalValueCache<T>(value);
        }

        //public static void StoryOverride(this ITaskRunner runner, params Func<Thing>[] assignOverrides)
        //{

        //}

        //public static IRunner<Task<T>> RunnerStoryActiveAsync<T>(this Thing2 t, Func<IRunner<Task<T>>> overrideFunction)
        //{
        //    //if null thiniggy does something else or does nothing
        //    var t2 = new Thing2();
        //    overrideFunction();

        //    return null;
        //}

        //public static bool AnyActive(this Thing2 thing)
        //{
        //    //won't do null
        //    return false;
        //}
        //public static Thing2 ForStories(this ITaskRunner runner, params string[] stories)
        //{
        //    return null;
        //}

        //rename this method!
        public static async Task<ReturnType> XAsync<T, ParameterType, ReturnType>(this ITaskRunner runner, IRunner<T> mapWrapper, IRunner<ParameterType> parameterWrapper)
        where T : IMapper<ParameterType, Task<InternalValueCache<ReturnType>>>
        {
            var mapRunner = new AsyncCreateMapRunner<T, ParameterType, ReturnType>(runner.Wrap(), mapWrapper, parameterWrapper);
            return await runner.Run(mapRunner);
        }

        public static IRunner<ITaskRunner> Wrap(this ITaskRunner runner)
        {
            return new InternalValueCacheUnwrapper<ITaskRunner>(new InternalValueCache<ITaskRunner>(runner));
        }
    }

    public class AsyncMapperRunnerFactory : IAsyncMapperRunnerFactory, IRunner<IAsyncMapperRunnerFactory>
    {
        private ValueCacheRunner<ITaskRunner> runnerWrapper;

        public AsyncMapperRunnerFactory(ValueCacheRunner<ITaskRunner> runnerWrapper)
        {
            this.runnerWrapper = runnerWrapper;
        }

        public IAsyncMapperRunnerFactory Run()
        {
            return this;
        }

        public IAsyncMapperRunner<T, ParameterType, ReturnType> CreateRunner<T, ParameterType, ReturnType>(IRunner<T> mapper, IRunner<ParameterType> parameter)
        where T : IMapper<ParameterType, Task<InternalValueCache<ReturnType>>>
        {
            return new AsyncMapperRunner<T, ParameterType, ReturnType>(mapper, parameter, runnerWrapper);
        }
    }

    public interface IAsyncMapperRunnerFactory
    {
        IAsyncMapperRunner<T, ParameterType, ReturnType> CreateRunner<T, ParameterType, ReturnType>(IRunner<T> mapper, IRunner<ParameterType> parameter)
        where T : IMapper<ParameterType, Task<InternalValueCache<ReturnType>>>;
    }

    public interface IAsyncMapperRunner<T, ParameterType, ReturnType> : IRunner<Task<InternalValueCache<ReturnType>>>
    where T : IMapper<ParameterType, Task<InternalValueCache<ReturnType>>>
    {
    }

    public class AsyncMapperRunner<T, ParameterType, ReturnType> : IAsyncMapperRunner<T, ParameterType, ReturnType>
        where T: IMapper<ParameterType, Task<InternalValueCache<ReturnType>>>
    {
        private readonly IRunner<T> mapperWrapper;
        private readonly IRunner<ParameterType> parameterWrapper;
        private IRunner<ITaskRunner> runnerWrapper;

        public AsyncMapperRunner(IRunner<T> mapperWrapper, IRunner<ParameterType> parameterWrapper, IRunner<ITaskRunner> runnerWrapper)
        {
            this.mapperWrapper = mapperWrapper;
            this.parameterWrapper = parameterWrapper;
            this.runnerWrapper = runnerWrapper;
        }

        public async Task<InternalValueCache<ReturnType>> Run()
        {
            var runner = runnerWrapper.Run();
            var parameter = runner.Run(parameterWrapper);
            var mapper = runner.Run(mapperWrapper);

            return await mapper.Run(parameter);
        }
    }

    public interface IAsyncCreateMapRunner<out T, ParameterType, ReturnType>
        : IRunner<Task<ReturnType>>
    where T : IMapper<ParameterType, Task<InternalValueCache<ReturnType>>>
    {
        T Mapper { get; }
        IRunner<ParameterType> ParameterWrapper { get; }
    }

    public struct AsyncCreateMapRunner<T, ParameterType, ReturnType> :
        IAsyncCreateMapRunner<T, ParameterType, ReturnType>
        where T : IMapper<ParameterType, Task<InternalValueCache<ReturnType>>>
    {
        private ITaskRunner runner;
        private IRunner<T> mapperWrapper;

        public AsyncCreateMapRunner(IRunner<ITaskRunner> runnerWrapper, IRunner<T> mapperWrapper, IRunner<ParameterType> parameterWrapper)
        {
            this.runner = runnerWrapper.Run();
            this.mapperWrapper = mapperWrapper;
            this.ParameterWrapper = parameterWrapper;
        }

        public async Task<ReturnType> Run()
        {
            var parameter = runner.Run(ParameterWrapper);
            var cache = await Mapper.Run(parameter);

            return runner.Run(cache);
        }

        public T Mapper { get { return runner.Run(mapperWrapper); } }
        public IRunner<ParameterType> ParameterWrapper { get; }
    }

    //public class Thing2
    //{
    //    public void ActiveStories(params string[] storyNumbers) { }
    //}
}