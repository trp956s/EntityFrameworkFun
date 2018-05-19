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

        public static ITaskMapRunner2 CreateMapRunner(this ITaskRunner runner)
        {
            var taskMapFactory = new TaskMapRunner2(runner);
            return runner.Run(taskMapFactory);
        }

        public static ITaskMapRunner3 CreateMapRunnerAdaptor(this ITaskRunner runner)
        {
            var baseRunner = CreateMapRunner(runner);
            var factory = new TaskMapRunnerAdaptor(baseRunner);

            return runner.Run(factory);
        }

        //        public static async Task<ReturnType> RunAsync<ReturnType>(this ITaskMapRunner2 runner, IMapper<ITaskRunner, Task<ReturnType>> mapper)
        //        {
        //            throw new Exception();
        //            var adaptor = new TaskMapRunnerAdaptor<ReturnType>().Create;
        ////            return await runner.Run().RunAsync();
        //        }
    }

    //always fake the ITaskMapRunner3 and make better names, also to track this is what you would look for in the runner
    public class TaskMapRunnerAdaptor : ITaskMapRunner3
    {
        private ITaskMapRunner2 taskMapRunner2;

        public TaskMapRunnerAdaptor(ITaskMapRunner2 taskMapRunner2)
        {
            this.taskMapRunner2 = taskMapRunner2;
        }

        //todo: consider an empty interface that says 'wrap me in a value wrapper runner class and then get me OR mock me there!'
        public ITaskMapRunner3 Run()
        {
            return this;
        }

        public async Task<ReturnType> RunAsync<ReturnType>(IMapper<ITaskRunner, Task<ReturnType>> mapper)
        {
            return await taskMapRunner2.RunAsync(new AnotherAdaptor<ReturnType>(mapper));
        }
    }

    internal class AnotherAdaptor<ReturnType> : IAsyncMapper2<ITaskRunner, ReturnType>
    {
        private IMapper<ITaskRunner, Task<ReturnType>> mapper;

        public AnotherAdaptor(IMapper<ITaskRunner, Task<ReturnType>> mapper)
        {
            this.mapper = mapper;
        }

        public async Task<InternalValueCache<ReturnType>> MapAsync(WrappedParameter<ITaskRunner> wrappedParameter)
        {
            var value = await mapper.Run(wrappedParameter.GetValue());
            return new InternalValueCache<ReturnType>(value);
        }
    }

    //todo: move
    public interface ITaskMapRunner3 : IRunner<ITaskMapRunner3>
    {
        Task<ReturnType> RunAsync<ReturnType>(IMapper<ITaskRunner, Task<ReturnType>> mapper);
    }

    public class TaskMapRunner3 : ITaskMapRunner3
    {
        private ITaskRunner runner;

        public TaskMapRunner3(ITaskRunner runner)
        {
            this.runner = runner;
        }

        public async Task<ReturnType> RunAsync<ReturnType>(IMapper<ITaskRunner, Task<ReturnType>> mapper)
        {
            return await mapper.Run(runner);
        }

        public ITaskMapRunner3 Run()
        {
            return this;
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

        public async Task<T> RunAsync<T>(IAsyncMapper2<ITaskRunner, T> mapper)
        {
            var cache = await mapper.MapAsync(new WrappedParameter<ITaskRunner>(runner));
            return runner.Run(new InternalValueCacheUnwrapper<T>(cache));
        }
    }

    public interface ITaskMapRunner2 :
        IRunner<ITaskMapRunner2>
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
