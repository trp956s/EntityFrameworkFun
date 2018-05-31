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

        public static IMapRunner<ReturnType> ToMapRunner<ReturnType>(this ITaskRunner runner)
        {
            var mapRunnerFactoryFactory = new MapRunnerFactory(runner);
            var mapRunnerFactory = runner.Run(mapRunnerFactoryFactory);
            return mapRunnerFactory.CreateMapRunner<ReturnType>();
        }

        public static async Task<ReturnType> Map<ParameterType, T, ReturnType>(
            this IMapRunner<ReturnType> mapRunner,
            T arg,
            IRunner<ParameterType> parameterFactory
        )
            where T : IMapper<ParameterType, Task<ReturnType>>
        {
            IMapRunnerFactory mapRunnerFactory = new MapRunnerFactory(mapRunner);
            var unwrappedMapRunnerFactory = mapRunner.Run(mapRunnerFactory);
            var unwrappedMapRunner2 = unwrappedMapRunnerFactory.CreateMapRunner<IUnwrappedMapRunner<ReturnType>>();

            //todo: remove await
            var unwrappedMapRunner = await unwrappedMapRunner2.Map(new UnwrappedMapRunnerFactory<ReturnType>(mapRunner), mapRunner);
            return await unwrappedMapRunner.Map(arg, parameterFactory);
        }
    }

    public class UnwrappedMapRunnerFactory<ReturnType> : IMapper<
        WrappedParameter<IMapRunner<ReturnType>>,
        Task<IUnwrappedMapRunner<ReturnType>>
    >
    {
        private ITaskRunner runner;

        public UnwrappedMapRunnerFactory(ITaskRunner runner)
        {
            this.runner = runner;
        }

        public async Task<IUnwrappedMapRunner<ReturnType>> Run(
            WrappedParameter<IMapRunner<ReturnType>> arg
        )
        {
            await Task.CompletedTask;
            var unwrappedMapRunnerFactory = new UnwrappedMapRunner<ReturnType>(arg);
            return runner.Run(unwrappedMapRunnerFactory);
        }
    }

    public class UnwrappedMapRunner<ReturnType> : IUnwrappedMapRunner<ReturnType>
    {
        private WrappedParameter<IMapRunner<ReturnType>> mapRunner;

        public UnwrappedMapRunner(WrappedParameter<IMapRunner<ReturnType>> mapRunner)
        {
            this.mapRunner = mapRunner;
        }

        public async Task<ReturnType> Map<ParameterType, T>(
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

        public IMapRunner<ReturnType> CreateMapRunner<ReturnType>()
        {
            return runner.Run(new MapRunner<ReturnType>(runner));
        }
    }

    public interface IUnwrappedMapRunner<ReturnType> : IRunner<IUnwrappedMapRunner<ReturnType>>
    {
        Task<ReturnType> Map<ParameterType, T>(
            T arg, IRunner<ParameterType> parameterFactory
        )
            where T : IMapper<ParameterType, Task<ReturnType>>;
    }

    public interface IMapRunner<ReturnType> : IRunner<IMapRunner<ReturnType>>, ITaskRunner
    {
        Task<ReturnType> Map<ParameterType, T>(
            T arg, IRunner<ParameterType> parameterFactory
        )
            where T : IMapper<WrappedParameter<ParameterType>, Task<ReturnType>>;
    }

    public struct MapRunner<ReturnType> : IMapRunner<ReturnType>
    {
        private ITaskRunner runner;

        public MapRunner(ITaskRunner runner)
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

        public IMapRunner<ReturnType> Run()
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
