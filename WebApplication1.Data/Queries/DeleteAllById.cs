using ExecutionStrategyCore;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace WebApplication1.Data.Queries
{
    public struct DeleteAllById<T> : IMapper<BloggingContext, Task<InternalRunnerWrapper<int>>>
    where T : class
    {
        private readonly T deleteEntity;

        public DeleteAllById(T deleteEntity)
        {
            this.deleteEntity = deleteEntity;
        }

        public async Task<InternalRunnerWrapper<int>> Run(BloggingContext bloggingContext)
        {
            bloggingContext.Remove(deleteEntity);
            var result = await bloggingContext.SaveChangesAsync();
            return result.ToWrapper();
        }
    }
}
