using ExecutionStrategyCore;
using Microsoft.EntityFrameworkCore;

namespace WebApplication1.Data.Queries
{
    public class DbSetWrapper<T> : IRunner<DbSet<T>> where T : class
    {
        public DbSetWrapper(){
        }

        public DbSet<T> Run()
        {
            throw new System.Exception("nmot implimented");
        }
    }
}
