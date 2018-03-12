using System;
using System.Collections.Generic;
using System.Text;

namespace WebApplication1.Data.Helpers
{
    /// <summary>
    /// Exposes a single DbSet.  For use with DbSetInjection.
    /// </summary>
    /// <typeparam name="T"></typeparam>
    public interface IDbSetWrapper<T> where T : class
    {
        IEnumerable<T> DbSet { get; }
    }

}
