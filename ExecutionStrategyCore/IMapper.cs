using System;
using System.Collections.Generic;
using System.Text;

namespace ExecutionStrategyCore
{
    public interface IMapper<T1, T2>
    {
        T2 Run(T1 arg);
    }
}
