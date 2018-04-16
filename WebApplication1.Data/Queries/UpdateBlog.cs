using ExecutionStrategyCore;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;
using WebApplication1.Data.Models;

namespace WebApplication1.Data.Queries
{
    public struct UpdateBlog : IMapper<BloggingContext, Task<InternalRunnerWrapper<int>>>
    {
        private readonly Blog originalEntity;
        private readonly Blog newValues;

        public UpdateBlog(Blog originalEntity, Blog newValues)
        {
            this.originalEntity = originalEntity;
            this.newValues = newValues;
        }

        public async Task<InternalRunnerWrapper<int>> Run(BloggingContext context)
        {
            newValues.Id = originalEntity.Id;
            context.Entry(originalEntity).CurrentValues.SetValues(newValues);
            var returnValue = await context.SaveChangesAsync();

            return returnValue.ToWrapper();
        }
    }
}
