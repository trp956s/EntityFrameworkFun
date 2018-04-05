using System;
using System.Collections.Generic;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
using System.Linq;

namespace ExecutionStrategyCore
{
    public class StoryOverrideRunner : ITaskRunner
    {
        private readonly ITaskRunner wrappedRunner;
        private readonly IRunner<ActiveStories> storiesGetter;

        public StoryOverrideRunner(ITaskRunner wrappedRunner, IRunner<ActiveStories> storiesGetter) {
            this.wrappedRunner = wrappedRunner;
            this.storiesGetter = storiesGetter;
        }

        public T Run<T>(IRunner<T> executionStrategy)
        {
            if(typeof(StoryOverrideFunctionRunner<T>).IsInstanceOfType(executionStrategy))
            {
                var funcRunner = ((StoryOverrideFunctionRunner<T>)executionStrategy);
                var functionOverride = new FunctionOverrideFilter<T>(funcRunner.Overrides, storiesGetter).Run();
                if (functionOverride != null)
                {
                    executionStrategy = new FunctionRunner<T>(functionOverride);
                }
            }
            return wrappedRunner.Run<T>(executionStrategy);
        }
    }
}
