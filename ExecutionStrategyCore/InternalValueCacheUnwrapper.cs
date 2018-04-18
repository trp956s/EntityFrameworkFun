using System;
using System.Collections.Generic;
using System.Text;

namespace ExecutionStrategyCore
{
    internal class InternalValueCacheUnwrapper<T> : IRunner<T>
    {
        private readonly InternalValueCache<T> valueCache;

        internal InternalValueCacheUnwrapper(InternalValueCache<T> valueCache) {
            this.valueCache = valueCache;
        }

        public T Run()
        {
            return valueCache.Value;
        }
    }
}
