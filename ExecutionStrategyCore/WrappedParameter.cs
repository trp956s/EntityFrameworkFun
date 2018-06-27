using System;
using System.Threading.Tasks;

namespace ExecutionStrategyCore
{
    public static class WrappedParameterExtensions
    {
        private const string NULL_MESSAGE = "Wrapped Parameters are only provided " +
            "within the context of a runner.Run / runner.RunAsync extension " +
            "method.  There is no other way to get an instance of " +
            "WrappedParameter or a WrappedMapValues";

        //todo: do not return only set
        public static T GetValue<T>(this WrappedParameter<T> wrappedParameter)
        {
            if (wrappedParameter == null)
            {
                throw new ArgumentNullException(nameof(wrappedParameter),
                    NULL_MESSAGE
                );
            }

            return wrappedParameter.Value;
        }

        public static IAsyncMapRunner<ReturnType> ToAsyncMapRunner<ReturnType>(this ITaskRunner runner)
        {
            var mapRunnerFactoryFactory = new MapRunnerFactory(runner);
            var mapRunnerFactory = runner.Run(mapRunnerFactoryFactory);
            return mapRunnerFactory.CreateMapRunnerAsync<ReturnType>();
        }

        public static IMapRunner<ReturnType> ToMapRunner<ReturnType>(this ITaskRunner runner)
        {
            var mapRunnerFactoryFactory = new MapRunnerFactory(runner);
            var mapRunnerFactory = runner.Run(mapRunnerFactoryFactory);
            return mapRunnerFactory.CreateMapRunner<ReturnType>();
        }

        public static async Task<ReturnType> MapUnwrapped<ParameterType, T, ReturnType>(
            this IAsyncMapRunner<ReturnType> mapRunner,
            T arg,
            IRunner<ParameterType> parameterFactory
        )
            where T : IMapper<ParameterType, Task<ReturnType>>
        {
            var unwrappedMapRunner = mapRunner.Map(new MapFactory<ReturnType>());
            return await unwrappedMapRunner.MapAsync(arg, parameterFactory);
        }

        public static T2 Map<T1, T2>(this T1 runner, IMapper<T1, T2> mapper)
            where T1 : ITaskRunner
        {
            return mapper.Run(runner);
        }

        public static async Task<T2> MapAsync<T1, T2>(this T1 runner, IMapper<T1, Task<T2>> mapper)
            where T1 : ITaskRunner
        {
            return await mapper.Run(runner);
        }
    }

    public interface ITypedMapper<T>
    {
        T2 Map<T2>(T arg);
    }

    public class MapFactory<ReturnType> :
        IMapper<ITaskRunner, IUnwrappedMapRunner<ReturnType>>

    {
        public IUnwrappedMapRunner<ReturnType> Run(ITaskRunner runner)
        {
            var mapRunner = runner.Map(new AsyncRunnerFactory<ReturnType>());
            return mapRunner.Map(new UnwrappedAsyncMapRunner<ReturnType>());
        }
    }

    public class UnwrappedAsyncMapRunner<ReturnType> :
        IMapper<IAsyncMapRunner<ReturnType>, IUnwrappedMapRunner<ReturnType>>
    {
        public IUnwrappedMapRunner<ReturnType> Run(IAsyncMapRunner<ReturnType> mapRunner)
        {
            var unwrappedRunnerMapperRunner = mapRunner.Map(new MapRunnerFactory<ReturnType>());
            var unwrappedMapRunnerFactory = mapRunner.Map(new UnwrappedAsyncMapRunnerFactoryFactory<ReturnType>());
            return unwrappedRunnerMapperRunner.Map(unwrappedMapRunnerFactory, mapRunner);
        }
    }

    public class AsyncRunnerFactory<ReturnType>
        : IMapper<ITaskRunner, IAsyncMapRunner<ReturnType>>
    {
        public IAsyncMapRunner<ReturnType> Run(ITaskRunner arg)
        {
            return new AsyncMapRunner<ReturnType>(arg);
        }
    }

    public class UnwrappedAsyncMapRunnerFactoryFactory<ReturnType> :
        IMapper<ITaskRunner, UnwrappedAsyncMapRunnerFactory<ReturnType>>
    {
        public UnwrappedAsyncMapRunnerFactory<ReturnType> Run(ITaskRunner runner)
        {
            return new UnwrappedAsyncMapRunnerFactory<ReturnType>(runner);
        }
    }

    public class MapRunnerFactory<ReturnType> : IMapper<ITaskRunner, IMapRunner<IUnwrappedMapRunner<ReturnType>>>
    {
        public IMapRunner<IUnwrappedMapRunner<ReturnType>> Run(ITaskRunner runner)
        {
            IMapRunnerFactory mapRunnerFactoryFactory = new MapRunnerFactory(runner);
            var mapRunnerFactory = runner.Run(mapRunnerFactoryFactory);
            return mapRunnerFactory.CreateMapRunner<IUnwrappedMapRunner<ReturnType>>();
        }
    }

    public class UnwrappedAsyncMapRunnerFactory<ReturnType> : 
        IMapper<WrappedParameter<IAsyncMapRunner<ReturnType>>, IUnwrappedMapRunner<ReturnType>>
    {
        private ITaskRunner runner;

        public UnwrappedAsyncMapRunnerFactory(ITaskRunner runner)
        {
            this.runner = runner;
        }

        public IUnwrappedMapRunner<ReturnType> Run(WrappedParameter<IAsyncMapRunner<ReturnType>> arg)
        {
            var parameterValue = arg.GetValue();
            var unWrappedMapRunnerFactory = new UnwrappedMapRunner<ReturnType>(arg);
            return runner.Run(unWrappedMapRunnerFactory);
        }
    }

    public class UnwrappedMapRunner<ReturnType> : IUnwrappedMapRunner<ReturnType>
    {
        private WrappedParameter<IAsyncMapRunner<ReturnType>> mapRunner;

        public UnwrappedMapRunner(WrappedParameter<IAsyncMapRunner<ReturnType>> mapRunner)
        {
            this.mapRunner = mapRunner;
        }

        public async Task<ReturnType> MapAsync<ParameterType, T>(
            T arg, 
            IRunner<ParameterType> parameterFactory
        )
            where T : IMapper<ParameterType, Task<ReturnType>>
        {
            var unwrappedParameterMapper = new UnwrappedParameterMapper<ParameterType, T, ReturnType>(arg);
            return await mapRunner.GetValue().Map(unwrappedParameterMapper, parameterFactory);
        }

        public IUnwrappedMapRunner<ReturnType> Run()
        {
            return this;
        }
    }

    public class UnwrappedParameterMapper<ParameterType, T, ReturnType> : 
        IMapper<WrappedParameter<ParameterType>, Task<ReturnType>>
        where T : IMapper<ParameterType, Task<ReturnType>>
    {
        private T arg;

        public UnwrappedParameterMapper(T arg)
        {
            this.arg = arg;
        }

        public async Task<ReturnType> Run(WrappedParameter<ParameterType> wrappedParameter)
        {
            var parameter = wrappedParameter.GetValue();
            return await arg.Run(parameter);
        }
    }

    public interface IMapRunnerFactory : IRunner<IMapRunnerFactory>
    {
        IAsyncMapRunner<ReturnType> CreateMapRunnerAsync<ReturnType>();
        IMapRunner<ReturnType> CreateMapRunner<ReturnType>();
    }

    public class MapRunnerFactory : IMapRunnerFactory
    {
        private ITaskRunner runner;

        public MapRunnerFactory(ITaskRunner runner)
        {
            this.runner = runner;
        }

        public IMapRunnerFactory Run()
        {
            return this;
        }

        public IAsyncMapRunner<ReturnType> CreateMapRunnerAsync<ReturnType>()
        {
            return runner.Run(new AsyncMapRunner<ReturnType>(runner));
        }

        public IMapRunner<ReturnType> CreateMapRunner<ReturnType>()
        {
            return runner.Run(new MapRunner<ReturnType>(runner));
        }
    }

    internal class MapRunner<ReturnType> : 
        IMapRunner<ReturnType>
    {
        private ITaskRunner runner;

        public MapRunner(ITaskRunner runner)
        {
            this.runner = runner;
        }

        public ReturnType Map<ParameterType, T>(T arg, IRunner<ParameterType> parameterFactory) 
            where T : IMapper<WrappedParameter<ParameterType>, ReturnType>
        {
            var parameter = runner.Run(CreateWrappedParameter(parameterFactory));
            return arg.Run(parameter);
        }

        [Obsolete("This /will be/ replaced with a unique class which is 1. mockable and 2. not contain a publically exposed constructor")]
        private IRunner<WrappedParameter<ParameterType>> CreateWrappedParameter<ParameterType>(IRunner<ParameterType> parameterFactory)
        {
            //TODO: encapsulate the next 2 lines into their own mockable factory class
            var parameter = runner.Run(parameterFactory);
            return new ValueCacheRunner<WrappedParameter<ParameterType>>(new WrappedParameter<ParameterType>(parameter));
        }

        public IMapRunner<ReturnType> Run()
        {
            return this;
        }

        public T Run<T>(IRunner<T> wrapper)
        {
            return runner.Run(wrapper);
        }
    }

    public interface IUnwrappedMapRunner<ReturnType> : IRunner<IUnwrappedMapRunner<ReturnType>>
    {
        Task<ReturnType> MapAsync<ParameterType, T>(
            T arg, IRunner<ParameterType> parameterFactory
        )
            where T : IMapper<ParameterType, Task<ReturnType>>;
    }

    public interface IAsyncMapRunner<ReturnType> : 
        IRunner<IAsyncMapRunner<ReturnType>>, 
        IBaseMapRunner<Task<ReturnType>>
    {
    }

    public interface IMapRunner<ReturnType> : 
        ITaskRunner,
        IRunner<IMapRunner<ReturnType>>, 
        IBaseMapRunner<ReturnType>
    {
    }

    public interface IBaseMapRunner<ReturnType> : ITaskRunner
    {
        ReturnType Map<ParameterType, T>(
            T arg, IRunner<ParameterType> parameterFactory
        )
        where T : IMapper<WrappedParameter<ParameterType>, ReturnType>;
    }

    public struct AsyncMapRunner<ReturnType> : 
        IAsyncMapRunner<ReturnType>
    {
        private ITaskRunner runner;

        public AsyncMapRunner(ITaskRunner runner)
        {
            this.runner = runner;
        }

        public async Task<ReturnType> Map<ParameterType, T>(
            T arg, IRunner<ParameterType> parameterFactory
        )
            where T : IMapper<WrappedParameter<ParameterType>, Task<ReturnType>>
        {
            var parameter = runner.Run(CreateWrappedParameter(parameterFactory));
            return await arg.Run(parameter);
        }

        public IAsyncMapRunner<ReturnType> Run()
        {
            return this;
        }

        [Obsolete("This /will be/ replaced with a unique class which is 1. mockable and 2. not contain a publically exposed constructor")]
        private IRunner<WrappedParameter<ParameterType>> CreateWrappedParameter<ParameterType>(IRunner<ParameterType> parameterFactory)
        {
            //TODO: encapsulate the next 2 lines into their own mockable factory class
            var parameter = runner.Run(parameterFactory);
            return new ValueCacheRunner<WrappedParameter<ParameterType>>(new WrappedParameter<ParameterType>(parameter));
        }

        public T Run<T>(IRunner<T> wrapper)
        {
            return runner.Run(wrapper);
        }
    }

    public class WrappedParameter<T>
    {
        internal WrappedParameter(T value){
            Value = value;
        }

        internal T Value { get; }
    }
}
