using System.Collections.Generic;
using System.Data.Entity.Infrastructure;
using FakeItEasy;
using Microsoft.EntityFrameworkCore;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using System.Linq.Expressions;

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
            var context = new DbContextFake<T>(data);
            var dbSet = A.Fake<DbSet<T>>(optionsBuilder => optionsBuilder.Wrapping(context.Data));
//            var dbSet2 = A.Fake<FakeDbSet<T>>();
//            var dataEnumerator = data.GetEnumerator();

            var fakeQueryProvider = A.Fake<DbAsyncQueryProvider>();
            //A.CallTo(() =>
            //    dbSet2.Provider
            //).Returns(fakeQueryProvider);
            //var fakeAsyncEnumerator = A.Fake<IDbAsyncEnumerator<T>>();
            //A.CallTo(() =>
            //   dbSet2.GetAsyncEnumerator()
            //).Returns(fakeAsyncEnumerator);
            //A.CallTo(() =>
            //   fakeAsyncEnumerator.Current
            //).Invokes(() => Task.FromResult(dataEnumerator.Current));
            //A.CallTo(() =>
            //   fakeAsyncEnumerator.Dispose()
            //).Invokes(() => dataEnumerator.Dispose());
            //A.CallTo(() =>
            //   fakeAsyncEnumerator.MoveNextAsync(A<CancellationToken>.Ignored)
            //).Invokes(() => Task.FromResult(dataEnumerator.MoveNext()));

            //A.CallTo(() =>
            //    dbSet2.Provider
            //).Returns(fakeQueryProvider);

            var v = A.CallTo(fakeQueryProvider).Where(f =>
                true
            ).Invokes(call => {
                System.Console.WriteLine(call.FakedObject);
                System.Console.WriteLine(call.Method);
                System.Console.WriteLine(call.Arguments);
            });

            

            return dbSet;
        }
    }

    public abstract class DbAsyncQueryProvider : IDbAsyncQueryProvider
    {
        public abstract IQueryable CreateQuery(Expression expression);
        public abstract IQueryable<TElement> CreateQuery<TElement>(Expression expression);
        public abstract object Execute(Expression expression);
        public abstract TResult Execute<TResult>(Expression expression);
        public abstract Task<object> ExecuteAsync(Expression expression, CancellationToken cancellationToken);
        public abstract Task<TResult> ExecuteAsync<TResult>(Expression expression, CancellationToken cancellationToken);
    }

}
