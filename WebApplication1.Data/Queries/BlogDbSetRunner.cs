using ExecutionStrategyCore;
using System.Linq;
using WebApplication1.Data.Models;

namespace WebApplication1.Data.Queries
{
    //todo: avoid boxing and unboxing types
    public class BlogDbSetRunner : IRunner<IQueryable<Blog>>, IRunner<BloggingContext>
    {
        private readonly IRunner<BloggingContext> context;

        public BlogDbSetRunner(IRunner<BloggingContext> context){
            this.context = context;
        }

        public IQueryable<Blog> Run()
        {
            return context.Run().Blogs;
        }

        BloggingContext IRunner<BloggingContext>.Run()
        {
            return this.context.Run();
        }
    }
}
