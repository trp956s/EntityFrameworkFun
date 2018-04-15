using ExecutionStrategyCore;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;
using WebApplication1.Data.Models;

namespace WebApplication1.Data.Queries
{
    public struct UpdateBlog : IMapper<BloggingContext, Task<InternalRunnerWrapper<Blog>>>
    {
        private Blog blog;

        public UpdateBlog(Blog blog)
        {
            this.blog = blog;
        }

        public Task<InternalRunnerWrapper<Blog>> Run(BloggingContext arg)
        {
            throw new NotImplementedException();
        }
    }
}
