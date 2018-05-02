using ExecutionStrategyCore;
using System.Linq;
using WebApplication1.Data.Models;

namespace WebApplication1.Data.Queries
{
    public class BlogDbSetRunner : IRunner<IQueryable<Blog>>, IRunner<BloggingContext>
    {
        private readonly IRunner<BloggingContext> context;

        public BlogDbSetRunner(IRunner<BloggingContext> context){
            this.context = context;
        }


        //todo refactor IRunner to use Run<T> so that explicit overrides are not necessary
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
