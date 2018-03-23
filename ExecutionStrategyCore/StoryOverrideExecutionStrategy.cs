using System;
using System.Collections.Generic;
using System.Text;

namespace ExecutionStrategyCore
{
    public class StoryOverrideExecutionStrategy<T> : ExecutionStrategy<T>
    {
        public StoryOverrideExecutionStrategy(ExecutionStrategy<T> originalStrategy, Func<T> executionStrategyRunOverride) : this(
            originalStrategy,
            () => ExecutionStrategy.Create<T>(executionStrategyRunOverride)
        )
        {
        }

        public StoryOverrideExecutionStrategy(ExecutionStrategy<T> originalStrategy, Func<ExecutionStrategy<T>> storyExecutionStrategyFunction) : base(originalStrategy.Run)
        {
            StoryExecutionStrategies = new Func<ExecutionStrategy<T>>[] {
                storyExecutionStrategyFunction
            };
        }

        internal IEnumerable<Func<ExecutionStrategy<T>>> StoryExecutionStrategies { get; }
    }
}
