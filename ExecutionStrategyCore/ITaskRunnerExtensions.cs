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

        public static bool IsStoryOverrideActive(this ITaskRunner runner, out IStoryOverride storyDefinitionFilter, params string[] stories)
        {
            var potentialStoryDefinitionFilter = runner.Run(new EmptyStoryOverride(stories));
            if(potentialStoryDefinitionFilter.AnyStoriesActive())
            {
                storyDefinitionFilter = potentialStoryDefinitionFilter;
            }
            else
            {
                storyDefinitionFilter = null;
            }

            return potentialStoryDefinitionFilter.AnyStoriesActive();
        }

        //TODO: accept the default as an argument here
        public static T Run<T>(this ITaskRunner runner, IStoryOverride storyOverride, Func<T> storyOverrideFunc)
        {
            if(storyOverride == null || storyOverrideFunc == null || !storyOverride.AnyStoriesActive())
            {
                return default(T);
            }

            var overrideFunction = storyOverride.CreateOverride<T>(storyOverrideFunc);

            if(overrideFunction == null)
            {
                return default(T);
            }

            return runner.Run(overrideFunction);
        }

        public static IRunner<T> CreateOverride<T>(IStoryOverride storyOverride, Func<T> storyOverrideFunction)
        {
            return new ValueCacheRunner<T>();
        }

        //rename this method!
        public static async Task<ReturnType> XAsync<T, ParameterType, ReturnType>(this ITaskRunner runner, IRunner<T> mapWrapper, IRunner<ParameterType> parameterWrapper)
        where T : IMapper<ParameterType, Task<InternalValueCache<ReturnType>>>
        {
            var mapRunner = new AsyncCreateMapRunner<T, ParameterType, ReturnType>(runner.Wrap(), mapWrapper, parameterWrapper);
            return await runner.Run(mapRunner);
        }

        //rename this method!
        public static async Task<ReturnType> XAsync2<T, ParameterType, ReturnType>(this ITaskRunner runner, T mapWrapper, IRunner<ParameterType> parameterWrapper)
        where T : IRunner<InternalValueCache<IMapper<ParameterType, Task<ReturnType>>>>
        {
            var mapRunner = new AsyncCreateMapRunner2<T, ParameterType, ReturnType>(runner.Wrap(), mapWrapper, parameterWrapper);
            return await runner.Run(mapRunner);
        }

        public static IRunner<ITaskRunner> Wrap(this ITaskRunner runner)
        {
            return new InternalValueCacheUnwrapper<ITaskRunner>(new InternalValueCache<ITaskRunner>(runner));
        }
    }

    public interface IStoryOverride
    {
        bool AnyStoriesActive();
        IRunner<T> CreateOverride<T>(Func<T> storyOverrideFunction);
    }

    public struct EmptyStoryOverride : IStoryOverride, IMapper<IRunner<ActiveStories>, IStoryOverride>, IRunner<IStoryOverride>
    {
        private readonly string[] stories;
        internal EmptyStoryOverride(string[] stories) {
            this.stories = stories;
        }

        public bool AnyStoriesActive()
        {
            return false;
        }

        //the overridden ITaskRunner with story overrides calls this method
        public IStoryOverride Run(IRunner<ActiveStories> activeStories)
        {
            var activeFilteredStoryOverride = new ActiveFilteredStoryOverride(stories, activeStories);
            if (activeFilteredStoryOverride.AnyStoriesActive())
            {
                return activeFilteredStoryOverride;
            }

            return this;
        }

        public IStoryOverride Run()
        {
            return this;
        }

        public IRunner<T> CreateOverride<T>(Func<T> storyOverrideFunction)
        {
            return new ValueCacheRunner<T>();
        }
    }

    public struct ActiveFilteredStoryOverride : IStoryOverride
    {
        private string[] stories;
        private IRunner<ActiveStories> activeStories;
        //todo: add ITaskRunner jto constructor

        internal ActiveFilteredStoryOverride(string[] stories, IRunner<ActiveStories> activeStories)
        {
            this.stories = stories;
            this.activeStories = activeStories;
        }

        public bool AnyStoriesActive()
        {
            if(activeStories == null)
            {
                return false;
            }

            var unwrappedActiveStories = activeStories.Run();

            return unwrappedActiveStories != null && unwrappedActiveStories.Any() && unwrappedActiveStories.AnyMatching(stories);
        }

        public IRunner<T> CreateOverride<T>(Func<T> storyOverrideFunction)
        {
            if(!AnyStoriesActive())
            {
                return new ValueCacheRunner<T>();
            }
            return new FunctionRunner<T>(storyOverrideFunction);
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

    public struct AsyncCreateMapRunner2<T, ParameterType, ReturnType>:IRunner<Task<ReturnType>>
    where T : IRunner<InternalValueCache<IMapper<ParameterType, Task<ReturnType>>>>
    {
        private ITaskRunner runner;

        public AsyncCreateMapRunner2(IRunner<ITaskRunner> runnerWrapper, T mapperWrapper, IRunner<ParameterType> parameterWrapper)
        {
            this.runner = runnerWrapper.Run();
            this.MapperWrapper = mapperWrapper;
            this.ParameterWrapper = parameterWrapper;
        }

        public async Task<ReturnType> Run()
        {
            throw new NotImplementedException();
            //var parameter = runner.Run(ParameterWrapper);
            //var mapperCache = runner.Run(mapWrapper);
            //var mapper = new InternalValueCacheUnwrapper<T>(mapperCache);

            //            return await Mapper.Run(parameter);
        }

        public T MapperWrapper { get; }

        private IMapper<ParameterType, Task<ReturnType>> GetMapper()
        {
            var cache = runner.Run(MapperWrapper);
            return cache.Value;
        }

        public IRunner<ParameterType> ParameterWrapper { get; }
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
}