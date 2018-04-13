using ExecutionStrategyCore;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;
using WebApplication1.Data.Models;

namespace WebApplication1.Data.Queries
{
    public class CreateBlog : IMapper<BloggingContext, Task<InternalRunnerWrapper<Blog>>>
    {
        private Blog blog;

        public CreateBlog(Blog blog)
        {
            this.blog = blog;
        }

        public Task<InternalRunnerWrapper<Blog>> Run(BloggingContext arg)
        {
            throw new NotImplementedException();
        }
    }
}
