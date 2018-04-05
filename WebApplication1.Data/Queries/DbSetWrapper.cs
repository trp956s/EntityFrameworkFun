using ExecutionStrategyCore;
using Microsoft.EntityFrameworkCore;

namespace WebApplication1.Data.Queries
{
    public class DbSetWrapper<T> : IRunner<DbSet<T>> where T : class
    {
        public DbSet<T> DbSet { get; set; }

        public DbSet<T> Run()
        {
            return DbSet;
        }
    }
}
