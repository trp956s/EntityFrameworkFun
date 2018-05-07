using ExecutionStrategyCore;
using System.Threading.Tasks;
using WebApplication1.Data.GeneralInterfaces;
using WebApplication1.Data.Models;

namespace WebApplication1.Data.Queries
{
    public struct DeleteBlog2 : IInternalRunner<BloggingContext, Task<int>>,
        IRunner<InternalValueCache<IMapper<BloggingContext, Task<int>>>>
    {
        private readonly Blog deleteEntity;

        public DeleteBlog2(Blog deleteEntity)
        {
            this.deleteEntity = deleteEntity;
        }

        public InternalValueCache<IMapper<BloggingContext, Task<int>>> Run()
        {
            return this.Wrap();
        }

        async Task<int> IInternalRunner<BloggingContext, Task<int>>.Run(BloggingContext bloggingContext)
        {
            bloggingContext.Blogs.Remove(deleteEntity);
            return await bloggingContext.SaveChangesAsync();
        }
    }
}
