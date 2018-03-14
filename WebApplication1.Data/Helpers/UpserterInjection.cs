using System;
using System.Collections.Generic;
using System.Text;
using WebApplication1.Data.Core;
using WebApplication1.Data.Injectors;

namespace WebApplication1.Data.Helpers
{
    public class UpserterInjection<T> : IDependencyInjectionWrapper<IUpsertDbSet<T>>
    where T : class
    {
        public UpserterInjection(BlogDbSetInjector blogContext)
        {
            DbSetWrapper = blogContext;
        }

        public IUpsertDbSet<T> Dependency => throw new NotImplementedException();

        public BlogDbSetInjector DbSetWrapper { get; }
    }
}
