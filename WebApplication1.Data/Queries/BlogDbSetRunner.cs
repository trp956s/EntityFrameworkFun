using ExecutionStrategyCore;
using Microsoft.EntityFrameworkCore;
using WebApplication1.Data.Models;

namespace WebApplication1.Data.Queries
{
    public class BlogDbSetRunner : IRunner<DbSet<Blog>>
    {
        private readonly IRunner<BloggingContext> context;

        public BlogDbSetRunner(IRunner<BloggingContext> context){
            this.context = context;
        }

        public DbSet<Blog> Run()
        {
            return context.Run().Blogs;
        }
    }
}
