using ExecutionStrategyCore;
using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Text;
using System.Threading.Tasks;
using WebApplication1.Data.ModelInterfaces;

namespace WebApplication1.Data.Queries
{
    public struct GetAll<T>
    where T : class, IHasId
    {
        private readonly DbSetWrapper<T> datasetWrapper;

        public GetAll(DbSetWrapper<T> datasetWrapper)
        {
            this.datasetWrapper = datasetWrapper;
        }

        public ExecutionStrategy<IEnumerable<T>> CreateExecutionStrategy()
        {
            return new ExecutionStrategy<IEnumerable<T>>(Invoke, this);
        }

        //todo: support pageing
        private async Task<IEnumerable<T>> Invoke()
        {
            return await datasetWrapper.DbSet.ToArrayAsync();
        }

        public override bool Equals(object obj)
        {
            if(false == (obj is GetAll<T>))
            {
                return false;
            }

            return Equals((GetAll<T>)obj);
        }

        public bool Equals(GetAll<T> obj)
        {
            return obj.datasetWrapper == datasetWrapper;
        }
    }
}
