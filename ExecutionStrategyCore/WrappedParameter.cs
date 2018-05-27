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
            this IMapRunner<ReturnType> taskMapRunner9,
            T arg,
            IRunner<ParameterType> parameterFactory
        )
            where T : IMapper<WrappedParameter<ParameterType>, Task<ReturnType>>
        {
            var wrappedParameter = taskMapRunner9.CreateWrappedParameter(parameterFactory);
            return await taskMapRunner9.Map(arg, wrappedParameter);
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

    public interface IMapRunner<ReturnType> : IRunner<IMapRunner<ReturnType>>
    {
        Task<ReturnType> Map<ParameterType, T>(
            T arg, IRunner<WrappedParameter<ParameterType>> parameterFactory
        )
            where T : IMapper<WrappedParameter<ParameterType>, Task<ReturnType>>;

        IRunner<WrappedParameter<ParameterType>> CreateWrappedParameter<ParameterType>(IRunner<ParameterType> parameterFactory);
    }

    public struct MapRunner<ReturnType> : IMapRunner<ReturnType>
    {
        private ITaskRunner runner;

        internal MapRunner(ITaskRunner runner)
        {
            this.runner = runner;
        }

        public async Task<ReturnType> Map<ParameterType, T>(
            T arg, IRunner<WrappedParameter<ParameterType>> parameterFactory
        )
            where T : IMapper<WrappedParameter<ParameterType>, Task<ReturnType>>
        {
            var parameter = runner.Run(parameterFactory);
            return await arg.Run(parameter);
        }

        public IMapRunner<ReturnType> Run()
        {
            return this;
        }

        [Obsolete("This /will be/ replaced with a unique class which is 1. mockable and 2. not contain a publically exposed constructor")]
        public IRunner<WrappedParameter<ParameterType>> CreateWrappedParameter<ParameterType>(IRunner<ParameterType> parameterFactory)
        {
            //TODO: encapsulate the next 2 lines into their own mockable factory class
            var parameter = runner.Run(parameterFactory);
            return new ValueCacheRunner<WrappedParameter<ParameterType>>(new WrappedParameter<ParameterType>(parameter));
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
