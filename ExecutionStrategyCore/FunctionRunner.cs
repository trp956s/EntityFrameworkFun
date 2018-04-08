using System;

namespace ExecutionStrategyCore
{
    public class FunctionRunner<T> : IRunner<T>
    {
        private readonly Func<T> func;

        public FunctionRunner(Func<T> func)
        {
            this.func = func;
        }

        public T Run()
        {
            return func();
        }
    }
}
