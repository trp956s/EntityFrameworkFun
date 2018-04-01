using System;
using System.Collections.Generic;
using System.Text;

namespace ExecutionStrategyCore
{
    public interface IRunner<T>
    {
        T Run();
    }
}
