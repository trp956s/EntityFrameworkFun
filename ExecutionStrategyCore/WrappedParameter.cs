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
            var tFacotry = new TaskMapRunner4<ITaskRunner, ReturnType>(runner);
            return await tFacotry.RunAsync(mapper);
        }

        //this method passes responsiblity for producing the value back from  to TaskMapRunner2
        public async Task<T> RunAsync<T>(IAsyncMapper2<ITaskRunner, T> mapper)
        {
            var tFacotry = new TaskMapRunner4<ITaskRunner, T>(runner);
            var adaptor = new AnotherAdaptor<ITaskRunner, T>(mapper);
            return await tFacotry.RunAsync(adaptor);
        }
    }

    public class AnotherAdaptor<ParameterType, ReturnType> : IMapper<ITaskRunner, Task<ReturnType>>
    {
        private IAsyncMapper2<ITaskRunner, ReturnType> mapper;

        public AnotherAdaptor(IAsyncMapper2<ITaskRunner, ReturnType> mapper)
        {
            this.mapper = mapper;
        }

        public async Task<ReturnType> Run(ITaskRunner arg)
        {
            var v = arg.CreateMapRunner();
            var x = await v.RunAsync(mapper);

            return x;
        }
    }

    public interface IAsyncMapper2<T1, T2>
    {
        Task<InternalValueCache<T2>> MapAsync(WrappedParameter<T1> wrappedParameter);
    }

    public interface ITaskMapRunner4<ParameterType, ReturnType>
         : IRunner<ITaskMapRunner4<ParameterType, ReturnType>>
    {
        Task<ReturnType> RunAsync<T>(T mapper)
            where T : IMapper<ParameterType, Task<ReturnType>>;
    }

    public class TaskMapRunner4<ParameterType, ReturnType>
        : ITaskMapRunner4<ParameterType, ReturnType>
    {
        private ParameterType runner;

        public TaskMapRunner4(ParameterType runner)
        {
            this.runner = runner;
        }

        public ITaskMapRunner4<ParameterType, ReturnType> Run()
        {
            return this;
        }

        //this method is the signally point of responsiblity for running MapAsync
        public async Task<ReturnType> RunAsync<T>(T mapper)
            where T : IMapper<ParameterType, Task<ReturnType>>
        {
            return await mapper.Run(runner);
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
