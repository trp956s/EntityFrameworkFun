using System.Collections.Generic;
using FakeItEasy;
using Microsoft.EntityFrameworkCore;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using System.Linq.Expressions;
using System;
using System.Collections;

namespace WebApplication1.Test.Helpers
{
    public static class DbFake
    {
        public static DbContextOptions<T> CreateInMemoryDatabaseOptions<T>()
        where T : DbContext => new DbContextOptionsBuilder<T>().UseInMemoryDatabase(
                databaseName: System.IO.Path.GetRandomFileName()
            ).Options;

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

    public abstract class DbAsyncQueryProvider<T> : IQueryable<T>, IAsyncEnumerable<T>
    where T : class
    {
        public abstract Type ElementType { get; }
        public abstract Expression Expression { get; }
        public abstract IQueryProvider Provider { get; }

        public abstract IEnumerator<T> GetEnumerator();

        IEnumerator IEnumerable.GetEnumerator()
        {
            throw new NotImplementedException();
        }

        IAsyncEnumerator<T> IAsyncEnumerable<T>.GetEnumerator()
        {
            throw new NotImplementedException();
        }
    }

}
