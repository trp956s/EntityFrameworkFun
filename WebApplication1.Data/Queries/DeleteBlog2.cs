using ExecutionStrategyCore;
using System.Threading.Tasks;
using WebApplication1.Data.GeneralInterfaces;
using WebApplication1.Data.Models;

namespace WebApplication1.Data.Queries
{
    //todo: make delete<T> where T is the datacontext type
    //todo: consider a static extension to run the delete passing only the id
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

    public struct DeleteBlog3 : IMapper<BloggingContext, Task<int>>
    {
        private readonly Blog deleteEntity;

        public DeleteBlog3(Blog deleteEntity)
        {
            this.deleteEntity = deleteEntity;
        }

        public async Task<int> Run(BloggingContext bloggingContext)
        {
            bloggingContext.Blogs.Remove(deleteEntity);
            return await bloggingContext.SaveChangesAsync();
        }
    }

    public struct DeleteBlog4 : IMapper<WrappedParameter<BloggingContext>, Task<int>>
    {
        private readonly Blog deleteEntity;

        public DeleteBlog4(Blog deleteEntity)
        {
            this.deleteEntity = deleteEntity;
        }

        public async Task<int> Run(WrappedParameter<BloggingContext> wrappedBloggingContext)
        {
            var bloggingContext = wrappedBloggingContext.GetValue();
            bloggingContext.Blogs.Remove(deleteEntity);
            return await bloggingContext.SaveChangesAsync();
        }

        public override string ToString()
        {
            return base.ToString() + " where blog = " + deleteEntity.ToString();
        }
    }
}
