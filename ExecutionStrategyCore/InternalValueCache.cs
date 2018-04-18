using System;
using System.Collections.Generic;
using System.Text;

namespace ExecutionStrategyCore
{
    public class InternalValueCache<T>
    {
        public InternalValueCache(T value)
        {
            Value = value;
        }

        internal T Value { get; }
    }
}
