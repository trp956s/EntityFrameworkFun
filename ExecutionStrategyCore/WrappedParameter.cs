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
            return runner.Run(new TaskMapRunner5(runner));
        }

        public static ITaskMapRunner12 CreateAsyncMapRuner2(
            this ITaskRunner runner
        )
        {
            return runner.Run(new TaskMapRunner12());
        }

        public static ITaskMapRunner6And7<IAsyncMapper2<ITaskRunner, ReturnType>, ReturnType> For6<ReturnType>(this TaskMapRunner5 runner)
        {
            return new TaskMapRunner6<ReturnType>(runner); //todo: actualy pass 5 so 5 will process mapper and 6
        }

        public static ITaskMapRunner6And7<IMapper<ITaskRunner, Task<ReturnType>>, ReturnType> For7<ReturnType>(this TaskMapRunner5 runner)
        {
            return new TaskMapRunner7<ReturnType>(runner); //todo: actualy pass 5 so 5 will process mapper and 6
        }

        public static TaskMapRunner8<ReturnType> For8<ReturnType>(this ITaskRunner runner)
        {
            return new TaskMapRunner8<ReturnType>(runner); //todo: actualy pass 5 so 5 will process mapper and 6
        }

        public static TaskMapRunner9<ReturnType> For9<ReturnType>(this ITaskRunner runner)
        {
            return new TaskMapRunner9<ReturnType>(runner); //todo: actualy pass 5 so 5 will process mapper and 6
        }

        public static TaskMapRunner13<ParameterType> For13<ParameterType>(this ITaskRunner runer, IRunner<ParameterType> parameterFactory)
        {
            return new TaskMapRunner13<ParameterType>(runer, parameterFactory);
        }

        public static TaskMapRunner15<ParameterType> For15<ParameterType>(this ITaskRunner runer, IRunner<ParameterType> parameterFactory)
        {
            return new TaskMapRunner15<ParameterType>(runer, parameterFactory);
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

    public struct TaskMapRunner11<ParameterType, ReturnType>
    {
        private ITaskRunner runner;
        private readonly IRunner<ParameterType> parameterFactory;

        internal TaskMapRunner11(ITaskRunner runner, IRunner<ParameterType> parameterFactory)
        {
            this.runner = runner;
            this.parameterFactory = parameterFactory;
        }

        internal async Task<ReturnType> Run5<T>(T arg)
            where T : IMapper<ParameterType, Task<ReturnType>>
        {
            var parameter = runner.Run(parameterFactory);
            return await arg.Run(parameter);
        }
    }

    public interface ITaskMapRunner12 : IRunner<ITaskMapRunner12>
    {
        Task<ReturnType> Run6<ParameterType, T, ReturnType>(
                    T arg,
                    TaskMapRunner11<ParameterType, ReturnType> taskRunner11
                )
                    where T : IMapper<ParameterType, Task<ReturnType>>
        ;
    }

    public struct TaskMapRunner12 : ITaskMapRunner12
    {
        public ITaskMapRunner12 Run()
        {
            return this;
        }

        public async Task<ReturnType> Run6<ParameterType, T, ReturnType>(
            T arg,
            TaskMapRunner11<ParameterType, ReturnType> taskRunner11
        )
            where T : IMapper<ParameterType, Task<ReturnType>>
        {
            return await taskRunner11.Run5(arg);
        }
    }

    public struct TaskMapRunner8<ReturnType>
    {
        private ITaskRunner runner;

        internal TaskMapRunner8(ITaskRunner runner)
        {
            this.runner = runner;
        }

        public TaskMapRunner11<ParameterType, ReturnType> CreateRunner<ParameterType>(
            IRunner<ParameterType> parameterFactory
        )
        {
            return new TaskMapRunner11<ParameterType, ReturnType>(runner, parameterFactory);
        }
    }

    public struct TaskMapRunner9<ReturnType>
    {
        private ITaskRunner runner;

        internal TaskMapRunner9(ITaskRunner runner)
        {
            this.runner = runner;
        }

        public TaskMapRunner14<WrappedParameter<ParameterType>, ReturnType> CreateRunner<ParameterType>(
            IRunner<ParameterType> parameterFactory
        )
        {
            //TODO: encapsulate the next 2 lines into their own mockable factory class
            var parameter = runner.Run(parameterFactory);
            var wrappedParameter = new WrappedParameter<ParameterType>(parameter);

            var newParameterFactory = new ValueCacheRunner<WrappedParameter<ParameterType>>(wrappedParameter);
            return new TaskMapRunner14<WrappedParameter<ParameterType>, ReturnType>(runner, newParameterFactory);
        }
    }

    public struct TaskMapRunner15<ParameterType>
    {
        private ITaskRunner runner;
        private readonly IRunner<ParameterType> parameterFactory;

        internal TaskMapRunner15(ITaskRunner runner, IRunner<ParameterType> parameterFactory)
        {
            this.runner = runner;
            this.parameterFactory = parameterFactory;
        }

        public TaskMapRunner14<ParameterType, ReturnType> Run7<ReturnType>()
        {
            return new TaskMapRunner14<ParameterType, ReturnType>(runner, parameterFactory);
        }
    }

    public struct TaskMapRunner13<ParameterType>
    {
        private ITaskRunner runner;
        private readonly IRunner<ParameterType> parameterFactory;

        internal TaskMapRunner13(ITaskRunner runner, IRunner<ParameterType> parameterFactory )
        {
            this.runner = runner;
            this.parameterFactory = parameterFactory;
        }

        internal IRunner<WrappedParameter<ParameterType>> CreateRunner()
        {
            //TODO: encapsulate the next 2 lines into their own mockable factory class
            var parameter = runner.Run(parameterFactory);
            var wrappedParameter = new WrappedParameter<ParameterType>(parameter);

            return new ValueCacheRunner<WrappedParameter<ParameterType>>(wrappedParameter);
        }

        public TaskMapRunner14<WrappedParameter<ParameterType>, ReturnType> Run7<ReturnType>()
        {
            return new TaskMapRunner14<WrappedParameter<ParameterType>, ReturnType>(runner, CreateRunner());
        }
    }

    public struct TaskMapRunner14<ParameterType, ReturnType>
    {
        private ITaskRunner runner;
        private IRunner<ParameterType> parameterFactory;

        public TaskMapRunner14(ITaskRunner runner, IRunner<ParameterType> parameterFactory)
        {
            this.runner = runner;
            this.parameterFactory = parameterFactory;
        }

        public async Task<ReturnType> Run8<T>(T arg)
            where T: IMapper<ParameterType, Task<ReturnType>>
        {
            var factoryFactory = new TaskMapRunner17();
            var factory = runner.Run(factoryFactory);
            var factory16 = new TaskMapRunner16<ParameterType, ReturnType>(runner);

            return await factory.Run9(arg, parameterFactory, factory16);
        }
    }

    public interface ITaskMapRunner17 : IRunner<ITaskMapRunner17>
    {
        Task<ReturnType> Run9<ParameterType, T, ReturnType>(
            T mapper, 
            IRunner<ParameterType> parameterFactory, 
            TaskMapRunner16<ParameterType, ReturnType> runner
        )
            where T : IMapper<ParameterType, Task<ReturnType>>;
    }

    public struct TaskMapRunner17 : ITaskMapRunner17
    {
        public ITaskMapRunner17 Run()
        {
            return this;
        }

        public async Task<ReturnType> Run9<ParameterType, T, ReturnType>(T mapper, IRunner<ParameterType> parameterFactory, TaskMapRunner16<ParameterType, ReturnType> runner)
            where T : IMapper<ParameterType, Task<ReturnType>>
        {
            return await runner.Run8(mapper, parameterFactory);
        }
    }

    public struct TaskMapRunner16<ParameterType, ReturnType>
    {
        private ITaskRunner runner;

        public TaskMapRunner16(ITaskRunner runner)
        {
            this.runner = runner;
        }

        public async Task<ReturnType> Run8<T>(T arg, IRunner<ParameterType> parameterFactory)
            where T : IMapper<ParameterType, Task<ReturnType>>
        {
            var parameter = runner.Run(parameterFactory);
            return await arg.Run(parameter);
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

    public struct TaskMapRunner5 : ITaskRunner, IRunner<TaskMapRunner5>
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

        public TaskMapRunner5 Run()
        {
            return this;
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
