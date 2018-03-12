using System;
using System.Collections.Generic;
using System.Text;

namespace WebApplication1.Data.Core
{
    public interface IDependencyInjectionWrapper<T>
    {
        T Dependency { get; }
    }
}
