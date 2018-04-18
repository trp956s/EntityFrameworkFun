using ExecutionStrategyCore;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;
using WebApplication1.Data.Models;

namespace WebApplication1.Data.Queries
{
    public struct CreateBlog : IMapper<BloggingContext, Task<InternalValueCache<int>>>
    {
        private Blog blog;

        public CreateBlog(Blog blog)
        {
            this.blog = blog;
        }

        public async Task<InternalValueCache<int>> Run(BloggingContext bloggingContext)
        {
            bloggingContext.Blogs.Add(blog);
            var count = await bloggingContext.SaveChangesAsync();
            return count.ToWrapper();
        }
    }
}
