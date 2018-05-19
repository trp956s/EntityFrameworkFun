using System;
using System.Threading.Tasks;

namespace ExecutionStrategyCore
{
    public static class WrappedParameterExtensions
    {
        private const string NULL_MESSAGE= "Wrapped Parameters are only provided " +
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

        public static ITaskMapRunner3 CreateMapRunner(this ITaskRunner runner)
        {
            var taskMapFactory = new TaskMapRunner3(runner);
            return runner.Run(taskMapFactory);
        }
    }

    public class AnotherAdaptor2<T> : IAsyncMapper2<ITaskRunner, T>
    {
        private IMapper<ITaskRunner, Task<T>> mapper;

        public AnotherAdaptor2(IMapper<ITaskRunner, Task<T>> mapper)
        {
            this.mapper = mapper;
        }

        //this method is what allows 3s to execute 2s
        public async Task<InternalValueCache<T>> MapAsync(WrappedParameter<ITaskRunner> wrappedParameter)
        {
            var value = await mapper.Run(wrappedParameter.GetValue());
            return new InternalValueCache<T>(value);
        }
    }

    public interface ITaskMapRunner3 : IRunner<ITaskMapRunner3>
    {
        Task<ReturnType> RunAsync<ReturnType>(IMapper<ITaskRunner, Task<ReturnType>> mapper);
        Task<T> RunAsync<T>(IAsyncMapper2<ITaskRunner, T> mapper);
    }

    public class TaskMapRunner3 : ITaskMapRunner3
    {
        //generate a type to do everything so that can be the thing that gets mocked!  That can also be the thing that gets replaced
        private ITaskRunner runner;

        //todo: consider an empty interface that says 'wrap me in a value wrapper runner class and then get me OR mock me there!'
        public TaskMapRunner3(ITaskRunner runner)
        {
            this.runner = runner;
        }

        public ITaskMapRunner3 Run()
        {
            return this;
        }

        //this method passes responsiblity for producing the value back from  to TaskMapRunner2
        public async Task<ReturnType> RunAsync<ReturnType>(IMapper<ITaskRunner, Task<ReturnType>> mapper)
        {
            var adaptor = new AnotherAdaptor2<ReturnType>(mapper);
            return await RunAsync(adaptor);
        }

        //this method passes responsiblity for producing the value back from  to TaskMapRunner2
        public async Task<T> RunAsync<T>(IAsyncMapper2<ITaskRunner, T> mapper)
        {
            var factory = new TaskMapRunner2(runner);
            var mapRunner2 = runner.Run(factory);

            return await mapRunner2.RunAsync(mapper);
        }
    }

    public class TaskMapRunner2 : ITaskMapRunner2
    {
        private ITaskRunner runner;

        public TaskMapRunner2(ITaskRunner runner)
        {
            this.runner = runner;
        }

        public ITaskMapRunner2 Run()
        {
            return this;
        }

        //this method is the signally point of responsiblity for running MapAsync
        public async Task<T> RunAsync<T>(IAsyncMapper2<ITaskRunner, T> mapper)
        {
            var cache = await mapper.MapAsync(new WrappedParameter<ITaskRunner>(runner));
            return runner.Run(new InternalValueCacheUnwrapper<T>(cache));
        }
    }

    public interface ITaskMapRunner2 : IRunner<ITaskMapRunner2>
    {
        Task<T> RunAsync<T>(IAsyncMapper2<ITaskRunner, T> mapper);
    }

    public interface IAsyncMapper2<T1, T2>
    {
        Task<InternalValueCache<T2>> MapAsync(WrappedParameter<T1> wrappedParameter);
    }

    public class WrappedParameter<T>
    {
        internal WrappedParameter(T value){
            Value = value;
        }

        internal T Value { get; }
    }
}
