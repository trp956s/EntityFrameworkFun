using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Threading.Tasks;

namespace ExecutionStrategyCore
{
    public static class StoryOverrideExecutionStrategy
    {
        public static StoryOverrideExecutionStrategy<T> Create<T>(
            ExecutionStrategy<T> originalStrategy,
            params Func<Task<T>>[] executionStrategyRunOverrideFunctions
        )
        {
            var strategyOverrides = executionStrategyRunOverrideFunctions.
                Select(x => new Func<FuncOverrideExecutionStrategy<T>>(() =>
                    new FuncOverrideExecutionStrategy<T>(x, originalStrategy.Source)
                )
            ).ToArray();

            return new StoryOverrideExecutionStrategy<T>(originalStrategy, strategyOverrides);
        }
    }

    public class StoryOverrideExecutionStrategy<T> : ExecutionStrategy<T>, IRunner<IEnumerable<Func<FuncOverrideExecutionStrategy<T>>>>
    {
        public StoryOverrideExecutionStrategy(ExecutionStrategy<T> originalStrategy, 
            params Func<FuncOverrideExecutionStrategy<T>>[] storyExecutionStrategyFunctions
        ) : base(((IRunner<Task<T>>) originalStrategy).Run, originalStrategy.Source)
        {
            StoryExecutionStrategies = storyExecutionStrategyFunctions;
        }

        private IEnumerable<Func<FuncOverrideExecutionStrategy<T>>> StoryExecutionStrategies { get; }

        IEnumerable<Func<FuncOverrideExecutionStrategy<T>>> IRunner<IEnumerable<Func<FuncOverrideExecutionStrategy<T>>>>.Run()
        {
            return StoryExecutionStrategies;
        }
    }

    public class FuncOverrideExecutionStrategy<T> : ExecutionStrategy<T>
    {
        public FuncOverrideExecutionStrategy(Func<Task<T>> func, object source) : base(func, source)
        {
            this.Method = func.Method;
        }

        public MethodInfo Method{ get; }
    }
}
