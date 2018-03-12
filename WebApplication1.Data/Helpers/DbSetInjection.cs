using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using WebApplication1.Data.Core;

namespace WebApplication1.Data.Helpers
{
    /// <summary>
    /// Provides an ienumerable of a wrapped dbset.  For use with an IAsyncExecutableRunner dependent upon on a 
    /// single dbset.
    /// </summary>
    /// <typeparam name="T"></typeparam>
    public class DbSetInjection<T> : IDependencyInjectionWrapper<IAsyncEnumerable<T>>
    where T : class
    {
        public DbSetInjection(IDbSetWrapper<T> dbSetWrapper)
        {
            DbSetWrapper = dbSetWrapper;
        }
        public IAsyncEnumerable<T> Dependency => DbSetWrapper.DbSet.ToAsyncEnumerable<T>();

        public IDbSetWrapper<T> DbSetWrapper { get; }
    }
}
