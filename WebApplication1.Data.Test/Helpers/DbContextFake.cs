using System.Collections.Generic;
using Microsoft.EntityFrameworkCore;

namespace WebApplication1.Data.Test.Helpers
{
    public class DbContextFake<T> : DbContext
    where T : class
    {
        public DbContextFake() : this(new List<T>()) { }

        public DbContextFake(IEnumerable<T> data) : base(
            DbFake.CreateInMemoryDatabaseOptions<DbContextFake<T>>()
        )
        {
            Data.AddRange(data);
            SaveChanges();
        }

        public DbSet<T> Data { get; set; }
    }
}
