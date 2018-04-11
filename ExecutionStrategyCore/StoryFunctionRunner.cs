using System;
using System.Collections.Generic;
using System.Text;

namespace ExecutionStrategyCore
{
    public class StoryFunctionRunner<T> : FunctionRunner<T>
    {
        public StoryFunctionRunner(Func<T> func, params string[] storyNumbers) : base(func) {
            StoryNumbers = storyNumbers;
        }

        public string[] StoryNumbers { get; }
    }
}
