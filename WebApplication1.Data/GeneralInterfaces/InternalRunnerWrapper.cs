using ExecutionStrategyCore;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace WebApplication1.Data.GeneralInterfaces
{
    internal class InternalRunnerWrapper<T1, T2> : IMapper<T1, T2>
    {
        private readonly IInternalRunner<T1, T2> internalRunner;

        internal InternalRunnerWrapper(IInternalRunner<T1, T2> internalRunner)
        {
            this.internalRunner = internalRunner;
        }

        public T2 Run(T1 arg)
        {
            return internalRunner.Run(arg);
        }
    }
}
