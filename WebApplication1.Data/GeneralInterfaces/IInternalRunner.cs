using System;
using System.Collections.Generic;
using System.Text;

namespace WebApplication1.Data.GeneralInterfaces
{
    internal interface IInternalRunner<T1, T2>
    {
        T2 Run(T1 arg);
    }
}
