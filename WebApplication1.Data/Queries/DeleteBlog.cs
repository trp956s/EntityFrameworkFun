using ExecutionStrategyCore;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;
using WebApplication1.Data.Models;

namespace WebApplication1.Data.Queries
{
    public struct DeleteBlog : IMapper<BloggingContext, Task<InternalValueCache<int>>>,
        IRunner<DeleteBlog>
    {
        private readonly Blog deleteEntity;

        public DeleteBlog(Blog deleteEntity)
        {
            this.deleteEntity = deleteEntity;
        }

        public async Task<InternalValueCache<int>> Run(BloggingContext bloggingContext)
        {
            bloggingContext.Blogs.Remove(deleteEntity);
            var result = await bloggingContext.SaveChangesAsync();
            return result.ToWrapper();
        }

        public DeleteBlog Run()
        {
            return this; 
        }
    }
}
