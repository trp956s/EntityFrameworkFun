using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Extensions.Internal;
using WebApplication1.Data.Helpers;

namespace WebApplication1.Data.Queries.General
{
    public class QueryAll<T, Context> : IQuery<T, Context, IEnumerable<T>>
    where T : class
    {
        public async Task<ICollection<T>> Execute(IAsyncEnumerable<T> queryable)
        {
            return await queryable.ToList();
        }

       public async Task<IEnumerable<T>> Execute(IQueryable<T> queryable)
        {
            return await EntityFrameworkQueryableExtensions.ToListAsync(queryable);
        }

        public Microsoft.EntityFrameworkCore.DbSet<T> GetDataSet(Context content)
        {
            throw new NotImplementedException();
        }
    }
}
