using System;
using System.Collections.Generic;
using System.Text;

namespace ExecutionStrategyCore
{
    public class StoryOverrideExecutionStrategy<T> : ExecutionStrategy<T>
    {
        public StoryOverrideExecutionStrategy(ExecutionStrategy<T> originalStrategy, Func<T> executionStrategyRunOverride) : base(originalStrategy.Run)
        {
            StoryExecutionStrategy = ()=> ExecutionStrategy.Create<T>(executionStrategyRunOverride);
        }

        public StoryOverrideExecutionStrategy(ExecutionStrategy<T> originalStrategy, Func<ExecutionStrategy<T>> storyExecutionStrategyFunction) : base(originalStrategy.Run)
        {
            StoryExecutionStrategy = storyExecutionStrategyFunction;
        }

        internal Func<ExecutionStrategy<T>> StoryExecutionStrategy { get; }
    }
}
