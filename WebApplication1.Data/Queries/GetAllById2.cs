using ExecutionStrategyCore;
using System.Collections.Generic;
using Microsoft.EntityFrameworkCore;
using System.Threading.Tasks;
using WebApplication1.Data.GeneralInterfaces;
using WebApplication1.Data.ModelInterfaces;
using System.Linq;

namespace WebApplication1.Data.Queries
{
    public struct GetAllById2<T> :
        IInternalRunner<IQueryable<T>, Task<T>>,
        IAsyncQuerySingleFactory<T>

        //TODO: make all IDbSetQuery also inherit the IRunner used
        // in this class

        //todo: make a special type of class so that InternalValueCache<IMapper<IQueryable<T>, Task<T>>> isn't so wordy
    where T : class, IHasId
    {
        private readonly int id;

        public GetAllById2(int id)
        {
            this.id = id;
        }

        public Task<T> Run(IQueryable<T> arg)
        {
            throw new System.NotImplementedException();
        }

        public InternalValueCache<IMapper<IQueryable<T>, Task<T>>> Run()
        {
            return this.Wrap();
        }
    }

    public struct GetAllById3<T> : IMapper<IQueryable<T>, Task<T>>

        //TODO: make all IDbSetQuery also inherit the IRunner used
        // in this class

        //todo: make a special type of class so that InternalValueCache<IMapper<IQueryable<T>, Task<T>>> isn't so wordy
    where T : class, IHasId
    {
        private readonly int id;

        public GetAllById3(int id)
        {
            this.id = id;
        }

        public async Task<T> Run(IQueryable<T> dbSet)
        {
            var searchId = id;
            var match = await dbSet.FirstOrDefaultAsync(x => x.Id == searchId);
            return match;
        }
    }

    public struct GetAllById4<T> : IMapper<WrappedParameter<IQueryable<T>>, Task<T>>

        //TODO: make all IDbSetQuery also inherit the IRunner used
        // in this class

        //todo: make a special type of class so that InternalValueCache<IMapper<IQueryable<T>, Task<T>>> isn't so wordy
    where T : class, IHasId
    {
        private readonly int id;

        public GetAllById4(int id)
        {
            this.id = id;
        }

        public async Task<T> Run(WrappedParameter<IQueryable<T>> dbSet)
        {
            var searchId = id;
            var match = await dbSet.GetValue().FirstOrDefaultAsync(x => x.Id == searchId);
            return match;
        }

        public override string ToString()
        {
            return base.ToString() + " where id = " + id.ToString();
        }
    }
}
