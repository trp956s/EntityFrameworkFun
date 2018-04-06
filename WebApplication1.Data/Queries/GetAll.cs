using ExecutionStrategyCore;
using System;
using System.Collections.Generic;
using Microsoft.EntityFrameworkCore;
using System.Text;
using System.Threading.Tasks;
using WebApplication1.Data.GeneralInterfaces;
using WebApplication1.Data.ModelInterfaces;

namespace WebApplication1.Data.Queries
{
    public class GetAll<T> : IDbSetQuery<T, IEnumerable<T>>, IMapper<DbSet<T>, Task<InternalRunnerWrapper<IEnumerable<T>>>>
    where T : class, IHasId
    {
        //todo: support pageing
        public async Task<InternalRunnerWrapper<IEnumerable<T>>> Run(DbSet<T> dbSet)
        {
            var all = await dbSet.ToArrayAsync();
            return all.ToWrapper<IEnumerable<T>>();
        }
    }
}
