using System;
using System.Collections.Generic;
using System.Linq;
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
                Select(x => new Func<ExecutionStrategy<T>>(() =>
                    new ExecutionStrategy<T>(x, originalStrategy.Source)
                )
            ).ToArray();

            return new StoryOverrideExecutionStrategy<T>(originalStrategy, strategyOverrides);
        }
    }

    public class StoryOverrideExecutionStrategy<T> : ExecutionStrategy<T>
    {
        public StoryOverrideExecutionStrategy(ExecutionStrategy<T> originalStrategy, 
            params Func<ExecutionStrategy<T>>[] storyExecutionStrategyFunctions
        ) : base(originalStrategy.Run, originalStrategy.Source)
        {
            StoryExecutionStrategies = storyExecutionStrategyFunctions;
        }

        internal IEnumerable<Func<ExecutionStrategy<T>>> StoryExecutionStrategies { get; }
    }
}
