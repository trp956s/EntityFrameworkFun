using System.Collections.Generic;
using FakeItEasy;
using Microsoft.EntityFrameworkCore;
using System.Linq;
using System.Threading.Tasks;

namespace WebApplication1.Test.Helpers
{
    public static class DbFake
    {
        public static DbContextOptions<T> CreateInMemoryDatabaseOptions<T>()
        where T : DbContext => new DbContextOptionsBuilder<T>().UseInMemoryDatabase(
                databaseName: System.IO.Path.GetRandomFileName()
            ).Options;

        public static DbSet<T> CreateAsyncEnumeratorDbSet<T>()
        where T : class
        {
            return CreateAsyncEnumeratorDbSet<T>(Enumerable.Empty<T>());
        }

        public static DbSet<T> CreateAsyncEnumeratorDbSet<T>(IEnumerable<T> data)
        where T : class
        {
            var context = new DbContextFake<T>(data);
            var dataEnum = context.Data.AsEnumerable().GetEnumerator();
            var dbSet = A.Fake<DbSet<T>>(optionsBuilder =>
            {
                optionsBuilder.Implements<IAsyncEnumerable<T>>();
                optionsBuilder.Wrapping(context.Data);
            }
            );

            var asyncEnum = A.Fake<IAsyncEnumerator<T>>();
            A.CallTo(dbSet).Where(call=>call.Method.Name == "GetEnumerator").
                WithReturnType<IAsyncEnumerator<T>>().
                Returns(asyncEnum);
            A.CallTo(asyncEnum).Where(call => call.Method.Name == "MoveNext").
                WithReturnType<Task<System.Boolean>>().
                ReturnsLazily(c => 
                    Task.FromResult(dataEnum.MoveNext())
                );
            A.CallTo(() => asyncEnum.Current).
                ReturnsLazily(c => 
                    dataEnum.Current
                );

            return dbSet;
        }
    }
}
