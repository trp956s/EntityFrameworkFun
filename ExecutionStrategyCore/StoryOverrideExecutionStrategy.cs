using System;
using System.Collections.Generic;
using System.Linq;

namespace ExecutionStrategyCore
{
    public class StoryOverrideExecutionStrategy<T> : ExecutionStrategy<T>
    {
        public StoryOverrideExecutionStrategy(ExecutionStrategy<T> originalStrategy, 
            params Func<T>[] executionStrategyRunOverrideFunctions
        ) : base(originalStrategy.Run)
        {
            StoryExecutionStrategies = executionStrategyRunOverrideFunctions.
                Select(x =>
                    new Func<ExecutionStrategy<T>>(() => 
                        ExecutionStrategy.Create<T>(x)
                ));
        }

        public StoryOverrideExecutionStrategy(ExecutionStrategy<T> originalStrategy, 
            params Func<ExecutionStrategy<T>>[] storyExecutionStrategyFunctions
        ) : base(originalStrategy.Run)
        {
            StoryExecutionStrategies = storyExecutionStrategyFunctions;
        }

        internal IEnumerable<Func<ExecutionStrategy<T>>> StoryExecutionStrategies { get; }
    }
}
