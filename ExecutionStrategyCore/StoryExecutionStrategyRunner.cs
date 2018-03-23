using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;
using System.Linq;

namespace ExecutionStrategyCore
{
    ///WHEN the strategy is being replaced 1. impliment IStoryDependentExecutionStrategy on your strategy parameters class
    ///the StoryExecutionStrategyRunner will look for this on the parameters property (some strategies will have a 'dead-end' 
    ///parameters that have no properties of their own, but this is fine, in fact this exemplifies why you'd want to have
    ///this type of thing in the first place)

    ///The IStoryDependentExecutionStrategy will expose the story numbers this change applies to
    ///it will also have an alternative Run

    ///In the event that the parameters or dependencies need to be changed of a strategy:
    ///Copy and make a new class with the appropriate properties
    ///Make sure the parent logic is written to be executed with an execution strategy
    ///make sure that the execution strategy mentioned above implements IStoryDependentExecutionStrategy on
    ///the parameters class as stated above


    public class StoryExecutionStrategyRunner : IExecutionStrategyRunner
    {
        private readonly ActiveStories stories;
        private readonly ExecutionStrategyRunner runner;

        public StoryExecutionStrategyRunner(ActiveStories stories, ExecutionStrategyRunner runner)
        {
            this.stories = stories;
            this.runner = runner;
        }

        public async Task<T> Run<T>(ExecutionStrategy<T> executionStrategy)
        {
            if (stories.Any() && executionStrategy is StoryOverrideExecutionStrategy<T>)
            {
                var storyExecutionStrategies = ((StoryOverrideExecutionStrategy<T>)
                    executionStrategy).StoryExecutionStrategies.Reverse();

                foreach(var storyExecutionStrategyFactory in storyExecutionStrategies)
                {
                    var strategy = storyExecutionStrategyFactory();
                    var strategyRunMethod = strategy.Run.Method;
                    var strategyAttributes = strategyRunMethod.GetCustomAttributes(
                        typeof(StoryAttribute), true);
                    var strategyStories = strategyAttributes.
                        OfType<StoryAttribute>().Select(
                            x => x.StoryFlag
                        );

                    if (stories.AnyMatching(strategyStories))
                    {
                        executionStrategy = strategy;
                        break;
                    }
                }
            }

            return await runner.Run<T>(executionStrategy);
        }
    }
}
