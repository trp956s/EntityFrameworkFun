using System;
using System.Collections.Generic;
using System.Linq;

namespace ExecutionStrategyCore
{
    public class StoryOverrideFunctionRunner<T> : FunctionRunner<T>
    {
        public StoryOverrideFunctionRunner(Func<T> func, params Func<T>[] overrides) : base(func)
        {
            OverridesList = overrides.ToList();
        }

        public StoryOverrideFunctionRunner(Func<T> func) : base(func)
        {
            OverridesList = new List<Func<T>>();
        }

        public void AddOverride(Func<T> overrideFunction, params string[] storyNumbers)
        {
            var storyRunner = new StoryFunctionRunner<T>(overrideFunction, storyNumbers);
            OverridesList.Add(storyRunner.Run);
        }

        private List<Func<T>> OverridesList { get; }
        public Func<T>[] Overrides { get { return OverridesList.ToArray(); } }
    }
}
