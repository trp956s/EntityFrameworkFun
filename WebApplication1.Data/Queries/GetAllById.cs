using ExecutionStrategyCore;
using System.Collections.Generic;
using Microsoft.EntityFrameworkCore;
using System.Threading.Tasks;
using WebApplication1.Data.GeneralInterfaces;
using WebApplication1.Data.ModelInterfaces;
using System.Linq;

namespace WebApplication1.Data.Queries
{
    public struct GetAllById<T> : IDbSetQuery<T, T>,
        IRunner<GetAllById<T>> //TODO: make all IDbSetQuery also inherit IRunner<IDbSetQuery<T, T>>
        //AND use in and out params to allow IRunner<thisType>
    where T : class, IHasId
    {
        private readonly int id;

        public GetAllById(int id)
        {
            this.id = id;
        }

        public async Task<InternalValueCache<T>> Run(IQueryable<T> dbSet)
        {
            var searchId = id;
            var match = await dbSet.FirstOrDefaultAsync(x=>x.Id == searchId);
            return match.ToWrapper();
        }

        public GetAllById<T> Run()
        {
            return this;
        }
    }
}
