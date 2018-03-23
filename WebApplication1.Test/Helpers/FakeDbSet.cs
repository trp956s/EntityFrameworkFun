using System;
using System.Collections.Generic;
using System.Data.Entity.Infrastructure;
using System.Text;
using System.Threading.Tasks;
using System.Linq;
using Microsoft.EntityFrameworkCore;
using System.Collections;
using System.Linq.Expressions;

namespace WebApplication1.Test.Helpers
{
    public abstract class FakeDbSet<T> : IDbAsyncEnumerable<T>, IQueryable<T>
    where T : class
    {
        public abstract Type ElementType { get; set; }

        public abstract Expression Expression { get; set; }

        IQueryProvider IQueryable.Provider { get{ return Provider; } }
        public abstract IQueryProvider Provider { get; set; }

        public abstract IDbAsyncEnumerator<T> GetAsyncEnumerator();

        public abstract IEnumerator<T> GetEnumerator();

        IDbAsyncEnumerator IDbAsyncEnumerable.GetAsyncEnumerator() => GetAsyncEnumerator();

        IEnumerator IEnumerable.GetEnumerator() => GetEnumerator();
    }
}
