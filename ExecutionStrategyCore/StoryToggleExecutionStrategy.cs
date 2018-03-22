using System;
using System.Collections.Generic;
using System.Text;

namespace ExecutionStrategyCore
{
    public class StoryToggleExecutionStrategy<T> : ExecutionStrategy<T>
    {
        public StoryToggleExecutionStrategy(ExecutionStrategy<T> originalStrategy, Func<ExecutionStrategy<T>> storyExecutionStrategyFunction) : base(originalStrategy.Run)
        {
            StoryExecutionStrategy = storyExecutionStrategyFunction;
        }

        internal Func<ExecutionStrategy<T>> StoryExecutionStrategy { get; }
    }
}
