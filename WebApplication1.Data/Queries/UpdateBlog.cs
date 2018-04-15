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
        private readonly Blog newValues;

        public UpdateBlog(Blog originalEntity, Blog newValues)
        {
            blog = originalEntity;
            this.newValues = newValues;
        }

        public Task<InternalRunnerWrapper<Blog>> Run(BloggingContext arg)
        {
            throw new NotImplementedException();
        }
    }
}
