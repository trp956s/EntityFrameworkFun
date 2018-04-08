using ExecutionStrategyCore;
using Microsoft.EntityFrameworkCore;
using System.Linq;
using WebApplication1.Data.Models;

namespace WebApplication1.Data.Queries
{
    public class BlogDbSetRunner : IRunner<IQueryable<Blog>>
    {
        private readonly IRunner<BloggingContext> context;

        public BlogDbSetRunner(IRunner<BloggingContext> context){
            this.context = context;
        }

        public IQueryable<Blog> Run()
        {
            return context.Run().Blogs;
        }
    }
}
