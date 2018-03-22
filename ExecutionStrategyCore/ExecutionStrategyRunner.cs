using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace ExecutionStrategyCore
{
    public class ExecutionStrategyRunner : IExecutionStrategyRunner
    {
        public async Task<T> Run<T>(ExecutionStrategy<T> executionStrategy)
        {
            return await executionStrategy.Run();
        }
    }
}
