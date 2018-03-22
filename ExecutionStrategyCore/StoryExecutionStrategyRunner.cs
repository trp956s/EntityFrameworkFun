using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

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
        public Task<T> Run<T>(ExecutionStrategy<T> executionStrategy)
        {
            throw new NotImplementedException();
        }
    }
}
