using System;
using System.Collections.Generic;
using System.Text;
using WebApplication1.Data.Core;
using WebApplication1.Data.Injectors;
using WebApplication1.Data.Models;

namespace WebApplication1.Data.Helpers
{
    public class UpserterInjection<T> : IDependencyInjectionWrapper<IUpsertDbSet<T>>
    where T : class
    {
        public UpserterInjection(IUpsertDbSet<T> blogContext)
        {
            Dependency = blogContext;
        }

        public IUpsertDbSet<T> Dependency { get; }
    }
}
