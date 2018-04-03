using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace ExecutionStrategyCore
{
    public interface IExecutionStrategyRunner
    {
        Task<T> Run<T>(ExecutionStrategy<T> executionStrategy);
    }

    public interface ITaskRunner //will replace IExecutionStrategyRunner
    {
        Task<T> Run<T>(IRunner<Task<T>> wrapper);
    }
}