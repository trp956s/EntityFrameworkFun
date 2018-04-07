using ExecutionStrategyCore;
using Microsoft.EntityFrameworkCore;
using WebApplication1.Data.Models;

namespace WebApplication1.Data.Queries
{
    public class BlogDbSetRunner : IRunner<DbSet<Blog>>
    {
        public BlogDbSetRunner(IRunner<BloggingContext> context){
        }

        public DbSet<Blog> Run()
        {
            throw new System.Exception("nmot implimented");
        }
    }
}
