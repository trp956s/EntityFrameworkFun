using Microsoft.EntityFrameworkCore;

namespace WebApplication1.Data.Queries
{
    public class DbSetWrapper<T> where T : class
    {
        public DbSet<T> DbSet { get; set; }
    }
}
