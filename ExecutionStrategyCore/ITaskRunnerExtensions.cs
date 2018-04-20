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
        
        ////
        public static async Task<ReturnType> XAsync<ParameterType, ReturnType>(this ITaskRunner runner, IRunner<IMapper<ParameterType, Task<InternalValueCache<ReturnType>>>> mapWrapper, IRunner<ParameterType> parameterWrapper)
        {
            // todo wrap this in a runner constructor
            var mapRunner = runner.CreateMapRunner<IMapper<ParameterType, Task<InternalValueCache<ReturnType>>>, ParameterType, ReturnType>(
                mapWrapper, parameterWrapper);
            var value = await runner.Run(mapRunner);
            return runner.Run(value);
        }

        public static IAsyncMapperRunner<IMapper<ParameterType, Task<InternalValueCache<ReturnType>>>, ParameterType, ReturnType> CreateMapRunner<T, ParameterType, ReturnType>(this ITaskRunner runner, IRunner<IMapper<ParameterType, Task<InternalValueCache<ReturnType>>>> mapper, IRunner<ParameterType> parameterWrapper)
        {
            //todo wrap this in a runner constructor OR use the factory?
            var factory = runner.Run(new AsyncMapperRunnerFactory()); // you can mock this factory and subsequently mock the create runner
            return factory.CreateRunner<IMapper<ParameterType, Task<InternalValueCache<ReturnType>>>, ParameterType, ReturnType>(
                mapper, parameterWrapper, new ValueCacheRunner<ITaskRunner>(runner)
            );
        }
    }

    public class AsyncMapperRunnerFactory : IAsyncMapperRunnerFactory, IRunner<IAsyncMapperRunnerFactory>
    {
        public IAsyncMapperRunnerFactory Run()
        {
            return this;
        }

        public IAsyncMapperRunner<T, ParameterType, ReturnType> CreateRunner<T, ParameterType, ReturnType>(IRunner<T> mapper, IRunner<ParameterType> parameter, IRunner<ITaskRunner> runner)
        where T : IMapper<ParameterType, Task<InternalValueCache<ReturnType>>>
        {
            return new AsyncMapperRunner<T, ParameterType, ReturnType>(mapper, parameter, runner);
        }
    }

    public interface IAsyncMapperRunnerFactory
    {
        IAsyncMapperRunner<T, ParameterType, ReturnType> CreateRunner<T, ParameterType, ReturnType>(IRunner<T> mapper, IRunner<ParameterType> parameter, IRunner<ITaskRunner> runner)
            where T : IMapper<ParameterType, Task<InternalValueCache<ReturnType>>>;
    }

    public interface IAsyncMapperRunner<T, ParameterType, ReturnType> : IRunner<Task<InternalValueCache<ReturnType>>>
        where T : IMapper<ParameterType, Task<InternalValueCache<ReturnType>>>
    {
    }

    public class AsyncMapperRunner<T, ParameterType, ReturnType> : IAsyncMapperRunner<T, ParameterType, ReturnType>
        where T: IMapper<ParameterType, Task<InternalValueCache<ReturnType>>>
    {
        private IRunner<ITaskRunner> runner;

        public AsyncMapperRunner(IRunner<T> mapper, IRunner<ParameterType> parameter, IRunner<ITaskRunner> runner)
        {
            this.runner = runner;
        }

        public Task<InternalValueCache<ReturnType>> Run()
        {
            throw new NotImplementedException();
        }
    }

    //public class Thing2
    //{
    //    public void ActiveStories(params string[] storyNumbers) { }
    //}
}