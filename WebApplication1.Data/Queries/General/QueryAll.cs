using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using WebApplication1.Data.Helpers;

namespace WebApplication1.Data.Queries.General
{
    public class QueryAll<T, Context> : IQuery<T, Context, IEnumerable<T>>
    where T : class
    {
        public Task<IEnumerable<T>> Execute(IQueryable<T> queryable)
        {
            throw new NotImplementedException();
        }

        public DbSet<T> GetDataSet(Context content)
        {
            throw new NotImplementedException();
        }
    }
}
