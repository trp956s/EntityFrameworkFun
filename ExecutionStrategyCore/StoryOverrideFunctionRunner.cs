using System;
using System.Collections.Generic;
using System.Text;

namespace ExecutionStrategyCore
{
    public class StoryOverrideFunctionRunner<T> : FunctionRunner<T>
    {
        public StoryOverrideFunctionRunner(Func<T> func, params Func<T>[] overrides) : base(func) {
            Overrides = overrides;
        }

        public Func<T>[] Overrides { get; }
    }
}
