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

        public static ITaskMapRunner3<int> CreateMapRunner(this ITaskRunner runner)
        {
            var taskMapFactory = new TaskMapRunner3<int>(runner);
            return runner.Run(taskMapFactory);
        }

        public static TaskMapRunner5 CreateAsyncMapRuner(
            this ITaskRunner runner
            )
        {
            return new TaskMapRunner5(runner);
        }

        public static ITaskMapRunner6And7<IAsyncMapper2<ITaskRunner, ReturnType>, ReturnType> For6<ReturnType>(this TaskMapRunner5 runner)
        {
            return new TaskMapRunner6<ReturnType>(runner); //todo: actualy pass 5 so 5 will process mapper and 6
        }

        public static ITaskMapRunner6And7<IMapper<ITaskRunner, Task<ReturnType>>, ReturnType> For7<ReturnType>(this TaskMapRunner5 runner)
        {
            return new TaskMapRunner7<ReturnType>(runner); //todo: actualy pass 5 so 5 will process mapper and 6
        }

        public static TaskMapRunner8<ReturnType> For8<ReturnType>(this TaskMapRunner5 runner)
        {
            return new TaskMapRunner8<ReturnType>(runner); //todo: actualy pass 5 so 5 will process mapper and 6
        }

        public static TaskMapRunner9<ReturnType> For9<ReturnType>(this TaskMapRunner5 runner)
        {
            return new TaskMapRunner9<ReturnType>(runner.For8<ReturnType>()); //todo: actualy pass 5 so 5 will process mapper and 6
        }
    }

    public struct TaskMapRunner7<ReturnType> : 
        ITaskMapRunner6And7<IMapper<ITaskRunner, Task<ReturnType>>, ReturnType>
    {
        private TaskMapRunner5 runner;

        public TaskMapRunner7(TaskMapRunner5 runner)
        {
            this.runner = runner;
        }

        public async Task<ReturnType> Run<T>(T arg)
            where T : IMapper<ITaskRunner, Task<ReturnType>>
        {
            return await arg.Run(runner);
        }

        public async Task<ReturnType> Run2<T, ParameterType>(
            T arg, IRunner<ParameterType> parameterFactory
        )
            where T : IMapper<ParameterType, Task<ReturnType>>
        {
            var parameter = runner.Run(parameterFactory);
            return await arg.Run(parameter);
        }
    }

    public struct TaskMapRunner8<ReturnType>
    {
        private TaskMapRunner5 runner;

        public TaskMapRunner8(TaskMapRunner5 runner)
        {
            this.runner = runner;
        }

        public async Task<ReturnType> Run2<T, ParameterType>(
            T arg, IRunner<ParameterType> parameterFactory
        )
            where T : IMapper<ParameterType, Task<ReturnType>>
        {
            var parameter = runner.Run(parameterFactory);
            return await arg.Run(parameter);
        }
    }

    public struct TaskMapRunner9<ReturnType>
    {
        private TaskMapRunner8<ReturnType> runner;

        public TaskMapRunner9(TaskMapRunner8<ReturnType> runner)
        {
            this.runner = runner;
        }

        public async Task<ReturnType> Run3<T, ParameterType>(
            T arg, IRunner<ParameterType> parameterFactory
        )
            where T : IMapper<WrappedParameter<ParameterType>, Task<ReturnType>>
        {
            //todo: use the runner to get the parameter from the factory...
            var parameter = parameterFactory.Run();
            var wrappedParameter = new WrappedParameter<ParameterType>(parameter);
            var newParameterFactory = new ValueCacheRunner<WrappedParameter<ParameterType>>(wrappedParameter);
            return await runner.Run2(arg, newParameterFactory);
        }
    }


    public interface ITaskMapRunner6And7<T, ReturnType>
    {
        Task<ReturnType> Run<ArgType>(ArgType arg)
            where ArgType : T;
    }

    public struct TaskMapRunner6<ReturnType> : ITaskMapRunner6And7<IAsyncMapper2<ITaskRunner, ReturnType>, ReturnType>
    {
        private TaskMapRunner5 runner;

        public TaskMapRunner6(TaskMapRunner5 runner)
        {
            this.runner = runner;
        }

        public async Task<ReturnType> Run<ArgType>(ArgType arg)
            where ArgType : IAsyncMapper2<ITaskRunner, ReturnType>
        {
            var value = await arg.MapAsync(new WrappedParameter<ITaskRunner>(runner));
            return runner.Run(value);
        }
    }

    public struct TaskMapRunner5 : ITaskRunner
    {
        private ITaskRunner runner;

        public TaskMapRunner5(ITaskRunner runner)
        {
            this.runner = runner;
        }

        public T Run<T>(IRunner<T> wrapper)
        {
            return runner.Run(wrapper);
        }

        internal async Task<ResultType> RunAsync<T, ResultType>(T action, TaskMapRunner6<ResultType> mapper)
        {
            //todo use runner
            throw new Exception();
//            return await mapper.Run(action);
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

    //todo replace int with IAsyncMapper2<ITaskRunner, ReturnType>
    public interface ITaskMapRunner3<T> : 
        IRunner<ITaskMapRunner3<T>>,
        ITaskRunner
    {
        Task<ReturnType> RunAsync<ReturnType>(IMapper<ITaskRunner, Task<ReturnType>> mapper);
        Task<ReturnType> RunAsync<ReturnType>(IAsyncMapper2<ITaskRunner, ReturnType> mapper);
    }

    public class TaskMapRunner3<T> : ITaskMapRunner3<T>
    {
        //generate a type to do everything so that can be the thing that gets mocked!  That can also be the thing that gets replaced
        private ITaskRunner runner;

        //todo: consider an empty interface that says 'wrap me in a value wrapper runner class and then get me OR mock me there!'
        public TaskMapRunner3(ITaskRunner runner)
        {
            this.runner = runner;
        }

        public ITaskMapRunner3<T> Run()
        {
            return this;
        }

        public T Run<T>(IRunner<T> wrapper)
        {
            return runner.Run(wrapper);
        }

        //this method passes responsiblity for producing the value back from  to TaskMapRunner2
        public async Task<ReturnType> RunAsync<ReturnType>(IMapper<ITaskRunner, Task<ReturnType>> mapper)
        {
            var tFacotry = new TaskMapRunner4<ITaskRunner, ReturnType>(runner);
            return await tFacotry.RunAsync(mapper);
        }

        //this method passes responsiblity for producing the value back from  to TaskMapRunner2
        public async Task<ReturnType> RunAsync<ReturnType>(IAsyncMapper2<ITaskRunner, ReturnType> mapper)
        {
            var tFacotry = new TaskMapRunner4<ITaskRunner, ReturnType>(runner);
            var adaptor = new AnotherAdaptor<ITaskRunner, ReturnType>(mapper);
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
            //todo: wrap and unwrap value
            var t = await mapper.MapAsync(new WrappedParameter<ITaskRunner>(arg));
            return t.Value;
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
