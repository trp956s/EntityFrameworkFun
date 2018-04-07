using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace ExecutionStrategyCore
{
    public interface ITaskRunner
    {
        T Run<T>(IRunner<T> wrapper);
    }
}