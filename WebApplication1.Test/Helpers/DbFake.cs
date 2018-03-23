using System.Collections.Generic;
using Microsoft.EntityFrameworkCore;

namespace WebApplication1.Test.Helpers
{
    public static class DbFake
    {
        public static DbContextOptions<T> CreateInMemoryDatabaseOptions<T>()
        where T : DbContext => new DbContextOptionsBuilder<T>().UseInMemoryDatabase(
                databaseName: System.IO.Path.GetRandomFileName()
            ).Options;

        public static DbSet<T> CreateDbSet<T>(IEnumerable<T> data)
        where T : class
        {
            return new DbContextFake<T>(data).Data;
        }
    }
}
