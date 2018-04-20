using ExecutionStrategyCore;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;
using WebApplication1.Data.Models;

namespace WebApplication1.Data.Queries
{
    public struct DeleteBlog : IMapper<BloggingContext, Task<InternalValueCache<int>>>,
        IRunner<IMapper<BloggingContext, Task<InternalValueCache<int>>>>
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

        public IMapper<BloggingContext, Task<InternalValueCache<int>>> Run()
        {
            var v = new ExecutionStrategyRunner().CreateMapRunner<DeleteBlog, BloggingContext, int>(this, null);
            var x = new ExecutionStrategyRunner().Run(v);
            return this; //mock this on run by type, then mock the Run method

            //or runner.Run(runner.Run(new MapperRunnerFactory()).CreateRunner<IMapper<DeleteBlog>>(thisThing,params,runner));
            //I can intercept the Factory and mock it, then I can expect and or mock the CreateRunner method and mock it
            //I could alternatively expect and mock any IMapRunner<DeleteBlogs,T> and do my thing there 
        }
    }
}
