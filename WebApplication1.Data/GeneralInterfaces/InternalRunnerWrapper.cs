﻿using ExecutionStrategyCore;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace WebApplication1.Data.GeneralInterfaces
{
    internal class InternalRunnerWrapper<T, T1, T2> : IMapper<T1, T2>
        where T : IInternalRunner<T1, T2>
    {
        private readonly T internalRunner;

        internal InternalRunnerWrapper(T internalRunner)
        {
            this.internalRunner = internalRunner;
        }

        public T2 Run(T1 arg)
        {
            return internalRunner.Run(arg);
        }
    }
}
