using ExecutionStrategyCore;
using System.Collections.Generic;
using Microsoft.EntityFrameworkCore;
using System.Threading.Tasks;
using WebApplication1.Data.GeneralInterfaces;
using WebApplication1.Data.ModelInterfaces;
using System.Linq;

namespace WebApplication1.Data.Queries
{
    public struct GetAll<T> : IDbSetQuery<T, IEnumerable<T>>
    where T : class
    {
        //todo: support pageing
        public async Task<InternalValueCache<IEnumerable<T>>> Run(IQueryable<T> dbSet)
        {
            var all = await dbSet.ToArrayAsync();
            return all.ToWrapper<IEnumerable<T>>();
        }
    }
}
