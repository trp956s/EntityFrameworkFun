using ExecutionStrategyCore;
using System.Collections.Generic;
using Microsoft.EntityFrameworkCore;
using System.Threading.Tasks;
using WebApplication1.Data.GeneralInterfaces;
using WebApplication1.Data.ModelInterfaces;
using System.Linq;

namespace WebApplication1.Data.Queries
{
    public struct GetAllById<T> : IDbSetQuery<T, T>
    where T : class, IHasId
    {
        private readonly int id;

        public GetAllById(int id)
        {
            this.id = id;
        }

        public async Task<InternalRunnerWrapper<T>> Run(IQueryable<T> dbSet)
        {
            return await Task.FromResult(default(T).ToWrapper());
            //            var searchId = id;
            //            var all = await dbSet.SingleAsync(x=>x.Id == searchId);
            //            return all.ToWrapper();
        }
    }
}
